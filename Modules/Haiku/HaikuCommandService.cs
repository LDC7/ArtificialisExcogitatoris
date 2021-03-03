namespace Haiku
{
  using ArtificialisExcogitatoris.Base;
  using Discord;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public class HaikuCommandService : ICommandService
  {
    private const string FOLDER = "HaikuGenerator";
    private const string BEGININGFILE = "BeginingHaiku.txt";
    private const string MIDDLEFILE = "MiddleHaiku.txt";
    private const string ENDINGFILE = "EndingHaiku.txt";
    private readonly IEnumerable<string> begining;
    private readonly IEnumerable<string> middle;
    private readonly IEnumerable<string> ending;

    public bool CanExecuteCustomCommand => false;

    public IEnumerable<(string name, string description)> ImplementedCommands { get; } = new[]
    {
      ("хокку", "составить рандомное хокку")
    };

    public async Task ExecuteCommand(ExecuteCommandArgs args)
    {
      if (args.Command != "хокку")
        throw new UnknownCommandException();

      var name = await this.GetRandomOnlineUserName(args.OurGuild);
      await args.Message.Channel.SendMessageAsync(this.GenerateHaiku(name));
    }

    private async Task<string> GetRandomOnlineUserName(IGuild guild)
    {
      var users = await guild.GetUsersAsync();
      var user = users.Where(u => u.Status == UserStatus.Online).GetRandomElement();
      return string.IsNullOrWhiteSpace(user.Nickname) ? user.Username : user.Nickname;
    }

    public string GenerateHaiku(string name)
    {
      var sb = new StringBuilder()
        .AppendFormat(begining.GetRandomElement(), name)
        .Append('\n')
        .Append(middle.GetRandomElement())
        .Append('\n')
        .Append(ending.GetRandomElement());

      return sb.ToString();
    }

    public HaikuCommandService()
    {
      using (var stream1 = new FileStream($"{FOLDER}\\{BEGININGFILE}", FileMode.Open))
      using (var stream2 = new FileStream($"{FOLDER}\\{MIDDLEFILE}", FileMode.Open))
      using (var stream3 = new FileStream($"{FOLDER}\\{ENDINGFILE}", FileMode.Open))
      using (var reader1 = new StreamReader(stream1))
      using (var reader2 = new StreamReader(stream2))
      using (var reader3 = new StreamReader(stream3))
      {
        begining = reader1.ReadToEnd().Split('\n');
        middle = reader2.ReadToEnd().Split('\n');
        ending = reader3.ReadToEnd().Split('\n');
      }
    }
  }
}
