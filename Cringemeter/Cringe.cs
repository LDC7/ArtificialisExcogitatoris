namespace Cringemeter
{
  using System;

  public struct Cringe
  {
    public DateTime Time { get; }

    public string Message { get; }

    public Cringe(DateTime time, string message)
    {
      this.Time = time;
      this.Message = message;
    }
  }
}
