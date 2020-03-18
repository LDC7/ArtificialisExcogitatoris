namespace Translator
{
  using AppSettings;
  using Newtonsoft.Json.Linq;
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Threading.Tasks;
  using System.Web;

  public class YandexTranslator
  {
    private const string URL = "https://translate.yandex.net/api/v1.5/tr.json/translate?";

    private static readonly Lazy<YandexTranslator> instance = new Lazy<YandexTranslator>(() => new YandexTranslator());

    public static YandexTranslator Instance => instance.Value;

    private readonly string apiKey;

    public Task<string> Translate(string text, Language target)
    {
      return Task.Run(() =>
      {
        IDictionary<string, string> parameters = new Dictionary<string, string>
        {
          { "key", this.apiKey },
          { "text", text },
          { "lang", target.ToString() },
          { "options", "0" }
        };

        var url = URL + string.Join("&", parameters.Select(pair => $"{pair.Key}={HttpUtility.UrlEncode(pair.Value)}"));
        WebRequest request = WebRequest.Create(url);

        WebResponse response = request.GetResponseAsync().Result;
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
          JObject jsonResponse = JObject.Parse(reader.ReadToEnd());
          return jsonResponse["text"].First().ToString();
        }
      });
    }

    private YandexTranslator()
    {
      this.apiKey = AppSettings.Get<string>("YANDEX_API_KEY");
    }
  }
}
