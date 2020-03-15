using DiiagramrAPI.Application.Dialogs;
using System;

namespace DiiagramrAPI.Application.ShellCommands.ToolCommands
{
    public class OpenOptionsDialogCommand : ShellCommandBase, IToolbarCommand
    {
        private readonly OptionDialog _optionsDialog;
        private readonly DialogHostBase _dialogHost;

        public OpenOptionsDialogCommand(
            Func<OptionDialog> optionsDialogFactory,
            Func<DialogHostBase> dialogHostFactory)
        {
            _optionsDialog = optionsDialogFactory();
            _dialogHost = dialogHostFactory();
        }

        public string ParentName => "Tools";

        public float Weight => .12f;

        public override string Name => "Options...";

        protected override bool CanExecuteInternal() => true;

        protected override void ExecuteInternal(object parameter)
        {
            _dialogHost.OpenDialog(_optionsDialog);
        }
    }
}