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
            // Only a 20 byte address, or 21 bytes with the 0x41 prefix: slicing anything longer would change the recipient.
            var valid = bytes.Length == 20 || (bytes.Length == 21 && bytes[0] == 0x41);
            if (!valid) throw new ArgumentException($"Invalid address: {tronAddress}", nameof(tronAddress));
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

        /// <summary>
        /// Extracts the reason of a failed constant call: the Solidity Error(string) message if present, else the node's message.
        /// </summary>
        internal static string GetFailureReason(TransactionExtention result)
        {
            if (result.ConstantResult.Count > 0)
            {
                var output = result.ConstantResult[0].ToByteArray();
                // Error(string): 0x08c379a0 ++ offset ++ length ++ utf8 bytes
                if (output.Length >= 68 && output.AsSpan(0, 4).SequenceEqual(new byte[] { 0x08, 0xc3, 0x79, 0xa0 }))
                {
                    var length = (int)new BigInteger(output.AsSpan(36, 32), isUnsigned: true, isBigEndian: true);
                    if (length >= 0 && 68 + length <= output.Length)
                    {
                        return Encoding.UTF8.GetString(output, 68, length);
                    }
                }
            }

            return result.Result?.Message.ToStringUtf8();
        }

        /// <summary>
        /// Executes the call on the node without creating a transaction. A revert is reported in Transaction.Ret
        /// while Result.Result stays true, so both are checked.
        /// </summary>
        private async Task<TransactionExtention> SimulateAsync(Wallet.WalletClient wallet, ByteString contractAddress, ByteString ownerAddress, byte[] data)
        {
            var trigger = new TriggerSmartContract
            {
                ContractAddress = contractAddress,
                Data = ByteString.CopyFrom(data),
            };
            if (ownerAddress != null) trigger.OwnerAddress = ownerAddress;

            var result = await wallet.TriggerConstantContractAsync(trigger, headers: _walletClient.GetHeaders());

            var reverted = result.Transaction?.Ret.Count > 0 && result.Transaction.Ret[0].Ret == Transaction.Types.Result.Types.code.Failed;
            if (result.Result == null || !result.Result.Result || reverted)
            {
                throw new InvalidOperationException($"Contract call failed: {GetFailureReason(result)}");
            }

            return result;
        }

        private async Task<byte[]> CallConstantAsync(Wallet.WalletClient wallet, ByteString contractAddress, ByteString ownerAddress, byte[] data)
        {
            var result = await SimulateAsync(wallet, contractAddress, ownerAddress, data);
            var output = result.ConstantResult.Count > 0 ? result.ConstantResult[0].ToByteArray() : null;
            if (output == null || output.Length < 32)
            {
                throw new InvalidOperationException("Constant call returned no ABI word (is this a TRC20 contract?).");
            }

            return output;
        }

        private async Task<int> GetDecimalsAsync(Wallet.WalletClient wallet, ByteString contractAddress, ByteString ownerAddress)
        {
            var output = await CallConstantAsync(wallet, contractAddress, ownerAddress, EncodeDecimals());
            return DecodeDecimals(output);
        }

        /// <summary>decimals() is a uint8; reject anything else so a hostile contract cannot force huge powers of ten.</summary>
        internal static int DecodeDecimals(byte[] output)
        {
            var value = DecodeUint256(output);
            if (value > byte.MaxValue) throw new InvalidOperationException($"Invalid decimals() value: {value}.");
            return (int)value;
        }

        internal static BigInteger DecodeUint256(byte[] output)
        {
            if (output == null || output.Length < 32) throw new InvalidOperationException("Expected a 32 byte ABI word.");
            return new BigInteger(output.AsSpan(0, 32), isUnsigned: true, isBigEndian: true);
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

                var data = EncodeTransfer(toAddress, rawAmount);

                // Dry-run first: TriggerContract only builds the transaction, so a transfer that would revert
                // (e.g. insufficient token balance) would otherwise be broadcast and burn its fee on chain.
                try
                {
                    await SimulateAsync(wallet, contractAddressBytes, ownerAddressBytes, data);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning("[transfer] simulation failed, not broadcasting: {Message}", ex.Message);
                    return null;
                }

                var trigger = new TriggerSmartContract
                {
                    ContractAddress = contractAddressBytes,
                    OwnerAddress = ownerAddressBytes,
                    Data = ByteString.CopyFrom(data),
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

                var balance = DecodeUint256(output);
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
