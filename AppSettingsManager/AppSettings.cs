namespace AppSettings
{
  using AppSettingsManager.Properties;
  using System.ComponentModel;
  using System.Configuration;

  public static class AppSettings
  {
    public static T Get<T>(string key)
    {
      var appSetting = Resources.ResourceManager.GetString(key);
      if (string.IsNullOrWhiteSpace(appSetting))
        throw new SettingsPropertyNotFoundException(key);

      var converter = TypeDescriptor.GetConverter(typeof(T));
      return (T)(converter.ConvertFromInvariantString(appSetting));
    }
  }
}
