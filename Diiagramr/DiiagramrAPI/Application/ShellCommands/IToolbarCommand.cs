using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Application;

namespace DiiagramrAPI.Application.ShellCommands
{
    /// <summary>
    /// Command that lives in the shell toolbar.
    /// </summary>
    public interface IToolbarCommand : ISingletonService
    {
        /// <summary>
        /// The name of the parent menu for this toolbar command.
        /// </summary>
        string ParentName { get; }

        /// <summary>
        /// The user visible name of the command.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The commands weight on the toolbar, lower weights start on the top left.
        /// </summary>
        public float Weight { get; }
    }

    /// <summary>
    /// Useful <see cref="IToolbarCommand"/> extension methods for executing toolbar commnds.
    /// </summary>
    public static class ToolBarCommandExtensions
    {
        /// <summary>
        /// Executes a toolbar command with a null parameter.
        /// </summary>
        /// <param name="toolbarCommand">The command to execute.</param>
        public static void Execute(this IToolbarCommand toolbarCommand) => toolbarCommand.Execute(null);

        /// <summary>
        /// Executes a toolbar command.
        /// </summary>
        /// <param name="toolbarCommand">The command to execute.</param>
        /// <param name="parameter">The parameter to pass to the command.</param>
        public static void Execute(this IToolbarCommand toolbarCommand, object parameter)
        {
            if (toolbarCommand is IShellCommand command)
            {
                command.Execute(parameter);
            }
        }
    }
}