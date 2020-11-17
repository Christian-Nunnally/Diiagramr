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

        /// <summary>
        /// Creates a new instance of <see cref="VisualDropStartScreenCommand"/>
        /// </summary>
        /// <param name="visualDropStartScreenFactory">Factory to get an instance of an <see cref="VisualDropStartScreen"/>.</param>
        /// <param name="screenHostFactory">Factory to get an instance of an <see cref="ScreenHostBase"/>.</param>
        public VisualDropStartScreenCommand(
            Func<VisualDropStartScreen> visualDropStartScreenFactory,
            Func<ScreenHostBase> screenHostFactory)
        {
            _visualDropStartScreenViewModel = visualDropStartScreenFactory();
            _screenHost = screenHostFactory();
        }

        /// <inheritdoc/>
        public override string Name => "Start";

        /// <inheritdoc/>
        protected override bool CanExecuteInternal()
        {
            return true;
        }

        /// <inheritdoc/>
        protected override void ExecuteInternal(object parameter)
        {
            _screenHost.ShowScreen(_visualDropStartScreenViewModel);
        }
    }
}