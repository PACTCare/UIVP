namespace UIVP.Protocol.Core.Repository
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Security.Cryptography;
  using System.Threading.Tasks;

  using Tangle.Net.Entity;
  using Tangle.Net.Repository;
  using Tangle.Net.Utils;

  using UIVP.Protocol.Core.Entity;
  using UIVP.Protocol.Core.Services;

  public class IotaInvoiceRepository : IInvoiceRepository
  {
    private IIotaRepository IotaRepository { get; }

    public IotaInvoiceRepository(IIotaRepository iotaRepository)
    {
      this.IotaRepository = iotaRepository;
    }

    /// <inheritdoc />
    public async Task<TryteString> PublishInvoiceHashAsync(byte[] document, CngKey key)
    {
      var payload = new InvoicePayload(DocumentHash.Create(document),
        Encryption.CreateSignatureScheme(key).SignData(document));

      var bundle = new Bundle();
      bundle.AddTransfer(new Transfer
      {
        Address = new Address(Seed.Random().Value),
        Message = payload.ToTryteString(),
        Timestamp = Timestamp.UnixSecondsTimestamp,
        Tag = new Tag("CHECKCHEQUE")
      });

      bundle.Finalize();
      bundle.Sign();

      await this.IotaRepository.SendTrytesAsync(bundle.Transactions, 2);

      return bundle.Hash;
    }

    /// <inheritdoc />
    public async Task<InvoicePayload> LoadInvoiceInformationAsync(Hash bundleHash)
    {
      var bundleTransactions = await this.IotaRepository.FindTransactionsByBundlesAsync(new List<Hash> {bundleHash});
      var bundle = await this.IotaRepository.GetBundlesAsync(bundleTransactions.Hashes, false);
      var rawPayload = bundle.First().AggregateFragments();

      return InvoicePayload.FromTrytePayload(rawPayload);
    }
  }
}
