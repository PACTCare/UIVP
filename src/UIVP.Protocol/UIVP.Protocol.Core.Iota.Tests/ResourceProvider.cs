namespace UIVP.Protocol.Core.Iota.Tests
{
  using System.Collections.Generic;

  using Tangle.Net.ProofOfWork;
  using Tangle.Net.Repository;
  using Tangle.Net.Repository.Client;

  public static class ResourceProvider
  {
    public static IIotaRepository Repository =>
      new RestIotaRepository(
        new FallbackIotaClient(new List<string> { "https://nodes.devnet.thetangle.org:443" }, 5000),
        new PoWService(new CpuPearlDiver()));
  }
}