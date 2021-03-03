namespace Cringemeter
{
  using ArtificialisExcogitatoris.Base;
  using Newtonsoft.Json;
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public class CringeCommandService : ICommandService
  {
    private const string FILENAME = "cringecounter";

    public bool CanExecuteCustomCommand => false;

    public IEnumerable<(string name, string description)> ImplementedCommands { get; } = new[]
    {
      ("cringe", "CringeLord сделал свой ход")
    };

    public Task ExecuteCommand(ExecuteCommandArgs args)
    {
      if (args.Command != "cringe")
        throw new UnknownCommandException();

      var content = args.Message.GetMessageContent();
      var newCringe = new Cringe(DateTime.Now, content);
      int count = Save(newCringe, out Cringe? lastCrienge);
      var message = lastCrienge.HasValue ? CreateMessage(newCringe, count, lastCrienge.Value) : CreateMessage(newCringe, count);

      return args.Message.Channel.SendMessageAsync(message);
    }

    private static string CreateMessage(Cringe cringe, int count)
    {
      var builder = new StringBuilder()
        .Append("Произошёл кринж: ")
        .Append(cringe.Message)
        .AppendLine(char.IsSymbol(cringe.Message.Last()) ? "." : string.Empty)
        .Append("Это ")
        .Append(count + 1)
        .AppendLine(" кринж на моей памяти.");

      return builder.ToString();
    }

    private static string CreateMessage(Cringe cringe, int count, Cringe lastCrienge)
    {
      var interval = cringe.Time - lastCrienge.Time;
      var builder = new StringBuilder(CreateMessage(cringe, count))
        .Append("С последнего кринжа прошло: ")
        .AppendLine(CreateIntervalMessage(interval))
        .Append("Последний кринж был: ")
        .Append('[')
        .Append(lastCrienge.Time.ToString("U"))
        .Append("+04] ")
        .Append(lastCrienge.Message)
        .AppendLine(char.IsSymbol(cringe.Message.Last()) ? "." : string.Empty);

      return builder.ToString();
    }

    private static string CreateIntervalMessage(TimeSpan interval)
    {
      var builder = new StringBuilder();
      builder
        .Append(interval.Days > 0 ? $"{interval.Days} дней " : string.Empty)
        .Append(interval.Hours > 0 || builder.Length > 0 ? $"{interval.Hours} часов " : string.Empty)
        .Append(interval.Minutes > 0 || builder.Length > 0 ? $"{interval.Minutes} минут " : string.Empty)
        .Append(interval.Seconds)
        .Append(" секунд.");

      return builder.ToString();
    }

    private static int Save(Cringe cringe, out Cringe? last)
    {
      var homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
      var filepath = $@"{homeFolder}\{FILENAME}";

      string[] data = null;
      if (File.Exists(filepath))
        data = File.ReadAllLines(filepath);

      File.AppendAllLines(filepath, new string[] { JsonConvert.SerializeObject(cringe) });

      last = null;
      if (data == null || data.Length == 0)
        return 0;

      last = JsonConvert.DeserializeObject<Cringe>(data.Last());

      return data.Length;
    }
  }
}
