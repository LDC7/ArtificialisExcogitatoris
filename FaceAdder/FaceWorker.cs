namespace FaceAdder
{
  using System;
  using System.Drawing;
  using System.Drawing.Imaging;
  using System.IO;
  using System.Net;

  public static class FaceWorker
  {
    private static readonly Lazy<string> descriptions = new Lazy<string>(FaceAdder.GetDescriptions);

    public static string FacesDescriptions => descriptions.Value;

    public static Tuple<byte[], string> AddFaceFromUrl(string url, string facename = null)
    {
      FaceAdder.SelectedFace = facename;
      using (var memoryStream = new MemoryStream())
      using (WebClient client = new WebClient())
      {
        client.OpenRead(url).CopyTo(memoryStream);
        var buffer = new byte[memoryStream.Length];
        memoryStream.Position = 0;
        memoryStream.Read(buffer, 0, buffer.Length);
        return url.EndsWith(".gif") ? AddFaceToGIF(buffer) : AddFaceToImage(buffer);
      }
    }

    private static Tuple<byte[], string> AddFaceToGIF(byte[] gif)
    {
      return new Tuple<byte[], string>(FaceAdder.GifHandler(gif), "file.gif");
    }

    private static Tuple<byte[], string> AddFaceToImage(byte[] imageBuffer)
    {
      Bitmap image;
      byte[] result = null;
      using (Stream imageStream = new MemoryStream(imageBuffer))
      {
        image = (Bitmap)Image.FromStream(imageStream);
      }

      image = FaceAdder.AddFace(image);
      Size size = image.Size;
      if (size.Width > 500)
      {
        double coof = 500.0 / size.Width;
        size = new Size((int)(size.Width * coof), (int)(size.Height * coof));
      }
      if (size.Height > 500)
      {
        double coof = 500.0 / size.Height;
        size = new Size((int)(size.Width * coof), (int)(size.Height * coof));
      }
      if (size != image.Size)
      {
        image = new Bitmap(image, size);
      }

      if (image != null)
      {
        using (var stream = new MemoryStream())
        {
          image.Save(stream, ImageFormat.Png);
          stream.Position = 0;
          result = new byte[stream.Length];
          stream.Read(result, 0, (int)stream.Length);
        }
      }

      return new Tuple<byte[], string>(result, "file.png");
    }
  }
}
