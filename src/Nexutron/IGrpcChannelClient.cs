using Grpc.Net.Client;

namespace Nexutron;

public interface IGrpcChannelClient
{
    GrpcChannel GetProtocol();
    GrpcChannel GetSolidityProtocol();
}