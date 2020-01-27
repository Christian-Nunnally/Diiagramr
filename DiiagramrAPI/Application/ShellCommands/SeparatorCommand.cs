using System;

namespace DiiagramrAPI.Application.ShellCommands
{
    public abstract class SeparatorCommand : ToolBarCommand
    {
        public override string Name { get; } = new Random().NextDouble().ToString();

        protected override void ExecuteInternal(object parameter)
        {
        }
    }
}