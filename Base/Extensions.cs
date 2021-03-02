namespace ArtificialisExcogitatoris.Base
{
  using Discord;
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;

  public static class Extensions
  {
    public static string GetMessageContent(this IMessage message)
    {
      if (message.Attachments.Count >= 1)
      {
        var attachment = message.Attachments.FirstOrDefault();
        if (attachment != null)
          return attachment.Url;
      }

      return message.Content.Substring(message.Content.IndexOf(' ') + 1).Trim();
    }

    public static T GetRandomElement<T>(this IEnumerable<T> enumerable)
    {
      return enumerable.ElementAt(new Random().Next(0, enumerable.Count()));
    }

    public static Task SendImage(this IMessageChannel channel, byte[] data, string filename = "file.png")
    {
      using (Stream stream = new MemoryStream(data))
      {
        return channel.SendFileAsync(stream, filename);
      }
    }
  }
}
