namespace AnimeFace
{
  using ArtificialisExcogitatoris.Base;
  using Newtonsoft.Json.Linq;
  using System;
  using System.Collections.Generic;
  using System.Drawing;
  using System.Drawing.Imaging;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Text;
  using System.Threading.Tasks;

  public class AnimeFaceCommandService : ICommandService
  {
    private const string URL = @"https://waifu.lofiu.com/waifu_web/selfie2waifu";
    private const string BOUNDARY = "----WebKitFormBoundaryRolknEQvN1k5jQdz";
    private const int SIZE_LIMIT = 800;

    private readonly Lazy<byte[]> FormDataPart1 = new Lazy<byte[]>(CreateFormPartData1);
    private readonly Lazy<byte[]> FormDataPart3 = new Lazy<byte[]>(CreateFormPartData3);

    public bool CanExecuteCustomCommand => false;

    public IEnumerable<(string name, string description)> ImplementedCommands { get; } = new[]
    {
      ("kawaii", "конвертация в аниме")
    };

    public Task ExecuteCommand(ExecuteCommandArgs args)
    {
      if (args.Command != "kawaii")
        throw new UnknownCommandException();

      var result = this.ConvertFace(args.Message.GetMessageContent());
      if (result != null)
      {
        return args.Message.Channel.SendImage(result, "anime.jpg");
      }
      else
      {
        return args.Message.Channel.SendMessageAsync("У меня не получилось...");
      }
    }

    private byte[] ConvertFace(string url)
    {
      var request = WebRequest.Create(URL);
      request.Method = "POST";
      request.ContentType = $@"multipart/form-data; boundary={BOUNDARY}";
      request.Timeout = 10000;

      Bitmap bitmap;
      byte[] sourceData;
      using (var memoryStream = new MemoryStream())
      using (var client = new WebClient())
      {
        client.OpenRead(url).CopyTo(memoryStream);
        memoryStream.Position = 0;
        bitmap = new Bitmap(memoryStream);
      }

      try
      {
        if (bitmap.Width > SIZE_LIMIT || bitmap.Height > SIZE_LIMIT)
        {
          var size = new Size(bitmap.Height, bitmap.Width);
          if (size.Width > SIZE_LIMIT)
            size = new Size(SIZE_LIMIT, (int)(size.Height * (size.Width / SIZE_LIMIT * 1.0)));
          if (size.Height > SIZE_LIMIT)
            size = new Size((int)(size.Width * (size.Height / SIZE_LIMIT * 1.0)), SIZE_LIMIT);

          bitmap = new Bitmap(bitmap, size);
        }

        using (var memoryStream = new MemoryStream())
        {
          bitmap.Save(memoryStream, ImageFormat.Jpeg);
          sourceData = new byte[memoryStream.Length];
          memoryStream.Position = 0;
          memoryStream.Read(sourceData, 0, (int)memoryStream.Length);
        }
      }
      catch
      {
        throw new Exception("Произошла ошибка при конвертации в jpg, либо изменении размера изображения.");
      }

      try
      {
        byte[] formData = FormDataPart1.Value.AsQueryable().Concat(sourceData).Concat(FormDataPart3.Value).ToArray();
        request.ContentLength = formData.Length;
        using (Stream dataStream = request.GetRequestStream())
        {
          dataStream.Write(formData, 0, formData.Length);
        }

        string resultString;
        var response = request.GetResponseAsync().Result;
        using (var stream = response.GetResponseStream())
        {
          using (var reader = new StreamReader(stream))
          {
            var jsonResponse = JObject.Parse(reader.ReadToEnd());
            resultString = jsonResponse["fake_base64"].Value<string>();
          }
        }
        response.Close();

        return Convert.FromBase64String(resultString);
      }
      catch
      {
        return null;
      }
    }

    private static byte[] CreateFormPartData1()
    {
      var formData = new StringBuilder()
        .Append("--")
        .AppendLine(BOUNDARY)
        .AppendLine("Content-Disposition: form-data; name=\"selfie\"; filename=\"selfie.jpg\"")
        .AppendLine("Content-Type: image/jpeg")
        .AppendLine();

      return Encoding.ASCII.GetBytes(formData.ToString());
    }

    private static byte[] CreateFormPartData3()
    {
      var formData = new StringBuilder()
        .Append("--")
        .AppendLine(BOUNDARY)
        .AppendLine("Content-Disposition: form-data; name=\"action_type\"")
        .AppendLine()
        .AppendLine("camera")
        .Append("--")
        .Append(BOUNDARY)
        .Append("--");

      return Encoding.ASCII.GetBytes(formData.ToString());
    }
  }
}
