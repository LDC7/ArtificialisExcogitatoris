namespace ArtificialisExcogitatoris
{
  using Discord;
  using Discord.WebSocket;
  using System;
  using System.Threading.Tasks;

  public class Program
  {
    private readonly string token = "NTc1MzczMzc1OTA4NzQxMTI1.XNO5uA.kmpdVMZFGL-YEI1hlfcexxhL_LM";
    private readonly ulong channelId = 574251966440407043;
    private readonly ulong serverId = 488731299406938132;
    private DiscordSocketClient client;
    private CommandExecutor commandExecutor;
    private DockflowWorker dockflowWorker;

    [STAThread]
    public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

    private async Task MainAsync()
    {
      this.client = new DiscordSocketClient(new DiscordSocketConfig()
      {
        LogLevel = LogSeverity.Critical
      });

      this.client.MessageReceived += this.Client_MessageReceived;
      this.client.Log += this.Client_Log;

      await this.client.LoginAsync(TokenType.Bot, this.token);

      this.commandExecutor = new CommandExecutor(this.client, this.channelId, this.serverId);
      this.dockflowWorker = new DockflowWorker();

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
        /*var messageChannel = socketMessage.Channel as IMessageChannel;
        if (messageChannel != null)
            await messageChannel.SendMessageAsync(this.dockflowWorker.CreateAnswer(socketMessage));*/
        await this.commandExecutor.ExecuteCommandAsync(socketMessage);
      }
      else


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

    private bool ListeningValidation(SocketMessage socketMessage)
    {
      return socketMessage.Author.IsBot == false
          && socketMessage.Channel.Id == this.channelId;
    }
  }
}
