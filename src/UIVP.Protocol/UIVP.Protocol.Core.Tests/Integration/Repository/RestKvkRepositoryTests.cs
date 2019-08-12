namespace UIVP.Protocol.Core.Tests.Integration.Repository
{
  using System.Linq;
  using System.Security.Cryptography;
  using System.Threading.Tasks;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using RestSharp;

  using UIVP.Protocol.Core.Repository;
  using UIVP.Protocol.Core.Services;

  [TestClass]
  [Ignore]
  public class RestKvkRepositoryTests
  {
    [TestMethod]
    public async Task TestPublicKeyCanBePublishedAndIsReadCorrectlyFromResponse()
    {
      var client = new RestClient("https://localhost:44381");
      var repository = new RestKvkRepository(client);

      var key = Encryption.Create();
      await repository.RegisterCompanyPublicKeyAsync("242630600", key);

      var publicKey = await repository.GetCompanyPublicKeyAsync("242630600");

      Assert.IsTrue(publicKey.SequenceEqual(key.Export(CngKeyBlobFormat.EccFullPublicBlob)));
    }
  }
}
