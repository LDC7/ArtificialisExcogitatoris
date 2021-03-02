namespace Cringemeter
{
  using System;

  internal struct Cringe : IEquatable<Cringe>
  {
    public DateTime Time { get; }

    public string Message { get; }

    public bool Equals(Cringe other)
    {
      return this.Message.Equals(other.Message)
        && this.Time.Equals(other.Time);
    }

    public Cringe(DateTime time, string message)
    {
      this.Time = time;
      this.Message = message;
    }
  }
}
