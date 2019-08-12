namespace UIVP.Protocol.Core.Services
{
  using System.Linq;
  using System.Security.Cryptography;
  using System.Threading.Tasks;

  using Multiformats.Hash;

  using Tangle.Net.Entity;

  using UIVP.Protocol.Core.Entity;
  using UIVP.Protocol.Core.Repository;

  public class InvoiceVerificator
  {
    private IInvoiceRepository InvoiceRepository { get; }
    private IKvkRepository KvkRepository { get; }

    public InvoiceVerificator(IInvoiceRepository invoiceRepository, IKvkRepository kvkRepository)
    {
      this.InvoiceRepository = invoiceRepository;
      this.KvkRepository = kvkRepository;
    }

    public async Task<bool> IsValid(Invoice invoice)
    {
      var hashedInvoice = invoice.CreateHash(HashType.SHA2_256);
      var invoiceEntry = await this.InvoiceRepository.LoadInvoiceInformationAsync(Hash.Empty);

      if (!hashedInvoice.SequenceEqual(invoiceEntry.InvoiceHash))
      {
        return false;
      }

      var companyPublicKey = await this.KvkRepository.GetCompanyPublicKeyAsync(invoice.KvkNumber);
      var key = CngKey.Import(companyPublicKey, CngKeyBlobFormat.EccFullPublicBlob);
      var signatureScheme = Encryption.CreateSignatureScheme(key);

      return signatureScheme.VerifyData(invoice.Signature, invoiceEntry.InvoiceSignature);
    }
  }
}
