using DiiagramrAPI.Application.ShellCommands;

namespace DiiagramrAPIUnitTests
{
    internal class FakeStartCommand : ShellCommandBase
    {
        public int ExecutCount { get; set; }
        public int CanExecutCount { get; set; }

        public override string Name => "Fake Start Command";

        protected override bool CanExecuteInternal()
        {
            CanExecutCount++;
            return true;
        }

        protected override void ExecuteInternal(object parameter)
        {
            ExecutCount++;
        }
    }
}