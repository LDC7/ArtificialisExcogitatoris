namespace ArtificialDungeon
{
  using ArtificialisExcogitatoris.Base;
  using Discord;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using System.Timers;
  using Translator;

  public class DungeonCommandService : ICommandService
  {
    private const int DUNGEON_LIFETIME_MINUTES = 30;

    private readonly IServiceProvider serviceProvider;

    private Dungeon dungeon;

    private readonly object locker = new object();

    private readonly Timer timer;

    public bool CanExecuteCustomCommand => false;

    public IEnumerable<(string name, string description)> ImplementedCommands { get; } = new[]
    {
      ("enterthedungeon", "создать приключение"),
      ("d", "действие в приключении")
    };

    public Task ExecuteCommand(ExecuteCommandArgs args)
    {
      switch (args.Command)
      {
        case "enterthedungeon":
          return this.EnterTheDungeon(args);

        case "d":
          return this.DungeonAction(args);

        default:
          throw new UnknownCommandException();
      }
    }

    private async Task EnterTheDungeon(ExecuteCommandArgs args)
    {
      string message;
      var name = await this.GetRandomOnlineUserName(args.OurGuild);
      lock (this.locker)
      {
        this.dungeon?.Dispose();
        this.dungeon = new Dungeon();

        var storyDescription = this.dungeon.CreateAdventure(name);
        var translator = this.serviceProvider.GetService<TranslateService>();
        message = translator.Translate(storyDescription, Language.ru).Result;
      }

      await args.Message.Channel.SendMessageAsync(message);
    }

    private Task DungeonAction(ExecuteCommandArgs args)
    {
      string message;
      var action = args.Message.GetMessageContent();
      lock (this.locker)
      {
        var translator = this.serviceProvider.GetService<TranslateService>();
        var engAction = translator.Translate(action, Language.en).Result;
        var response = this.dungeon.Do(engAction);
        message = translator.Translate(response, Language.ru).Result;
      }

      return args.Message.Channel.SendMessageAsync(message);
    }

    private async Task<string> GetRandomOnlineUserName(IGuild guild)
    {
      var users = await guild.GetUsersAsync();
      var user = users.Where(u => u.Status == UserStatus.Online).GetRandomElement();
      return string.IsNullOrWhiteSpace(user.Nickname) ? user.Username : user.Nickname;
    }

    private void CheckDungeonActivity(object sender, ElapsedEventArgs args)
    {
      lock (this.locker)
      {
        if (this.dungeon != null && this.dungeon.LastActivity.AddMinutes(DUNGEON_LIFETIME_MINUTES) < DateTime.Now)
        {
          this.dungeon?.Dispose();
          this.dungeon = null;
        }
      }
    }

    public DungeonCommandService(IServiceProvider provider)
    {
      this.serviceProvider = provider;
      this.timer = new Timer(60000);
      this.timer.Elapsed += this.CheckDungeonActivity;
      this.timer.Enabled = true;
    }
  }
}
