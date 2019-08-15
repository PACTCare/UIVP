namespace UIVP.Protocol.Core.Services
{
  using System.IO;
  using System.Linq;
  using System.Net.Http;
  using System.Net.Http.Headers;
  using System.Threading.Tasks;

  using Newtonsoft.Json;

  using UIVP.Protocol.Core.Entity;
  using UIVP.Protocol.Image.Recognition;

  public class ImageInvoiceParser : IInvoiceParser
  {
    private string SubscriptionKey { get; }

    private string ApiUri { get; }

    public ImageInvoiceParser(string apiUri, string subscriptionKey)
    {
      this.SubscriptionKey = subscriptionKey;
      this.ApiUri = apiUri;
    }

    /// <inheritdoc />
    public async Task<Invoice> ParseInvoiceAsync(string pathToInvoice)
    {
      var client = new HttpClient();

      // Request headers.
      client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.SubscriptionKey);

      var requestParameters = "language=unk&detectOrientation=true";
      var uri = this.ApiUri + "?" + requestParameters;

      HttpResponseMessage response;
      var byteData = GetImageAsByteArray(pathToInvoice);

      using (var content = new ByteArrayContent(byteData))
      {
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        response = await client.PostAsync(uri, content);
      }

      var contentString = await response.Content.ReadAsStringAsync();
      var parsedImage = JsonConvert.DeserializeObject<ParsedImage>(contentString);

      return ExtractInvoice(parsedImage);
    }

    private static Invoice ExtractInvoice(ParsedImage parsedImage)
    {
      var kvkNumber = string.Empty;
      var bankAccount = string.Empty;
      var address = string.Empty;
      var amount = 0.00d;

      foreach (var imageRegion in parsedImage.regions)
      {
        foreach (var line in imageRegion.lines)
        {
          if (line.words.Any(l => l.text.ToLower().Contains("kvk")))
          {
            kvkNumber = line.words[1].text;
          }

          if (line.words.Any(l => l.text.ToLower().Contains("bank")))
          {
            line.words.Skip(1).Take(line.words.Count - 1).ToList().ForEach(w => bankAccount += w.text);
          }

          if (line.words.Any(l => l.text.ToLower().Contains("address")))
          {
            line.words.Skip(1).Take(line.words.Count - 1).ToList().ForEach(w => address += $" {w.text}");
          }

          if (line.words.Any(l => l.text.ToLower() == "total"))
          {
            var possibleAmount = imageRegion.lines.Last().words.Last().text;

            if (possibleAmount.Contains('.'))
            {
              double.TryParse(possibleAmount.Replace('.', ','), out amount);
            }
          }
        }
      }

      return new Invoice { KvkNumber = kvkNumber, BankAccountNumber = bankAccount, IssuerAddress = address.TrimStart(' '), Amount = amount };
    }

    private static byte[] GetImageAsByteArray(string imageFilePath)
    {
      using (var fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
      {
        using (var binaryReader = new BinaryReader(fileStream))
        {
          return binaryReader.ReadBytes((int)fileStream.Length);
        }
      }
    }
  }
}