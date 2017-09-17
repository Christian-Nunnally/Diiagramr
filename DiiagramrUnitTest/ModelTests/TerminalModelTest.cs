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
            var terminalModel = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var wireMoq = new Mock<WireModel>(terminalModel, terminalModel);
            var semanticsChanged = false;
            terminalModel.SemanticsChanged += () => semanticsChanged = true;
            terminalModel.ConnectedWire = wireMoq.Object;

            Assert.IsTrue(semanticsChanged);
        }
    }
}
