namespace ArtificialisExcogitatoris
{
  using AppSettings;
  using Discord;
  using Discord.WebSocket;
  using System;
  using System.Threading.Tasks;

  public class ArtificialisExcogitatoris
  {
    private readonly string token = AppSettings.Get<string>("BOT_TOKEN");
    private readonly ulong channelId = AppSettings.Get<ulong>("CHANNEL_ID");
    private readonly ulong serverId = AppSettings.Get<ulong>("SERVER_ID");
    private DiscordSocketClient client;
    private CommandExecutor commandExecutor;

    public async Task StartAsync()
    {
      this.client = new DiscordSocketClient(new DiscordSocketConfig()
      {
        LogLevel = LogSeverity.Critical
      });

      this.client.MessageReceived += this.Client_MessageReceived;
      this.client.Log += this.Client_Log;

      await this.client.LoginAsync(TokenType.Bot, this.token);

      this.commandExecutor = new CommandExecutor(this.client, this.serverId);

      await this.client.StartAsync();
      Console.WriteLine($"[{DateTime.Now}] START!");
      await Task.Delay(-1);
    }

    private async Task Client_Log(LogMessage logMessage)
    {
      Console.WriteLine($"[{DateTime.Now}] {logMessage.Source}: {logMessage.Message}");
    }

    private async Task Client_MessageReceived(SocketMessage socketMessage)
    {
      Console.WriteLine($"[{DateTime.Now}] [{socketMessage.Author}] {socketMessage.Content}");
      if (socketMessage.Author.Username == "mrLDC")
      {
        await this.commandExecutor.ExecuteCommandAsync(socketMessage);
      }
      else
      {
        if (this.ListeningValidation(socketMessage))
        {
          if (socketMessage.Content.StartsWith('!'))
          {
            await this.commandExecutor.ExecuteCommandAsync(socketMessage);
          }
          else
          {

          }
        }
      }
    }

    private bool ListeningValidation(SocketMessage socketMessage)
    {
      return !socketMessage.Author.IsBot && socketMessage.Channel.Id == this.channelId;
    }
  }
}
