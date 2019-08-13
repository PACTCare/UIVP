namespace UIVP.Protocol.Core.Entity
{
  using System.Text;

  using Multiformats.Hash;

  public class Invoice
  {
    public double Amount { get; set; }
    public string BankAccountNumber { get; set; }
    public string IssuerAddress { get; set; }
    public string KvkNumber { get; set; }
    public byte[] Payload => Encoding.UTF8.GetBytes($"{this.IssuerAddress}|{this.BankAccountNumber}|{this.Amount}|{this.KvkNumber}");

    public byte[] CreateHash(HashType algorithm)
    {
      return Multihash.Sum(algorithm, this.Payload, 64);
    }
  }
}