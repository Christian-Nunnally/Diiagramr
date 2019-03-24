using DiiagramrAPI.Diagram;
using DiiagramrAPI.Diagram.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class OutputTerminalViewModelTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructor_NullTerminal_ThrowsException()
        {
            new OutputTerminal(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructor_TerminalKindInput_ThrowsException()
        {
            new OutputTerminal(new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0));
        }

        [TestMethod]
        public void TestConstructor_TerminalKindOutput_Passes()
        {
            new OutputTerminal(new TerminalModel("", typeof(int), Direction.North, TerminalKind.Output, 0));
        }
    }
}
