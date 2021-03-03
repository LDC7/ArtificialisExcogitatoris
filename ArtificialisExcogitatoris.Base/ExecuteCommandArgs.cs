namespace ArtificialisExcogitatoris.Base
{
  using Discord;

  public class ExecuteCommandArgs
  {
    public string Command { get; set; }

    public bool IsCustom { get; set; }

    public IDiscordClient Client { get; set; }

    public IGuild OurGuild { get; set; }

    public IMessage Message { get; set; }
  }
}
