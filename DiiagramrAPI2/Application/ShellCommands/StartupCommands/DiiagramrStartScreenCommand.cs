using System;

namespace DiiagramrAPI.Application.ShellCommands.StartupCommands
{
    public class DiiagramrStartScreenCommand : ShellCommandBase
    {
        private readonly StartScreenViewModel _startScreenViewModel;

        public DiiagramrStartScreenCommand(Func<StartScreenViewModel> startScreenViewModelFactory)
        {
            _startScreenViewModel = startScreenViewModelFactory.Invoke();
        }

        public override string Name => ShellViewModel.StartCommandId + "DISABLED";

        internal override void ExecuteInternal(IApplicationShell shell, object parameter)
        {
            shell.ShowScreen(_startScreenViewModel);
            _startScreenViewModel.LoadCanceled += () => shell.ShowScreen(_startScreenViewModel);
        }
    }
}