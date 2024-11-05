using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Grpc.Net.Client;

namespace Nexutron;

class GrpcChannelClient : IGrpcChannelClient
{
    private readonly ILogger<GrpcChannelClient> _logger;
    private readonly IOptions<NexutronOptions> _options;

    public GrpcChannelClient(ILogger<GrpcChannelClient> logger, IOptions<NexutronOptions> options)
    {
        _logger = logger;
        _options = options;
    }

    public GrpcChannel GetProtocol()
    {
        return Helpers.GrpcChannelClientHelpers.GetProtocol(_options.Value.Channel.Host, _options.Value.Channel.Port);
    }
    public GrpcChannel GetSolidityProtocol()
    {
        return Helpers.GrpcChannelClientHelpers.GetSolidityProtocol(_options.Value.SolidityChannel.Host, _options.Value.SolidityChannel.Port);
    }
}
