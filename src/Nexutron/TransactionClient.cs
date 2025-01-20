using Google.Protobuf;
using Microsoft.Extensions.Options;
using Nexutron.Extensions;
using Nexutron.Helpers;
using Nexutron.Protocol;
using System;
using System.Threading.Tasks;

namespace Nexutron
{
    class TransactionClient(IWalletClient walletClient, IOptions<NexutronOptions> options) : ITransactionClient
    {
        public async Task<TransactionExtention> CreateTransactionAsync(string from, string to, long amount)
        {
            var wallet = walletClient.GetWalletClient();
            var newestBlock = await wallet.GetNowBlock2Async(new EmptyMessage(), headers: options.Value.GetgRPCHeaders());

            var transaction = TransactionHelper.CreateTransaction(newestBlock, from, to, amount, DateTime.UtcNow, 10 * 60 * 1000);

            if (transaction == null)
            {
                return new TransactionExtention
                {
                    Result = new Return { Result = false, Code = Return.Types.response_code.OtherError },
                };
            }

            return new TransactionExtention
            {
                Transaction = transaction,
                Txid = ByteString.CopyFromUtf8(transaction.GetTxid()),
                Result = new Return { Result = true, Code = Return.Types.response_code.Success },
            };
        }

        public Transaction GetTransactionSign(Transaction transaction, string privateKey)
        {
            return TransactionHelper.SignTransaction(transaction, privateKey);
        }

        public async Task<Return> BroadcastTransactionAsync(Transaction transaction)
        {
            var wallet = walletClient.GetWalletClient();
            var result = await wallet.BroadcastTransactionAsync(transaction, headers: options.Value.GetgRPCHeaders());

            return result;
        }

    }
}
