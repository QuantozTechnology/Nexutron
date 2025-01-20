using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Grpc.Net.Client;

namespace Nexutron;

class GrpcChannelClient(IOptions<NexutronOptions> options) : IGrpcChannelClient
{
    public GrpcChannel GetProtocol()
    {
        return Helpers.GrpcChannelClientHelpers.GetProtocol(options.Value.Channel.Host, options.Value.Channel.Port);
    }
    public GrpcChannel GetSolidityProtocol()
    {
        return Helpers.GrpcChannelClientHelpers.GetSolidityProtocol(options.Value.SolidityChannel.Host, options.Value.SolidityChannel.Port);
    }
}
