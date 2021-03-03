namespace FaceAdder
{
  /*using Gifed;
  using System;
  using System.Collections.Generic;

  internal class GifSceneWrapper : IDisposable
  {
    private const int neighbours = 10;
    private bool facesBounded = false;
    internal List<FacedFrameWrapper> Frames { get; private set; }

    internal GifSceneWrapper()
    {
      this.Frames = new List<FacedFrameWrapper>();
    }

    internal void AddFrame(GifFrame frame)
    {
      this.Frames.Add(new FacedFrameWrapper(frame));
    }

    internal void DeleteExcessFaces()
    {
      if (!this.facesBounded)
        this.BoundFaces();

      foreach (var frame in this.Frames)
        frame.DeleteNotBoundedFaces();
    }

    internal void AddBoundFaces()
    {
      if (!this.facesBounded)
        this.BoundFaces();

      for (int i = 0; i < this.Frames.Count - 1; i++)
      {
        FacedFrameWrapper.AddBoundFaces(this.Frames[i], this.Frames[i + 1]);
      }
    }

    private void BoundFaces()
    {
      for (int i = 0; i < this.Frames.Count; i++)
      {
        for (int j = i + 1, k = 0; k < neighbours && j < this.Frames.Count; j++, k++)
        {
          if (BoundFaces(this.Frames[i], this.Frames[j]))
          {
            break;
          }
        }
      }

      this.facesBounded = true;
    }

    private static bool BoundFaces(FacedFrameWrapper frame, FacedFrameWrapper nextFrame)
    {
      FaceRectangleWrapper next;
      foreach (var f in frame.Faces)
      {
        if (f.BoundedForward == null)
        {
          next = nextFrame.GetIntersectFace(f);
          if (next != null)
          {
            f.BoundForward(next);
            next.BoundBack(f);
            return true;
          }
        }
      }

      return false;
    }

    internal void AddFaceToScene()
    {
      foreach (var frame in this.Frames)
      {
        frame.AddFaceToFrame();
      }
    }

    public void Dispose()
    {
      foreach (var frame in this.Frames)
      {
        frame.Dispose();
      }
      this.Frames = null;
    }
  }*/
}
