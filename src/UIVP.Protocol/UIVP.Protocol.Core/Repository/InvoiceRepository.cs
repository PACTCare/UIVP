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
    protected InvoiceRepository(IPublicKeyRepository publicKeyRepository)
    {
      this.PublicKeyRepository = publicKeyRepository;
    }

    private IPublicKeyRepository PublicKeyRepository { get; }

    public abstract Task<InvoiceMetadata> LoadInvoiceInformationAsync(Invoice invoice);

    public abstract Task PublishInvoiceAsync(Invoice invoice, CngKey key);

    public async Task<VerificationStatus> VerifyInvoiceAsync(Invoice invoice)
    {
      var hashedInvoice = this.HashInvoice(invoice);
      InvoiceMetadata invoiceEntry;

      try
      {
        invoiceEntry = await this.LoadInvoiceInformationAsync(invoice);
      }
      catch
      {
        return VerificationStatus.MetadataUnavailable;
      }

      if (!hashedInvoice.SequenceEqual(invoiceEntry.Hash))
      {
        return VerificationStatus.HashMismatch;
      }

      byte[] companyPublicKey;
      try
      {
        companyPublicKey = await this.PublicKeyRepository.GetCompanyPublicKeyAsync(invoice.KvkNumber);
        if (companyPublicKey.Length == 0)
        {
          return VerificationStatus.PublicKeyNotFound;
        }
      }
      catch
      {
        return VerificationStatus.KvkNumberUnavailable;
      }

      var key = CngKey.Import(companyPublicKey, CngKeyBlobFormat.EccFullPublicBlob);
      var signatureScheme = Encryption.CreateSignatureScheme(key);

      return signatureScheme.VerifyData(invoice.Payload, invoiceEntry.Signature) ? VerificationStatus.Success : VerificationStatus.InvalidSignature;
    }

    protected virtual InvoiceMetadata HashAndSign(Invoice invoice, CngKey key)
    {
      return new InvoiceMetadata(this.HashInvoice(invoice), Encryption.CreateSignatureScheme(key).SignData(invoice.Payload));
    }

    protected virtual byte[] HashInvoice(Invoice invoice)
    {
      return invoice.CreateHash(HashType.SHA2_256);
    }
  }
}