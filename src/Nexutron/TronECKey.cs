using System;
using Nethereum.Signer.Crypto;
using Nexutron.Crypto;
using Nexutron.Extensions;

namespace Nexutron
{
    public class TronECKey
    {
        private readonly ECKey _ecKey;
        private string _publicAddress = null;
        private string _privateKeyHex = null;
        public TronECKey(string privateKey)
        {
            _ecKey = new ECKey(privateKey.HexToByteArray(), true);
        }

        public TronECKey(byte[] vch, bool isPrivate)
        {
            _ecKey = new ECKey(vch, isPrivate);
        }

        internal TronECKey(ECKey ecKey)
        {
            _ecKey = ecKey;
        }

        // internal TronECKey(TronNetwork network)
        // {
        //     _ecKey = new ECKey();
        //     _network = network;
        // }

        // public static TronECKey GenerateKey(TronNetwork network)
        // {
        //     return new TronECKey(network);
        // }

        internal static byte GetPublicAddressPrefix()
        {
            return 0x41;
        }

        public static string GetPublicAddress(string privateKey)
        {
            var key = new TronECKey(privateKey.HexToByteArray(), true);

            return key.GetPublicAddress();
        }

        public string GetPublicAddress()
        {
            if (!string.IsNullOrWhiteSpace(_publicAddress)) return _publicAddress;

            var initaddr = _ecKey.GetPubKeyNoPrefix().ToKeccakHash();
            var address = new byte[initaddr.Length - 11];
            Array.Copy(initaddr, 12, address, 1, 20);
            address[0] = GetPublicAddressPrefix();

            var hash = Base58Encoder.TwiceHash(address);
            var bytes = new byte[4];
            Array.Copy(hash, bytes, 4);
            var addressChecksum = new byte[25];
            Array.Copy(address, 0, addressChecksum, 0, 21);
            Array.Copy(bytes, 0, addressChecksum, 21, 4);

            _publicAddress = Base58Encoder.Encode(addressChecksum);

            return _publicAddress;
        }
        public string GetPrivateKey()
        {
            if (string.IsNullOrWhiteSpace(_privateKeyHex))
            {
                _privateKeyHex = _ecKey.PrivateKey.D.ToByteArrayUnsigned().ToHex();
            }
            return _privateKeyHex;
        }

        public byte[] GetPubKey()
        {
            return _ecKey.GetPubKey(true);
        }
    }
}
