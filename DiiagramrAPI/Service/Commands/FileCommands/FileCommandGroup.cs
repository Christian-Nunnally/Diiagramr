using DiiagramrAPI.ViewModel;

namespace DiiagramrAPI.Service.Commands.FileCommands
{
    public class FileCommandGroup : DiiagramrCommand
    {
        public override string Name => "File";

        public override void Execute(ShellViewModel shell)
        {
            shell.ShowContextMenu(SubCommandItems);
        }
    }
}
