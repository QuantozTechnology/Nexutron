using System.Threading.Tasks;
using Nexutron.Protocol;

namespace Nexutron
{
    public interface ITransactionClient
    {
        /// <summary>Builds an unsigned TRX transfer. <paramref name="amount"/> is in sun (1 TRX = 1,000,000 sun).</summary>
        Task<TransactionExtention> CreateTransactionAsync(string from, string to, long amount);

        /// <summary>Builds an unsigned TRX transfer with a UTF-8 memo. <paramref name="amount"/> is in sun.</summary>
        Task<TransactionExtention> CreateTransactionAsync(string from, string to, long amount, string memo);

        Transaction GetTransactionSign(Transaction transaction, string privateKey);

        Task<Return> BroadcastTransactionAsync(Transaction transaction);
    }
}
