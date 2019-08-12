namespace UIVP.Protocol.Core.Entity
{
  using Tangle.Net.Entity;

  public class Invoice
  {
    public Hash Hash { get; set; }
    public byte[] Payload { get; set; }
    public string KvkNumber { get; set; }
  }
}