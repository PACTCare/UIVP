

namespace UIVP.Protocol.Console
{
  using System;
  using System.Configuration;

  using UIVP.Protocol.Core.Services;

  class Program
  {
    static void Main(string[] args)
    {
      var imageService = new ImageInvoiceParser(ConfigurationManager.AppSettings["ApiUri"], ConfigurationManager.AppSettings["SubscriptionKey"]);
      var invoice = imageService.ParseInvoiceAsync("D:\\Projects\\Florence\\IMG_20190413_192812.jpg").Result;

      Console.ReadKey();
    }
  }
}
