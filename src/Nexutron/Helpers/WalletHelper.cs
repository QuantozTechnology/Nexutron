using Google.Protobuf;
using Grpc.Core;
using Nexutron.Crypto;
using System;

namespace Nexutron.Helpers
{
    public static class WalletHelper
    {
        public static Metadata GetHeaders(string apiKey)
        {
            var headers = new Metadata
            {
                { "TRON-PRO-API-KEY", apiKey }
            };

            return headers;
        }
    }
}
