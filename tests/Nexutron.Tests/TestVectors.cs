namespace Nexutron.Tests;

/// <summary>
/// Reference values produced independently with TronWeb 6 (the official JavaScript SDK).
/// </summary>
internal static class TestVectors
{
    public const string PrivateKey1 = "fd605fb953fcdabb952be161265a75b8a3ce1c0def2c7db72265f9db9a471be4";
    public const string Address1 = "TBDCyrZ1hT1PDDFf2yRABwPrFica5qqPUX";

    public const string PrivateKey2 = "62075119d64f17ebd3248df7a864cd84380fcb9e5771f0968af2167a25717bb2";
    public const string Address2 = "THe8MeboLUbvi1TYntzPKK8SsfcCiJCZ19";

    public const string PrivateKeyLeadingZeros = "00000000000000000000000000000000000000000000000000000000000000ff";
    public const string AddressLeadingZeros = "THHdJjkPUngpM4PdJd8Wq8Rq77CFoKHj1u";

    public const string ToAddress = "TH8fU6BLpU6EjqvZTWWSB1uhpEVxzT35Xj";
    public const string ToAddressHex = "414e92b786312e1a5111ee9312a96b65ae48100353";

    /// <summary>transfer(TH8fU6BLpU6EjqvZTWWSB1uhpEVxzT35Xj, 1500000)</summary>
    public const string Trc20TransferData = "a9059cbb0000000000000000000000004e92b786312e1a5111ee9312a96b65ae48100353000000000000000000000000000000000000000000000000000000000016e360";

    /// <summary>
    /// TransferContract of 2 TRX from Address2 to ToAddress, ref_block_bytes 1234, ref_block_hash 0102030405060708,
    /// timestamp 1700000000000, expiration 1700000600000, memo "hello tron".
    /// </summary>
    public const string TransferRawDataHex = "0a0212342208010203040506070840c09fbaffbc31520a68656c6c6f2074726f6e5a67080112630a2d747970652e676f6f676c65617069732e636f6d2f70726f746f636f6c2e5472616e73666572436f6e747261637412320a1541542539bd6fd77b28842825b912378908664b28221215414e92b786312e1a5111ee9312a96b65ae481003531880897a7080d095ffbc31";
    public const string TransferTxId = "f6c6a1f5374f13764a944abea05997f4ba3ee9019ca77e2fefe34169a4b2fcb5";

    /// <summary>TransferRawDataHex signed with PrivateKey2.</summary>
    public const string TransferSignature = "4e5b3551e689804b81fbaa16cdafe427e7dd26cde4997b26a9ca22c975236a2b5344c226fd1295116b355fdb10e29db215662e02f7db1e657a2dc8d8445bf0c11b";
}
