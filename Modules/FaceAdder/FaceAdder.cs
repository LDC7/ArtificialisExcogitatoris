namespace FaceAdder
{
  using Emgu.CV;
  using Emgu.CV.CvEnum;
  using Emgu.CV.Structure;
  //using Gifed;
  using System;
  using System.Collections.Generic;
  using System.Drawing;
  using System.IO;
  using System.Linq;
  using System.Text;
  using Size = System.Drawing.Size;

  internal static class FaceAdder
  {
    private const string facesFolder = "FaceAdderData\\Faces";
    private static Rgba RgbaAlpha0 = new Rgba(0, 0, 0, 0);
    private static readonly CascadeClassifier eyeClassifier;
    private static readonly CascadeClassifier[] faceClassifiers;
    private static readonly Dictionary<string, FaceWrapper> faces;
    private static string selectedFace;

    public static string SelectedFace
    {
      get { return selectedFace; }
      set
      {
        if (ValidateFacename(value))
        {
          selectedFace = value;
        }
        else
        {
          selectedFace = null;
        }
      }
    }

    private static bool ValidateFacename(string value)
    {
      return faces.Any(f => f.Key == value);
    }

    static FaceAdder()
    {
      CascadeClassifier faceClassifier_default = new CascadeClassifier($"FaceAdderData\\haarcascade_frontalface_default.xml");
      CascadeClassifier faceClassifier_alt = new CascadeClassifier($"FaceAdderData\\haarcascade_frontalface_alt.xml");
      CascadeClassifier faceClassifier_alt2 = new CascadeClassifier($"FaceAdderData\\haarcascade_frontalface_alt2.xml");
      faceClassifiers = new CascadeClassifier[] { faceClassifier_default /*, faceClassifier_alt, faceClassifier_alt2*/ };
      eyeClassifier = new CascadeClassifier($"FaceAdderData\\haarcascade_eye.xml");

      faces = new Dictionary<string, FaceWrapper>();
      DirectoryInfo dir = new DirectoryInfo(facesFolder);
      foreach (var file in dir.GetFiles())
      {
        if (file.Extension == string.Empty)
        {
          faces.Add(file.Name, new FaceWrapper(file.FullName));
        }
      }
    }

    public static Bitmap AddFace(Bitmap bitmap)
    {
      if (bitmap == null)
        return null;

      Image<Rgba, byte> image = bitmap.ToImage<Rgba, byte>();
      image = AddFaces(image);

      return image?.AsBitmap();
    }

    private static Image<Rgba, byte> AddFaces(Image<Rgba, byte> image)
    {
      Image<Rgba, byte> result = image.Clone();
      List<Rectangle> faces = FindFaces(image, -1);

      if (faces.Count == 0)
        return null;

      foreach (var face in faces)
      {
        result = AddFace(result, face);
      }

      return result;
    }

    private static List<Rectangle> FindFaces(Image<Rgba, byte> image, int classifierNumber)
    {
      List<Rectangle> faces = new List<Rectangle>();
      Image<Gray, byte> grayImage = image.Convert<Gray, byte>();

      if (classifierNumber >= 0 && classifierNumber < faceClassifiers.Length)
      {
        faces.AddRange(faceClassifiers[classifierNumber].DetectMultiScale(grayImage, minSize: new Size(1, 1), maxSize: Size.Empty));
      }
      else
      {
        foreach (var classifier in faceClassifiers)
        {
          faces.AddRange(classifier.DetectMultiScale(grayImage, minSize: new Size(1, 1), maxSize: Size.Empty));
        }
      }

      if (faces.Count == 0)
        return new List<Rectangle>();

      return PrepareFaceList(faces);
    }

    internal static List<Rectangle> FindFaces(Bitmap bitmap, int classifierNumber = -1)
    {
      return FindFaces(bitmap.ToImage<Rgba, byte>(), classifierNumber);
    }

    internal static List<Rectangle> PrepareFaceList(List<Rectangle> faces)
    {
      List<Rectangle> result = faces.Distinct().ToList();
      bool flag = true;

      while (flag)
      {
        flag = false;
        for (int i = 0; i < result.Count; i++)
        {
          for (int j = i + 1; j < result.Count; j++)
          {
            var temp = SumRectangles(result[i], result[j]);
            if (temp.HasValue)
            {
              result.RemoveAt(j);
              result.RemoveAt(i);
              result.Add(temp.Value);
              flag = true;
            }
          }
        }
      }

      return result;
    }

    private static Rectangle? SumRectangles(Rectangle f, Rectangle s)
    {
      Rectangle intersect = f;
      intersect.Intersect(s);

      if (intersect.IsEmpty)
        return null;

      int Sf = f.Width * f.Height;
      int Ss = s.Width * s.Height;
      int Si = intersect.Width * intersect.Height;

      if (Sf == Si)
        return s;

      if (Ss == Si)
        return f;

      if (Sf / 8 < Si || Ss / 8 < Si)
      {
        return new Rectangle(
          Math.Min(f.X, s.X),
          Math.Min(f.Y, s.Y),
          Math.Max(f.Right, s.Right) - Math.Min(f.X, s.X),
          Math.Max(f.Bottom, s.Bottom) - Math.Min(f.Y, s.Y));
      }

      return null;
    }

    private static Image<Rgba, byte> AddFace(Image<Rgba, byte> source, Rectangle rec)
    {
      FaceWrapper chosenFace;
      if (selectedFace != null)
      {
        chosenFace = faces[selectedFace];
      }
      else
      {
        chosenFace = faces.ElementAt(new Random().Next(0, faces.Count())).Value;
      }

      var buf = chosenFace.Image.Clone();
      var result = source.Clone();
      var face = result.Copy(rec);
      if (chosenFace.CanBeRotated)
      {
        var sourceEyes = eyeClassifier.DetectMultiScale(face, minSize: new Size(1, 1), maxSize: Size.Empty);
        if (sourceEyes.Length == 2)
        {
          var eye1 = sourceEyes[0];
          var eye2 = sourceEyes[1];
          var rotate = Math.Atan2(eye1.Y - eye2.Y, eye1.X - eye2.X);
          buf = buf.Rotate(chosenFace.Rotation - rotate, RgbaAlpha0);
        }
      }

      double widthScale = 1.1 * ((double)rec.Width / chosenFace.Rectangle.Width);
      double heightScale = 1.1 * ((double)rec.Height / chosenFace.Rectangle.Height);

      int bufWidth = (int)(widthScale * chosenFace.Image.Width);
      int bufHeight = (int)(heightScale * chosenFace.Image.Height);

      int offsetL = (rec.Left + (rec.Width / 2)) - (bufWidth / 2);
      int offsetT = (rec.Top + (rec.Height / 2)) - (bufHeight / 2);
      int offsetR = (result.Width - rec.Right + (rec.Width / 2)) - (bufWidth / 2);
      int offsetB = (result.Height - rec.Bottom + (rec.Height / 2)) - (bufHeight / 2);

      buf = buf.Resize(result.Width - offsetL - offsetR, result.Height - offsetT - offsetB, Inter.Cubic);

      if (offsetL < 0 || offsetT < 0 || offsetR < 0 || offsetB < 0)
      {
        var rectangle = new Rectangle(0, 0, buf.Width, buf.Height);
        if (offsetL < 0)
        {
          rectangle.X -= offsetL;
          rectangle.Width += offsetL;
          offsetL = 0;
        }
        if (offsetT < 0)
        {
          rectangle.Y -= offsetT;
          rectangle.Height += offsetT;
          offsetT = 0;
        }
        if (offsetR < 0)
        {
          rectangle.Width += offsetR;
        }
        if (offsetB < 0)
        {
          rectangle.Height += offsetB;
        }

        buf = buf.Copy(rectangle);
      }

      for (int w = 0; w < buf.Width; w++)
      {
        for (int h = 0; h < buf.Height; h++)
        {
          if (buf[h, w].Alpha > 0.6)
            result[h + offsetT, w + offsetL] = buf[h, w];
        }
      }

      return result;
    }

    internal static Bitmap AddFace(Bitmap source, Rectangle rec)
    {
      return AddFace(source.ToImage<Rgba, byte>(), rec)?.ToBitmap();
    }

    public static byte[] GifHandler(byte[] gifBuffer)
    {
      throw new NotImplementedException();
      /*if (selectedFace == null)
      {
        selectedFace = faces.ElementAt(new Random().Next(0, faces.Count())).Key;
      }

      using (Stream gifStream = new MemoryStream(gifBuffer))
      using (var gif = AnimatedGif.LoadFrom(gifStream))
      using (var handler = new GifHandler(gif))
      using (var facedGif = handler.GetFacedGif())
      using (Stream stream = new MemoryStream())
      {
        facedGif.Save(stream);
        stream.Position = 0;
        byte[] result = new byte[stream.Length];
        stream.Read(result, 0, (int)stream.Length);
        return result;
      }*/
    }

    public static string GetDescriptions()
    {
      StringBuilder sb = new StringBuilder();

      foreach (var face in faces)
      {
        sb.Append(face.Key);
        sb.Append(" - ");
        sb.Append(face.Value.Description);
        sb.Append(Environment.NewLine);
      }

      return sb.ToString();
    }
  }
}
