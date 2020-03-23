namespace ArtificialisExcogitatoris
{
  using Discord;
  using Discord.WebSocket;
  using System;
  using System.Threading.Tasks;

  internal class CommandExecutor
  {
    private readonly DiscordSocketClient client;
    private readonly ulong serverId;

    public CommandExecutor(DiscordSocketClient socketClient, ulong serverId)
    {
      this.client = socketClient;
      this.serverId = serverId;
    }

    public async Task ExecuteCommandAsync(SocketMessage socketMessage)
    {
      try
      {
        var message = socketMessage.Content.Trim();
        var index = message.IndexOf(' ');
        var command = index != -1 ? message.Substring(1, index - 1) : message.Substring(1);
        new CommandContainer(this.client, this.serverId, socketMessage).Execute(command);
      }
      catch (Exception ex)
      {
        (socketMessage.Channel as IMessageChannel)?.SendMessageAsync($"Произошла ошибка: {ex.Message}");
      }
    }
  }
}
