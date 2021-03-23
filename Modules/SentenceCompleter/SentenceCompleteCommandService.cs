namespace SentenceCompleter
{
  using ArtificialisExcogitatoris.Base;
  using Newtonsoft.Json.Linq;
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Text;
  using System.Threading.Tasks;

  public class SentenceCompleteCommandService : ICommandService
  {
    private const string URL = @"https://pelevin.gpt.dobro.ai/generate/";

    public bool CanExecuteCustomCommand => false;

    public IEnumerable<(string name, string description)> ImplementedCommands { get; } = new[]
    {
      ("story", "дополнить историю")
    };

    public Task ExecuteCommand(ExecuteCommandArgs args)
    {
      if (args.Command != "story")
        throw new UnknownCommandException();

      var begining = args.Message.GetMessageContent();
      if (!string.IsNullOrWhiteSpace(begining))
      {
        var smile = args.OurGuild.Emotes.GetRandomElement();
        return args.Message.Channel.SendMessageAsync($"{Complete(begining)} {smile}");
      }

      throw new Exception("Нужно задать начало предложения.");
    }

    private static string Complete(string begining)
    {
      string fullStory = begining;
      WebRequest request = WebRequest.Create(URL);
      request.Method = "POST";
      request.ContentType = "text/plain;charset=UTF-8";
      request.Timeout = 10000;

      byte[] byteArray = CreateRequestPayload(begining);
      request.ContentLength = byteArray.Length;

      using (Stream dataStream = request.GetRequestStream())
      {
        dataStream.Write(byteArray, 0, byteArray.Length);
      }

      WebResponse response = request.GetResponseAsync().Result;
      using (Stream stream = response.GetResponseStream())
      using (StreamReader reader = new StreamReader(stream))
      {
        JObject jsonResponse = JObject.Parse(reader.ReadToEnd());
        JArray answers = (JArray)jsonResponse["replies"];
        fullStory += answers.FirstOrDefault()?.ToString();
      }
      response.Close();

      return fullStory;
    }

    private static byte[] CreateRequestPayload(string prompt)
    {
      string payload = "{ \"prompt\": \"" + prompt + "\", \"length\": 30, \"num_samples\": 1}";
      return Encoding.UTF8.GetBytes(payload);
    }
  }
}
