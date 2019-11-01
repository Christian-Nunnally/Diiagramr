using DiiagramrAPI.Shell;
using System;

namespace DiiagramrAPI.Shell.ShellCommands.StartupCommands
{
    public class DiiagramrStartScreenCommand : DiiagramrCommand
    {
        private readonly StartScreenViewModel _startScreenViewModel;

        public DiiagramrStartScreenCommand(Func<StartScreenViewModel> startScreenViewModelFactory)
        {
            _startScreenViewModel = startScreenViewModelFactory.Invoke();
        }

        public override string Name => ShellViewModel.StartCommandId + "DISABLED";

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
            shell.ShowScreen(_startScreenViewModel);
            _startScreenViewModel.LoadCanceled += () => shell.ShowScreen(_startScreenViewModel);
        }
    }
}
