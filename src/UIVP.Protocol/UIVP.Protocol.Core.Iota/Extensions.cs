namespace UIVP.Protocol.Core.Iota
{
  using Tangle.Net.Entity;

  using UIVP.Protocol.Core.Entity;

  public static class Extensions
  {
    public static InvoiceMetadata ToInvoiceMetadata(this TryteString trytes)
    {
      return new InvoiceMetadata(trytes.GetChunk(0, 132).ToBytes(), trytes.GetChunk(132, trytes.TrytesLength - 132).ToBytes());
    }

    public static TryteString ToTryteString(this InvoiceMetadata metadata)
    {
      return TryteString.FromBytes(metadata.Hash).Concat(TryteString.FromBytes(metadata.Signature));
    }
  }
}