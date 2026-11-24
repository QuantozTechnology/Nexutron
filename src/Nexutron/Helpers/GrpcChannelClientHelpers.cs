using System;
using Grpc.Core;
using Grpc.Net.Client;

namespace Nexutron.Helpers;

public static class GrpcChannelClientHelpers
{
    public static GrpcChannel GetProtocol(string host, int port) => CreateChannel(host, port, useTls: true);

    public static GrpcChannel GetSolidityProtocol(string host, int port) => CreateChannel(host, port, useTls: true);

    public static GrpcChannel CreateChannel(GrpcChannelOption option)
    {
        ArgumentNullException.ThrowIfNull(option);
        return CreateChannel(option.Host, option.Port, option.UseTls);
    }

    public static GrpcChannel CreateChannel(string host, int port, bool useTls)
    {
        if (string.IsNullOrWhiteSpace(host)) throw new ArgumentNullException(nameof(host));

        var options = new GrpcChannelOptions
        {
            Credentials = useTls ? ChannelCredentials.SecureSsl : ChannelCredentials.Insecure,
        };

        var scheme = useTls ? "https" : "http";
        return GrpcChannel.ForAddress(new Uri($"{scheme}://{host}:{port}"), options);
    }
}
