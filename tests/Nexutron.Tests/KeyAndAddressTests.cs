using Nexutron.Accounts;
using Nexutron.Crypto;
using Nexutron.Extensions;
using Nexutron.Helpers;
using Xunit;

namespace Nexutron.Tests;

public class KeyAndAddressTests
{
    [Theory]
    [InlineData(TestVectors.PrivateKey1, TestVectors.Address1)]
    [InlineData(TestVectors.PrivateKey2, TestVectors.Address2)]
    [InlineData(TestVectors.PrivateKeyLeadingZeros, TestVectors.AddressLeadingZeros)]
    public void Derives_address_from_private_key(string privateKey, string expectedAddress)
    {
        Assert.Equal(expectedAddress, new TronECKey(privateKey).GetPublicAddress());
        Assert.Equal(expectedAddress, TronECKey.GetPublicAddress(privateKey));
        Assert.Equal(expectedAddress, new TronAccount(privateKey).Address);
    }

    [Fact]
    public void Accepts_0x_prefixed_private_key()
    {
        Assert.Equal(TestVectors.Address1, new TronECKey("0x" + TestVectors.PrivateKey1).GetPublicAddress());
    }

    [Fact]
    public void Private_key_round_trips_as_64_hex_chars_even_with_leading_zeros()
    {
        var key = new TronECKey(TestVectors.PrivateKeyLeadingZeros);
        Assert.Equal(TestVectors.PrivateKeyLeadingZeros, key.GetPrivateKey());
        Assert.Equal(TestVectors.PrivateKeyLeadingZeros, new TronAccount(key).PrivateKey);
    }

    [Fact]
    public void Generated_keys_are_valid_and_unique()
    {
        var a = TronECKeyGenerator.GenerateKey();
        var b = AccountHelper.GenerateAccount();

        Assert.NotEqual(a.GetPrivateKey(), b.PrivateKey);
        foreach (var (privateKey, address) in new[] { (a.GetPrivateKey(), a.GetPublicAddress()), (b.PrivateKey, b.Address) })
        {
            Assert.Equal(64, privateKey.Length);
            Assert.StartsWith("T", address);
            Assert.Equal(34, address.Length);
            Assert.NotNull(Base58Encoder.DecodeFromBase58Check(address));
            // Re-deriving from the exported private key gives the same address.
            Assert.Equal(address, TronECKey.GetPublicAddress(privateKey));
        }
    }

    [Fact]
    public void Parses_base58_and_hex_addresses_to_the_same_bytes()
    {
        var fromBase58 = AccountHelper.ParseAddress(TestVectors.ToAddress).ToByteArray().ToHex();
        var fromHex = AccountHelper.ParseAddress(TestVectors.ToAddressHex).ToByteArray().ToHex();

        Assert.Equal(TestVectors.ToAddressHex, fromBase58);
        Assert.Equal(TestVectors.ToAddressHex, fromHex);
        Assert.Equal(TestVectors.ToAddress, AccountHelper.GetBase58Address(TestVectors.ToAddressHex));
        Assert.Equal(TestVectors.ToAddressHex, AccountHelper.GetHexAddress(TestVectors.ToAddress));
    }

    [Fact]
    public void Base58Check_rejects_a_bad_checksum()
    {
        var corrupted = TestVectors.ToAddress[..^1] + (TestVectors.ToAddress[^1] == 'j' ? 'k' : 'j');
        Assert.Null(Base58Encoder.DecodeFromBase58Check(corrupted));
    }
}
