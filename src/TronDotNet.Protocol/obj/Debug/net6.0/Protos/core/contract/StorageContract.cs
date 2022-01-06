// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/core/contract/storage_contract.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace TronDotNet.Protocol {

  /// <summary>Holder for reflection information generated from Protos/core/contract/storage_contract.proto</summary>
  public static partial class StorageContractReflection {

    #region Descriptor
    /// <summary>File descriptor for Protos/core/contract/storage_contract.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static StorageContractReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CitQcm90b3MvY29yZS9jb250cmFjdC9zdG9yYWdlX2NvbnRyYWN0LnByb3Rv",
            "Eghwcm90b2NvbCI/ChdCdXlTdG9yYWdlQnl0ZXNDb250cmFjdBIVCg1vd25l",
            "cl9hZGRyZXNzGAEgASgMEg0KBWJ5dGVzGAIgASgDIjoKEkJ1eVN0b3JhZ2VD",
            "b250cmFjdBIVCg1vd25lcl9hZGRyZXNzGAEgASgMEg0KBXF1YW50GAIgASgD",
            "IkMKE1NlbGxTdG9yYWdlQ29udHJhY3QSFQoNb3duZXJfYWRkcmVzcxgBIAEo",
            "DBIVCg1zdG9yYWdlX2J5dGVzGAIgASgDIkMKF1VwZGF0ZUJyb2tlcmFnZUNv",
            "bnRyYWN0EhUKDW93bmVyX2FkZHJlc3MYASABKAwSEQoJYnJva2VyYWdlGAIg",
            "ASgFQkFaKWdpdGh1Yi5jb20vdHJvbnByb3RvY29sL2dycGMtZ2F0ZXdheS9j",
            "b3JlqgITVHJvbkRvdE5ldC5Qcm90b2NvbGIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::TronDotNet.Protocol.BuyStorageBytesContract), global::TronDotNet.Protocol.BuyStorageBytesContract.Parser, new[]{ "OwnerAddress", "Bytes" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::TronDotNet.Protocol.BuyStorageContract), global::TronDotNet.Protocol.BuyStorageContract.Parser, new[]{ "OwnerAddress", "Quant" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::TronDotNet.Protocol.SellStorageContract), global::TronDotNet.Protocol.SellStorageContract.Parser, new[]{ "OwnerAddress", "StorageBytes" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::TronDotNet.Protocol.UpdateBrokerageContract), global::TronDotNet.Protocol.UpdateBrokerageContract.Parser, new[]{ "OwnerAddress", "Brokerage" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class BuyStorageBytesContract : pb::IMessage<BuyStorageBytesContract>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<BuyStorageBytesContract> _parser = new pb::MessageParser<BuyStorageBytesContract>(() => new BuyStorageBytesContract());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<BuyStorageBytesContract> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::TronDotNet.Protocol.StorageContractReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BuyStorageBytesContract() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BuyStorageBytesContract(BuyStorageBytesContract other) : this() {
      ownerAddress_ = other.ownerAddress_;
      bytes_ = other.bytes_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BuyStorageBytesContract Clone() {
      return new BuyStorageBytesContract(this);
    }

    /// <summary>Field number for the "owner_address" field.</summary>
    public const int OwnerAddressFieldNumber = 1;
    private pb::ByteString ownerAddress_ = pb::ByteString.Empty;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString OwnerAddress {
      get { return ownerAddress_; }
      set {
        ownerAddress_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "bytes" field.</summary>
    public const int BytesFieldNumber = 2;
    private long bytes_;
    /// <summary>
    /// storage bytes for buy
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long Bytes {
      get { return bytes_; }
      set {
        bytes_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as BuyStorageBytesContract);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(BuyStorageBytesContract other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (OwnerAddress != other.OwnerAddress) return false;
      if (Bytes != other.Bytes) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (OwnerAddress.Length != 0) hash ^= OwnerAddress.GetHashCode();
      if (Bytes != 0L) hash ^= Bytes.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (OwnerAddress.Length != 0) {
        output.WriteRawTag(10);
        output.WriteBytes(OwnerAddress);
      }
      if (Bytes != 0L) {
        output.WriteRawTag(16);
        output.WriteInt64(Bytes);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (OwnerAddress.Length != 0) {
        output.WriteRawTag(10);
        output.WriteBytes(OwnerAddress);
      }
      if (Bytes != 0L) {
        output.WriteRawTag(16);
        output.WriteInt64(Bytes);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (OwnerAddress.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(OwnerAddress);
      }
      if (Bytes != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(Bytes);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(BuyStorageBytesContract other) {
      if (other == null) {
        return;
      }
      if (other.OwnerAddress.Length != 0) {
        OwnerAddress = other.OwnerAddress;
      }
      if (other.Bytes != 0L) {
        Bytes = other.Bytes;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            OwnerAddress = input.ReadBytes();
            break;
          }
          case 16: {
            Bytes = input.ReadInt64();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            OwnerAddress = input.ReadBytes();
            break;
          }
          case 16: {
            Bytes = input.ReadInt64();
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class BuyStorageContract : pb::IMessage<BuyStorageContract>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<BuyStorageContract> _parser = new pb::MessageParser<BuyStorageContract>(() => new BuyStorageContract());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<BuyStorageContract> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::TronDotNet.Protocol.StorageContractReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BuyStorageContract() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BuyStorageContract(BuyStorageContract other) : this() {
      ownerAddress_ = other.ownerAddress_;
      quant_ = other.quant_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BuyStorageContract Clone() {
      return new BuyStorageContract(this);
    }

    /// <summary>Field number for the "owner_address" field.</summary>
    public const int OwnerAddressFieldNumber = 1;
    private pb::ByteString ownerAddress_ = pb::ByteString.Empty;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString OwnerAddress {
      get { return ownerAddress_; }
      set {
        ownerAddress_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "quant" field.</summary>
    public const int QuantFieldNumber = 2;
    private long quant_;
    /// <summary>
    /// trx quantity for buy storage (sun)
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long Quant {
      get { return quant_; }
      set {
        quant_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as BuyStorageContract);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(BuyStorageContract other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (OwnerAddress != other.OwnerAddress) return false;
      if (Quant != other.Quant) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (OwnerAddress.Length != 0) hash ^= OwnerAddress.GetHashCode();
      if (Quant != 0L) hash ^= Quant.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (OwnerAddress.Length != 0) {
        output.WriteRawTag(10);
        output.WriteBytes(OwnerAddress);
      }
      if (Quant != 0L) {
        output.WriteRawTag(16);
        output.WriteInt64(Quant);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (OwnerAddress.Length != 0) {
        output.WriteRawTag(10);
        output.WriteBytes(OwnerAddress);
      }
      if (Quant != 0L) {
        output.WriteRawTag(16);
        output.WriteInt64(Quant);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (OwnerAddress.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(OwnerAddress);
      }
      if (Quant != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(Quant);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(BuyStorageContract other) {
      if (other == null) {
        return;
      }
      if (other.OwnerAddress.Length != 0) {
        OwnerAddress = other.OwnerAddress;
      }
      if (other.Quant != 0L) {
        Quant = other.Quant;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            OwnerAddress = input.ReadBytes();
            break;
          }
          case 16: {
            Quant = input.ReadInt64();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            OwnerAddress = input.ReadBytes();
            break;
          }
          case 16: {
            Quant = input.ReadInt64();
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class SellStorageContract : pb::IMessage<SellStorageContract>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<SellStorageContract> _parser = new pb::MessageParser<SellStorageContract>(() => new SellStorageContract());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SellStorageContract> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::TronDotNet.Protocol.StorageContractReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SellStorageContract() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SellStorageContract(SellStorageContract other) : this() {
      ownerAddress_ = other.ownerAddress_;
      storageBytes_ = other.storageBytes_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SellStorageContract Clone() {
      return new SellStorageContract(this);
    }

    /// <summary>Field number for the "owner_address" field.</summary>
    public const int OwnerAddressFieldNumber = 1;
    private pb::ByteString ownerAddress_ = pb::ByteString.Empty;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString OwnerAddress {
      get { return ownerAddress_; }
      set {
        ownerAddress_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "storage_bytes" field.</summary>
    public const int StorageBytesFieldNumber = 2;
    private long storageBytes_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long StorageBytes {
      get { return storageBytes_; }
      set {
        storageBytes_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as SellStorageContract);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(SellStorageContract other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (OwnerAddress != other.OwnerAddress) return false;
      if (StorageBytes != other.StorageBytes) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (OwnerAddress.Length != 0) hash ^= OwnerAddress.GetHashCode();
      if (StorageBytes != 0L) hash ^= StorageBytes.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (OwnerAddress.Length != 0) {
        output.WriteRawTag(10);
        output.WriteBytes(OwnerAddress);
      }
      if (StorageBytes != 0L) {
        output.WriteRawTag(16);
        output.WriteInt64(StorageBytes);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (OwnerAddress.Length != 0) {
        output.WriteRawTag(10);
        output.WriteBytes(OwnerAddress);
      }
      if (StorageBytes != 0L) {
        output.WriteRawTag(16);
        output.WriteInt64(StorageBytes);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (OwnerAddress.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(OwnerAddress);
      }
      if (StorageBytes != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(StorageBytes);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(SellStorageContract other) {
      if (other == null) {
        return;
      }
      if (other.OwnerAddress.Length != 0) {
        OwnerAddress = other.OwnerAddress;
      }
      if (other.StorageBytes != 0L) {
        StorageBytes = other.StorageBytes;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            OwnerAddress = input.ReadBytes();
            break;
          }
          case 16: {
            StorageBytes = input.ReadInt64();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            OwnerAddress = input.ReadBytes();
            break;
          }
          case 16: {
            StorageBytes = input.ReadInt64();
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class UpdateBrokerageContract : pb::IMessage<UpdateBrokerageContract>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<UpdateBrokerageContract> _parser = new pb::MessageParser<UpdateBrokerageContract>(() => new UpdateBrokerageContract());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<UpdateBrokerageContract> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::TronDotNet.Protocol.StorageContractReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public UpdateBrokerageContract() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public UpdateBrokerageContract(UpdateBrokerageContract other) : this() {
      ownerAddress_ = other.ownerAddress_;
      brokerage_ = other.brokerage_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public UpdateBrokerageContract Clone() {
      return new UpdateBrokerageContract(this);
    }

    /// <summary>Field number for the "owner_address" field.</summary>
    public const int OwnerAddressFieldNumber = 1;
    private pb::ByteString ownerAddress_ = pb::ByteString.Empty;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString OwnerAddress {
      get { return ownerAddress_; }
      set {
        ownerAddress_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "brokerage" field.</summary>
    public const int BrokerageFieldNumber = 2;
    private int brokerage_;
    /// <summary>
    /// 1 mean 1%
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Brokerage {
      get { return brokerage_; }
      set {
        brokerage_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as UpdateBrokerageContract);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(UpdateBrokerageContract other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (OwnerAddress != other.OwnerAddress) return false;
      if (Brokerage != other.Brokerage) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (OwnerAddress.Length != 0) hash ^= OwnerAddress.GetHashCode();
      if (Brokerage != 0) hash ^= Brokerage.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (OwnerAddress.Length != 0) {
        output.WriteRawTag(10);
        output.WriteBytes(OwnerAddress);
      }
      if (Brokerage != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Brokerage);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (OwnerAddress.Length != 0) {
        output.WriteRawTag(10);
        output.WriteBytes(OwnerAddress);
      }
      if (Brokerage != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Brokerage);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (OwnerAddress.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(OwnerAddress);
      }
      if (Brokerage != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Brokerage);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(UpdateBrokerageContract other) {
      if (other == null) {
        return;
      }
      if (other.OwnerAddress.Length != 0) {
        OwnerAddress = other.OwnerAddress;
      }
      if (other.Brokerage != 0) {
        Brokerage = other.Brokerage;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            OwnerAddress = input.ReadBytes();
            break;
          }
          case 16: {
            Brokerage = input.ReadInt32();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            OwnerAddress = input.ReadBytes();
            break;
          }
          case 16: {
            Brokerage = input.ReadInt32();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
