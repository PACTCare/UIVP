namespace UIVP.Protocol.Core.Iota.Tests.Repository
{
  using System;
  using System.Threading.Tasks;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Moq;

  using Multiformats.Hash;

  using Tangle.Net.Entity;

  using UIVP.Protocol.Core.Entity;
  using UIVP.Protocol.Core.Iota.Repository;
  using UIVP.Protocol.Core.Repository;
  using UIVP.Protocol.Core.Services;

  [TestClass]
  public class IotaInvoiceRepositoryTest
  {
    [TestMethod]
    public async Task TestPublishReceiveFlow()
    {
      var repository = new IotaInvoiceRepository(ResourceProvider.Repository, new Mock<IKvkRepository>().Object);
      var invoice = new Invoice
                      {
                        KvkNumber = "1231455",
                        IssuerAddress = "Somestreet 123, 1011AB Sometown",
                        Amount = 9.99D,
                        BankAccountNumber = Seed.Random().Value
                      };

      var expectedHash = invoice.CreateHash(HashType.SHA2_256);

      await repository.PublishInvoiceAsync(invoice, Encryption.CreateKey());
      var metadata = await repository.LoadInvoiceInformationAsync(invoice);

      Assert.AreEqual(Convert.ToBase64String(expectedHash), Convert.ToBase64String(metadata.Hash));
    }
  }
}