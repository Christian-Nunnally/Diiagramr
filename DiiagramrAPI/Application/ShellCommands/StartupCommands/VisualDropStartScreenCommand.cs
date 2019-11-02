using System;

namespace DiiagramrAPI.Application.ShellCommands.StartupCommands
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

        internal override void ExecuteInternal(IApplicationShell shell, object parameter)
        {
            shell.ShowScreen(_visualDropStartScreenViewModel);
        }
    }
}