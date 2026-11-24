using System;
using System.Linq;
using System.Text;
using Google.Protobuf;
using Nexutron.Crypto;
using Nexutron.Extensions;
using Nexutron.Helpers;
using Nexutron.Protocol;
using Xunit;

namespace Nexutron.Tests;

public class TransactionTests
{
    private static Transaction ReferenceTransaction()
    {
        var block = new BlockExtention { BlockHeader = new BlockHeader { RawData = new BlockHeader.Types.raw { Number = 0x1234 } } };
        var tx = TransactionHelper.CreateTransaction(block, TestVectors.Address2, TestVectors.ToAddress, 2_000_000,
            DateTimeOffset.FromUnixTimeMilliseconds(1700000000000).UtcDateTime, 600_000, "hello tron");

        // Pin the ref block hash to the reference value (it is derived from the block header hash).
        tx.RawData.RefBlockHash = ByteString.CopyFrom("0102030405060708".HexToByteArray());
        return tx;
    }

    [Fact]
    public void Builds_trx_transfer_identical_to_tronweb()
    {
        var tx = ReferenceTransaction();

        Assert.Equal(TestVectors.TransferRawDataHex, tx.RawData.ToByteArray().ToHex());
        Assert.Equal(TestVectors.TransferTxId, tx.GetTxid());
    }

    [Fact]
    public void Memo_is_utf8()
    {
        var tx = ReferenceTransaction();
        Assert.Equal("hello tron", tx.RawData.Data.ToStringUtf8());
    }

    [Fact]
    public void Reference_block_fields_come_from_the_block_header()
    {
        var header = new BlockHeader.Types.raw { Number = 0x0102030405060708, Timestamp = 42 };
        var block = new BlockExtention { BlockHeader = new BlockHeader { RawData = header } };

        var tx = TransactionHelper.CreateTransaction(block, TestVectors.Address2, TestVectors.ToAddress, 1, DateTime.UtcNow, 60_000);

        Assert.Equal("0708", tx.RawData.RefBlockBytes.ToByteArray().ToHex());
        Assert.Equal(header.ToByteArray().ToSHA256Hash().Skip(8).Take(8).ToArray().ToHex(), tx.RawData.RefBlockHash.ToByteArray().ToHex());
        Assert.Equal(60_000, tx.RawData.Expiration - tx.RawData.Timestamp);
        Assert.True(tx.RawData.Data.IsEmpty);
    }

    [Fact]
    public void Signature_is_identical_to_tronweb()
    {
        var tx = ReferenceTransaction();

        var signed = TransactionHelper.SignTransaction(tx, TestVectors.PrivateKey2);

        Assert.Single(signed.Signature);
        Assert.Equal(TestVectors.TransferSignature, signed.Signature[0].ToByteArray().ToHex());
        // Signing returns a copy and leaves the input untouched; the txid does not change.
        Assert.Empty(tx.Signature);
        Assert.Equal(TestVectors.TransferTxId, signed.GetTxid());
    }

    [Fact]
    public void Signature_recovers_to_the_signer_for_random_keys()
    {
        for (var i = 0; i < 25; i++)
        {
            var key = TronECKeyGenerator.GenerateKey();
            var tx = ReferenceTransaction();
            tx.RawData.Timestamp += i;

            var sig = TransactionHelper.SignTransaction(tx, key.GetPrivateKey()).Signature[0].ToByteArray();

            Assert.Equal(65, sig.Length);
            Assert.Contains(sig[64], new byte[] { 27, 28 });
            Assert.Equal(key.GetPublicAddress(), FakeTronNode.RecoverAddress(tx.RawData.ToByteArray().ToSHA256Hash(), sig));
        }
    }
}
