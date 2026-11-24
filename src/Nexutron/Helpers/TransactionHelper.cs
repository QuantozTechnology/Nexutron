using System;
using System.Text;
using Google.Protobuf;
using Nethereum.Signer;
using Nexutron.Crypto;
using Nexutron.Protocol;

namespace Nexutron.Helpers
{
    public static class TransactionHelper
    {
        public static Transaction CreateTransaction(BlockExtention newestBlock, string from, string to, long amount, DateTime timestamp, long expirationTime, string memo = null)
        {
            ArgumentNullException.ThrowIfNull(newestBlock);
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount (in sun) must be positive.");

            var transferContract = new TransferContract
            {
                OwnerAddress = AccountHelper.ParseAddress(from),
                ToAddress = AccountHelper.ParseAddress(to),
                Amount = amount
            };

            var contract = new Transaction.Types.Contract
            {
                Type = Transaction.Types.Contract.Types.ContractType.TransferContract,
                Parameter = Google.Protobuf.WellKnownTypes.Any.Pack(transferContract),
            };

            var transaction = new Transaction { RawData = new Transaction.Types.raw() };
            transaction.RawData.Contract.Add(contract);

            // Note: the network charges an extra fee for transactions carrying a memo.
            if (!string.IsNullOrEmpty(memo))
            {
                transaction.RawData.Data = ByteString.CopyFrom(Encoding.UTF8.GetBytes(memo));
            }

            // milliseconds (since unix epoch)
            var currentTimestamp = new DateTimeOffset(DateTime.SpecifyKind(timestamp.ToUniversalTime(), DateTimeKind.Utc)).ToUnixTimeMilliseconds();

            transaction.RawData.Timestamp = currentTimestamp;
            transaction.RawData.Expiration = currentTimestamp + expirationTime;

            SetReferenceBlock(transaction, newestBlock);

            return transaction;
        }

        /// <summary>
        /// Sets ref_block_bytes / ref_block_hash (TaPoS) on the transaction from the given block.
        /// </summary>
        public static void SetReferenceBlock(Transaction transaction, BlockExtention block)
        {
            var blockHeight = block.BlockHeader.RawData.Number;
            var blockHash = Sha256Sm3Hash.Of(block.BlockHeader.RawData.ToByteArray()).GetBytes();

            var refBlockNum = BitConverter.GetBytes(blockHeight);
            if (BitConverter.IsLittleEndian) Array.Reverse(refBlockNum);

            transaction.RawData.RefBlockHash = ByteString.CopyFrom(blockHash, 8, 8);
            transaction.RawData.RefBlockBytes = ByteString.CopyFrom(refBlockNum, 6, 2);
        }

        /// <summary>
        /// Signs the transaction's raw data with the given private key and returns a signed copy.
        /// The signature is the 65 byte r || s || v form (v = 27/28) used by java-tron.
        /// </summary>
        public static Transaction SignTransaction(Transaction transaction, string privateKey)
        {
            ArgumentNullException.ThrowIfNull(transaction);
            if (string.IsNullOrWhiteSpace(privateKey)) throw new ArgumentNullException(nameof(privateKey));

            var ecKey = new EthECKey(privateKey.HexToByteArray(), true);
            var transactionSigned = transaction.Clone();
            var hash = transactionSigned.RawData.ToByteArray().ToSHA256Hash();

            var signature = ecKey.SignAndCalculateYParityV(hash);

            var sign = new byte[65];
            CopyLeftPadded(signature.R, sign, 0);
            CopyLeftPadded(signature.S, sign, 32);
            // Recovery id as 27/28, like java-tron and TronWeb produce.
            var v = signature.V is { Length: > 0 } ? signature.V[^1] : (byte)0;
            sign[64] = v < 27 ? (byte)(v + 27) : v;

            transactionSigned.Signature.Add(ByteString.CopyFrom(sign));

            return transactionSigned;
        }

        private static void CopyLeftPadded(byte[] value, byte[] destination, int offset)
        {
            // R and S may come with a leading 0x00 sign byte or with fewer than 32 bytes.
            var start = Math.Max(0, value.Length - 32);
            var length = value.Length - start;
            Array.Copy(value, start, destination, offset + 32 - length, length);
        }
    }
}
