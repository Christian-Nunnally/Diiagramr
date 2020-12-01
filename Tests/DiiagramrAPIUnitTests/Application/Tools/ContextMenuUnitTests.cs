using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Service.Application;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Xunit;

namespace DiiagramrAPIUnitTests
{
    public class ContextMenuUnitTests : UnitTest
    {
        [Fact]
        public void ShowContextMenu_XSetToPointX()
        {
            var contextMenu = new ContextMenu();
            Assert.NotEqual(5, contextMenu.X);
            contextMenu.ShowContextMenu(new List<IShellCommand>(), new Point(5, 5));

            Assert.Equal(5, contextMenu.X);
        }

        [Fact]
        public void ShowContextMenu_YSetToPointY()
        {
            var contextMenu = new ContextMenu();
            Assert.NotEqual(5, contextMenu.Y);
            contextMenu.ShowContextMenu(new List<IShellCommand>(), new Point(5, 5));

            Assert.Equal(5, contextMenu.Y);
        }

        [Fact]
        public void ShowContextMenuWithCommand_CommandAddedToObservableCollection()
        {
            var fakeCommand = new FakeCommand();
            var contextMenu = new ContextMenu();
            Assert.False(contextMenu.Commands.FirstOrDefault() == fakeCommand);
            contextMenu.ShowContextMenu(new List<IShellCommand> { fakeCommand }, new Point(5, 5));

            Assert.True(contextMenu.Commands.Single() == fakeCommand);
        }

        [Fact]
        public void ShowContextMenuWithCommands_CommandsAddedToObservableCollectionInOrder()
        {
            var fakeCommand1 = new FakeCommand();
            var fakeCommand2 = new FakeCommand();
            var contextMenu = new ContextMenu();
            Assert.False(contextMenu.Commands.FirstOrDefault() == fakeCommand1);
            Assert.False(contextMenu.Commands.LastOrDefault() == fakeCommand2);
            contextMenu.ShowContextMenu(new List<IShellCommand> { fakeCommand1, fakeCommand2 }, new Point(5, 5));

            Assert.True(contextMenu.Commands.First() == fakeCommand1);
            Assert.True(contextMenu.Commands.Last() == fakeCommand2);
        }

        [Fact]
        public void ContextMenuVisibleWithCommand_ShowContextMenuWithNewCommand_OnlyNewCommandInMenu()
        {
            var oldCommand = new FakeCommand();
            var newCommand = new FakeCommand();
            var contextMenu = new ContextMenu();
            contextMenu.ShowContextMenu(new List<IShellCommand> { oldCommand }, new Point(5, 5));
            Assert.True(contextMenu.Commands.Single() == oldCommand);

            contextMenu.ShowContextMenu(new List<IShellCommand> { newCommand }, new Point(5, 5));

            Assert.True(contextMenu.Commands.Single() == newCommand);
        }

        [Fact]
        public void ExecuteCommand_CommandExecuted()
        {
            var fakeCommand = new FakeCommand();
            var contextMenu = new ContextMenu();
            Assert.Equal(0, fakeCommand.ExecuteCount);
            contextMenu.ExecuteCommand(fakeCommand);

            Assert.Equal(1, fakeCommand.ExecuteCount);
        }

        [Fact]
        public void ExecuteCommand_CommandsEmpty()
        {
            var fakeCommand = new FakeCommand();
            var contextMenu = new ContextMenu();
            contextMenu.ShowContextMenu(new[] { fakeCommand }, new Point(0, 0));
            Assert.NotEmpty(contextMenu.Commands);
            contextMenu.ExecuteCommand(fakeCommand);

            Assert.Empty(contextMenu.Commands);
        }

        [Fact]
        public void ShowContextManu_CommandsPropertyChanged()
        {
            var fakeCommand = new FakeCommand();
            var contextMenu = new ContextMenu();
            var propertyChangedEvents = new List<PropertyChangedEventArgs>();
            contextMenu.PropertyChanged += (s, e) => propertyChangedEvents.Add(e);

            contextMenu.ShowContextMenu(new[] { fakeCommand }, new Point(0, 0));

            Assert.Equal(1, propertyChangedEvents.Count(x => x.PropertyName == nameof(contextMenu.Commands)));
        }

        [Fact]
        public void ExecuteCommand_CommandsPropertyChanged()
        {
            var fakeCommand = new FakeCommand();
            var contextMenu = new ContextMenu();
            var propertyChangedEvents = new List<PropertyChangedEventArgs>();
            contextMenu.PropertyChanged += (s, e) => propertyChangedEvents.Add(e);

            contextMenu.ExecuteCommand(fakeCommand);

            Assert.Equal(1, propertyChangedEvents.Count(x => x.PropertyName == nameof(contextMenu.Commands)));
        }

        private void ContextMenu_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}