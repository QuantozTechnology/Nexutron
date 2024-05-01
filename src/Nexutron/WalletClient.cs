using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Nexutron.Accounts;
using Nexutron.Helpers;
using Nexutron.Protocol;

namespace Nexutron
{
    class WalletClient : IWalletClient
    {
        private readonly IGrpcChannelClient _channelClient;
        private readonly IOptions<TronDotNetOptions> _options;

        public WalletClient(IGrpcChannelClient channelClient, IOptions<TronDotNetOptions> options)
        {
            _channelClient = channelClient;
            _options = options;
        }

        public Wallet.WalletClient GetProtocol()
        {
            var channel = _channelClient.GetProtocol();
            var wallet = new Wallet.WalletClient(channel);
            return wallet;
        }

        public ITronAccount GenerateAccount()
        {
            return AccountHelper.GenerateAccount(_options.Value.Network);
        }

        public ITronAccount GetAccount(string privateKey)
        {
            return AccountHelper.GetAccount(privateKey, _options.Value.Network);
        }

        public WalletSolidity.WalletSolidityClient GetSolidityProtocol()
        {
            var channel = _channelClient.GetSolidityProtocol();
            var wallet = new WalletSolidity.WalletSolidityClient(channel);

            return wallet;
        }

        public ByteString ParseAddress(string address)
        {
            return AccountHelper.ParseAddress(address);
        }

        public Metadata GetHeaders()
        {
            return WalletHelper.GetHeaders(_options.Value.ApiKey);
        }
    }
}
