namespace UIVP.Protocol.Core.Tests.Unit.Entity
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Multiformats.Hash;

  using Tangle.Net.Entity;

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

      Assert.AreEqual(
        "R9JBIEQC99WEKG9ITCBIPEFESDGAGBBHBCNAKBEEWDIELHL9P9ADNCABWDW9MFPDTGBC9999999999999999999999999999999999999999999999999999999999999999",
        TryteString.FromBytes(hash).Value);
    }
  }
}