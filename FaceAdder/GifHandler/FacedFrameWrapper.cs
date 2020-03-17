namespace FaceAdder
{
  using Gifed;
  using System;
  using System.Collections.Generic;
  using System.Drawing;
  using System.Linq;

  internal class FacedFrameWrapper : IDisposable
  {
    private const double rectangleEpsilon = 0.5;
    internal GifFrame Frame { get; set; }
    internal List<FaceRectangleWrapper> Faces { get; private set; }

    internal FacedFrameWrapper(GifFrame frame)
    {
      this.Frame = frame;
      this.Faces = FaceAdder.FindFaces((Bitmap)this.Frame.Image, -1)
          .Select(r => new FaceRectangleWrapper(r)).ToList();
    }

    internal void AddFaceToFrame()
    {
      Bitmap bitmap = (Bitmap)this.Frame.Image;
      Bitmap facedBitmap;
      if (this.Faces.Count == 0)
        return;

      var rectangles = this.Faces.Select(f => f.Rectangle).ToList();
      rectangles = FaceAdder.PrepareFaceList(rectangles);
      foreach (var rectangle in rectangles)
      {
        facedBitmap = FaceAdder.AddFace(bitmap, rectangle);
        if (facedBitmap != null)
          bitmap = facedBitmap;
      }

      this.Frame = new GifFrame(bitmap, this.Frame.Delay);
    }

    internal void DeleteNotBoundedFaces()
    {
      this.Faces.RemoveAll(f => f.BoundedForward == null && f.BoundedBack == null);
    }

    internal FaceRectangleWrapper GetIntersectFace(FaceRectangleWrapper face)
    {
      for (int i = 0; i < this.Faces.Count; i++)
      {
        if (SameRectangle(face, this.Faces[i]))
        {
          return this.Faces[i];
        }
      }

      return null;
    }

    private static bool SameRectangle(FaceRectangleWrapper rec1, FaceRectangleWrapper rec2)
    {
      return FaceRectangleWrapper.GetDelta(rec1, rec2) > rectangleEpsilon;
    }

    internal static void AddBoundFaces(FacedFrameWrapper frame, FacedFrameWrapper nextFrame)
    {
      foreach (var face in frame.Faces)
      {
        if (face.BoundedForward != null && !nextFrame.Faces.Contains(face.BoundedForward))
        {
          var faceRectangle = new FaceRectangleWrapper(
              new Rectangle((face.Rectangle.X + face.BoundedForward.Rectangle.X) / 2,
              (face.Rectangle.Y + face.BoundedForward.Rectangle.Y) / 2,
              (face.Rectangle.Width + face.BoundedForward.Rectangle.Width) / 2,
              (face.Rectangle.Height + face.BoundedForward.Rectangle.Height) / 2));

          faceRectangle.BoundForward(face.BoundedForward);
          faceRectangle.BoundBack(face);
          faceRectangle.BoundedForward.BoundBack(faceRectangle, true);
          face.BoundForward(faceRectangle, true);
          nextFrame.Faces.Add(faceRectangle);
        }
      }
    }

    public void Dispose()
    {
      this.Frame.Dispose();
      this.Frame = null;
      foreach (var face in this.Faces)
      {
        face.Dispose();
      }
      this.Faces = null;
    }
  }
}
