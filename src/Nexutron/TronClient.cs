using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nexutron.Contracts;
using Nexutron.Protocol;

namespace Nexutron
{
    class TronClient(IGrpcChannelClient channelClient, IWalletClient walletClient, ITransactionClient transactionClient) : ITronClient
    {
        private readonly IGrpcChannelClient _channelClient = channelClient;
        private readonly IWalletClient _walletClient = walletClient;
        private readonly ITransactionClient _transactionClient = transactionClient;

        public IGrpcChannelClient GetChannel()
        {
            return _channelClient;
        }
        public IWalletClient GetWallet()
        {
            return _walletClient;
        }

        public ITransactionClient GetTransaction()
        {
            return _transactionClient;
        }

        public IContractClient GetContract()
        {
            return null;
        }
    }
}
