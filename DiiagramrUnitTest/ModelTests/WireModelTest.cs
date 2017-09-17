using System;
using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ModelTests
{
    [TestClass]
    public class WireModelTest
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
        public void TestConstructor_NullFirstArgument_ThrowsException()
        {
            new WireModel(null, _outputTerminalMoq.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructor_NullSecondArgument_ThrowsException()
        {
            new WireModel(_inputTerminalMoq.Object, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructor_TwoInputTerminalsPassedIn_ThrowsException()
        {
            new WireModel(_inputTerminalMoq.Object, _inputTerminalMoq.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructor_TwoOutputTerminalsPassedIn_ThrowsException()
        {
            new WireModel(_outputTerminalMoq.Object, _outputTerminalMoq.Object);
        }

        [TestMethod]
        public void TestConstructor_InputTerminalFirstArgument_SinkTerminalSetToInputTerminal()
        {
            var wire = new WireModel(_inputTerminalMoq.Object, _outputTerminalMoq.Object);
            Assert.AreEqual(_inputTerminalMoq.Object, wire.SinkTerminal);
        }

        [TestMethod]
        public void TestConstructor_InputTerminalSecondArgument_SinkTerminalSetToInputTerminal()
        {
            var wire = new WireModel(_outputTerminalMoq.Object, _inputTerminalMoq.Object);
            Assert.AreEqual(_inputTerminalMoq.Object, wire.SinkTerminal);
        }

        [TestMethod]
        public void TestConstructor_OutputTerminalFirstArgument_SourceTerminalSetToInputTerminal()
        {
            var wire = new WireModel(_inputTerminalMoq.Object, _outputTerminalMoq.Object);
            Assert.AreEqual(_outputTerminalMoq.Object, wire.SourceTerminal);
        }

        [TestMethod]
        public void TestConstructor_OutputTerminalSecondArgument_SourceTerminalSetToInputTermina()
        {
            var wire = new WireModel(_outputTerminalMoq.Object, _inputTerminalMoq.Object);
            Assert.AreEqual(_outputTerminalMoq.Object, wire.SourceTerminal);
        }

        [TestMethod]
        public void TestConstructor_DisconnectsSourceTerminal()
        {
            new WireModel(_outputTerminalMoq.Object, _inputTerminalMoq.Object);
            _outputTerminalMoq.Verify(m => m.DisconnectWire());
        }

        [TestMethod]
        public void TestConstructor_DisconnectsSinkTerminal()
        {
            new WireModel(_outputTerminalMoq.Object, _inputTerminalMoq.Object);
            _inputTerminalMoq.Verify(m => m.DisconnectWire());
        }

        [TestMethod]
        public void TestConstructor_SetsSourceTerminalConnectedWireToSelf()
        {
            var wire = new WireModel(_outputTerminalMoq.Object, _inputTerminalMoq.Object);
            _outputTerminalMoq.VerifySet(m => m.ConnectedWire = wire);
        }

        [TestMethod]
        public void TestConstructor_SetsSinkTerminalConnectedWireToSelf()
        {
            var wire = new WireModel(_outputTerminalMoq.Object, _inputTerminalMoq.Object);
            _inputTerminalMoq.VerifySet(m => m.ConnectedWire = wire);
        }

        [TestMethod]
        public void TestConstructor_StartPointSetToSinkPosition()
        {
            _inputTerminalMoq.SetupGet(m => m.X).Returns(5);
            _inputTerminalMoq.SetupGet(m => m.Y).Returns(6);
            var wire = new WireModel(_outputTerminalMoq.Object, _inputTerminalMoq.Object);
            
            Assert.AreEqual(5, wire.X1);
            Assert.AreEqual(6, wire.Y1);
        }

        [TestMethod]
        public void TestConstructor_EndPointSetToSourcePosition()
        {
            _outputTerminalMoq.SetupGet(m => m.X).Returns(5);
            _outputTerminalMoq.SetupGet(m => m.Y).Returns(6);
            var wire = new WireModel(_outputTerminalMoq.Object, _inputTerminalMoq.Object);

            Assert.AreEqual(5, wire.X2);
            Assert.AreEqual(6, wire.Y2);
        }

        [TestMethod]
        public void TestConstructor_TypesIncompatible_DoesNotSetConnectedWire()
        {
            var inputTerminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var outputTerminalMoq = new Mock<TerminalModel>("", typeof(string), Direction.North, TerminalKind.Output, 0);
            var wire = new WireModel(outputTerminalMoq.Object, inputTerminalMoq.Object);
            inputTerminalMoq.VerifySet(m => m.ConnectedWire = wire, Times.Never);
            outputTerminalMoq.VerifySet(m => m.ConnectedWire = wire, Times.Never);
        }

        [TestMethod]
        public void TestConstructor_InputCanBeCastToOutput_ConnectsTerminals()
        {
            var inputTerminalMoq = new Mock<TerminalModel>("", typeof(Parent), Direction.North, TerminalKind.Input, 0);
            var outputTerminalMoq = new Mock<TerminalModel>("", typeof(Child), Direction.North, TerminalKind.Output, 0);
            var wire = new WireModel(outputTerminalMoq.Object, inputTerminalMoq.Object);
            inputTerminalMoq.VerifySet(m => m.ConnectedWire = wire);
            outputTerminalMoq.VerifySet(m => m.ConnectedWire = wire);
        }

        [TestMethod]
        public void TestConstructor_InputCantBeCastToOutput_DoesNotConnectTerminals()
        {
            var inputTerminalMoq = new Mock<TerminalModel>("", typeof(Child), Direction.North, TerminalKind.Input, 0);
            var outputTerminalMoq = new Mock<TerminalModel>("", typeof(Parent), Direction.North, TerminalKind.Output, 0);
            var wire = new WireModel(outputTerminalMoq.Object, inputTerminalMoq.Object);
            inputTerminalMoq.VerifySet(m => m.ConnectedWire = wire, Times.Never);
            outputTerminalMoq.VerifySet(m => m.ConnectedWire = wire, Times.Never);
        }

        private class Parent { }
        private class Child : Parent { }
    }
}
