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

  [TestClass]
  public class InvoiceVerificatorTests
  {
    [TestMethod]
    public async Task TestInvoiceHashMismatchShouldReturnFalse()
    {
      var dltPayload = new InvoicePayload(Encoding.UTF8.GetBytes("Somebody once told me"),
        Encoding.UTF8.GetBytes("the world is gonna roll me"));

      var invoiceRepository = new Mock<InvoiceRepository>();
      invoiceRepository.Setup(i => i.LoadInvoiceInformationAsync(It.IsAny<Hash>())).ReturnsAsync(dltPayload);

      var verificator = new InvoiceVerificator(invoiceRepository.Object, new Mock<IKvkRepository>().Object);
      Assert.IsFalse(await verificator.IsValid(new Invoice
        { KvkNumber = "123456789", Signature = Encoding.UTF8.GetBytes("Somebody once")}));
    }

    [TestMethod]
    public async Task TestSignatureMismatchShouldReturnFalse()
    {
      var invoice = new Invoice
        { KvkNumber = "123456789", Signature = Encoding.UTF8.GetBytes("Somebody once told me") };
      var signatureScheme = Encryption.CreateSignatureScheme();

      var dltPayload = new InvoicePayload(invoice.CreateHash(HashType.SHA2_256),
        signatureScheme.SignData(Encoding.UTF8.GetBytes("Somebody once told me the world is gonna roll me")));

      var invoiceRepository = new Mock<InvoiceRepository>();
      invoiceRepository.Setup(i => i.LoadInvoiceInformationAsync(It.IsAny<Hash>())).ReturnsAsync(dltPayload);

      var kvkRepository = new Mock<IKvkRepository>();
      kvkRepository.Setup(k => k.GetCompanyPublicKeyAsync(It.IsAny<string>()))
        .ReturnsAsync(signatureScheme.Key.Export(CngKeyBlobFormat.EccFullPublicBlob));

      var verificator = new InvoiceVerificator(invoiceRepository.Object, kvkRepository.Object);

      Assert.IsFalse(await verificator.IsValid(invoice));
    }

    [TestMethod]
    public async Task TestSignatureMatchShouldReturnTrue()
    {
      var invoice = new Invoice
        { KvkNumber = "123456789", Signature = Encoding.UTF8.GetBytes("Somebody once told me") };
      var signatureScheme = Encryption.CreateSignatureScheme();

      var dltPayload = new InvoicePayload(invoice.CreateHash(HashType.SHA2_256),
        signatureScheme.SignData(invoice.CreateHash(HashType.SHA2_256)));

      var invoiceRepository = new Mock<InvoiceRepository>();
      invoiceRepository.Setup(i => i.LoadInvoiceInformationAsync(It.IsAny<Hash>())).ReturnsAsync(dltPayload);

      var kvkRepository = new Mock<IKvkRepository>();
      kvkRepository.Setup(k => k.GetCompanyPublicKeyAsync(It.IsAny<string>()))
        .ReturnsAsync(signatureScheme.Key.Export(CngKeyBlobFormat.EccFullPublicBlob));

      var verificator = new InvoiceVerificator(invoiceRepository.Object, kvkRepository.Object);

      Assert.IsTrue(await verificator.IsValid(invoice));
    }
  }
}
