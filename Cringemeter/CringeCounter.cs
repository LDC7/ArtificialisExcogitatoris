namespace Cringemeter
{
  using Newtonsoft.Json;
  using System;
  using System.IO;
  using System.Linq;
  using System.Text;

  public class CringeCounter
  {
    private const string FILENAME = "cringecounter";

    private static readonly Lazy<CringeCounter> instance = new Lazy<CringeCounter>(() => new CringeCounter());

    public static CringeCounter Instance => instance.Value;

    public string Crienge(string message)
    {
      var newCringe = new Cringe(DateTime.Now, message);
      int count = Save(newCringe, out Cringe? lastCrienge);
      return lastCrienge.HasValue ? CreateMessage(newCringe, count, lastCrienge.Value) : CreateMessage(newCringe, count);
    }

    private static string CreateMessage(Cringe cringe, int count)
    {
      StringBuilder builder = new StringBuilder();
      builder
        .Append("Произошёл кринж: ")
        .Append(cringe.Message)
        .AppendLine(char.IsSymbol(cringe.Message.Last()) ? "." : string.Empty)
        .AppendLine($"Это {count + 1} кринж на моей памяти.");

       return builder.ToString();
    }

    private static string CreateMessage(Cringe cringe, int count, Cringe lastCrienge)
    {
      StringBuilder builder = new StringBuilder(CreateMessage(cringe, count));
      var interval = cringe.Time - lastCrienge.Time;
      builder
        .AppendLine($"С последнего кринжа прошло: {CreateIntervalMessage(interval)}")
        .Append("Последний кринж был: ")
        .Append($"[{lastCrienge.Time.ToString("U")}+04] ")
        .Append(lastCrienge.Message)
        .AppendLine(char.IsSymbol(cringe.Message.Last()) ? "." : string.Empty);

      return builder.ToString();
    }

    private static string CreateIntervalMessage(TimeSpan interval)
    {
      StringBuilder builder = new StringBuilder();
      builder
        .Append(interval.Days > 0 ? $"{interval.Days} дней " : string.Empty)
        .Append(interval.Hours > 0 || builder.Length > 0 ? $"{interval.Hours} часов " : string.Empty)
        .Append(interval.Minutes > 0 || builder.Length > 0 ? $"{interval.Minutes} минут " : string.Empty)
        .Append($"{interval.Seconds} секунд.");

      return builder.ToString();
    }

    private static int Save(Cringe cringe, out Cringe? last)
    {
      string homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
      string filepath = $@"{homeFolder}\{FILENAME}";

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

    private CringeCounter() { }
  }
}
