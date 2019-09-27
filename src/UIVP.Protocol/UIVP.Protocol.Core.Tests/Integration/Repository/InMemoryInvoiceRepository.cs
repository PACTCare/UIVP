namespace UIVP.Protocol.Core.Tests.Integration.Repository
{
  using System.Security.Cryptography;
  using System.Threading.Tasks;

  using UIVP.Protocol.Core.Entity;
  using UIVP.Protocol.Core.Repository;

  public class InMemoryInvoiceRepository : InvoiceRepository
  {
    /// <inheritdoc />
    public InMemoryInvoiceRepository(IPublicKeyRepository publicKeyRepository, InvoiceMetadata dataToReturn)
      : base(publicKeyRepository)
    {
      this.DataToReturn = dataToReturn;
    }

    private InvoiceMetadata DataToReturn { get; set; }

    /// <inheritdoc />
    public override async Task<InvoiceMetadata> LoadInvoiceInformationAsync(Invoice invoice)
    {
      return this.DataToReturn;
    }

    /// <inheritdoc />
    public override async Task<PublishStatus> PublishInvoiceAsync(Invoice invoice, CngKey key)
    {
      this.DataToReturn = this.HashAndSign(invoice, key);
      return PublishStatus.Success;
    }
  }
}