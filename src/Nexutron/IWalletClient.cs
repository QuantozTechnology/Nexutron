using Google.Protobuf;
using Grpc.Core;
using Nexutron.Accounts;
using Nexutron.Protocol;

namespace Nexutron
{
    public interface IWalletClient
    {
        Wallet.WalletClient GetProtocol();
        WalletSolidity.WalletSolidityClient GetSolidityProtocol();
        ITronAccount GenerateAccount();
        ITronAccount GetAccount(string privateKey);
        ByteString ParseAddress(string address);

        Metadata GetHeaders();
    }
}
