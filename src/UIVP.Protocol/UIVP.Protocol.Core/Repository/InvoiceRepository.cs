namespace UIVP.Protocol.Core.Repository
{
  using System.Linq;
  using System.Security.Cryptography;
  using System.Threading.Tasks;

  using Multiformats.Hash;

  using UIVP.Protocol.Core.Entity;
  using UIVP.Protocol.Core.Services;

  public abstract class InvoiceRepository
  {
    protected InvoiceRepository(IKvkRepository kvkRepository)
    {
      this.KvkRepository = kvkRepository;
    }

    private IKvkRepository KvkRepository { get; }

    protected abstract Task<InvoicePayload> LoadInvoiceInformationAsync(string storageAddress);

    public abstract Task<string> PublishInvoiceHashAsync(Invoice invoice, CngKey key);

    public async Task<bool> ValidateInvoice(Invoice invoice)
    {
      var hashedInvoice = invoice.CreateHash(HashType.SHA2_256);
      var invoiceEntry = await this.LoadInvoiceInformationAsync(this.GenerateInvoiceAddress(invoice));

      if (!hashedInvoice.SequenceEqual(invoiceEntry.Hash))
      {
        return false;
      }

      var companyPublicKey = await this.KvkRepository.GetCompanyPublicKeyAsync(invoice.KvkNumber);
      var key = CngKey.Import(companyPublicKey, CngKeyBlobFormat.EccFullPublicBlob);
      var signatureScheme = Encryption.CreateSignatureScheme(key);

      return signatureScheme.VerifyData(hashedInvoice, invoiceEntry.Signature);
    }

    protected abstract string GenerateInvoiceAddress(Invoice invoice);
  }
}