using Grpc.Core;
using Nexutron.Helpers;

namespace Nexutron
{
    public class NexutronOptions
    {
        public GrpcChannelOption Channel { get; set; }

        public GrpcChannelOption SolidityChannel { get; set; }

        public string ApiKey { get; set; }

        internal Metadata GetgRPCHeaders()
        {
            return WalletHelper.GetHeaders(ApiKey);
        }
    }
}
