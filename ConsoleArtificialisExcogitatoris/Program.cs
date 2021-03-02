namespace ConsoleArtificialisExcogitatoris
{
  using ArtificialisExcogitatoris;
  using Microsoft.Extensions.Hosting;
  using System.Threading.Tasks;

  public class Program
  {
    public static async Task Main(string[] args) => await new ArtificialisExcogitatoris().CreateHost().RunAsync();
  }
}
