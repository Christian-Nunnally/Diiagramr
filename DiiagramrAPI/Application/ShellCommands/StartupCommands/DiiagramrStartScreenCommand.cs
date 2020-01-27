using System;

namespace DiiagramrAPI.Application.ShellCommands.StartupCommands
{
    public class DiiagramrStartScreenCommand : ShellCommandBase
    {
        private readonly StartScreen _startScreenViewModel;
        private readonly ScreenHost _screenHost;

        public DiiagramrStartScreenCommand(
            Func<ScreenHost> screenHostFactory,
            Func<StartScreen> startScreenFactory)
        {
            _screenHost = screenHostFactory();
            _startScreenViewModel = startScreenFactory();
        }

        public override string Name => Shell.StartCommandId + "DISABLED";

        protected override bool CanExecuteInternal()
        {
            return true;
        }

        protected override void ExecuteInternal(object parameter)
        {
            _screenHost.ShowScreen(_startScreenViewModel);
            _startScreenViewModel.LoadCanceled += () => _screenHost.ShowScreen(_startScreenViewModel);
        }
    }
}