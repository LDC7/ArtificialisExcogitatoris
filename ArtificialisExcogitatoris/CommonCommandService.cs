namespace ArtificialisExcogitatoris
{
  using global::ArtificialisExcogitatoris.Base;
  using Microsoft.Extensions.DependencyInjection;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  internal class CommonCommandService : ICommandService
  {
    private readonly IServiceProvider provider;

    public bool CanExecuteCustomCommand => false;

    public IEnumerable<(string name, string description)> ImplementedCommands { get; } = new[]
    {
      ("help", "помощь")
    };

    public Task ExecuteCommand(ExecuteCommandArgs args)
    {
      switch (args.Command)
      {
        case "help":
          return this.Help(args);

        default:
          throw new UnknownCommandException();
      }
    }

    private Task Help(ExecuteCommandArgs args)
    {
      var services = provider.GetServices<ICommandService>();
      var commands = services.SelectMany(s => s.ImplementedCommands).ToArray();

      var sb = new StringBuilder();
      for (var i = 0; i < commands.Length; i++)
      {
        sb.Append(i + 1)
          .Append(") !")
          .Append(commands[i].name)
          .Append(" - ")
          .AppendLine(commands[i].description);
      }

      return args.Message.Channel.SendMessageAsync(sb.ToString());
    }

    public CommonCommandService(IServiceProvider provider)
    {
      this.provider = provider;
    }
  }
}
