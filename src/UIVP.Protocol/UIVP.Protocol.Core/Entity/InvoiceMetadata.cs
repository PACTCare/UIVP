namespace UIVP.Protocol.Core.Entity
{
  public class InvoiceMetadata
  {
    public InvoiceMetadata(byte[] hash, byte[] signature)
    {
      this.Hash = hash;
      this.Signature = signature;
    }

    public byte[] Hash { get; }
    public byte[] Signature { get; }
  }
}