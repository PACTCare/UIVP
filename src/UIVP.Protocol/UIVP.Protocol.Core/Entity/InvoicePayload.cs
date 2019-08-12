namespace UIVP.Protocol.Core.Entity
{
  using Tangle.Net.Entity;

  public class InvoicePayload
  {
    public InvoicePayload(byte[] invoiceHash, byte[] invoiceSignature)
    {
      this.InvoiceHash = invoiceHash;
      this.InvoiceSignature = invoiceSignature;
    }

    public byte[] InvoiceHash { get; }
    public byte[] InvoiceSignature { get; }

    public TryteString ToTryteString()
    {
      return TryteString.FromBytes(this.InvoiceHash).Concat(TryteString.FromBytes(this.InvoiceSignature));
    }

    public static InvoicePayload FromTrytePayload(TryteString trytes)
    {
      return new InvoicePayload(trytes.GetChunk(0, 64).ToBytes(), trytes.GetChunk(64, 128).ToBytes());
    }
  }
}