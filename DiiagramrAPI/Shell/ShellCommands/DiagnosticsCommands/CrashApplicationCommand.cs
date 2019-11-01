using DiiagramrAPI.Shell;
using System;

namespace DiiagramrAPI.Service.Commands.DiagnosticsCommands
{
    public class CrashApplicationCommand : DiiagramrCommand
    {
        public override string Name => "Crash Application";
        public override string Parent => "Diagnostics";

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
            throw new CrashApplicationCommandException("The application has crashed intentionally so that crash behavior can be tested.");
        }

        [Serializable]
        private class CrashApplicationCommandException : Exception
        {
            public CrashApplicationCommandException(string message) : base(message)
            {
            }
        }
    }
}
