using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nethereum.Signer;
using Nexutron.Crypto;
using Nexutron.Helpers;
using Nexutron.Protocol;

namespace Nexutron.Tests;

/// <summary>
/// A minimal in-process Tron full node speaking the real gRPC protocol over plaintext HTTP/2 (like TronGrid's
/// public gRPC endpoints). It verifies signatures on broadcast and keeps state for a single TRC20 token.
/// </summary>
public sealed class FakeTronNode : Wallet.WalletBase, IAsyncDisposable
{
    public const string TokenAddress = "TXLAQ63Xg1NAzckPwKHvzw7CSEmLMEqcdj";

    private WebApplication _app;

    public int TokenDecimals { get; set; } = 6;
    public ConcurrentDictionary<string, BigInteger> TokenBalances { get; } = new();
    public ConcurrentQueue<Transaction> Broadcasts { get; } = new();
    public ConcurrentQueue<string> ApiKeysSeen { get; } = new();
    public long BlockNumber { get; } = 4_242_424;
    public int Port { get; private set; }

    public static async Task<FakeTronNode> StartAsync()
    {
        var node = new FakeTronNode();
        var builder = WebApplication.CreateSlimBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.ConfigureKestrel(k => k.Listen(System.Net.IPAddress.Loopback, 0, o => o.Protocols = HttpProtocols.Http2));
        builder.Services.AddGrpc();
        builder.Services.AddSingleton(node);
        node._app = builder.Build();
        node._app.MapGrpcService<FakeTronNode>();
        await node._app.StartAsync();

        var address = node._app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.First();
        node.Port = new Uri(address).Port;
        return node;
    }

    public async ValueTask DisposeAsync()
    {
        if (_app != null) await _app.DisposeAsync();
    }

    public static string RecoverAddress(byte[] hash, byte[] signature)
    {
        var sig = EthECDSASignatureFactory.FromComponents(signature[..32], signature[32..64], signature[64]);
        var key = EthECKey.RecoverFromSignature(sig, hash);
        var pubKeyHash = key.GetPubKeyNoPrefix().ToKeccakHash();
        return Base58Encoder.EncodeFromHex(pubKeyHash[12..], 0x41);
    }

    private void RecordApiKey(ServerCallContext context)
    {
        ApiKeysSeen.Enqueue(context.RequestHeaders.GetValue(WalletHelper.ApiKeyHeader.ToLowerInvariant()));
    }

    private BlockExtention CurrentBlock() => new()
    {
        BlockHeader = new BlockHeader
        {
            RawData = new BlockHeader.Types.raw { Number = BlockNumber, Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
        },
    };

    private static byte[] Word(BigInteger value)
    {
        var bytes = value.ToByteArray(isUnsigned: true, isBigEndian: true);
        var word = new byte[32];
        bytes.CopyTo(word, 32 - bytes.Length);
        return word;
    }

    public static byte[] RevertReason(string reason)
    {
        var text = System.Text.Encoding.UTF8.GetBytes(reason);
        var padded = new byte[(text.Length + 31) / 32 * 32];
        text.CopyTo(padded, 0);
        return new byte[] { 0x08, 0xc3, 0x79, 0xa0 }.Concat(Word(32)).Concat(Word(text.Length)).Concat(padded).ToArray();
    }

    private static string AbiAddressToBase58(byte[] data, int offset) =>
        Base58Encoder.EncodeFromHex(data[(offset + 12)..(offset + 32)], 0x41);

    public override Task<BlockExtention> GetNowBlock2(EmptyMessage request, ServerCallContext context)
    {
        RecordApiKey(context);
        return Task.FromResult(CurrentBlock());
    }

    public override Task<TransactionExtention> TriggerConstantContract(TriggerSmartContract request, ServerCallContext context)
    {
        RecordApiKey(context);
        var data = request.Data.ToByteArray();
        var selector = data[..4].ToHex();
        var result = new TransactionExtention { Result = new Return { Result = true } };

        switch (selector)
        {
            case "313ce567": // decimals()
                result.ConstantResult.Add(ByteString.CopyFrom(Word(TokenDecimals)));
                break;
            case "70a08231": // balanceOf(address)
                result.ConstantResult.Add(ByteString.CopyFrom(Word(TokenBalances.GetValueOrDefault(AbiAddressToBase58(data, 4)))));
                break;
            case "a9059cbb": // transfer(address,uint256) dry-run; a revert looks like java-tron's response
                var amount = new BigInteger(data[36..68], isUnsigned: true, isBigEndian: true);
                result.Transaction = new Transaction();
                if (TokenBalances.GetValueOrDefault(AccountHelper.GetBase58Address(request.OwnerAddress)) < amount)
                {
                    result.Transaction.Ret.Add(new Transaction.Types.Result { Ret = Transaction.Types.Result.Types.code.Failed });
                    result.Result.Message = ByteString.CopyFromUtf8("REVERT opcode executed");
                    result.ConstantResult.Add(ByteString.CopyFrom(RevertReason("insufficient balance")));
                }
                else
                {
                    result.ConstantResult.Add(ByteString.CopyFrom(Word(1)));
                }
                break;
            default:
                result.Result = new Return { Result = false, Code = Return.Types.response_code.ContractValidateError, Message = ByteString.CopyFromUtf8("REVERT") };
                break;
        }

        return Task.FromResult(result);
    }

    public override Task<TransactionExtention> TriggerContract(TriggerSmartContract request, ServerCallContext context)
    {
        RecordApiKey(context);
        var tx = new Transaction { RawData = new Transaction.Types.raw() };
        tx.RawData.Contract.Add(new Transaction.Types.Contract
        {
            Type = Transaction.Types.Contract.Types.ContractType.TriggerSmartContract,
            Parameter = Any.Pack(request),
        });
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        tx.RawData.Timestamp = now;
        tx.RawData.Expiration = now + 60_000;
        TransactionHelper.SetReferenceBlock(tx, CurrentBlock());

        return Task.FromResult(new TransactionExtention
        {
            Transaction = tx,
            Txid = ByteString.CopyFrom(tx.RawData.ToByteArray().ToSHA256Hash()),
            Result = new Return { Result = true, Code = Return.Types.response_code.Success },
        });
    }

    public override Task<Return> BroadcastTransaction(Transaction request, ServerCallContext context)
    {
        RecordApiKey(context);
        if (request.Signature.Count != 1)
            return Task.FromResult(Fail(Return.Types.response_code.Sigerror, "missing signature"));

        var contract = request.RawData.Contract[0];
        var owner = contract.Type switch
        {
            Transaction.Types.Contract.Types.ContractType.TransferContract => contract.Parameter.Unpack<TransferContract>().OwnerAddress,
            Transaction.Types.Contract.Types.ContractType.TriggerSmartContract => contract.Parameter.Unpack<TriggerSmartContract>().OwnerAddress,
            _ => null,
        };
        if (owner == null)
            return Task.FromResult(Fail(Return.Types.response_code.ContractValidateError, "unsupported contract"));

        var signer = RecoverAddress(request.RawData.ToByteArray().ToSHA256Hash(), request.Signature[0].ToByteArray());
        if (signer != AccountHelper.GetBase58Address(owner))
            return Task.FromResult(Fail(Return.Types.response_code.Sigerror, "signature does not match owner"));

        if (contract.Type == Transaction.Types.Contract.Types.ContractType.TriggerSmartContract)
        {
            var trigger = contract.Parameter.Unpack<TriggerSmartContract>();
            var data = trigger.Data.ToByteArray();
            if (data[..4].ToHex() != "a9059cbb")
                return Task.FromResult(Fail(Return.Types.response_code.ContractValidateError, "unsupported method"));

            var from = AccountHelper.GetBase58Address(owner);
            var to = AbiAddressToBase58(data, 4);
            var amount = new BigInteger(data[36..68], isUnsigned: true, isBigEndian: true);
            if (TokenBalances.GetValueOrDefault(from) < amount)
                return Task.FromResult(Fail(Return.Types.response_code.ContractValidateError, "insufficient balance"));

            TokenBalances.AddOrUpdate(from, _ => -amount, (_, b) => b - amount);
            TokenBalances.AddOrUpdate(to, _ => amount, (_, b) => b + amount);
        }

        Broadcasts.Enqueue(request);
        return Task.FromResult(new Return { Result = true, Code = Return.Types.response_code.Success });
    }

    private static Return Fail(Return.Types.response_code code, string message) =>
        new() { Result = false, Code = code, Message = ByteString.CopyFromUtf8(message) };
}
