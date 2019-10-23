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
        public void TestConstructor_WireModelSet()
        {
            var wireMoq = new Mock<WireModel>(_inputTerminalMoq.Object, _outputTerminalMoq.Object);
            var wireViewModel = new Wire(wireMoq.Object);

            Assert.AreEqual(wireMoq.Object, wireViewModel.Model);
        }
    }
}
