namespace ArtificialisExcogitatoris
{
  using ArtificialDungeon;
  using Discord;
  using Discord.WebSocket;
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;

  public class CommandExecutor
  {
    private readonly DiscordSocketClient client;
    private readonly ulong channelId;
    private readonly ulong serverId;
    private SocketMessage message;
    private IReadOnlyCollection<GuildEmote> serverSmiles;
    private Random random;

    public CommandExecutor(DiscordSocketClient socketClient, ulong channelId, ulong serverId)
    {
      this.client = socketClient;
      this.channelId = channelId;
      this.serverId = serverId;
      this.random = new Random();
    }

    public async Task ExecuteCommandAsync(SocketMessage socketMessage)
    {
      try
      {
        this.message = socketMessage;
        var index = this.message.Content.IndexOf(' ');
        var command = index != -1 ? this.message.Content.Substring(0, index) : this.message.Content;
        switch (command)
        {
          case "!help":
            await this.Help();
            break;
          case "!хокку":
            await this.Haiku();
            break;
          case "!facehelp":
            await this.Facehelp();
            break;
          case "!story":
            await this.MakeStory();
            break;
          case "!kawaii":
            await this.AnimeConvertion();
            break;
          case "!enterthedungeon":
            await this.EnterTheDungeon();
            break;
          case "!d":
            await this.DungeonAction();
            break;
          default:
            if (this.Face(command)) break;
            break;
        }
      }
      catch (Exception ex)
      {
        (this.message.Channel as IMessageChannel)?.SendMessageAsync($"Произошла ошибка: {ex.Message}");
      }
    }

    private async Task Help()
    {
      const string helpMessage = "1) !help  - помощь\n" +
        "2) !хокку - составить рандомное хокку\n" +
        "3) !facehelp  - список 'лицевых' команд\n" +
        "4) !story - дополнить историю\n" +
        "5) !kawaii - конвертация в аниме\n" +
        "6) !enterthedungeon - создать приключение\n" +
        "7) !d - действие в приключении";

      if (this.message.Channel is IMessageChannel messageChannel)
      {
        await messageChannel.SendMessageAsync(helpMessage);
      }
    }

    private async Task Haiku()
    {
      if (this.message.Channel is IMessageChannel messageChannel)
      {
        var name = this.GetRandomOnlineUserName();
        await messageChannel.SendMessageAsync(HaikuGenerator.Generate(name));
      }
    }

    private async Task Facehelp()
    {
      if (this.message.Channel is IMessageChannel messageChannel)
      {
        await messageChannel.SendMessageAsync(FaceWorker.GetFacesDescriptions());
      }
    }

    private bool Face(string command)
    {
      if (!command.Contains("face"))
        return false;

      if (this.message.Channel is IMessageChannel messageChannel)
      {
        Tuple<Stream, string> result = null;
        string url = this.GetMessageContent();

        if (command == "!face")
        {
          result = FaceWorker.AddFaceFromUrl(url);
        }
        else
        {
          result = FaceWorker.AddFaceFromUrl(url, command.Substring(1));
        }

        if (result != null)
        {
          this.SendImage(messageChannel, result.Item1, result.Item2);
        }
        else
        {
          messageChannel.SendMessageAsync("У меня не получилось...");
        }
      }

      return true;
    }

    private async void SendImage(IMessageChannel messageChannel, Stream stream, string filename = "file.png")
    {
      try
      {
        if (messageChannel != null && stream != null)
        {
          await messageChannel.SendFileAsync(stream, filename);
        }
      }
      finally
      {
        stream.Dispose();
      }
    }

    private async Task MakeStory()
    {
      var begining = this.message.Content.Substring(this.message.Content.IndexOf(' ') + 1);
      if (!string.IsNullOrWhiteSpace(begining) && this.message.Channel is IMessageChannel messageChannel)
      {
        if (this.serverSmiles == null)
          this.serverSmiles = this.client.GetGuild(this.serverId).Emotes;

        var smile = this.serverSmiles.ElementAt(this.random.Next(0, this.serverSmiles.Count()));
        await messageChannel.SendMessageAsync($"{StoryMaker.Complete(begining)} {smile.ToString()}");
      }
    }

    private async Task AnimeConvertion()
    {
      if (this.message.Channel is IMessageChannel messageChannel)
      {
        var result = AnimeConverter.ConvertFace(this.GetMessageContent());
        if (result != null)
        {
          this.SendImage(messageChannel, result, "anime.jpg");
        }
        else
        {
          await messageChannel.SendMessageAsync("У меня не получилось...");
        }
      }
    }

    private async Task EnterTheDungeon()
    {
      if (this.message.Channel is IMessageChannel messageChannel)
      {
        var name = this.GetRandomOnlineUserName();
        await messageChannel.SendMessageAsync(ArtificialDungeon.Instance.EnterTheDungeon(name));
      }
    }

    private async Task DungeonAction()
    {
      if (this.message.Channel is IMessageChannel messageChannel)
      {
        var text = this.GetMessageContent();
        await messageChannel.SendMessageAsync(ArtificialDungeon.Instance.Act(text));
      }
    }

    private string GetMessageContent()
    {
      if (this.message.Attachments.Count >= 1)
      {
        var attachment = this.message.Attachments.FirstOrDefault();
        if (attachment != null)
        {
          return attachment.Url;
        }
      }

      return this.message.Content.Substring(this.message.Content.IndexOf(' ') + 1);
    }

    private string GetRandomOnlineUserName()
    {
      var onlineUsers = this.client
        .GetGuild(this.serverId)
        .Users
        .Where(u => u.Status == UserStatus.Online);

      var user = onlineUsers.ElementAt(new Random().Next(0, onlineUsers.Count()));
      return string.IsNullOrWhiteSpace(user.Nickname) ? user.Username : user.Nickname;
    }
  }
}
