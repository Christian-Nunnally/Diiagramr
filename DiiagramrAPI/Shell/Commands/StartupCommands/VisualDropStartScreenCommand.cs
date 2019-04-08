using DiiagramrAPI.Shell;
using System;

namespace DiiagramrAPI.Service.Commands.StartupCommands
{
    public class VisualDropStartScreenCommand : DiiagramrCommand
    {
        private readonly VisualDropStartScreenViewModel _visualDropStartScreenViewModel;

        public VisualDropStartScreenCommand(Func<VisualDropStartScreenViewModel> visualDropStartScreenViewModelFactory)
        {
            _visualDropStartScreenViewModel = visualDropStartScreenViewModelFactory.Invoke();
        }

        public override string Name => ShellViewModel.StartCommandId;

        public override float Weight => 0.2f;

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
            shell.ShowScreen(_visualDropStartScreenViewModel);
            shell.SetWindowTitle("Visual Drop");
        }
    }
}
