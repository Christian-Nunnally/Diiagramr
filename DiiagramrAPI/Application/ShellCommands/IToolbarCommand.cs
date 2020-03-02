using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Application;

namespace DiiagramrAPI.Application.ShellCommands
{
    public interface IToolbarCommand : ISingletonService
    {
        string ParentName { get; }

        string Name { get; }

        public float Weight { get; }
    }

    public static class ToolBarCommandExtensions
    {
        public static void Execute(this IToolbarCommand toolbarCommand) => toolbarCommand.Execute(null);

        public static void Execute(this IToolbarCommand toolbarCommand, object parameter)
        {
            if (toolbarCommand is IShellCommand command)
            {
                command.Execute(parameter);
            }
        }
    }
}