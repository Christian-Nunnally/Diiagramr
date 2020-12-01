using DiiagramrAPI.Application.Tools;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPIUnitTests
{
    public class FakeToolbar : ToolbarBase
    {
        public override ObservableCollection<string> TopLevelMenuNames => throw new System.NotImplementedException();

        public override void OpenContextMenuForTopLevelMenu(Point position, string topLevelMenuName)
        {
        }

        public override void OpenContextMenuForTopLevelMenuHandler(object sender, MouseEventArgs e)
        {
        }
    }
}