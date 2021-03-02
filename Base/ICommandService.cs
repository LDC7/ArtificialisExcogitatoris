namespace ArtificialisExcogitatoris.Base
{
  using System.Collections.Generic;
  using System.Threading.Tasks;

  public interface ICommandService
  {
    bool CanExecuteCustomCommand { get; }

    IEnumerable<(string name, string description)> ImplementedCommands { get; }

    Task ExecuteCommand(ExecuteCommandArgs args);
  }
}
