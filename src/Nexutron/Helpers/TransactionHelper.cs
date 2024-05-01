using Google.Protobuf;
using Nethereum.Signer;
using Nethereum.Signer.Crypto;
using Nexutron.Crypto;
using Nexutron.Extensions;
using Nexutron.Protocol;
using System;

namespace Nexutron.Helpers
{
    public static class TransactionHelper
    {
        public static Transaction CreateTransaction(BlockExtention newestBlock, string from, string to, long amount, long expirationTime)
        {
            var fromAddress = AccountHelper.ParseAddress(from);
            var toAddress = AccountHelper.ParseAddress(to);

            var transferContract = new TransferContract
            {
                OwnerAddress = fromAddress,
                ToAddress = toAddress,
                Amount = amount
            };

            var transaction = new Transaction();

            var contract = new Transaction.Types.Contract();

            try
            {
                contract.Parameter = Google.Protobuf.WellKnownTypes.Any.Pack(transferContract);
            }
            catch (Exception)
            {
                return null;
            }

            contract.Type = Transaction.Types.Contract.Types.ContractType.TransferContract;
            transaction.RawData = new Transaction.Types.raw();
            transaction.RawData.Contract.Add(contract);
            transaction.RawData.Timestamp = DateTime.Now.Ticks;
            transaction.RawData.Expiration = newestBlock.BlockHeader.RawData.Timestamp + expirationTime;// 10 * 60 * 60 * 1000;
            var blockHeight = newestBlock.BlockHeader.RawData.Number;
            var blockHash = Sha256Sm3Hash.Of(newestBlock.BlockHeader.RawData.ToByteArray()).GetBytes();

            var bb = ByteBuffer.Allocate(8);
            bb.PutLong(blockHeight);

            var refBlockNum = bb.ToArray();

            transaction.RawData.RefBlockHash = ByteString.CopyFrom(blockHash.SubArray(8, 8));
            transaction.RawData.RefBlockBytes = ByteString.CopyFrom(refBlockNum.SubArray(6, 2));

            return transaction;
        }

        public static Transaction SignTransaction(Transaction transaction, string privateKey)
        {
            var ecKey = new EthECKey(privateKey.HexToByteArray(), true);
            var transactionSigned = Transaction.Parser.ParseFrom(transaction.ToByteArray());
            var rawdata = transactionSigned.RawData.ToByteArray();
            var hash = rawdata.ToSHA256Hash();
            var ethSig = ecKey.SignAndCalculateYParityV(hash);
            var sig = ECDSASignature.FromDER(ethSig.ToDER());
            sig.V = ethSig.V;
            var sign = sig.ToByteArray();

            transactionSigned.Signature.Add(ByteString.CopyFrom(sign));

            return transactionSigned;
        }
    }
}
