using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.VisualDrop;
using System;

namespace DiiagramrAPI.Service.Commands.StartupCommands
{
    public class VisualDropStartScreenCommand : DiiagramrCommand
    {
        private readonly VisualDropStartScreenViewModel _idDropStartScreenViewModel;

        public VisualDropStartScreenCommand(Func<VisualDropStartScreenViewModel> idDropStartScreenViewModelFactory)
        {
            _idDropStartScreenViewModel = idDropStartScreenViewModelFactory.Invoke();
        }

        public override string Name => ShellViewModel.StartCommandId + "DISABLED";

        public override float Weight => 0.1f;

        internal override void ExecuteInternal(ShellViewModel shell, object parameter)
        {
            shell.ShowScreen(_idDropStartScreenViewModel);
            shell.WindowTitle = "Visual Drop";
        }
    }
}
