namespace UIVP.Protocol.Core.Services
{
  using System.Security.Cryptography;

  public static class Encryption
  {
    public static CngKey CreateKey(CngAlgorithm algorithm = null)
    {
      var keyCreationParameters = new CngKeyCreationParameters
                                    {
                                      ExportPolicy = CngExportPolicies.AllowPlaintextExport, KeyUsage = CngKeyUsages.Signing
                                    };

      return CngKey.Create(algorithm ?? CngAlgorithm.ECDsaP256, null, keyCreationParameters);
    }

    public static ECDsaCng CreateSignatureScheme(CngKey key = null, CngAlgorithm algorithm = null)
    {
      return new ECDsaCng(key ?? CreateKey()) { HashAlgorithm = algorithm ?? CngAlgorithm.Sha256 };
    }
  }
}