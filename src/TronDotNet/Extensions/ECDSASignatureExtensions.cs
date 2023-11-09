using System;
using Nethereum.Signer.Crypto;
using Org.BouncyCastle.Math;

namespace TronDotNet.Extensions;

public static class ECDSASignatureExtensions
{
    public static byte[] ToByteArray(this ECDSASignature signature)
    {
        return ByteArrary.Merge(BigIntegerToBytes(signature.R, 32), BigIntegerToBytes(signature.S, 32), signature.V);
    }

    private static byte[] BigIntegerToBytes(BigInteger b, int numBytes)
    {
        if (b == null) return null;
        var bytes = new byte[numBytes];
        var biBytes = b.ToByteArray();
        var start = (biBytes.Length == numBytes + 1) ? 1 : 0;
        var length = Math.Min(biBytes.Length, numBytes);
        Array.Copy(biBytes, start, bytes, numBytes - length, length);

        return bytes;
    }
}