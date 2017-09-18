using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ModelTests
{
    [TestClass]
    public class TerminalModelTest
    {
        [TestMethod]
        public void TestSemanticsChanged_ConnectedWireChanged_SemanticsChangedInvoked()
        {
            var terminalModelInput = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var terminalModelOutput = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Output, 0);
            var wireMoq = new Mock<WireModel>(terminalModelInput, terminalModelOutput);
            var semanticsChanged = false;
            terminalModelInput.SemanticsChanged += () => semanticsChanged = true;
            terminalModelInput.ConnectedWire = wireMoq.Object;

            Assert.IsTrue(semanticsChanged);
        }
    }
}
