using Nexutron.Protocol;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;

namespace Nexutron.Helpers;

public static class WalletClientHelper
{
    public static Wallet.WalletClient GetWalletClient(string host, int port)
    {
        return new Wallet.WalletClient(GrpcChannelClientHelpers.GetProtocol(host, port));
    }

    public static Wallet.WalletClient GetWalletClient(GrpcChannel channel)
    {
        return new Wallet.WalletClient(channel);
    }

    public static WalletSolidity.WalletSolidityClient GetSolidityClient(string host, int port)
    {
        return new WalletSolidity.WalletSolidityClient(GrpcChannelClientHelpers.GetSolidityProtocol(host, port));
    }

    public static WalletSolidity.WalletSolidityClient GetSolidityClient(GrpcChannel channel)
    {
        return new WalletSolidity.WalletSolidityClient(channel);
    }

    public static EmptyMessage GetEmptyMessage()
    {
        return new EmptyMessage();
    }

    public static NumberMessage GetNumberMessage(long number)
    {
        return new NumberMessage { Num = number };
    }

    public static async Task<BlockExtention> GetBlockExtention(Wallet.WalletClient wallet, string apiKey)
    {
        return await wallet.GetNowBlock2Async(new EmptyMessage(), headers: WalletHelper.GetHeaders(apiKey));
    }

    public static async Task<BlockExtention> GetBlockExtention(Wallet.WalletClient wallet, Metadata headers)
    {
        return await wallet.GetNowBlock2Async(new EmptyMessage(), headers: headers);
    }

    public static async Task<Return> BroadcastTransactionAsync(Wallet.WalletClient wallet, Transaction transaction, string apiKey)
    {
        return await wallet.BroadcastTransactionAsync(transaction, headers: WalletHelper.GetHeaders(apiKey));
    }

    public static async Task<Return> BroadcastTransactionAsync(Wallet.WalletClient wallet, Transaction transaction, Metadata headers)
    {
        return await wallet.BroadcastTransactionAsync(transaction, headers: headers);
    }
}
