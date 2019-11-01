using System;

namespace DiiagramrAPI.Shell.Commands.Transacting
{
    public class NullTransactor : ITransactor
    {
        public static NullTransactor Instance = new NullTransactor();

        private NullTransactor() { }

        public void Transact(ICommand command, object parameter)
        {
            command.Execute(parameter);
        }

        public void Redo() => throw new InvalidOperationException();

        public void Undo() => throw new InvalidOperationException();
    }
}
