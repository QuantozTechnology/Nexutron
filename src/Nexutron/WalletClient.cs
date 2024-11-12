using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Nexutron.Accounts;
using Nexutron.Helpers;
using Nexutron.Protocol;

namespace Nexutron;

public class WalletClient(IGrpcChannelClient channelClient, IOptions<NexutronOptions> options) : IWalletClient
{
    public Wallet.WalletClient GetWalletClient()
    {
        var channel = channelClient.GetProtocol();
        return WalletClientHelper.GetWalletClient(channel);
    }

    public ITronAccount GenerateAccount()
    {
        return AccountHelper.GenerateAccount();
    }

    public ITronAccount GetAccount(string privateKey)
    {
        return AccountHelper.GetAccount(privateKey);
    }

    public WalletSolidity.WalletSolidityClient GetSolidityClient()
    {
        var channel = channelClient.GetSolidityProtocol();
        return WalletClientHelper.GetSolidityClient(channel);
    }

    public ByteString ParseAddress(string address)
    {
        return AccountHelper.ParseAddress(address);
    }

    public Metadata GetHeaders()
    {
        return WalletHelper.GetHeaders(options.Value.ApiKey);
    }
}
