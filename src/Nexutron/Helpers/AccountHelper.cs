using Google.Protobuf;
using Nexutron.Accounts;
using Nexutron.Crypto;
using Nexutron.Extensions;
using System;

namespace Nexutron.Helpers
{
    public static class AccountHelper
    {
        public static ITronAccount GenerateAccount(TronNetwork network)
        {
            var tronKey = TronECKeyGenerator.GenerateKey(network);
            return new TronAccount(tronKey);
        }

        public static ITronAccount GetAccount(string privateKey, TronNetwork network)
        {
            return new TronAccount(privateKey, network);
        }

        public static ByteString ParseAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) throw new ArgumentNullException(nameof(address));

            byte[] raw;
            if (address.StartsWith("T"))
            {
                raw = Base58Encoder.DecodeFromBase58Check(address);
            }
            else if (address.StartsWith("41"))
            {
                raw = address.HexToByteArray();
            }
            else if (address.StartsWith("0x"))
            {
                raw = address[2..].HexToByteArray();
            }
            else
            {
                try
                {
                    raw = address.HexToByteArray();
                }
                catch (Exception)
                {
                    throw new ArgumentException($"Invalid address: " + address);
                }
            }
            return ByteString.CopyFrom(raw);
        }

        public static string GetBase58Address(ByteString address)
        {
            return Base58Encoder.EncodeFromHex(address.ToByteArray().ToHex(), 0x41);
        }

        public static string GetBase58Address(string address)
        {
            return Base58Encoder.EncodeFromHex(ParseAddress(address).ToByteArray().ToHex(), 0x41);
        }

        public static string GetHexAddress(ByteString address)
        {
            return address.ToByteArray().ToHex();
        }
    }
}
