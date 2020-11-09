using DiiagramrAPI.Application.ShellCommands;
using DiiagramrAPI.Application.Tools;
using System.Collections.Generic;
using System.Windows.Input;
using Xunit;

namespace DiiagramrAPIUnitTests
{
    public class HotkeyCommanderUnitTests : UnitTest
    {
        [Fact]
        public void HandleHotkeyPress_CommandWithHotkeyExecuted()
        {
            var fakeCommand = new FakeCommand();
            var hotkeyCommander = CreateHotkeyCommander(new[] { fakeCommand });

            hotkeyCommander.HandleHotkeyPress(fakeCommand.Hotkey);

            Assert.Equal(1, fakeCommand.ExecuteCount);
        }

        [Fact]
        public void InvalidKeyPressed_HandleHotkeyPress_ReturnsFalse()
        {
            var fakeCommand = new FakeCommand();
            var hotkeyCommander = CreateHotkeyCommander(new[] { fakeCommand });

            Assert.False(hotkeyCommander.HandleHotkeyPress(Key.B));
        }

        [Fact]
        public void InvalidKeyPressed_HandleHotkeyPress_CommandNotExecuted()
        {
            var fakeCommand = new FakeCommand();
            var hotkeyCommander = CreateHotkeyCommander(new[] { fakeCommand });
            Assert.NotEqual(Key.B, fakeCommand.Hotkey);

            hotkeyCommander.HandleHotkeyPress(Key.B);

            Assert.Equal(0, fakeCommand.ExecuteCount);
        }

        [Fact]
        public void ValidKeyPressed_HandleHotkeyPress_ReturnsTrue()
        {
            var fakeCommand = new FakeCommand();
            var hotkeyCommander = CreateHotkeyCommander(new[] { fakeCommand });

            Assert.True(hotkeyCommander.HandleHotkeyPress(fakeCommand.Hotkey));
        }

        private HotkeyCommander CreateHotkeyCommander(IEnumerable<IHotkeyCommand> commands)
        {
            return new HotkeyCommander(CreateFactoryFor(commands), () => new TestKeyboard());
        }

        private class TestKeyboard : IKeyboard
        {
            public bool IsKeyDown(Key key) => false;
        }
    }
}