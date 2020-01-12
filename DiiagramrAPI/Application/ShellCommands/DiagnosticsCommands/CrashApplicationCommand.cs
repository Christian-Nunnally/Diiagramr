using System;

namespace DiiagramrAPI.Application.ShellCommands.DiagnosticsCommands
{
    public class CrashApplicationCommand : ShellCommandBase
    {
        public override string Name => "Crash Application";

        public override string Parent => "Diagnostics";

        internal override void ExecuteInternal(IApplicationShell shell, object parameter)
        {
            throw new CrashApplicationCommandException("The application has crashed intentionally so that crash behavior can be tested.");
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