using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Nexutron.Contracts;
using Nexutron.Extensions;
using Nexutron.Protocol;
using Xunit;

namespace Nexutron.Tests;

/// <summary>
/// End-to-end tests through the DI registration, gRPC channel and protocol stubs against <see cref="FakeTronNode"/>.
/// </summary>
public class IntegrationTests : IAsyncLifetime
{
    private FakeTronNode _node;
    private ServiceProvider _services;

    public async Task InitializeAsync()
    {
        _node = await FakeTronNode.StartAsync();

        var services = new ServiceCollection();
        services.AddTronDotNet(x =>
        {
            x.Channel = new GrpcChannelOption { Host = "localhost", Port = _node.Port, UseTls = false };
            x.SolidityChannel = new GrpcChannelOption { Host = "localhost", Port = _node.Port, UseTls = false };
            x.ApiKey = "test-api-key";
        });
        _services = services.BuildServiceProvider();
    }

    public async Task DisposeAsync()
    {
        await _services.DisposeAsync();
        await _node.DisposeAsync();
    }

    [Fact]
    public async Task Trx_transfer_create_sign_broadcast()
    {
        var transactions = _services.GetRequiredService<ITransactionClient>();

        var created = await transactions.CreateTransactionAsync(TestVectors.Address2, TestVectors.ToAddress, TronUnit.TRXToSun(2), "invoice 42");
        Assert.True(created.Result.Result);
        Assert.Equal(created.Transaction.GetTxid(), created.Txid.ToByteArray().ToHex());

        var signed = transactions.GetTransactionSign(created.Transaction, TestVectors.PrivateKey2);
        var result = await transactions.BroadcastTransactionAsync(signed);

        Assert.True(result.Result, result.Message.ToStringUtf8());
        var broadcast = Assert.Single(_node.Broadcasts);
        var transfer = broadcast.RawData.Contract[0].Parameter.Unpack<TransferContract>();
        Assert.Equal(2_000_000, transfer.Amount);
        Assert.Equal("invoice 42", broadcast.RawData.Data.ToStringUtf8());
        Assert.Equal(
            BitConverter.GetBytes(_node.BlockNumber).Reverse().Skip(6).ToArray().ToHex(),
            broadcast.RawData.RefBlockBytes.ToByteArray().ToHex());
        Assert.All(_node.ApiKeysSeen, k => Assert.Equal("test-api-key", k));
    }

    [Fact]
    public async Task Broadcast_signed_with_the_wrong_key_is_rejected()
    {
        var transactions = _services.GetRequiredService<ITransactionClient>();
        var created = await transactions.CreateTransactionAsync(TestVectors.Address2, TestVectors.ToAddress, 1);

        var signed = transactions.GetTransactionSign(created.Transaction, TestVectors.PrivateKey1);
        var result = await transactions.BroadcastTransactionAsync(signed);

        Assert.False(result.Result);
        Assert.Equal(Return.Types.response_code.Sigerror, result.Code);
    }

    [Fact]
    public async Task Trc20_balance_and_transfer()
    {
        var wallet = _services.GetRequiredService<IWalletClient>();
        var trc20 = _services.GetRequiredService<IContractClientFactory>().CreateClient(ContractProtocol.TRC20);
        var owner = wallet.GetAccount(TestVectors.PrivateKey2);
        var recipient = wallet.GetAccount(TestVectors.PrivateKey1);

        _node.TokenBalances[owner.Address] = TokenAmount.ToRaw(100m, _node.TokenDecimals);

        Assert.Equal(100m, await trc20.BalanceOfAsync(FakeTronNode.TokenAddress, owner));
        Assert.Equal(0m, await trc20.BalanceOfAsync(FakeTronNode.TokenAddress, recipient));

        var txid = await trc20.TransferAsync(FakeTronNode.TokenAddress, owner, recipient.Address, 12.345678m, "memo", 10_000_000);

        Assert.NotNull(txid);
        var broadcast = Assert.Single(_node.Broadcasts);
        Assert.Equal(txid, broadcast.GetTxid());
        Assert.Equal(10_000_000, broadcast.RawData.FeeLimit);
        Assert.Equal("memo", broadcast.RawData.Data.ToStringUtf8());
        Assert.Equal(87.654322m, await trc20.BalanceOfAsync(FakeTronNode.TokenAddress, owner));
        Assert.Equal(12.345678m, await trc20.BalanceOfAsync(FakeTronNode.TokenAddress, recipient));
    }

    [Fact]
    public async Task Trc20_balance_with_18_decimals_beyond_long_range()
    {
        _node.TokenDecimals = 18;
        var trc20 = _services.GetRequiredService<ITronClient>().GetContract();
        var owner = _services.GetRequiredService<IWalletClient>().GetAccount(TestVectors.PrivateKey2);
        _node.TokenBalances[owner.Address] = BigInteger.Parse("123456789012345678901234567"); // 123,456,789.0123...

        Assert.Equal(123456789.012345678901234567m, await trc20.BalanceOfAsync(FakeTronNode.TokenAddress, owner));
    }

    [Fact]
    public async Task Trc20_transfer_with_insufficient_balance_returns_null()
    {
        var trc20 = _services.GetRequiredService<ITronClient>().GetContract();
        var owner = _services.GetRequiredService<IWalletClient>().GetAccount(TestVectors.PrivateKey2);

        var txid = await trc20.TransferAsync(FakeTronNode.TokenAddress, owner, TestVectors.ToAddress, 1m, null, 10_000_000);

        Assert.Null(txid);
        Assert.Empty(_node.Broadcasts);
    }

    [Fact]
    public void Channel_is_reused_across_clients()
    {
        var a = _services.GetRequiredService<IWalletClient>();
        var b = _services.GetRequiredService<IWalletClient>();
        var channels = _services.GetRequiredService<IGrpcChannelClient>();
        Assert.Same(channels.GetProtocol(), _services.GetRequiredService<IGrpcChannelClient>().GetProtocol());
        Assert.NotSame(a, b);
    }

    [Fact]
    public async Task Works_without_an_api_key()
    {
        var services = new ServiceCollection();
        services.AddTronDotNet(x =>
        {
            x.Channel = new GrpcChannelOption { Host = "localhost", Port = _node.Port, UseTls = false };
            x.SolidityChannel = x.Channel;
        });
        await using var provider = services.BuildServiceProvider();

        var created = await provider.GetRequiredService<ITransactionClient>().CreateTransactionAsync(TestVectors.Address2, TestVectors.ToAddress, 1);

        Assert.True(created.Result.Result);
        Assert.Null(_node.ApiKeysSeen.Last());
    }
}
