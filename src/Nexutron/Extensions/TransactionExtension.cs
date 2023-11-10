using Google.Protobuf;
using Nexutron.Crypto;

namespace Nexutron.Extensions
{
    public static class TransactionExtension
    {
        public static string GetTxid(this Protocol.Transaction transaction)
        {
            var txid = transaction.RawData.ToByteArray().ToSHA256Hash().ToHex();

            return txid;
        }
    }
}
