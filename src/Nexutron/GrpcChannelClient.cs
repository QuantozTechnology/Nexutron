using System;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Nexutron.Helpers;

namespace Nexutron;

/// <summary>
/// Owns the gRPC channels for the configured full node and solidity node. Channels are created lazily
/// once and reused, as recommended for <see cref="GrpcChannel"/>.
/// </summary>
sealed class GrpcChannelClient : IGrpcChannelClient, IDisposable
{
    private readonly Lazy<GrpcChannel> _channel;
    private readonly Lazy<GrpcChannel> _solidityChannel;

    public GrpcChannelClient(IOptions<NexutronOptions> options)
    {
        _channel = new Lazy<GrpcChannel>(() => GrpcChannelClientHelpers.CreateChannel(
            options.Value.Channel ?? throw new InvalidOperationException($"{nameof(NexutronOptions)}.{nameof(NexutronOptions.Channel)} is not configured.")));
        _solidityChannel = new Lazy<GrpcChannel>(() => GrpcChannelClientHelpers.CreateChannel(
            options.Value.SolidityChannel ?? throw new InvalidOperationException($"{nameof(NexutronOptions)}.{nameof(NexutronOptions.SolidityChannel)} is not configured.")));
    }

    public GrpcChannel GetProtocol() => _channel.Value;

    public GrpcChannel GetSolidityProtocol() => _solidityChannel.Value;

    public void Dispose()
    {
        if (_channel.IsValueCreated) _channel.Value.Dispose();
        if (_solidityChannel.IsValueCreated) _solidityChannel.Value.Dispose();
    }
}
