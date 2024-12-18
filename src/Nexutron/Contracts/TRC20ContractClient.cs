﻿using Google.Protobuf;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nexutron.Accounts;
using Nexutron.Crypto;
using Nexutron.Extensions;
using Nexutron.Protocol;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;

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

        private long GetDecimals(Wallet.WalletClient wallet, byte[] contractAddressBytes)
        {
            var trc20Decimals = new DecimalsFunction();

            var callEncoder = new FunctionCallEncoder();
            var functionABI = ABITypedRegistry.GetFunctionABI<DecimalsFunction>();

            var encodedHex = callEncoder.EncodeRequest(trc20Decimals, functionABI.Sha3Signature);

            var trigger = new TriggerSmartContract
            {
                ContractAddress = ByteString.CopyFrom(contractAddressBytes),
                Data = ByteString.CopyFrom(encodedHex.HexToByteArray()),
            };

            var txnExt = wallet.TriggerConstantContract(trigger, headers: _walletClient.GetHeaders());

            var result = txnExt.ConstantResult[0].ToByteArray().ToHex();

            return new FunctionCallDecoder().DecodeOutput<long>(result, new Parameter("uint8", "d"));
        }

        public async Task<string> TransferAsync(string contractAddress, ITronAccount ownerAccount, string toAddress, decimal amount, string memo, long feeLimit)
        {
            var contractAddressBytes = Base58Encoder.DecodeFromBase58Check(contractAddress);
            var callerAddressBytes = Base58Encoder.DecodeFromBase58Check(toAddress);
            var ownerAddressBytes = Base58Encoder.DecodeFromBase58Check(ownerAccount.Address);
            var wallet = _walletClient.GetWalletClient();
            var functionABI = ABITypedRegistry.GetFunctionABI<TransferFunction>();
            try
            {


                var toAddressBytes = new byte[20];
                Array.Copy(callerAddressBytes, 1, toAddressBytes, 0, toAddressBytes.Length);

                var toAddressHex = "0x" + toAddressBytes.ToHex();

                var decimals = GetDecimals(wallet, contractAddressBytes);

                var tokenAmount = amount;
                if (decimals > 0)
                {
                    tokenAmount = amount * Convert.ToDecimal(Math.Pow(10, decimals));
                }

                // TODO: Doublecheck it does not care about the address format (Hex/Base58)
                var trc20Transfer = new TransferFunction
                {
                    FromAddress = ownerAccount.Address,
                    To = toAddressHex,
                    Value = Convert.ToInt64(tokenAmount),
                };

                var txInput = trc20Transfer.CreateTransactionInput(contractAddress);


                var trigger = new TriggerSmartContract
                {
                    ContractAddress = ByteString.CopyFrom(contractAddressBytes),
                    OwnerAddress = ByteString.CopyFrom(ownerAddressBytes),
                    Data = ByteString.CopyFrom(txInput.Data.HexToByteArray()),
                };

                var transactionExtention = await wallet.TriggerConstantContractAsync(trigger, headers: _walletClient.GetHeaders());

                if (!transactionExtention.Result.Result)
                {
                    _logger.LogWarning($"[transfer]transfer failed, message={transactionExtention.Result.Message.ToStringUtf8()}.");
                    return null;
                }

                var transaction = transactionExtention.Transaction;

                if (transaction.Ret.Count > 0 && transaction.Ret[0].Ret == Nexutron.Protocol.Transaction.Types.Result.Types.code.Failed)
                {
                    return null;
                }

                transaction.RawData.Data = ByteString.CopyFromUtf8(memo);
                transaction.RawData.FeeLimit = feeLimit;

                var transSign = _transactionClient.GetTransactionSign(transaction, ownerAccount.PrivateKey);

                var result = await _transactionClient.BroadcastTransactionAsync(transSign);

                return transSign.GetTxid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }



        public async Task<decimal> BalanceOfAsync(string contractAddress, ITronAccount ownerAccount)
        {
            var contractAddressBytes = Base58Encoder.DecodeFromBase58Check(contractAddress);
            var ownerAddressBytes = Base58Encoder.DecodeFromBase58Check(ownerAccount.Address);
            var wallet = _walletClient.GetWalletClient();
            var functionABI = ABITypedRegistry.GetFunctionABI<BalanceOfFunction>();
            try
            {
                var addressBytes = new byte[20];
                Array.Copy(ownerAddressBytes, 1, addressBytes, 0, addressBytes.Length);

                var addressBytesHex = "0x" + addressBytes.ToHex();

                var balanceOf = new BalanceOfFunction { Owner = addressBytesHex };
                var decimals = GetDecimals(wallet, contractAddressBytes);

                var encodedHex = new FunctionCallEncoder().EncodeRequest(balanceOf, functionABI.Sha3Signature);

                var trigger = new TriggerSmartContract
                {
                    ContractAddress = ByteString.CopyFrom(contractAddressBytes),
                    OwnerAddress = ByteString.CopyFrom(ownerAddressBytes),
                    Data = ByteString.CopyFrom(encodedHex.HexToByteArray()),
                };

                var transactionExtention = await wallet.TriggerConstantContractAsync(trigger, headers: _walletClient.GetHeaders());

                if (!transactionExtention.Result.Result)
                {
                    throw new Exception(transactionExtention.Result.Message.ToStringUtf8());
                }
                if (transactionExtention.ConstantResult.Count == 0)
                {
                    throw new Exception($"result error, ConstantResult length=0.");
                }

                var result = new FunctionCallDecoder().DecodeFunctionOutput<BalanceOfOutputDTO>(transactionExtention.ConstantResult[0].ToByteArray().ToHex());

                var balance = Convert.ToDecimal(result.Balance);
                if (decimals > 0)
                {
                    balance /= Convert.ToDecimal(Math.Pow(10, decimals));
                }

                return balance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

    }

}
