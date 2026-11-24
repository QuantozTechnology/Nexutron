using System;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using Nexutron.Crypto;
using Nexutron.Helpers;
using Nexutron.Protocol;

namespace Nexutron
{
    class TransactionClient : ITransactionClient
    {
        private const long DefaultExpirationMs = 10 * 60 * 1000;

        private readonly IWalletClient _walletClient;
        private readonly IOptions<NexutronOptions> _options;

        public TransactionClient(IWalletClient walletClient, IOptions<NexutronOptions> options)
        {
            _walletClient = walletClient;
            _options = options;
        }

        public Task<TransactionExtention> CreateTransactionAsync(string from, string to, long amount)
            => CreateTransactionAsync(from, to, amount, memo: null);

        public async Task<TransactionExtention> CreateTransactionAsync(string from, string to, long amount, string memo)
        {
            var wallet = _walletClient.GetWalletClient();
            var newestBlock = await wallet.GetNowBlock2Async(new EmptyMessage(), headers: _options.Value.GetgRPCHeaders());

            var transaction = TransactionHelper.CreateTransaction(newestBlock, from, to, amount, DateTime.UtcNow, DefaultExpirationMs, memo);

            return new TransactionExtention
            {
                Transaction = transaction,
                Txid = ByteString.CopyFrom(transaction.RawData.ToByteArray().ToSHA256Hash()),
                Result = new Return { Result = true, Code = Return.Types.response_code.Success },
            };
        }

        public Transaction GetTransactionSign(Transaction transaction, string privateKey)
        {
            return TransactionHelper.SignTransaction(transaction, privateKey);
        }

        public async Task<Return> BroadcastTransactionAsync(Transaction transaction)
        {
            var wallet = _walletClient.GetWalletClient();
            return await wallet.BroadcastTransactionAsync(transaction, headers: _options.Value.GetgRPCHeaders());
        }
    }
}
