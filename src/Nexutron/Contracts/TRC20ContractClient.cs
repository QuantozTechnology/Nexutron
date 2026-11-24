using System;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Nethereum.Contracts;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nexutron.Accounts;
using Nexutron.Crypto;
using Nexutron.Extensions;
using Nexutron.Helpers;
using Nexutron.Protocol;

namespace Nexutron.Contracts
{
    class TRC20ContractClient : IContractClient
    {
        private readonly ILogger<TRC20ContractClient> _logger;
        private readonly IWalletClient _walletClient;
        private readonly ITransactionClient _transactionClient;

        public ContractProtocol Protocol => ContractProtocol.TRC20;

        public TRC20ContractClient(ILogger<TRC20ContractClient> logger, IWalletClient walletClient, ITransactionClient transactionClient)
        {
            _logger = logger;
            _walletClient = walletClient;
            _transactionClient = transactionClient;
        }

        /// <summary>
        /// Tron addresses are 21 bytes (0x41 prefix + 20 bytes); the ABI address type takes the trailing 20 bytes.
        /// </summary>
        internal static string ToAbiAddress(string tronAddress)
        {
            var bytes = AccountHelper.ParseAddress(tronAddress).ToByteArray();
            if (bytes.Length < 20) throw new ArgumentException($"Invalid address: {tronAddress}", nameof(tronAddress));
            return "0x" + bytes.AsSpan(bytes.Length - 20).ToArray().ToHex();
        }

        internal static byte[] EncodeTransfer(string toAddress, BigInteger rawAmount)
        {
            var transfer = new TransferFunction { To = ToAbiAddress(toAddress), Value = rawAmount };
            return transfer.GetCallData();
        }

        internal static byte[] EncodeBalanceOf(string ownerAddress)
        {
            return new BalanceOfFunction { Owner = ToAbiAddress(ownerAddress) }.GetCallData();
        }

        internal static byte[] EncodeDecimals()
        {
            return new DecimalsFunction().GetCallData();
        }

        private async Task<byte[]> CallConstantAsync(Wallet.WalletClient wallet, ByteString contractAddress, ByteString ownerAddress, byte[] data)
        {
            var trigger = new TriggerSmartContract
            {
                ContractAddress = contractAddress,
                Data = ByteString.CopyFrom(data),
            };
            if (ownerAddress != null) trigger.OwnerAddress = ownerAddress;

            var result = await wallet.TriggerConstantContractAsync(trigger, headers: _walletClient.GetHeaders());

            if (result.Result != null && !result.Result.Result)
            {
                throw new InvalidOperationException($"Constant call failed: {result.Result.Code} {result.Result.Message.ToStringUtf8()}");
            }
            if (result.ConstantResult.Count == 0)
            {
                throw new InvalidOperationException("Constant call returned no result.");
            }

            return result.ConstantResult[0].ToByteArray();
        }

        private async Task<int> GetDecimalsAsync(Wallet.WalletClient wallet, ByteString contractAddress, ByteString ownerAddress)
        {
            var output = await CallConstantAsync(wallet, contractAddress, ownerAddress, EncodeDecimals());
            return (int)new BigInteger(output, isUnsigned: true, isBigEndian: true);
        }

        public async Task<string> TransferAsync(string contractAddress, ITronAccount ownerAccount, string toAddress, decimal amount, string memo, long feeLimit)
        {
            ArgumentNullException.ThrowIfNull(ownerAccount);

            try
            {
                var wallet = _walletClient.GetWalletClient();
                var contractAddressBytes = AccountHelper.ParseAddress(contractAddress);
                var ownerAddressBytes = AccountHelper.ParseAddress(ownerAccount.Address);

                var decimals = await GetDecimalsAsync(wallet, contractAddressBytes, ownerAddressBytes);
                var rawAmount = TokenAmount.ToRaw(amount, decimals);

                var trigger = new TriggerSmartContract
                {
                    ContractAddress = contractAddressBytes,
                    OwnerAddress = ownerAddressBytes,
                    Data = ByteString.CopyFrom(EncodeTransfer(toAddress, rawAmount)),
                };

                // TriggerContract (not TriggerConstantContract) builds the state changing transaction to sign.
                var transactionExtention = await wallet.TriggerContractAsync(trigger, headers: _walletClient.GetHeaders());

                if (transactionExtention.Result == null || !transactionExtention.Result.Result)
                {
                    _logger.LogWarning("[transfer] trigger failed, code={Code}, message={Message}.",
                        transactionExtention.Result?.Code, transactionExtention.Result?.Message.ToStringUtf8());
                    return null;
                }

                var transaction = transactionExtention.Transaction;

                if (transaction.Ret.Count > 0 && transaction.Ret[0].Ret == Transaction.Types.Result.Types.code.Failed)
                {
                    _logger.LogWarning("[transfer] transaction execution failed.");
                    return null;
                }

                if (!string.IsNullOrEmpty(memo))
                {
                    transaction.RawData.Data = ByteString.CopyFrom(Encoding.UTF8.GetBytes(memo));
                }
                transaction.RawData.FeeLimit = feeLimit;

                var transSign = _transactionClient.GetTransactionSign(transaction, ownerAccount.PrivateKey);

                var broadcast = await _transactionClient.BroadcastTransactionAsync(transSign);
                if (!broadcast.Result)
                {
                    _logger.LogWarning("[transfer] broadcast failed, code={Code}, message={Message}.",
                        broadcast.Code, broadcast.Message.ToStringUtf8());
                    return null;
                }

                return transSign.GetTxid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[transfer] {Message}", ex.Message);
                return null;
            }
        }

        public async Task<decimal> BalanceOfAsync(string contractAddress, ITronAccount ownerAccount)
        {
            ArgumentNullException.ThrowIfNull(ownerAccount);

            try
            {
                var wallet = _walletClient.GetWalletClient();
                var contractAddressBytes = AccountHelper.ParseAddress(contractAddress);
                var ownerAddressBytes = AccountHelper.ParseAddress(ownerAccount.Address);

                var decimals = await GetDecimalsAsync(wallet, contractAddressBytes, ownerAddressBytes);
                var output = await CallConstantAsync(wallet, contractAddressBytes, ownerAddressBytes, EncodeBalanceOf(ownerAccount.Address));

                var balance = new BigInteger(output, isUnsigned: true, isBigEndian: true);
                return TokenAmount.FromRaw(balance, decimals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[balanceOf] {Message}", ex.Message);
                throw;
            }
        }
    }
}
