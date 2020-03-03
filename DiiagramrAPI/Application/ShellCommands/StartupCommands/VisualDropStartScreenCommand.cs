using System;

namespace DiiagramrAPI.Application.ShellCommands.StartupCommands
{
    /// <summary>
    /// Command that opens the visual drop themed start screen when executed.
    /// </summary>
    public class VisualDropStartScreenCommand : ShellCommandBase
    {
        private readonly VisualDropStartScreen _visualDropStartScreenViewModel;
        private readonly ScreenHostBase _screenHost;

        public VisualDropStartScreenCommand(
            Func<VisualDropStartScreen> visualDropStartScreenFactory,
            Func<ScreenHostBase> screenHostFactory)
        {
            _visualDropStartScreenViewModel = visualDropStartScreenFactory();
            _screenHost = screenHostFactory();
        }

        public override string Name => "Start";

        protected override bool CanExecuteInternal()
        {
            return true;
        }

        protected override void ExecuteInternal(object parameter)
        {
            _screenHost.ShowScreen(_visualDropStartScreenViewModel);
        }
    }
}