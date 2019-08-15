namespace UIVP.Protocol.Core.Repository
{
  using System;
  using System.Net;
  using System.Security.Cryptography;
  using System.Threading.Tasks;

  using Newtonsoft.Json;

  using RestSharp;

  public class KvkPublicKeyRepository : IPublicKeyRepository
  {
    private IRestClient Client { get; }

    public KvkPublicKeyRepository(IRestClient client)
    {
      this.Client = client;
    }

    /// <inheritdoc />
    public async Task<byte[]> GetCompanyPublicKeyAsync(string kvkNumber)
    {
      var request = new RestRequest($"api/Handelsregister/by-kvknumber/{kvkNumber}", Method.GET);
      var response = await this.Client.ExecuteTaskAsync(request);

      if (response.StatusCode != HttpStatusCode.OK)
      {
        return new byte[0];
      }

      var parsedResponse = JsonConvert.DeserializeObject<JsonObject>(response.Content);
      parsedResponse.TryGetValue("publicKey", out var publicKey);

      return publicKey != null ? Convert.FromBase64String(publicKey.ToString()) : new byte[0];
    }

    /// <inheritdoc />
    public async Task RegisterCompanyPublicKeyAsync(string kvkNumber, CngKey key)
    {
      var publicKey = key.Export(CngKeyBlobFormat.EccFullPublicBlob);
      var publicKeyPayload = Convert.ToBase64String(publicKey);

      var request = new RestRequest($"api/PublicKeys/add/{kvkNumber}", Method.POST);
      request.AddParameter("publicKey", publicKeyPayload);

      await this.Client.ExecuteTaskAsync(request);
    }
  }
}
