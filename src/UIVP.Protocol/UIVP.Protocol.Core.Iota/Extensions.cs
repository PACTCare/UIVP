namespace UIVP.Protocol.Core.Iota
{
  using Tangle.Net.Entity;

  using UIVP.Protocol.Core.Entity;

  public static class Extensions
  {
    public static TryteString ToTryteString(this InvoiceMetadata metadata)
    {
      return TryteString.FromBytes(metadata.Hash).Concat(TryteString.FromBytes(metadata.Signature));
    }
  }
}