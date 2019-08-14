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

    public abstract Task PublishInvoiceAsync(Invoice invoice, CngKey key);

    public async Task<bool> ValidateInvoiceAsync(Invoice invoice)
    {
      var hashedInvoice = this.HashInvoice(invoice);
      var invoiceEntry = await this.LoadInvoiceInformationAsync(invoice);

      if (!hashedInvoice.SequenceEqual(invoiceEntry.Hash))
      {
        return false;
      }

      var companyPublicKey = await this.KvkRepository.GetCompanyPublicKeyAsync(invoice.KvkNumber);
      var key = CngKey.Import(companyPublicKey, CngKeyBlobFormat.EccFullPublicBlob);
      var signatureScheme = Encryption.CreateSignatureScheme(key);

      return signatureScheme.VerifyData(invoice.Payload, invoiceEntry.Signature);
    }

    protected virtual byte[] HashInvoice(Invoice invoice)
    {
      return invoice.CreateHash(HashType.SHA2_256);
    }

    protected InvoiceMetadata HashAndSign(Invoice invoice, CngKey key)
    {
      return new InvoiceMetadata(this.HashInvoice(invoice),  Encryption.CreateSignatureScheme(key).SignData(invoice.Payload));
    }

    public abstract Task<InvoiceMetadata> LoadInvoiceInformationAsync(Invoice invoice);
  }
}