using DiiagramrAPI.Application.ShellCommands;
using DiiagramrAPI.Application.Tools;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DiiagramrAPIUnitTests
{
    public class FakeToolbar : ToolbarBase
    {
        public override ObservableCollection<IToolbarCommand> TopLevelMenuItems => throw new System.NotImplementedException();

        public override void ExecuteCommandHandler(object sender, MouseEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}