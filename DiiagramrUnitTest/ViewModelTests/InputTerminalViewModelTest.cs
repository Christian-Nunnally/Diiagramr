using DiiagramrAPI.Diagram;
using DiiagramrAPI.Diagram.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class InputTerminalViewModelTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructor_NullTerminal_ThrowsException()
        {
            new InputTerminal(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructor_TerminalKindOutput_ThrowsException()
        {
            new InputTerminal(new TerminalModel("", typeof(int), Direction.North, TerminalKind.Output, 0));
        }

        [TestMethod]
        public void TestConstructor_TerminalKindInput_Passes()
        {
            new InputTerminal(new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0));
        }
    }
}
