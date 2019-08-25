namespace AppSettings
{
  using System.ComponentModel;
  using System.Configuration;
  using AppSettingsManager.Properties;

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
