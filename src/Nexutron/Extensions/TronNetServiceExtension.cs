using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nexutron
{
    public static class TronDotNetServiceExtension
    {
        public static IServiceCollection AddTronDotNet(this IServiceCollection services, Action<NexutronOptions> setupAction)
        {
            ArgumentNullException.ThrowIfNull(setupAction);

            services.AddOptions();
            services.AddLogging();
            services.TryAddSingleton<IGrpcChannelClient, GrpcChannelClient>();
            services.TryAddTransient<ITransactionClient, TransactionClient>();
            services.TryAddTransient<ITronClient, TronClient>();
            services.TryAddTransient<IWalletClient, WalletClient>();
            services.TryAddSingleton<Contracts.IContractClientFactory, Contracts.ContractClientFactory>();
            services.TryAddTransient<Contracts.TRC20ContractClient>();
            services.Configure(setupAction);

            return services;
        }
    }
}
