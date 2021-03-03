namespace FaceAdder
{
  using System;
  using System.Drawing;

  internal class FaceRectangleWrapper : IDisposable
  {
    internal FaceRectangleWrapper BoundedForward { get; private set; }
    internal FaceRectangleWrapper BoundedBack { get; private set; }
    internal Rectangle Rectangle { get; set; }

    internal FaceRectangleWrapper(Rectangle rectangle)
    {
      this.Rectangle = rectangle;
    }

    internal static double GetDelta(FaceRectangleWrapper rec1, FaceRectangleWrapper rec2)
    {
      var intersect = Rectangle.Intersect(rec1.Rectangle, rec2.Rectangle);
      if (intersect.IsEmpty)
        return 0;

      double rS1 = rec1.Rectangle.Width * rec1.Rectangle.Height;
      double rS2 = rec2.Rectangle.Width * rec2.Rectangle.Height;
      double rSi = intersect.Width * intersect.Height;
      return rSi / Math.Max(rS1, rS2);
    }

    internal void BoundForward(FaceRectangleWrapper face, bool forced = false)
    {
      if (this.BoundedForward == null || forced)
      {
        this.BoundedForward = face;
      }
    }

    internal void BoundBack(FaceRectangleWrapper face, bool forced = false)
    {
      if (this.BoundedBack == null || forced)
      {
        this.BoundedBack = face;
      }
    }

    public void Dispose()
    {
      this.BoundedForward = null;
      this.BoundedBack = null;
    }
  }
}
