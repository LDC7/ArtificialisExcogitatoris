namespace ArtificialDungeon
{
  using System;
  using System.Timers;
  using Translator;

  public class ArtificialDungeon
  {
    private const int DUNGEON_LIFETIME_MINUTES = 30;

    private static readonly Lazy<ArtificialDungeon> instance = new Lazy<ArtificialDungeon>(() => new ArtificialDungeon());

    public static ArtificialDungeon Instance => instance.Value;

    private Dungeon dungeon;

    private readonly object locker = new object();

    private readonly Timer timer;

    public string EnterTheDungeon(string heroName)
    {
      lock (this.locker)
      {
        this.dungeon?.Dispose();
        this.dungeon = new Dungeon();

        var storyDescription = this.dungeon.CreateAdventure(heroName);
        return YandexTranslator.Instance.Translate(storyDescription, Language.ru).Result;
      }
    }

    public string Act(string action)
    {
      lock (this.locker)
      {
        var engAction = YandexTranslator.Instance.Translate(action, Language.en).Result;
        var response = this.dungeon.Do(engAction);
        return YandexTranslator.Instance.Translate(response, Language.ru).Result;
      }
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

    private ArtificialDungeon()
    {
      this.timer = new Timer(60000);
      this.timer.Elapsed += new ElapsedEventHandler(this.CheckDungeonActivity);
      this.timer.Enabled = true;
    }
  }
}
