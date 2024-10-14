using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nexutron.Contracts;

namespace Nexutron
{
    public interface ITronClient
    {
        IGrpcChannelClient GetChannel();
        IWalletClient GetWallet();
        ITransactionClient GetTransaction();
        IContractClient GetContract();
    }
}
