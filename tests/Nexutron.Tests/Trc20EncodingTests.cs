using System;
using System.Numerics;
using Nexutron.Contracts;
using Xunit;

namespace Nexutron.Tests;

public class Trc20EncodingTests
{
    [Fact]
    public void Transfer_call_data_is_identical_to_tronweb()
    {
        Assert.Equal(TestVectors.Trc20TransferData, TRC20ContractClient.EncodeTransfer(TestVectors.ToAddress, 1_500_000).ToHex());
        Assert.Equal(TestVectors.Trc20TransferData, TRC20ContractClient.EncodeTransfer(TestVectors.ToAddressHex, 1_500_000).ToHex());
    }

    [Fact]
    public void Selectors_match_the_erc20_standard()
    {
        Assert.Equal("313ce567", TRC20ContractClient.EncodeDecimals().ToHex());
        Assert.StartsWith("70a08231" + "0000000000000000000000004e92b786312e1a5111ee9312a96b65ae48100353",
            TRC20ContractClient.EncodeBalanceOf(TestVectors.ToAddress).ToHex());
    }

    [Theory]
    [InlineData("1.5", 6, "1500000")]
    [InlineData("99", 6, "99000000")]
    [InlineData("0.000001", 6, "1")]
    [InlineData("123456789.123456789012345678", 18, "123456789123456789012345678")]
    [InlineData("7", 0, "7")]
    public void Token_amount_conversion_is_exact(string amount, int decimals, string raw)
    {
        var value = decimal.Parse(amount, System.Globalization.CultureInfo.InvariantCulture);
        Assert.Equal(BigInteger.Parse(raw), TokenAmount.ToRaw(value, decimals));
        Assert.Equal(value, TokenAmount.FromRaw(BigInteger.Parse(raw), decimals));
    }

    [Fact]
    public void Large_18_decimal_amounts_do_not_overflow()
    {
        // 50 billion tokens with 18 decimals is far beyond long.MaxValue in raw units.
        var raw = TokenAmount.ToRaw(50_000_000_000m, 18);
        Assert.Equal(BigInteger.Parse("50000000000000000000000000000"), raw);
        Assert.Equal(50_000_000_000m, TokenAmount.FromRaw(raw, 18));
    }

    [Fact]
    public void Rejects_more_precision_than_the_token_supports()
    {
        Assert.Throws<ArgumentException>(() => TokenAmount.ToRaw(0.0000001m, 6));
    }
}
