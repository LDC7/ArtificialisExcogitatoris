namespace ArtificialisExcogitatoris
{
  using AnimeFace;
  using ArtificialDungeon;
  using FaceAdder;
  using global::ArtificialisExcogitatoris.Base;
  using Haiku;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using SentenceCompleter;

  public class ArtificialisExcogitatoris
  {
    public IHost CreateHost()
    {
      var host = new HostBuilder()
        .ConfigureServices(this.ConfigureServices)
        .Build();

      return host;
    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
      services
        .AddSingleton<CommandExecutor>()
        .AddSingleton<ICommandService, HaikuCommandService>()
        .AddSingleton<ICommandService, FaceCommandService>()
        .AddSingleton<ICommandService, AnimeFaceCommandService>()
        .AddSingleton<ICommandService, DungeonCommandService>()
        .AddTransient<ICommandService, CommonCommandService>()
        .AddTransient<ICommandService, SentenceCompleteCommandService>()
        .AddHostedService<ArtificialisExcogitatorisHost>();
    }
  }
}
