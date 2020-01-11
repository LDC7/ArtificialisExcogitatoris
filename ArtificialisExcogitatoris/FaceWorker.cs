namespace ArtificialisExcogitatoris
{
  using System;
  using System.Drawing;
  using System.Drawing.Imaging;
  using System.IO;
  using System.Net;
  using FaceAdder;

  public static class FaceWorker
  {
    private static string descriptions;

    public static Tuple<Stream, string> AddFaceFromUrl(string url, string facename = null)
    {
      FaceAdder.SelectedFace = facename;
      using (MemoryStream memoryStream = new MemoryStream())
      using (WebClient client = new WebClient())
      {
        client.OpenRead(url).CopyTo(memoryStream);
        memoryStream.Position = 0;
        if (url.EndsWith(".gif"))
          return AddFaceToGIF(memoryStream);

        return AddFaceToImage(memoryStream);
      }
    }

    private static Tuple<Stream, string> AddFaceToGIF(Stream gifStream)
    {
      return new Tuple<Stream, string>(FaceAdder.GifHandler(gifStream), "file.gif");
    }

    private static Tuple<Stream, string> AddFaceToImage(Stream imageStream)
    {
      MemoryStream stream = null;
      Bitmap image = (Bitmap)Image.FromStream(imageStream);
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
        stream = new MemoryStream();
        image.Save(stream, ImageFormat.Png);
        stream.Position = 0;
      }

      return new Tuple<Stream, string>(stream, "file.png");
    }

    public static string GetFacesDescriptions()
    {
      if (descriptions == null)
        descriptions = FaceAdder.GetDescriptions();

      return descriptions;
    }
  }
}
