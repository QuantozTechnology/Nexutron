using System;
using Nethereum.Signer;
using Nethereum.Signer.Crypto;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

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
    private static readonly SecureRandom SecureRandom = new SecureRandom();

    public static TronECKey GenerateKey(TronNetwork network)
    {
        var gen = new ECKeyPairGenerator("EC");
        var keyGenParam = new KeyGenerationParameters(SecureRandom, 256);
        gen.Init(keyGenParam);
        var keyPair = gen.GenerateKeyPair();
        var privateBytes = ((ECPrivateKeyParameters)keyPair.Private).D.ToByteArrayUnsigned();
        if (privateBytes.Length != 32)
            return GenerateKey(network);
        return new TronECKey(privateBytes, true, network);
    }
}

// public static class TronECKey : ECKey
// {
//     public ECKey GenerateTronECKey()
//     {
//         var generator = new ECKeyPairGenerator("EC");
//         generator.Init(new ECKeyGenerationParameters(CURVE, _secureRandom));
//         var pair = generator.GenerateKeyPair();
//         return new ECKey(pair.Private, true);
//     }
// }
