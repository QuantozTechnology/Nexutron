namespace Nexutron
{
    public class GrpcChannelOption
    {
        public string Host { get; set; } = "grpc.shasta.trongrid.io";
        public int Port { get; set; } = 50051;

        /// <summary>
        /// Connect over TLS (https). The public TronGrid gRPC endpoints (e.g. grpc.trongrid.io:50051,
        /// grpc.shasta.trongrid.io:50051, grpc.nile.trongrid.io:50051) are plaintext, so set this to
        /// <c>false</c> for those. Defaults to <c>true</c> for backwards compatibility.
        /// </summary>
        public bool UseTls { get; set; } = true;
    }
}
