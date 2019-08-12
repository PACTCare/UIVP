namespace UIVP.Protocol.Core.Repository
{
  using System.Security.Cryptography;
  using System.Threading.Tasks;

  public interface IKvkRepository
  {
    Task<byte[]> GetCompanyPublicKeyAsync(string kvkNumber);

    Task RegisterCompanyPublicKeyAsync(string kvkNumber, CngKey key);
  }
}
