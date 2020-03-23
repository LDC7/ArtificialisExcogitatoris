namespace Haiku
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;

  public static class HaikuGenerator
  {
    private const string FOLDER = "HaikuGenerator";
    private const string BEGININGFILE = "BeginingHaiku.txt";
    private const string MIDDLEFILE = "MiddleHaiku.txt";
    private const string ENDINGFILE = "EndingHaiku.txt";
    private static IEnumerable<string> begining;
    private static IEnumerable<string> middle;
    private static IEnumerable<string> ending;

    static HaikuGenerator()
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

    public static string Generate(string name)
    {
      StringBuilder sb = new StringBuilder();

      sb.Append(string.Format(begining.OrderBy(s => Guid.NewGuid()).First(), name));
      sb.Append('\n');
      sb.Append(middle.OrderBy(s => Guid.NewGuid()).First());
      sb.Append('\n');
      sb.Append(ending.OrderBy(s => Guid.NewGuid()).First());

      return sb.ToString();
    }
  }
}
