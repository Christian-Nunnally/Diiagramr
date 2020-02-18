using System;

namespace DiiagramrAPI.Application.ShellCommands
{
    public abstract class SeparatorCommand : ShellCommandBase, IToolbarCommand
    {
        public override string Name { get; } = new Random().NextDouble().ToString();

        public abstract string ParentName { get; }

        public abstract float Weight { get; }

        protected override void ExecuteInternal(object parameter)
        {
        }

        protected override bool CanExecuteInternal()
        {
            return true;
        }
    }
}