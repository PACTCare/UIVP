namespace UIVP.Protocol.Core.Iota.Repository
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Security.Cryptography;
  using System.Threading.Tasks;

  using Tangle.Net.Entity;
  using Tangle.Net.Repository;
  using Tangle.Net.Utils;

  using UIVP.Protocol.Core.Entity;
  using UIVP.Protocol.Core.Repository;

  public class IotaInvoiceRepository : InvoiceRepository
  {
    public IotaInvoiceRepository(IIotaRepository iotaRepository, IPublicKeyRepository publicKeyRepository)
      : base(publicKeyRepository)
    {
      this.IotaRepository = iotaRepository;
    }

    private IIotaRepository IotaRepository { get; }

    /// <inheritdoc />
    public override async Task<PublishStatus> PublishInvoiceAsync(Invoice invoice, CngKey key)
    {
      var invoiceMetadata = this.HashAndSign(invoice, key);
      var address = TryteString.FromBytes(invoiceMetadata.Hash).GetChunk(0, 81);

      var existingTransactions = await this.IotaRepository.FindTransactionsByAddressesAsync(new List<Address> { new Address(address.Value) });
      if (existingTransactions.Hashes.Count > 0)
      {
        return PublishStatus.AlreadyPublished;
      }

      var bundle = new Bundle();
      bundle.AddTransfer(
        new Transfer
          {
            Address = new Address(address.Value),
            Message = invoiceMetadata.ToTryteString(),
            Timestamp = Timestamp.UnixSecondsTimestamp,
            Tag = new Tag("CHECKCHEQUE")
          });

      bundle.Finalize();
      bundle.Sign();

      await this.IotaRepository.SendTrytesAsync(bundle.Transactions, 2);
      return PublishStatus.Success;
    }

    /// <inheritdoc />
    public override async Task<InvoiceMetadata> LoadInvoiceInformationAsync(Invoice invoice)
    {
      var address = TryteString.FromBytes(this.HashInvoice(invoice)).GetChunk(0, 81);
      var transactionHashList = await this.IotaRepository.FindTransactionsByAddressesAsync(new List<Address> { new Address(address.Value) });
      var transactionTrytes = await this.IotaRepository.GetTrytesAsync(transactionHashList.Hashes);
      var metadataTrytePayload = transactionTrytes.Select(tt => Transaction.FromTrytes(tt)).OrderBy(t => t.CurrentIndex).ToList().Aggregate(
        new TryteString(),
        (current, tryteString) => current.Concat(tryteString.Fragment));
      
      return metadataTrytePayload.ToInvoiceMetadata();
    }
  }
}