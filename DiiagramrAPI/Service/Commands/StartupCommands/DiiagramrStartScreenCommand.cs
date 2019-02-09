using DiiagramrAPI.ViewModel;
using System;

namespace DiiagramrAPI.Service.Commands.StartupCommands
{
    public class DiiagramrStartScreenCommand : DiiagramrCommand
    {
        private readonly StartScreenViewModel _startScreenViewModel;

        public DiiagramrStartScreenCommand(Func<StartScreenViewModel> startScreenViewModelFactory)
        {
            _startScreenViewModel = startScreenViewModelFactory.Invoke();
        }

        public override string Name => ShellViewModel.StartCommandId;

        public override void Execute(ShellViewModel shell)
        {
            shell.ShowScreen(_startScreenViewModel);
            _startScreenViewModel.LoadCanceled += () => shell.ShowScreen(_startScreenViewModel);
        }
    }
}
