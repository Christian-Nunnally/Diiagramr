using System;

namespace DiiagramrAPI.Application.ShellCommands.DiagnosticsCommands
{
    public class CrashApplicationCommand : ToolBarCommand
    {
        public override string Name => "Crash Application";

        public override string Parent => "Diagnostics";

        protected override bool CanExecuteInternal()
        {
            return true;
        }

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