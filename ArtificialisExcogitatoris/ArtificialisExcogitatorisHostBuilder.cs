namespace ArtificialisExcogitatoris
{
  using AnimeFace;
  using ArtificialDungeon;
  using ArtificialisExcogitatoris.Base;
  using FaceAdder;
  using Haiku;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using SentenceCompleter;
  using Translator;

  public class ArtificialisExcogitatorisHostBuilder
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
        .AddSingleton<TranslateService, YandexTranslateService>()
        .AddTransient<ICommandService, CommonCommandService>()
        .AddTransient<ICommandService, SentenceCompleteCommandService>()
        .AddHostedService<ArtificialisExcogitatorisHost>();
    }
  }
}
