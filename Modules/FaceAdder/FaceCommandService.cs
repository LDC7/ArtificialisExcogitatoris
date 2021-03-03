namespace FaceAdder
{
  using ArtificialisExcogitatoris.Base;
  using System;
  using System.Collections.Generic;
  using System.Drawing;
  using System.Drawing.Imaging;
  using System.IO;
  using System.Net;
  using System.Threading.Tasks;

  public class FaceCommandService : ICommandService
  {
    private readonly Lazy<string> descriptions = new Lazy<string>(FaceAdder.GetDescriptions);

    public bool CanExecuteCustomCommand => true;

    public IEnumerable<(string name, string description)> ImplementedCommands { get; } = new[]
    {
      ("facehelp", "список 'лицевых' команд")
    };

    public Task ExecuteCommand(ExecuteCommandArgs args)
    {
      switch (args.Command)
      {
        case "facehelp":
          return this.Help(args);

        default:
          return args.Command.Contains("face")
            ? this.AddFace(args)
            : Task.CompletedTask;
      }
    }

    private Task AddFace(ExecuteCommandArgs args)
    {
      string url = args.Message.GetMessageContent();
      Tuple<byte[], string> result;
      if (args.Command == "face")
        result = this.AddFaceFromUrl(url);
      else
        result = this.AddFaceFromUrl(url, args.Command);

      if (result != null)
        return args.Message.Channel.SendImage(result.Item1, result.Item2);

      return Task.CompletedTask;
    }

    private Task Help(ExecuteCommandArgs args)
    {
      return args.Message.Channel.SendMessageAsync(this.descriptions.Value);
    }

    public Tuple<byte[], string> AddFaceFromUrl(string url, string facename = null)
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

    private Tuple<byte[], string> AddFaceToGIF(byte[] gif)
    {
      throw new Exception("Добавление лиц в гифки пока/более не поддерживается.");
      return new Tuple<byte[], string>(FaceAdder.GifHandler(gif), "file.gif");
    }

    private Tuple<byte[], string> AddFaceToImage(byte[] imageBuffer)
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

      using (var stream = new MemoryStream())
      {
        image.Save(stream, ImageFormat.Png);
        stream.Position = 0;
        result = new byte[stream.Length];
        stream.Read(result, 0, (int)stream.Length);
      }

      return new Tuple<byte[], string>(result, "file.png");
    }
  }
}
