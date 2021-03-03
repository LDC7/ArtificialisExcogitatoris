namespace Translator
{
  using AppSettings;
  using Newtonsoft.Json.Linq;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Threading.Tasks;
  using System.Web;

  public class YandexTranslateService : TranslateService
  {
    private const string URL = "https://translate.yandex.net/api/v1.5/tr.json/translate?";

    private readonly string apiKey = AppSettings.Get<string>("YANDEX_API_KEY");

    public override Task<string> Translate(string text, Language target)
    {
      return Task.Run(() =>
      {
        var parameters = new Dictionary<string, string>
        {
          { "key", this.apiKey },
          { "text", text },
          { "lang", target.ToString() },
          { "options", "0" }
        };

        var url = URL + string.Join("&", parameters.Select(pair => $"{pair.Key}={HttpUtility.UrlEncode(pair.Value)}"));
        var request = WebRequest.Create(url);

        var response = request.GetResponseAsync().Result;
        using (var stream = response.GetResponseStream())
        using (var reader = new StreamReader(stream))
        {
          var jsonResponse = JObject.Parse(reader.ReadToEnd());
          return jsonResponse["text"].First().ToString();
        }
      });
    }
  }
}
