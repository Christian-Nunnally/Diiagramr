using DiiagramrAPI.Service;

namespace DiiagramrAPI.Shell
{
    public static class ShellCommand
    {
        internal static ShellCommandFactory CommandManager { get; set; }

        /// <summary>
        /// Attempts to execute a command from a static context.
        /// </summary>
        /// <param name="commandId">The path of the command, which looks like: <code>ParentCommandName:ChildCommandName</code>.</param>
        /// <returns>True if the command suceeded</returns>
        public static bool Execute(string commandId)
        {
            CommandManager?.ExecuteCommand(commandId);
            return CommandManager != null;
        }

        /// <summary>
        /// Attempts to execute a command from a static context.
        /// </summary>
        /// <param name="commandId">The path of the command, which looks like: <code>ParentCommandName:ChildCommandName</code>.</param>
        /// <param name="parameter">A command parameter.</param>
        /// <returns>True if the command suceeded</returns>
        public static bool Execute(string commandId, object parameter)
        {
            CommandManager?.ExecuteCommand(commandId, parameter);
            return CommandManager != null;
        }
    }
}