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

    public byte[] CreateHash(HashType algorithm)
    {
      var hashPayload = Encoding.UTF8.GetBytes($"{this.IssuerAddress}|{this.BankAccountNumber}|{this.Amount}|{this.KvkNumber}");
      return Multihash.Sum(algorithm, hashPayload, 64);
    }
  }
}