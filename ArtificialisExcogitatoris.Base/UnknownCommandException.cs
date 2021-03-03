namespace ArtificialisExcogitatoris.Base
{
  using System;

  public class UnknownCommandException : Exception
  {
    public UnknownCommandException() : base("Неизвестная команда.") { }
  }
}
