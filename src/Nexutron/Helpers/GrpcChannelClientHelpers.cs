using System;
using Grpc.Core;
using Grpc.Net.Client;

namespace Nexutron.Helpers;

public static class GrpcChannelClientHelpers
{
    public static GrpcChannel GetProtocol(string host, int port)
    {
        var options = new GrpcChannelOptions
        {
            Credentials = ChannelCredentials.SecureSsl
        };

        return GrpcChannel.ForAddress(new Uri($"https://{host}:{port}"), options);
    }

    public static GrpcChannel GetSolidityProtocol(string host, int port)
    {
        var options = new GrpcChannelOptions
        {
            Credentials = ChannelCredentials.SecureSsl
        };

        return GrpcChannel.ForAddress(new Uri($"https://{host}:{port}"), options);
    }
}