﻿using System;

namespace DiiagramrAPI.Application.ShellCommands.StartupCommands
{
    /// <summary>
    /// Command that opens the visual drop themed start screen when executed.
    /// </summary>
    public class VisualDropStartScreenCommand : ShellCommandBase, IToolbarCommand
    {
        private readonly VisualDropStartScreen _visualDropStartScreenViewModel;
        private readonly ScreenHost _screenHost;

        public VisualDropStartScreenCommand(
            Func<VisualDropStartScreen> visualDropStartScreenFactory,
            Func<ScreenHost> screenHostFactory)
        {
            _visualDropStartScreenViewModel = visualDropStartScreenFactory();
            _screenHost = screenHostFactory();
        }

        public override string Name => "start";

        public float Weight => 0.2f;

        public string ParentName => "start";

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