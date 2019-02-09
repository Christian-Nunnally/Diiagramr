using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.IdDrop;
using System;

namespace DiiagramrAPI.Service.Commands.StartupCommands
{
    public class IdDropStartScreenCommand : DiiagramrCommand
    {
        private readonly IdDropStartScreenViewModel _idDropStartScreenViewModel;

        public IdDropStartScreenCommand(Func<IdDropStartScreenViewModel> idDropStartScreenViewModelFactory)
        {
            _idDropStartScreenViewModel = idDropStartScreenViewModelFactory.Invoke();
        }

        public override string Name => ShellViewModel.StartCommandId;
        public override float Weight => -0.1f;

        public override void Execute(ShellViewModel shell)
        {
            shell.ShowScreen(_idDropStartScreenViewModel);
        }
    }
}
