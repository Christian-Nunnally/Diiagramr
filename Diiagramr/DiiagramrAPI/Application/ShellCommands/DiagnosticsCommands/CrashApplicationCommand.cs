using System;

namespace DiiagramrAPI.Application.ShellCommands.DiagnosticsCommands
{
    /// <summary>
    /// A debugging only command to test what happens when an exception is thrown from the toolbar.
    /// </summary>
    public class CrashApplicationCommand : ShellCommandBase, IToolbarCommand
    {
        /// <inheritdoc/>
        public override string Name => "Crash Application";

        /// <inheritdoc/>
        public string ParentName => "Diagnostics";

        /// <inheritdoc/>
        public float Weight => 1f;

        /// <inheritdoc/>
        protected override bool CanExecuteInternal()
        {
            return true;
        }

        /// <inheritdoc/>
        protected override void ExecuteInternal(object parameter)
        {
            throw new CrashApplicationCommandException("The command has crashed intentionally so that crash behavior can be tested.");
        }

        [Serializable]
        private class CrashApplicationCommandException : Exception
        {
            public CrashApplicationCommandException(string message)
                : base(message)
            {
            }
        }
    }
}