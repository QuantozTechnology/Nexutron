using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexutron.Helpers
{
    public static class GrpcChannelClientHelpers
    {
        public static Channel GetProtocol(string host, int port)
        {
            return new Channel(host, port, ChannelCredentials.Insecure);
        }
        public static Channel GetSolidityProtocol(string host, int port)
        {
            return new Channel(host, port, ChannelCredentials.Insecure);
        }
    }
}
