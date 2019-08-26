namespace UIVP.Protocol.Core.Repository
{
  public enum VerificationStatus
  {
    Success = 0,

    HashMismatch = 1,

    MetadataUnavailable = 2,

    KvkNumberUnavailable = 3,

    PublicKeyNotFound = 4,

    InvalidSignature = 5
  }
}