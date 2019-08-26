

namespace UIVP.Protocol.Console
{
  using System;
  using System.Collections.Generic;
  using System.Configuration;

  using RestSharp;

  using Tangle.Net.Entity;
  using Tangle.Net.ProofOfWork;
  using Tangle.Net.Repository;
  using Tangle.Net.Repository.Client;

  using UIVP.Protocol.Core.Iota.Repository;
  using UIVP.Protocol.Core.Repository;
  using UIVP.Protocol.Core.Services;

  class Program
  {
    static void Main(string[] args)
    {
      ExecuteAsync();
      Console.ReadKey();
    }

    private static async void ExecuteAsync()
    {
      // register key in memory, just for testing. in production this will be the KvKPublicKeyRepository
      var publicKeyRepository = new KvkPublicKeyRepository(new RestClient(ConfigurationManager.AppSettings["KvKApiUri"]));

      // in production, create instances via DI
      var imageService = new ImageInvoiceParser(ConfigurationManager.AppSettings["ApiUri"], ConfigurationManager.AppSettings["SubscriptionKey"]);
      var invoiceRepository = new IotaInvoiceRepository(
        new RestIotaRepository(
          new FallbackIotaClient(new List<string> { "https://nodes.devnet.thetangle.org:443" }, 5000),
          new PoWService(new CpuPearlDiver())),
        publicKeyRepository);

      // should be stored somewhere safe in production and loaded on demand
      var cngKey = Encryption.CreateSignatureScheme();
      await publicKeyRepository.RegisterCompanyPublicKeyAsync("401196200", cngKey.Key);

      // parse invoice data with the help of azure
      var invoice = await imageService.ParseInvoiceAsync("..\\..\\..\\IMG_20190413_192812.jpg");

      // publish the invoice in hash and signed form
      await invoiceRepository.PublishInvoiceAsync(invoice, cngKey.Key);

      // validate the invoice
      var isValid = await invoiceRepository.VerifyInvoiceAsync(invoice);
      Console.WriteLine(isValid);
    }
  }
}
