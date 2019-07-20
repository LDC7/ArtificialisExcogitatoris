namespace ArtificialisExcogitatoris
{
    using Discord;
    using Discord.WebSocket;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class CommandExecutor
    {
        private readonly DiscordSocketClient client;
        private readonly ulong channelId;
        private readonly ulong serverId;
        private SocketMessage message;

        public CommandExecutor(DiscordSocketClient socketClient, ulong channelId, ulong serverId)
        {
            this.client = socketClient;
            this.channelId = channelId;
            this.serverId = serverId;
        }

        public async Task ExecuteCommandAsync(SocketMessage socketMessage)
        {
            try
            {
                this.message = socketMessage;
                var index = this.message.Content.IndexOf(' ');
                var command = index != -1 ? this.message.Content.Substring(0, index) : this.message.Content;
                switch (command)
                {
                    case "!help":
                        await this.Help();
                        break;
                    case "!хокку":
                        await this.Haiku();
                        break;
                    case "!facehelp":
                        await this.Facehelp();
                        break;
                    default:
                        if (this.Face(command)) break;
                        break;
                }
            }
            catch (Exception ex)
            {
                (this.message.Channel as IMessageChannel)?.SendMessageAsync($"Произошла ошибка: {ex.Message}");
            }
        }

        private async Task Help()
        {
            const string helpMessage = "1) !help  - помощь\n" +
                "2) !хокку - составить рандомное хокку\n" +
                "3) !facehelp  - список 'лицевых' команд\n";

            if (this.message.Channel is IMessageChannel messageChannel)
            {
                await messageChannel.SendMessageAsync(helpMessage);
            }
        }

        private async Task Haiku()
        {
            if (this.message.Channel is IMessageChannel messageChannel)
            {
                var user = this.client
                .GetGuild(this.serverId)
                .Users
                .Where(u => u.Status == UserStatus.Online)
                .OrderBy(u => Guid.NewGuid())
                .First();

                var name = string.IsNullOrWhiteSpace(user.Nickname) ? user.Username : user.Nickname;

                await messageChannel.SendMessageAsync(HaikuGenerator.Generate(name));
            }
        }

        private async Task Facehelp()
        {
            if (this.message.Channel is IMessageChannel messageChannel)
            {
                await messageChannel.SendMessageAsync(FaceWorker.GetFacesDescriptions());
            }
        }

        private bool Face(string command)
        {
            if (!command.Contains("face"))
                return false;

            if (this.message.Channel is IMessageChannel messageChannel)
            {
                Tuple<Stream, string> result = null;
                string url = string.Empty;

                if (this.message.Attachments.Count == 1)
                {
                    var attachment = this.message.Attachments.FirstOrDefault();
                    if (attachment != null)
                    {
                        url = attachment.Url;
                    }
                }
                else
                {
                    url = this.message.Content.Substring(this.message.Content.IndexOf(' ') + 1);
                }

                if (command == "!face")
                {
                    result = FaceWorker.AddFaceFromUrl(url);
                }
                else
                {
                    result = FaceWorker.AddFaceFromUrl(url, command.Substring(1));
                }

                if (result != null)
                {
                    this.SendImage(messageChannel, result.Item1, result.Item2);
                }
                else
                {
                    messageChannel.SendMessageAsync("У меня не получилось...");
                }
            }

            return true;
        }

        private async void SendImage(IMessageChannel messageChannel, Stream stream, string filename = "file.png")
        {
            try
            {
                if (messageChannel != null && stream != null)
                {
                    await messageChannel.SendFileAsync(stream, filename);
                }
            }
            finally
            {
                stream.Dispose();
            }
        }
    }
}
