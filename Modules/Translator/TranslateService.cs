namespace Translator
{
  using System.Threading.Tasks;

  public abstract class TranslateService
  {
    public abstract Task<string> Translate(string text, Language target);
  }
}
