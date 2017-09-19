using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ModelTests
{
    
    [TestClass]
    public class TerminalModelTest
    {
        private TerminalModel _terminalOut;
        private TerminalModel _terminalIn;
        private Mock<WireModel> _wireMoq;

        [TestInitialize]
        public void SetupTests()
        {
            _terminalOut = new TerminalModel("", typeof(int), Direction.East, TerminalKind.Output, 0);
            _terminalIn = new TerminalModel("", typeof(int), Direction.East, TerminalKind.Input, 0);
            _wireMoq = new Mock<WireModel>(_terminalIn, _terminalOut);
            _terminalOut.ConnectedWire = _wireMoq.Object;
        }

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

        [TestMethod]
        public void TestEnableWire_CallsEnableWire()
        {
            _terminalOut.EnableWire();
            Assert.IsNotNull(_terminalOut.ConnectedWire);
            _wireMoq.Verify(m => m.EnableWire(), Times.Once);
        }

        [TestMethod]
        public void TestEnableWire_WireNullNoException()
        {
            var terminalModelInput = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0);
            terminalModelInput.EnableWire();
        }

        [TestMethod]
        public void TestDisableWire_CallsDisableWire()
        {
            _terminalOut.DisableWire();
            _wireMoq.Verify(m => m.DisableWire(), Times.Once);
        }

        [TestMethod]
        public void TestDisableWire_WireNullNoException()
        {
            var terminalModelInput = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0);
            terminalModelInput.DisableWire();
        }

        [TestMethod]
        public void TestResetWire_CallsResetWire()
        {
            _terminalOut.ResetWire();
            _wireMoq.Verify(m => m.ResetWire(), Times.Once);
        }

        [TestMethod]
        public void TestResetWire_WireNullNoException()
        {
            var terminalModelInput = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0);
            terminalModelInput.ResetWire();
        }
    }
}
