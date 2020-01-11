namespace ArtificialisExcogitatoris
{
  using System;
  using System.Drawing;
  using System.Drawing.Imaging;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Text;
  using Newtonsoft.Json.Linq;

  internal static class AnimeConverter
  {
    const string URL = @"https://waifu.lofiu.com/waifu_web/selfie2waifu";
    const string BOUNDARY = "----WebKitFormBoundaryRolknEQvN1k5jQdz";

    private static readonly Lazy<byte[]> FormDataPart1 = new Lazy<byte[]>(CreateFormPartData1);
    private static readonly Lazy<byte[]> FormDataPart3 = new Lazy<byte[]>(CreateFormPartData3);

    internal static Stream ConvertFace(string url)
    {
      try
      {
        WebRequest request = WebRequest.Create(URL);
        request.Method = "POST";
        request.ContentType = $@"multipart/form-data; boundary={BOUNDARY}";
        request.Timeout = 10000;

        Bitmap bitmap;
        using (MemoryStream memoryStream = new MemoryStream())
        using (WebClient client = new WebClient())
        {
          client.OpenRead(url).CopyTo(memoryStream);
          memoryStream.Position = 0;
          bitmap = new Bitmap(memoryStream);
        }

        if (bitmap.Width > 600 || bitmap.Height > 600)
        {
          Size size = new Size(bitmap.Height, bitmap.Width);
          if (size.Width > 600)
            size = new Size(600, (int)(size.Height * (size.Width / 600.0)));
          if (size.Height > 600)
            size = new Size((int)(size.Width * (size.Height / 600.0)), 600);

          bitmap = new Bitmap(bitmap, size);
        }

        byte[] sourceData;
        using (MemoryStream memoryStream = new MemoryStream())
        {
          bitmap.Save(memoryStream, ImageFormat.Jpeg);
          sourceData = new byte[memoryStream.Length];
          memoryStream.Position = 0;
          memoryStream.Read(sourceData, 0, (int)memoryStream.Length);
        }

        byte[] formData = FormDataPart1.Value.AsQueryable().Concat(sourceData).Concat(FormDataPart3.Value).ToArray();
        request.ContentLength = formData.Length;
        using (Stream dataStream = request.GetRequestStream())
        {
          dataStream.Write(formData, 0, formData.Length);
        }

        string resultString;
        WebResponse response = request.GetResponseAsync().Result;
        using (Stream stream = response.GetResponseStream())
        {
          using (StreamReader reader = new StreamReader(stream))
          {
            JObject jsonResponse = JObject.Parse(reader.ReadToEnd());
            resultString = jsonResponse["fake_base64"].Value<string>();
          }
        }
        response.Close();

        var resultImage = Convert.FromBase64String(resultString);
        var resultStream = new MemoryStream();
        resultStream.Write(resultImage, 0, resultImage.Length);
        resultStream.Position = 0;
        return resultStream;
      }
      catch
      {
        return null;
      }
    }

    private static byte[] CreateFormPartData1()
    {
      StringBuilder formData = new StringBuilder();
      formData.AppendLine($"--{BOUNDARY}");
      formData.AppendLine("Content-Disposition: form-data; name=\"selfie\"; filename=\"selfie.jpg\"");
      formData.AppendLine("Content-Type: image/jpeg");
      formData.AppendLine();

      return Encoding.ASCII.GetBytes(formData.ToString());
    }

    private static byte[] CreateFormPartData3()
    {
      StringBuilder formData = new StringBuilder();
      formData.AppendLine($"--{BOUNDARY}");
      formData.AppendLine("Content-Disposition: form-data; name=\"action_type\"");
      formData.AppendLine();
      formData.AppendLine("camera");
      formData.Append($"--{BOUNDARY}--");

      return Encoding.ASCII.GetBytes(formData.ToString());
    }
  }
}