namespace UIVP.Protocol.Core.Tests.Unit.Entity
{
  using System;
  using System.Text;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Multiformats.Hash;

  using UIVP.Protocol.Core.Entity;

  [TestClass]
  public class InvoiceTest
  {
    [TestMethod]
    public void TestSuccessfulHashing()
    {
      var invoice = new Invoice
                      {
                        KvkNumber = "1231455", IssuerAddress = "Somestreet 123, 1011AB Sometown", Amount = 9.99D, BankAccountNumber = "NLAKDJADKJHD"
                      };

      var hash = invoice.CreateHash(HashType.SHA2_256);

      Assert.AreEqual("EkCQYgCeyPNl9ZeNfyI92lMpQYyDkOQMEG1fN4MXr3zRUwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", Convert.ToBase64String(hash));
    }
  }
}