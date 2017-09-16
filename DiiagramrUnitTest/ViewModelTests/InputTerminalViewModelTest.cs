using System;
using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class InputTerminalViewModelTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructor_NullTerminal_ThrowsException()
        {
            new InputTerminalViewModel(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructor_TerminalKindOutput_ThrowsException()
        {
            new InputTerminalViewModel(new TerminalModel("", typeof(int), Direction.North, TerminalKind.Output, 0));
        }

        [TestMethod]
        public void TestConstructor_TerminalKindInput_Passes()
        {
            new InputTerminalViewModel(new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0));
        }

        [TestMethod]
        public void TestWireToTerminal_WireToInput_ReturnsFalse()
        {
            var terminalModel = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var inputTerminalViewModel = new InputTerminalViewModel(terminalModel);
            var otherTerminal = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0);
            Assert.IsFalse(inputTerminalViewModel.WireToTerminal(otherTerminal));
        }

        [TestMethod]
        public void TestWireToTerminal_WireToOutput_ReturnsTrue()
        {
            var terminalModel = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var inputTerminalViewModel = new InputTerminalViewModel(terminalModel);
            var otherTerminal = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Output, 0);
            Assert.IsTrue(inputTerminalViewModel.WireToTerminal(otherTerminal));
        }
    }
}
