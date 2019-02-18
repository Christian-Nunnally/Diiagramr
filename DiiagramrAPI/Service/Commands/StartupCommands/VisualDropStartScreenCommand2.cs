using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.VisualDrop;
using System;

namespace DiiagramrAPI.Service.Commands.StartupCommands
{
    public class VisualDropStartScreenCommand2 : DiiagramrCommand
    {
        private readonly VisualDropStartScreenViewModel _visualDropStartScreenViewModel;

        public VisualDropStartScreenCommand2(Func<VisualDropStartScreenViewModel> visualDropStartScreenViewModelFactory)
        {
            _visualDropStartScreenViewModel = visualDropStartScreenViewModelFactory.Invoke();
        }

        public override string Name => ShellViewModel.StartCommandId;

        public override float Weight => 0.2f;

        internal override void ExecuteInternal(ShellViewModel shell, object parameter)
        {
            shell.ShowScreen(_visualDropStartScreenViewModel);
            shell.WindowTitle = "Visual Drop";
            _visualDropStartScreenViewModel.LoadCanceled += () => shell.ShowScreen(_visualDropStartScreenViewModel);
        }
    }
}
