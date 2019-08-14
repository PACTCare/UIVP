namespace UIVP.Protocol.Core.Iota.Tests
{
  using System;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using UIVP.Protocol.Core.Entity;

  [TestClass]
  public class ExtensionTest
  {
    [TestMethod]
    public void TestTryteMetadataConversion()
    {
      var hash = Convert.FromBase64String("EkCQYgCeyPNl9ZeNfyI92lMpQYyDkOQMEG1fN4MXr3zRUwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
      var invoice = new InvoiceMetadata(hash, hash);

      var trytes = invoice.ToTryteString();
      var backFromTrytes = trytes.ToInvoiceMetadata();

      Assert.AreEqual("EkCQYgCeyPNl9ZeNfyI92lMpQYyDkOQMEG1fN4MXr3zRUwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", Convert.ToBase64String(backFromTrytes.Hash));
      Assert.AreEqual("EkCQYgCeyPNl9ZeNfyI92lMpQYyDkOQMEG1fN4MXr3zRUwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", Convert.ToBase64String(backFromTrytes.Signature));
    }
  }
}