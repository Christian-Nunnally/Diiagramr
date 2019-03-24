using DiiagramrAPI.Diagram;
using DiiagramrAPI.Diagram.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

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
            new Wire(null);
        }

        [TestMethod]
        public void TestConstructor_SetsPositionToWiresPositionAndAddBorderWidth()
        {
            var wireMoq = new Mock<WireModel>(_inputTerminalMoq.Object, _outputTerminalMoq.Object);
            wireMoq.SetupGet(m => m.X1).Returns(1);
            wireMoq.SetupGet(m => m.Y1).Returns(2);
            wireMoq.SetupGet(m => m.X2).Returns(3);
            wireMoq.SetupGet(m => m.Y2).Returns(4);
            var wireViewModel = new Wire(wireMoq.Object);

            Assert.AreEqual(wireMoq.Object.X1 + DiiagramrAPI.Diagram.Diagram.NodeBorderWidth, wireViewModel.X1);
            Assert.AreEqual(wireMoq.Object.Y1 + DiiagramrAPI.Diagram.Diagram.NodeBorderWidth, wireViewModel.Y1);
            Assert.AreEqual(wireMoq.Object.X2 + DiiagramrAPI.Diagram.Diagram.NodeBorderWidth, wireViewModel.X2);
            Assert.AreEqual(wireMoq.Object.Y2 + DiiagramrAPI.Diagram.Diagram.NodeBorderWidth, wireViewModel.Y2);
        }

        [TestMethod]
        public void TestConstructor_WireModelSet()
        {
            var wireMoq = new Mock<WireModel>(_inputTerminalMoq.Object, _outputTerminalMoq.Object);
            var wireViewModel = new Wire(wireMoq.Object);

            Assert.AreEqual(wireMoq.Object, wireViewModel.Model);
        }

        [TestMethod]
        public void TestDisconnectWire_WireModelDisconnectTerminalInvoked()
        {
            var wireMoq = new Mock<WireModel>(_inputTerminalMoq.Object, _outputTerminalMoq.Object);
            var wireViewModel = new Wire(wireMoq.Object);

            wireViewModel.DisconnectWire();
            wireMoq.Verify(m => m.DisconnectWire());
        }
    }
}
