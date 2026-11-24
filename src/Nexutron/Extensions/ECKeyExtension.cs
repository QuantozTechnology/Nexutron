using System;
using Nethereum.Signer;
using Nethereum.Signer.Crypto;

namespace Nexutron.Extensions;

public static class ECKeyExtension
{
    public static byte[] GetPubKeyNoPrefix(this ECKey key)
    {
        var pubKey = key.GetPubKey(false);
        var arr = new byte[pubKey.Length - 1];
        //remove the prefix
        Array.Copy(pubKey, 1, arr, 0, arr.Length);
        return arr;
    }
}

public static class TronECKeyGenerator
{
    /// <summary>
    /// Generates a new random secp256k1 key pair (Tron uses the same curve as Ethereum).
    /// </summary>
    public static TronECKey GenerateKey()
    {
        var ethKey = EthECKey.GenerateKey();
        return new TronECKey(ethKey.GetPrivateKeyAsBytes(), true);
    }
}
