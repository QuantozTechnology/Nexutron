using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Nexutron.Crypto;
using Nexutron.Extensions;
using Nexutron.Helpers;
using Nexutron.Protocol;
using Xunit;

namespace Nexutron.LocalNode.Tests;

/// <summary>
/// Connects the SDK to the local node and deploys one TRC20 token (owned by the genesis-funded account) for all tests.
/// </summary>
public sealed class LocalNodeFixture : IAsyncLifetime
{
    // Genesis-funded test account from tests/local-node/private.conf (test key, never use elsewhere).
    public const string FundedPrivateKey = "62075119d64f17ebd3248df7a864cd84380fcb9e5771f0968af2167a25717bb2";
    public const string FundedAddress = "THe8MeboLUbvi1TYntzPKK8SsfcCiJCZ19";
    public const int TokenDecimals = 6;
    public static readonly BigInteger TokenSupply = BigInteger.Pow(10, 6 + TokenDecimals); // 1,000,000 tokens

    public ServiceProvider Services { get; private set; }
    public string TokenAddress { get; private set; }

    public IWalletClient Wallet => Services.GetRequiredService<IWalletClient>();
    public ITransactionClient Transactions => Services.GetRequiredService<ITransactionClient>();

    public async Task InitializeAsync()
    {
        var host = Environment.GetEnvironmentVariable("NEXUTRON_LOCAL_NODE");
        if (string.IsNullOrWhiteSpace(host)) return;

        var services = new ServiceCollection();
        services.AddNexutron(x =>
        {
            x.Channel = new GrpcChannelOption { Host = host, Port = 50051, UseTls = false };
            x.SolidityChannel = new GrpcChannelOption { Host = host, Port = 50061, UseTls = false };
        });
        Services = services.BuildServiceProvider();

        TokenAddress = await DeployTokenAsync();
    }

    public async Task DisposeAsync()
    {
        if (Services != null) await Services.DisposeAsync();
    }

    private async Task<string> DeployTokenAsync()
    {
        var json = JsonDocument.Parse(await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Token.json")));
        var bytecode = json.RootElement.GetProperty("bytecode").GetString();

        // constructor(uint256 supply)
        var supplyWord = TokenSupply.ToByteArray(isUnsigned: true, isBigEndian: true);
        var constructorArgs = new byte[32];
        supplyWord.CopyTo(constructorArgs, 32 - supplyWord.Length);

        var owner = AccountHelper.ParseAddress(FundedAddress);
        var deploy = new CreateSmartContract
        {
            OwnerAddress = owner,
            NewContract = new SmartContract
            {
                OriginAddress = owner,
                Name = "LocalTestUSD",
                Bytecode = ByteString.CopyFrom(bytecode.HexToByteArray().Concat(constructorArgs).ToArray()),
                ConsumeUserResourcePercent = 100,
                OriginEnergyLimit = 10_000_000,
            },
        };

        var created = await Wallet.GetWalletClient().DeployContractAsync(deploy, headers: Wallet.GetHeaders());
        Assert.True(created.Result.Result, created.Result.Message.ToStringUtf8());

        var tx = created.Transaction;
        tx.RawData.FeeLimit = TronUnit.TRXToSun(1000);
        var signed = Transactions.GetTransactionSign(tx, FundedPrivateKey);
        var broadcast = await Transactions.BroadcastTransactionAsync(signed);
        Assert.True(broadcast.Result, $"{broadcast.Code} {broadcast.Message.ToStringUtf8()}");

        var info = await WaitForTransactionAsync(signed.GetTxid());
        Assert.Equal(TransactionInfo.Types.code.Sucess, info.Result);
        Assert.Equal(Transaction.Types.Result.Types.contractResult.Success, info.Receipt.Result);

        return AccountHelper.GetBase58Address(info.ContractAddress);
    }

    /// <summary>Polls until the transaction is in a block (the local witness produces one every 3 s).</summary>
    public async Task<TransactionInfo> WaitForTransactionAsync(string txid, int timeoutSeconds = 60)
    {
        var wallet = Wallet.GetWalletClient();
        var request = new BytesMessage { Value = ByteString.CopyFrom(txid.HexToByteArray()) };
        for (var waited = 0; waited < timeoutSeconds * 1000; waited += 500)
        {
            var info = await wallet.GetTransactionInfoByIdAsync(request, headers: Wallet.GetHeaders());
            if (info.BlockNumber > 0) return info;
            await Task.Delay(500);
        }

        throw new TimeoutException($"Transaction {txid} was not included in a block within {timeoutSeconds}s.");
    }

    public async Task<long> GetTrxBalanceAsync(string address)
    {
        var account = await Wallet.GetWalletClient().GetAccountAsync(new Account { Address = AccountHelper.ParseAddress(address) }, headers: Wallet.GetHeaders());
        return account.Balance;
    }
}

[CollectionDefinition(Name)]
public sealed class LocalNodeCollection : ICollectionFixture<LocalNodeFixture>
{
    public const string Name = "Local TRON node";
}
