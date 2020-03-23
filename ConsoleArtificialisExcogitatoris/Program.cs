namespace ConsoleArtificialisExcogitatoris
{
  using ArtificialisExcogitatoris;
  using System;

  public class Program
  {
    [STAThread]
    public static void Main(string[] args) => new ArtificialisExcogitatoris().StartAsync().GetAwaiter().GetResult();
  }
}
