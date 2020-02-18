using DiiagramrAPI.Service.Application;

namespace DiiagramrAPI.Application.ShellCommands
{
    public interface IToolbarCommand
    {
        string ParentName { get; }

        string Name { get; }

        public float Weight { get; }
    }

    public static class ToolBarCommandExtensions
    {
        public static bool CanExecute(this IToolbarCommand toolbarCommand)
        {
            return toolbarCommand is IShellCommand command && command.CanExecute();
        }

        public static void Execute(this IToolbarCommand toolbarCommand, object parameter)
        {
            if (toolbarCommand is IShellCommand command)
            {
                command.Execute(parameter);
            }
        }
    }
}