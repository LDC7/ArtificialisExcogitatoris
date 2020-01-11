namespace ArtificialisExcogitatoris
{
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Text;
  using Newtonsoft.Json.Linq;

  internal static class StoryMaker
  {
    const string URL = @"https://models.dobro.ai/gpt2/medium/";

    internal static string Complete(string begining)
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
      {
        using (StreamReader reader = new StreamReader(stream))
        {
          JObject jsonResponse = JObject.Parse(reader.ReadToEnd());
          JArray answers = (JArray)jsonResponse["replies"];
          fullStory += answers.FirstOrDefault()?.ToString();
        }
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