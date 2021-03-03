namespace ArtificialisExcogitatoris
{
  using AppSettings;
  using ArtificialisExcogitatoris.Base;
  using Discord;
  using Discord.WebSocket;
  using Microsoft.Extensions.DependencyInjection;
  using System;
  using System.Linq;
  using System.Threading.Tasks;

  internal class CommandExecutor
  {
    private readonly ulong serverId = AppSettings.Get<ulong>("SERVER_ID");
    private readonly IServiceProvider provider;

    public CommandExecutor(IServiceProvider provider)
    {
      this.provider = provider;
    }

    public async Task ExecuteCommandAsync(IDiscordClient client, SocketMessage socketMessage)
    {
      try
      {
        var command = GetCommand(socketMessage.Content);
        var services = provider.GetServices<ICommandService>();
        var filteredServices = services.Where(s => s.ImplementedCommands.Any(c => c.name == command)).ToArray();

        var args = new ExecuteCommandArgs()
        {
          Command = command,
          Client = client,
          OurGuild = await client.GetGuildAsync(this.serverId),
          Message = socketMessage
        };

        if (filteredServices.Length == 0)
        {
          args.IsCustom = true;
          filteredServices = services.Where(s => s.CanExecuteCustomCommand).ToArray();
        }

        await Task.WhenAll(filteredServices.Select(s => s.ExecuteCommand(args)).ToArray());
      }
      catch (Exception ex)
      {
        (socketMessage.Channel as IMessageChannel)?.SendMessageAsync($"Произошла ошибка: {ex.Message}");
      }
    }

    private static string GetCommand(string messageContent)
    {
      var message = messageContent.Trim();
      var index = message.IndexOf(' ');
      return index != -1 ? message.Substring(1, index - 1) : message.Substring(1);
    }
  }
}
