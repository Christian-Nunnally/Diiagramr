using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Service.Application;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPIUnitTests
{
    internal class FakeContextMenu : ContextMenuBase
    {
        public event Action<IList<IShellCommand>, Point> ShowContextMenuAction;

        public override ObservableCollection<IShellCommand> Commands { get; } = new ObservableCollection<IShellCommand>();

        public override void ExecuteCommand(IShellCommand command)
        {
        }

        public override void ExecuteCommandHandler(object sender, MouseEventArgs e)
        {
        }

        public override void MouseLeft()
        {
        }

        public override void ShowContextMenu(IList<IShellCommand> commands, Point position)
        {
            ShowContextMenuAction?.Invoke(commands, position);
        }
    }
}