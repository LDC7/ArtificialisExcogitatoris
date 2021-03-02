namespace ArtificialisExcogitatoris
{
  using AppSettings;
  using Discord;
  using Discord.WebSocket;
  using Microsoft.Extensions.Hosting;
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  internal class ArtificialisExcogitatorisHost : IHostedService
  {
    private readonly string token = AppSettings.Get<string>("BOT_TOKEN");
    private readonly ulong channelId = AppSettings.Get<ulong>("CHANNEL_ID");
    private readonly string adminName = AppSettings.Get<string>("ADMIN_NAME");
    private DiscordSocketClient client;
    private readonly IServiceProvider serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      this.client = new DiscordSocketClient(new DiscordSocketConfig()
      {
        LogLevel = LogSeverity.Critical
      });

      this.client.MessageReceived += this.Client_MessageLog;
      this.client.MessageReceived += this.Client_MessageReceived;
      this.client.Log += this.Client_Log;

      await this.client.LoginAsync(TokenType.Bot, this.token);

      Console.WriteLine($"[{DateTime.Now}] START!");
      await this.client.StartAsync();
    }

    private async Task Client_Log(LogMessage logMessage)
    {
      Console.WriteLine($"[{DateTime.Now}] {logMessage.Source}: {logMessage.Message}");
    }

    private async Task Client_MessageLog(SocketMessage socketMessage)
    {
      Console.WriteLine($"[{DateTime.Now}] [{socketMessage.Author}] {socketMessage.Content}");
    }

    private async Task Client_MessageReceived(SocketMessage socketMessage)
    {
      if (this.ListeningValidation(socketMessage) && socketMessage.Content.StartsWith('!'))
      {
        var commandExecutor = (CommandExecutor)this.serviceProvider.GetService(typeof(CommandExecutor));
        await commandExecutor.ExecuteCommandAsync(this.client, socketMessage);
      }
    }

    private bool ListeningValidation(SocketMessage socketMessage)
    {
      return !socketMessage.Author.IsBot
        && (socketMessage.Channel.Id == this.channelId
        || (socketMessage.Channel is IDMChannel && socketMessage.Author.Username == this.adminName));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      return this.client.StopAsync();
    }

    public ArtificialisExcogitatorisHost(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }
  }
}
