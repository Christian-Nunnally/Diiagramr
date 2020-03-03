using DiiagramrAPI.Application.ShellCommands;
using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Service.Application;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Xunit;

namespace DiiagramrAPIUnitTests
{
    public class ToolbarUnitTests : UnitTest
    {
        [Fact]
        public void TopLevelMenuWithOneChild_OpenContextMenuForTopLevelMenu_ContextMenuOpensWithCommand()
        {
            var fakeContextMenu = new FakeContextMenu();
            var fakeCommand = new FakeCommand();
            IList<IShellCommand> commandsInContextMenu = null;
            fakeContextMenu.ShowContextMenuAction += (commands, point) => commandsInContextMenu = commands;
            var toolbar = CreateToolbar(new[] { fakeCommand }, fakeContextMenu);

            toolbar.OpenContextMenuForTopLevelMenu(new Point(0, 0), fakeCommand.ParentName);

            Assert.True(commandsInContextMenu.Single() == fakeCommand);
        }

        [Fact]
        public void TopLevelMenuItemWithTwoChildren_OpenContextMenuForTopLevelMenu_CommandsInContextMenuOrderedByWeight()
        {
            var fakeContextMenu = new FakeContextMenu();
            var fakeCommand1 = new FakeCommand("CommandParent1", "Command1", 0f);
            var fakeCommand2 = new FakeCommand("CommandParent1", "Command2", 1f);
            IList<IShellCommand> commandsInContextMenu = null;
            fakeContextMenu.ShowContextMenuAction += (commands, point) => commandsInContextMenu = commands;
            var toolbar = CreateToolbar(new[] { fakeCommand1, fakeCommand2 }, fakeContextMenu);

            toolbar.OpenContextMenuForTopLevelMenu(new Point(0, 0), fakeCommand1.ParentName);

            Assert.Equal(fakeCommand1, commandsInContextMenu.First());
            Assert.Equal(fakeCommand2, commandsInContextMenu.Last());
        }

        [Fact]
        public void TwoTopLevelMenuItems_TopLevelMenuNamesOrderedByFirstChildWeight()
        {
            var fakeContextMenu = new FakeContextMenu();
            var fakeCommand1 = new FakeCommand("CommandParent1", "Command1", 0f);
            var fakeCommand2 = new FakeCommand("CommandParent2", "Command2", 1f);
            IList<IShellCommand> commandsInContextMenu = null;
            fakeContextMenu.ShowContextMenuAction += (commands, point) => commandsInContextMenu = commands;
            var toolbar = CreateToolbar(new[] { fakeCommand1, fakeCommand2 }, fakeContextMenu);

            toolbar.OpenContextMenuForTopLevelMenu(new Point(0, 0), fakeCommand1.ParentName);

            Assert.Equal(fakeCommand1.ParentName, toolbar.TopLevelMenuNames.First());
            Assert.Equal(fakeCommand2.ParentName, toolbar.TopLevelMenuNames.Last());
        }

        [Fact]
        public void TopLevelMenuWithOneChild_OpenContextMenuForTopLevelMenu_ContextMenuOpensAtPosition()
        {
            var fakeContextMenu = new FakeContextMenu();
            var fakeCommand = new FakeCommand();
            Point contextMenuPosition;
            fakeContextMenu.ShowContextMenuAction += (commands, point) => contextMenuPosition = point;
            var toolbar = CreateToolbar(new[] { fakeCommand }, fakeContextMenu);
            var expectedPosition = new Point(5, 10);

            toolbar.OpenContextMenuForTopLevelMenu(expectedPosition, fakeCommand.ParentName);

            Assert.Equal(expectedPosition, contextMenuPosition);
        }

        private Toolbar CreateToolbar(
            IEnumerable<IToolbarCommand> commands,
            ContextMenuBase contextMenuBase)
        {
            return new Toolbar(
                CreateFactoryFor(commands),
                CreateFactoryFor(contextMenuBase));
        }
    }
}