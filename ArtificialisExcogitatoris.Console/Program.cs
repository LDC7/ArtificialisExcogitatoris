namespace ArtificialisExcogitatoris.Console
{
  using Microsoft.Extensions.Hosting;
  using System.Threading.Tasks;

  public class Program
  {
    public static async Task Main(string[] args) => await new ArtificialisExcogitatorisHostBuilder().CreateHost().RunAsync();
  }
}
