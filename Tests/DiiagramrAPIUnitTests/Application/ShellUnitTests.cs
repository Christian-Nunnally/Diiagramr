using DiiagramrAPI.Application;
using DiiagramrAPI.Application.ShellCommands;
using DiiagramrAPI.Application.Tools;
using System.Linq;
using System.Windows.Input;
using Xunit;

namespace DiiagramrAPIUnitTests
{
    public class ShellUnitTests : UnitTest
    {
        [Fact]
        public void PreviewKeyDown_ShellCallsHotkeyCommander()
        {
            var hotkeyCommander = new FakeHotkeyCommander();
            var shell = CreateShell(null, hotkeyCommander, null, null, null, null);
            Assert.False(hotkeyCommander.HandledHotkeyPressed.Any());
            shell.PreviewKeyDown(Key.A);

            Assert.True(hotkeyCommander.HandledHotkeyPressed.Single() == Key.A);
        }

        [Fact]
        public void HotkeyHandled_PreviewKeyDownReturnsTrue()
        {
            var hotkeyCommander = new FakeHotkeyCommander { HandleHotkeyPressReturn = true };
            var shell = CreateShell(null, hotkeyCommander, null, null, null, null);

            Assert.True(shell.PreviewKeyDown(Key.A));
        }

        [Fact]
        public void HotkeyNotHandled_PreviewKeyDownReturnsFalse()
        {
            var fakeHotkeyCommander = new FakeHotkeyCommander { HandleHotkeyPressReturn = false };
            var shell = CreateShell(null, fakeHotkeyCommander, null, null, null, null);

            Assert.False(shell.PreviewKeyDown(Key.A));
        }

        [Fact]
        public void RequestCloseWithTrueDialogResult_ShellDoesNotRequestScreensClose()
        {
            var fakeScreenHost = new FakeScreenHost();
            var requestScreenCloseCount = 0;
            fakeScreenHost.InteractivelyCloseAllScreensAction += () => requestScreenCloseCount++;
            var shell = CreateShell(null, null, null, fakeScreenHost, null, null);
            Assert.Equal(0, requestScreenCloseCount);

            shell.RequestClose(true);

            Assert.Equal(0, requestScreenCloseCount);
        }

        [Fact]
        public void RequestCloseWithFalseDialogResult_ShellRequestScreensClose()
        {
            var fakeScreenHost = new FakeScreenHost();
            var requestScreenCloseCount = 0;
            fakeScreenHost.InteractivelyCloseAllScreensAction += () => requestScreenCloseCount++;
            var shell = CreateShell(null, null, null, fakeScreenHost, null, null);
            Assert.Equal(0, requestScreenCloseCount);

            shell.RequestClose();

            Assert.Equal(1, requestScreenCloseCount);
        }

        [Fact]
        public void ShellConstructed_StartCommandExecuted()
        {
            var fakeStartCommand = new FakeCommand();
            Assert.Equal(0, fakeStartCommand.ExecuteCount);
            CreateShell(fakeStartCommand, null, null, null, null, null);

            Assert.Equal(1, fakeStartCommand.ExecuteCount);
        }

        private Shell CreateShell(
            ShellCommandBase shellCommand,
            IHotkeyHandler hotkeyCommander,
            ContextMenuBase contextMenuBase,
            ScreenHostBase screenHostBase,
            DialogHostBase dialogHostBase,
            ToolbarBase toolbarBase)
        {
            return new Shell(
                CreateFactoryFor(shellCommand, "startCommand"),
                CreateFactoryFor(hotkeyCommander),
                CreateFactoryFor(contextMenuBase),
                CreateFactoryFor(screenHostBase),
                CreateFactoryFor(dialogHostBase),
                CreateFactoryFor(toolbarBase));
        }
    }
}