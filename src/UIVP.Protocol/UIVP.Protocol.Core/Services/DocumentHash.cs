namespace UIVP.Protocol.Core.Services
{
  using System.Security.Cryptography;

  public static class DocumentHash
  {
    public static byte[] Create(byte[] document)
    {
      using (var sha256Hash = SHA256.Create())
      {
        return sha256Hash.ComputeHash(document);
      }
    }
  }
}
