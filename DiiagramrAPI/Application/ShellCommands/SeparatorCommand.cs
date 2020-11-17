using System;

namespace DiiagramrAPI.Application.ShellCommands
{
    /// <summary>
    /// A seperator in the toolbar menu.
    /// </summary>
    public abstract class SeparatorCommand : ShellCommandBase, IToolbarCommand
    {
        /// <inheritdoc/>
        public override string Name { get; } = new Random().NextDouble().ToString();

        /// <inheritdoc/>
        public abstract string ParentName { get; }

        /// <inheritdoc/>
        public abstract float Weight { get; }

        /// <inheritdoc/>
        protected override void ExecuteInternal(object parameter)
        {
        }

        /// <inheritdoc/>
        protected override bool CanExecuteInternal()
        {
            return true;
        }
    }
}