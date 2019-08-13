namespace UIVP.Protocol.Core.Entity
{
  public class InvoicePayload
  {
    public byte[] Hash { get; set; }
    public byte[] Signature { get; set; }
  }
}