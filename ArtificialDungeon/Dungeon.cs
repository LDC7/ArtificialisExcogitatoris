namespace ArtificialDungeon
{
  using AppSettings;
  using Newtonsoft.Json.Linq;
  using System;
  using System.Linq;
  using System.Net;
  using System.Text;

  internal class Dungeon : IDisposable
  {
    private const string MODES_URL = "https://api.aidungeon.io/sessions/*/config";

    private const string DUNGEON_URL = "https://api.aidungeon.io/sessions";

    private static readonly string AccessToken = AppSettings.Get<string>("AI_DUNGEON_TOKEN");

    private readonly WebClient client;

    private string sessionId;

    public DateTime Created { get; }

    public DateTime LastActivity { get; private set; }

    public void Dispose()
    {
      this.client.Dispose();
    }

    public string CreateAdventure(string heroName)
    {
      this.LastActivity = DateTime.Now;
      var random = new Random();

      JObject jsonModes = JObject.Parse(this.client.DownloadString(MODES_URL));
      var jsonSettings = (JArray)jsonModes["modes"];
      var filteredJsonMode = jsonSettings
        .Where(mode => mode.ToString() != "custom");
      var jsonMode = filteredJsonMode.ElementAt(random.Next(0, filteredJsonMode.Count()));
      var jsonCharacters = jsonMode["characters"];
      var jsonCharacter = jsonCharacters.ElementAt(random.Next(0, filteredJsonMode.Count()));

      var modeName = ((JProperty)jsonMode).Name;
      var characterType = ((JProperty)jsonCharacter).Name;

      var payload = new JObject();
      payload.Add("storyMode", modeName);
      payload.Add("characterType", characterType);
      payload.Add("name", heroName);
      payload.Add("customPrompt", null);
      payload.Add("promptId", null);

      byte[] byteArray = Encoding.UTF8.GetBytes(payload.ToString());
      var responseData = this.client.UploadData(DUNGEON_URL, byteArray);
      var jsonResponse = JObject.Parse(Encoding.UTF8.GetString(responseData));

      this.sessionId = jsonResponse["id"].ToString();
      var jsonStories = (JArray)jsonResponse["story"];
      return jsonStories.Last["value"].ToString();
    }

    public string Do(string action)
    {
      this.LastActivity = DateTime.Now;

      var url = $"{DUNGEON_URL}/{this.sessionId}/inputs";
      var payload = $@"{{""text"": ""{action}""}}";
      var jsonResponse = JArray.Parse(this.client.UploadString(url, payload));
      return jsonResponse.Last["value"].ToString();
    }

    internal Dungeon()
    {
      this.Created = DateTime.Now;
      this.client = new WebClient();
      this.client.Headers.Add("x-access-token", AccessToken);
    }
  }
}
