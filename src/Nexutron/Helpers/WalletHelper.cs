using Grpc.Core;

namespace Nexutron.Helpers;

public static class WalletHelper
{
    public const string ApiKeyHeader = "TRON-PRO-API-KEY";

    /// <summary>
    /// Builds the gRPC headers for a request. The TronGrid API key header is only added when a key is set.
    /// </summary>
    public static Metadata GetHeaders(string apiKey)
    {
        var headers = new Metadata();
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            headers.Add(ApiKeyHeader, apiKey);
        }

        return headers;
    }
}
