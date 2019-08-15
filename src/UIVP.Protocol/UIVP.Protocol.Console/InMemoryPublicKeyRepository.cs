using System;
using System.Collections.Generic;
using System.Text;

namespace UIVP.Protocol.Console
{
  using System.Linq;
  using System.Security.Cryptography;
  using System.Threading.Tasks;

  using UIVP.Protocol.Core.Repository;

  public class InMemoryPublicKeyRepository : IPublicKeyRepository
  {
    private Dictionary<string, byte[]> Keys { get; set; }

    public InMemoryPublicKeyRepository()
    {
      this.Keys = new Dictionary<string, byte[]>();
    }

    /// <inheritdoc />
    public async Task<byte[]> GetCompanyPublicKeyAsync(string kvkNumber)
    {
      return this.Keys.First(k => k.Key == kvkNumber).Value;
    }

    /// <inheritdoc />
    public async Task RegisterCompanyPublicKeyAsync(string kvkNumber, CngKey key)
    {
      var publicKey = key.Export(CngKeyBlobFormat.EccFullPublicBlob);
      this.Keys.Add(kvkNumber, publicKey);
    }
  }
}
