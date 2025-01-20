using System;
using Microsoft.Extensions.DependencyInjection;

namespace Nexutron.Contracts
{
    class ContractClientFactory(IServiceProvider serviceProvider) : IContractClientFactory
    {
        public IContractClient CreateClient(ContractProtocol protocol)
        {

            IContractClient client = protocol switch
            {
                ContractProtocol.TRC20 => serviceProvider.GetService<TRC20ContractClient>(),
                _ => throw new NotImplementedException()
            };

            return client;
        }
    }
}
