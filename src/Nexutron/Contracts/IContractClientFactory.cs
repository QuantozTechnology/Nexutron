using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexutron.Contracts
{
    public interface IContractClientFactory
    {
        IContractClient CreateClient(ContractProtocol protocol);
    }
}
