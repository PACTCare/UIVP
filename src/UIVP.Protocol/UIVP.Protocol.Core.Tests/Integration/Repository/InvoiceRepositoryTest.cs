namespace UIVP.Protocol.Core.Tests.Unit.Services
{
  using System.Security.Cryptography;
  using System.Text;
  using System.Threading.Tasks;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Moq;

  using Multiformats.Hash;

  using UIVP.Protocol.Core.Entity;
  using UIVP.Protocol.Core.Repository;
  using UIVP.Protocol.Core.Services;
  using UIVP.Protocol.Core.Tests.Integration.Repository;

  [TestClass]
  public class InvoiceRepositoryTest
  {
    private Invoice Invoice =>
      new Invoice
        {
          KvkNumber = "1231455", IssuerAddress = "Somestreet 123, 1011AB Sometown", Amount = 9.99D, BankAccountNumber = "NLAHJDGAJHDGJAHD"
        };

    [TestMethod]
    public async Task TestInvoiceHashMismatchShouldReturnFalse()
    {
      var dltPayload = new InvoiceMetadata(Encoding.UTF8.GetBytes("Somebody once told me"),
        Encoding.UTF8.GetBytes("the world is gonna roll me"));

      var invoiceRepository = new InMemoryInvoiceRepository(new Mock<IKvkRepository>().Object, dltPayload);

      Assert.IsFalse(await invoiceRepository.ValidateInvoiceAsync(this.Invoice));
    }

    [TestMethod]
    public async Task TestSignatureMismatchShouldReturnFalse()
    {
      var signatureScheme = Encryption.CreateSignatureScheme();

      var dltPayload = new InvoiceMetadata(this.Invoice.CreateHash(HashType.SHA2_256),
        signatureScheme.SignData(Encoding.UTF8.GetBytes("Somebody once told me the world is gonna roll me")));

      var kvkRepository = new Mock<IKvkRepository>();
      kvkRepository.Setup(k => k.GetCompanyPublicKeyAsync(It.IsAny<string>()))
        .ReturnsAsync(signatureScheme.Key.Export(CngKeyBlobFormat.EccFullPublicBlob));

      var invoiceRepository = new InMemoryInvoiceRepository(kvkRepository.Object, dltPayload);

      Assert.IsFalse(await invoiceRepository.ValidateInvoiceAsync(this.Invoice));
    }

    [TestMethod]
    public async Task TestSignatureMatchShouldReturnTrue()
    {
      var signatureScheme = Encryption.CreateSignatureScheme();

      var kvkRepository = new Mock<IKvkRepository>();
      kvkRepository.Setup(k => k.GetCompanyPublicKeyAsync(It.IsAny<string>()))
        .ReturnsAsync(signatureScheme.Key.Export(CngKeyBlobFormat.EccFullPublicBlob));

      var invoiceRepository = new InMemoryInvoiceRepository(kvkRepository.Object, null);
      await invoiceRepository.PublishInvoiceAsync(this.Invoice, signatureScheme.Key);

      Assert.IsTrue(await invoiceRepository.ValidateInvoiceAsync(this.Invoice));
    }
  }
}
