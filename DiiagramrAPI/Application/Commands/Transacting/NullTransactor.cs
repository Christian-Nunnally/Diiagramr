using System;

namespace DiiagramrAPI.Application.Commands.Transacting
{
    public class NullTransactor : ITransactor
    {
        private NullTransactor()
        {
        }

        public static NullTransactor Instance { get; } = new NullTransactor();

        public void Redo() => throw new InvalidOperationException();

        public void Transact(ICommand command, object parameter)
        {
            command.Execute(parameter);
        }

        public void Undo() => throw new InvalidOperationException();
    }
}