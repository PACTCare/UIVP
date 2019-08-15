namespace UIVP.Protocol.Core.Tests.Services
{
  using System.Configuration;
  using System.Net.Mime;
  using System.Reflection;
  using System.Threading.Tasks;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using UIVP.Protocol.Core.Services;

  [TestClass]
  public class ImageInvoiceParserTest
  {
    [TestMethod]
    public async Task TestInvoiceExtraction()
    {
      var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
      var parser = new ImageInvoiceParser(config.AppSettings.Settings["ApiUri"].Value, config.AppSettings.Settings["SubscriptionKey"].Value);
      var invoice = await parser.ParseInvoiceAsync("..\\..\\..\\IMG_20190413_192812.jpg");

      Assert.AreEqual("NL63ABNA0265980487", invoice.BankAccountNumber);
      Assert.AreEqual("Stationsweg 20, 9726 AZ Groningen, The Netherlands", invoice.IssuerAddress);
      Assert.AreEqual(689.70D, invoice.Amount);
      Assert.AreEqual("401196200", invoice.KvkNumber);
    }
  }
}
