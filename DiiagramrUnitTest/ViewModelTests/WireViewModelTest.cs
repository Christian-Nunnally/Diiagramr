using System;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class WireViewModelTest
    {
        private Mock<TerminalModel> _inputTerminalMoq;
        private Mock<TerminalModel> _outputTerminalMoq;

        [TestInitialize]
        public void TestInitialize()
        {
            _inputTerminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Input, 0);
            _outputTerminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Output, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructor_WireNull_ThrowsException()
        {
            new WireViewModel(null);
        }

        [TestMethod]
        public void TestConstructor_SetsPositionToWiresPositionAndAddBorderWidth()
        {
            var wireMoq = new Mock<WireModel>(_inputTerminalMoq.Object, _outputTerminalMoq.Object);
            wireMoq.SetupGet(m => m.X1).Returns(1);
            wireMoq.SetupGet(m => m.Y1).Returns(2);
            wireMoq.SetupGet(m => m.X2).Returns(3);
            wireMoq.SetupGet(m => m.Y2).Returns(4);
            var wireViewModel = new WireViewModel(wireMoq.Object);

            Assert.AreEqual(wireMoq.Object.X1 + DiagramConstants.NodeBorderWidth, wireViewModel.X1);
            Assert.AreEqual(wireMoq.Object.Y1 + DiagramConstants.NodeBorderWidth, wireViewModel.Y1);
            Assert.AreEqual(wireMoq.Object.X2 + DiagramConstants.NodeBorderWidth, wireViewModel.X2);
            Assert.AreEqual(wireMoq.Object.Y2 + DiagramConstants.NodeBorderWidth, wireViewModel.Y2);
        }

        [TestMethod]
        public void TestConstructor_WireModelSet()
        {
            var wireMoq = new Mock<WireModel>(_inputTerminalMoq.Object, _outputTerminalMoq.Object);
            var wireViewModel = new WireViewModel(wireMoq.Object);

            Assert.AreEqual(wireMoq.Object, wireViewModel.WireModel);
        }

        [TestMethod]
        public void TestWireMouseDown_WireModelDisconnectTerminalInvoked()
        {
            var wireMoq = new Mock<WireModel>(_inputTerminalMoq.Object, _outputTerminalMoq.Object);
            var wireViewModel = new WireViewModel(wireMoq.Object);

            wireViewModel.WireMouseDown(null, null);
            wireMoq.Verify(m => m.DisconnectWire());
        }

        [TestMethod]
        public void TestDisconnectWire_WireModelDisconnectTerminalInvoked()
        {
            var wireMoq = new Mock<WireModel>(_inputTerminalMoq.Object, _outputTerminalMoq.Object);
            var wireViewModel = new WireViewModel(wireMoq.Object);

            wireViewModel.DisconnectWire();
            wireMoq.Verify(m => m.DisconnectWire());
        }
    }
}
