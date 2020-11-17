using DiiagramrAPI.Application.Dialogs;
using System;

namespace DiiagramrAPI.Application.ShellCommands.ToolCommands
{
    /// <summary>
    /// Command that opens the options dialog window.
    /// </summary>
    public class OpenOptionsDialogCommand : ShellCommandBase, IToolbarCommand
    {
        private readonly OptionDialog _optionsDialog;
        private readonly DialogHostBase _dialogHost;

        /// <summary>
        /// Creates a new instance of <see cref="OpenOptionsDialogCommand"/>
        /// </summary>
        /// <param name="optionsDialogFactory">Factory to get an instance of an <see cref="OptionDialog"/>.</param>
        /// <param name="dialogHostFactory">Factory to get an instance of an <see cref="DialogHostBase"/>.</param>
        public OpenOptionsDialogCommand(
            Func<OptionDialog> optionsDialogFactory,
            Func<DialogHostBase> dialogHostFactory)
        {
            _optionsDialog = optionsDialogFactory();
            _dialogHost = dialogHostFactory();
        }

        /// <inheritdoc/>
        public string ParentName => "Tools";

        /// <inheritdoc/>
        public float Weight => .12f;

        /// <inheritdoc/>
        public override string Name => "Options...";

        /// <inheritdoc/>
        protected override bool CanExecuteInternal() => true;

        /// <inheritdoc/>
        protected override void ExecuteInternal(object parameter)
        {
            _dialogHost.OpenDialog(_optionsDialog);
        }
    }
}