namespace ArtificialisExcogitatoris
{
  using AnimeFace;
  using ArtificialDungeon;
  using Discord;
  using Discord.WebSocket;
  using FaceAdder;
  using Haiku;
  using SentenceCompleter;
  using System;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Text;
  using System.Threading.Tasks;

  internal class CommandContainer
  {
    private readonly SocketMessage message;
    private readonly DiscordSocketClient client;
    private readonly ulong serverId;

    private IMessageChannel MessageChannel => this.message.Channel;

    [Command("help", "помощь")]
    protected async Task Help()
    {
      var attributes = this.GetType()
        .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
        .SelectMany(methodInfo => methodInfo.GetCustomAttributes(typeof(CommandAttribute), true))
        .Select(attribute => attribute as CommandAttribute)
        .Where(attribute => attribute != null);

      int index = 1;
      StringBuilder sb = new StringBuilder();
      foreach (var attribute in attributes.Where(a => a.Name != null))
      {
        sb.Append(index++)
          .Append(") !")
          .Append(attribute.Name)
          .Append(" - ")
          .Append(attribute.Description)
          .AppendLine();
      }

      await this.MessageChannel.SendMessageAsync(sb.ToString());
    }

    [Command("хокку", "составить рандомное хокку")]
    protected async Task Haiku()
    {
      var name = this.GetRandomOnlineUserName();
      await this.MessageChannel.SendMessageAsync(HaikuGenerator.Generate(name));
    }

    [Command("facehelp", "список 'лицевых' команд")]
    protected async Task Facehelp()
    {
      await this.MessageChannel.SendMessageAsync(FaceWorker.FacesDescriptions);
    }

    [Command("story", "дополнить историю")]
    protected async Task MakeStory()
    {
      var begining = this.GetMessageContent();
      if (!string.IsNullOrWhiteSpace(begining))
      {
        var serverSmiles = this.client.GetGuild(this.serverId).Emotes;
        var smile = serverSmiles.ElementAt(new Random().Next(0, serverSmiles.Count()));
        await this.MessageChannel.SendMessageAsync($"{StoryMaker.Complete(begining)} {smile.ToString()}");
      }
    }

    [Command("kawaii", "конвертация в аниме")]
    protected async Task AnimeConvertion()
    {
      var result = AnimeConverter.ConvertFace(this.GetMessageContent());
      if (result != null)
      {
        this.SendImage(result, "anime.jpg");
      }
      else
      {
        await this.MessageChannel.SendMessageAsync("У меня не получилось...");
      }
    }

    [Command("enterthedungeon", "создать приключение")]
    protected async Task EnterTheDungeon()
    {
      var name = this.GetRandomOnlineUserName();
      await this.MessageChannel.SendMessageAsync(ArtificialDungeon.Instance.EnterTheDungeon(name));
    }

    [Command("d", "действие в приключении")]
    protected async Task DungeonAction()
    {
      var text = this.GetMessageContent();
      await this.MessageChannel.SendMessageAsync(ArtificialDungeon.Instance.Act(text));
    }

    [Command(null)]
    protected void Face(string command)
    {
      if (!command.Contains("face"))
        return;

      string url = this.GetMessageContent();
      Tuple<byte[], string> result;
      if (command == "face")
        result = FaceWorker.AddFaceFromUrl(url);
      else
        result = FaceWorker.AddFaceFromUrl(url, command);

      if (result != null)
        this.SendImage(result.Item1, result.Item2);
      else
        this.MessageChannel.SendMessageAsync("У меня не получилось...");
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

    private async void SendImage(byte[] data, string filename = "file.png")
    {
      using (Stream stream = new MemoryStream(data))
      {
        await this.MessageChannel.SendFileAsync(stream, filename);
      }
    }

    public void Execute(string command)
    {
      var methods = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
      var method = methods.FirstOrDefault(methodInfo => methodInfo
          .GetCustomAttributes(typeof(CommandAttribute), true)
          .Any(attribute => attribute is CommandAttribute commandAttribute && commandAttribute.Name == command));

      if (method != null)
        method.Invoke(this, Array.Empty<object>());
      else
        this.ExecuteDefaultMethod(command);
    }

    private void ExecuteDefaultMethod(string command)
    {
      var methods = this.GetType()
        .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
        .Where(methodInfo => methodInfo
          .GetCustomAttributes(typeof(CommandAttribute), true)
          .Any(attribute => attribute is CommandAttribute commandAttribute && commandAttribute.Name == null));

      foreach (var method in methods)
        method.Invoke(this, new object[] { command });
    }

    public CommandContainer(DiscordSocketClient socketClient, ulong serverId, SocketMessage socketMessage)
    {
      this.client = socketClient;
      this.serverId = serverId;
      this.message = socketMessage;
    }
  }
}
