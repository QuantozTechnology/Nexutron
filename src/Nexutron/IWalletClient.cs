using Google.Protobuf;
using Grpc.Core;
using Nexutron.Accounts;
using Nexutron.Protocol;

namespace Nexutron
{
    public interface IWalletClient
    {
        Wallet.WalletClient GetWalletClient();
        WalletSolidity.WalletSolidityClient GetSolidityClient();
        ITronAccount GenerateAccount();
        ITronAccount GetAccount(string privateKey);
        ByteString ParseAddress(string address);

        Metadata GetHeaders();
    }
}
