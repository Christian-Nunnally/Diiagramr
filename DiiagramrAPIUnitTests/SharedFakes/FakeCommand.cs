using DiiagramrAPI.Application.ShellCommands;
using System.Windows.Input;

namespace DiiagramrAPIUnitTests
{
    internal class FakeCommand : ShellCommandBase, IToolbarCommand, IHotkeyCommand
    {
        public FakeCommand() : this("FakeCommandParentName", "FakeCommandName", 0)
        {
        }

        public FakeCommand(string parentName, string name, float weight)
        {
            ParentName = parentName;
            Name = name;
            Weight = weight;
        }

        public int ExecuteCount { get; set; }

        public int CanExecutCount { get; set; }

        public override string Name { get; }

        public string ParentName { get; }

        public float Weight { get; }

        public Key Hotkey => Key.A;

        public bool RequiresCtrlModifierKey => true;

        public bool RequiresAltModifierKey => true;

        public bool RequiresShiftModifierKey => true;

        protected override bool CanExecuteInternal()
        {
            CanExecutCount++;
            return true;
        }

        protected override void ExecuteInternal(object parameter)
        {
            ExecuteCount++;
        }
    }
}