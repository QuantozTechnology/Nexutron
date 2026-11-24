using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Nexutron.Contracts;
using Nexutron.Extensions;
using Nexutron.Helpers;
using Nexutron.Protocol;
using Xunit;

namespace Nexutron.LocalNode.Tests;

/// <summary>
/// End-to-end tests of the SDK against a real java-tron node over gRPC. Every transaction is executed and
/// included in a block by the node, then checked on chain.
/// </summary>
[Collection(LocalNodeCollection.Name)]
public class LocalNodeTests(LocalNodeFixture node)
{
    private IContractClient Trc20 => node.Services.GetRequiredService<IContractClientFactory>().CreateClient(ContractProtocol.TRC20);

    [LocalNodeFact]
    public async Task Reads_the_latest_block_over_grpc()
    {
        var block = await node.Wallet.GetWalletClient().GetNowBlock2Async(new EmptyMessage(), headers: node.Wallet.GetHeaders());
        Assert.True(block.BlockHeader.RawData.Number > 0);
    }

    [LocalNodeFact]
    public async Task Trx_transfer_is_executed_on_chain()
    {
        var recipient = node.Wallet.GenerateAccount();
        var amount = TronUnit.TRXToSun(5);

        var created = await node.Transactions.CreateTransactionAsync(LocalNodeFixture.FundedAddress, recipient.Address, amount, "nexutron local test");
        var signed = node.Transactions.GetTransactionSign(created.Transaction, LocalNodeFixture.FundedPrivateKey);
        var result = await node.Transactions.BroadcastTransactionAsync(signed);
        Assert.True(result.Result, $"{result.Code} {result.Message.ToStringUtf8()}");

        var info = await node.WaitForTransactionAsync(signed.GetTxid());

        Assert.Equal(TransactionInfo.Types.code.Sucess, info.Result);
        Assert.Equal(amount, await node.GetTrxBalanceAsync(recipient.Address));

        // The stored transaction carries our memo and signature unchanged.
        var onChain = await node.Wallet.GetWalletClient().GetTransactionByIdAsync(
            new BytesMessage { Value = ByteString.CopyFrom(signed.GetTxid().HexToByteArray()) }, headers: node.Wallet.GetHeaders());
        Assert.Equal("nexutron local test", onChain.RawData.Data.ToStringUtf8());
        Assert.Equal(signed.Signature[0], onChain.Signature[0]);
    }

    [LocalNodeFact]
    public async Task Transfer_signed_with_the_wrong_key_is_rejected()
    {
        var created = await node.Transactions.CreateTransactionAsync(LocalNodeFixture.FundedAddress, node.Wallet.GenerateAccount().Address, 1);
        var signed = node.Transactions.GetTransactionSign(created.Transaction, node.Wallet.GenerateAccount().PrivateKey);

        var result = await node.Transactions.BroadcastTransactionAsync(signed);

        Assert.False(result.Result);
        Assert.Equal(Return.Types.response_code.Sigerror, result.Code);
    }

    [LocalNodeFact]
    public async Task Trc20_balance_and_transfer_are_executed_on_chain()
    {
        var owner = node.Wallet.GetAccount(LocalNodeFixture.FundedPrivateKey);
        var recipient = node.Wallet.GenerateAccount();
        var ownerBefore = await Trc20.BalanceOfAsync(node.TokenAddress, owner);
        Assert.True(ownerBefore > 100m);
        Assert.Equal(0m, await Trc20.BalanceOfAsync(node.TokenAddress, recipient));

        var txid = await Trc20.TransferAsync(node.TokenAddress, owner, recipient.Address, 12.345678m, "trc20 memo", TronUnit.TRXToSun(100));

        Assert.NotNull(txid);
        var info = await node.WaitForTransactionAsync(txid);
        Assert.Equal(TransactionInfo.Types.code.Sucess, info.Result);
        Assert.Equal(Transaction.Types.Result.Types.contractResult.Success, info.Receipt.Result);

        // One Transfer(from, to, value) event with the exact raw amount.
        var log = Assert.Single(info.Log);
        Assert.Equal("ddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef", log.Topics[0].ToByteArray().ToHex());
        Assert.Equal(AccountHelper.GetHexAddress(recipient.Address)[2..], log.Topics[2].ToByteArray().ToHex()[24..]);
        Assert.Equal(12_345_678, (long)new System.Numerics.BigInteger(log.Data.ToByteArray(), isUnsigned: true, isBigEndian: true));

        Assert.Equal(12.345678m, await Trc20.BalanceOfAsync(node.TokenAddress, recipient));
        Assert.Equal(ownerBefore - 12.345678m, await Trc20.BalanceOfAsync(node.TokenAddress, owner));
    }

    [LocalNodeFact]
    public async Task Trc20_transfer_exceeding_the_balance_fails()
    {
        var poor = node.Wallet.GenerateAccount();

        // Fund the account with TRX only, so the call fails on the token balance and not on fees.
        var funding = await node.Transactions.CreateTransactionAsync(LocalNodeFixture.FundedAddress, poor.Address, TronUnit.TRXToSun(200));
        var signedFunding = node.Transactions.GetTransactionSign(funding.Transaction, LocalNodeFixture.FundedPrivateKey);
        Assert.True((await node.Transactions.BroadcastTransactionAsync(signedFunding)).Result);
        await node.WaitForTransactionAsync(signedFunding.GetTxid());

        var txid = await Trc20.TransferAsync(node.TokenAddress, poor, LocalNodeFixture.FundedAddress, 1m, null, TronUnit.TRXToSun(100));

        Assert.Null(txid);
    }

    [LocalNodeFact]
    public async Task Solidity_node_serves_confirmed_state()
    {
        var account = await node.Wallet.GetSolidityClient().GetAccountAsync(
            new Account { Address = AccountHelper.ParseAddress(LocalNodeFixture.FundedAddress) }, headers: node.Wallet.GetHeaders());

        Assert.True(account.Balance > 0);
    }
}
