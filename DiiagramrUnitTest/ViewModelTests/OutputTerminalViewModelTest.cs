using System;
using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class OutputTerminalViewModelTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructor_NullTerminal_ThrowsException()
        {
            new OutputTerminalViewModel(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructor_TerminalKindInput_ThrowsException()
        {
            new OutputTerminalViewModel(new TerminalModel("", typeof(int), Direction.None, TerminalKind.Input, 0));
        }

        [TestMethod]
        public void TestConstructor_TerminalKindOutput_Passes()
        {
            new OutputTerminalViewModel(new TerminalModel("", typeof(int), Direction.None, TerminalKind.Output, 0));
        }

        [TestMethod]
        public void TestWireToTerminal_WireToOutput_ReturnsFalse()
        {
            var terminalModel = new TerminalModel("", typeof(int), Direction.None, TerminalKind.Output, 0);
            var inputTerminalViewModel = new OutputTerminalViewModel(terminalModel);
            var otherTerminal = new TerminalModel("", typeof(int), Direction.None, TerminalKind.Output, 0);
            Assert.IsFalse(inputTerminalViewModel.WireToTerminal(otherTerminal));
        }

        [TestMethod]
        public void TestWireToTerminal_WireToInput_ReturnsTrue()
        {
            var terminalModel = new TerminalModel("", typeof(int), Direction.None, TerminalKind.Output, 0);
            var inputTerminalViewModel = new OutputTerminalViewModel(terminalModel);
            var otherTerminal = new TerminalModel("", typeof(int), Direction.None, TerminalKind.Input, 0);
            Assert.IsTrue(inputTerminalViewModel.WireToTerminal(otherTerminal));
        }
    }
}
