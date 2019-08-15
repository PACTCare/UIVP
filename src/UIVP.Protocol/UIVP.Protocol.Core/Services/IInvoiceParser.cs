namespace UIVP.Protocol.Core.Services
{
  using System.Threading.Tasks;

  using UIVP.Protocol.Core.Entity;

  public interface IInvoiceParser
  {
    Task<Invoice> ParseInvoiceAsync(string pathToInvoice);
  }
}