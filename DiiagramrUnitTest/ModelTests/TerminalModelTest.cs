using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.PluginNodeApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.ComponentModel;

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
            _terminalOut.ConnectWire(_wireMoq.Object);
        }

        [TestMethod]
        public void TestOnTerminalPropertyChanged_ConnectedWireChanged_SemanticsChangedInvoked()
        {
            var terminalModelInput = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var terminalModelOutput = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Output, 0);
            var wireMoq = new Mock<WireModel>(terminalModelInput, terminalModelOutput);
            var semanticsChanged = false;
            terminalModelInput.SemanticsChanged += () => semanticsChanged = true;
            terminalModelInput.ConnectWire(wireMoq.Object);

            Assert.IsTrue(semanticsChanged);
        }

        [TestMethod]
        public void TestEnableWire_CallsEnableWire()
        {
            _terminalOut.EnableWire();
            Assert.AreNotEqual(0, _terminalOut.ConnectedWires.Count);
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

        [TestMethod]
        public void TestAddToNode_TerminalNodePositionSet()
        {
            var terminalModel = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var nodeMoq = new Mock<NodeModel>(string.Empty);
            nodeMoq.SetupGet(model => model.X).Returns(20);
            nodeMoq.SetupGet(model => model.Y).Returns(30);

            terminalModel.AddToNode(nodeMoq.Object);

            Assert.AreEqual(20, terminalModel.NodeX);
            Assert.AreEqual(30, terminalModel.NodeY);
        }

        [TestMethod]
        public void TestNodePropertyChanged_NodeXChanged_NodeXOnTerminalSet()
        {
            var terminalModel = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var nodeMoq = new Mock<NodeModel>(string.Empty);
            nodeMoq.SetupGet(model => model.X).Returns(20);
            Assert.AreEqual(0, terminalModel.NodeX);

            terminalModel.NodePropertyChanged(nodeMoq.Object, new PropertyChangedEventArgs(nameof(NodeModel.X)));

            Assert.AreEqual(20, terminalModel.NodeX);
        }

        [TestMethod]
        public void TestNodePropertyChanged_NodeYChanged_NodeYOnTerminalSet()
        {
            var terminalModel = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var nodeMoq = new Mock<NodeModel>(string.Empty);
            nodeMoq.SetupGet(model => model.Y).Returns(20);
            Assert.AreEqual(0, terminalModel.NodeY);

            terminalModel.NodePropertyChanged(nodeMoq.Object, new PropertyChangedEventArgs(nameof(NodeModel.Y)));

            Assert.AreEqual(20, terminalModel.NodeY);
        }

        [TestMethod]
        public void TestOnTerminalPropertyChanged_NodeXChanged_TerminalXSetToOffsetXPlusNodeX()
        {
            var terminalModel = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0)
            {
                OffsetX = 2,
                NodeX = 1
            };
            Assert.AreEqual(terminalModel.NodeX + terminalModel.OffsetX, terminalModel.X);
        }

        [TestMethod]
        public void TestOnTerminalPropertyChanged_OffsetXChanged_TerminalXSetToOffsetXPlusNodeX()
        {
            var terminalModel = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0)
            {
                NodeX = 6,
                OffsetX = 4
            };
            Assert.AreEqual(terminalModel.NodeX + terminalModel.OffsetX, terminalModel.X);
        }

        [TestMethod]
        public void TestOnTerminalPropertyChanged_NodeYChanged_TerminalYSetToOffsetYPlusNodeX()
        {
            var terminalModel = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0)
            {
                OffsetY = 2,
                NodeY = 1
            };
            Assert.AreEqual(terminalModel.NodeY + terminalModel.OffsetY, terminalModel.Y);
        }

        [TestMethod]
        public void TestOnTerminalPropertyChanged_OffsetYChanged_TerminalXSetToOffsetXPlusNodeX()
        {
            var terminalModel = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0)
            {
                NodeY = 6,
                OffsetY = 4
            };
            Assert.AreEqual(terminalModel.NodeY + terminalModel.OffsetY, terminalModel.Y);
        }

        [TestMethod]
        public void TestDisconnectWire_DisconnectWireOnConnectedWireInvoked()
        {
            var terminalModelInput = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var terminalModelOutput = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Output, 0);
            var wireMoq = new Mock<WireModel>(terminalModelInput, terminalModelOutput);
            terminalModelInput.ConnectWire(wireMoq.Object);
            terminalModelInput.DisconnectWire(wireMoq.Object);
            Assert.AreEqual(0, terminalModelInput.ConnectedWires.Count);
        }
    }
}
