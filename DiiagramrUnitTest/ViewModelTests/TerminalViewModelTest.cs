using DiiagramrAPI.Diagram;
using DiiagramrAPI.Diagram.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class TerminalViewModelTest
    {
        private readonly Mock<TerminalModel> _terminalModelMoq = new Mock<TerminalModel>(string.Empty, typeof(int), Direction.North, TerminalKind.Input, 0);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructor_NullTerminal_ThrowsException()
        {
            new Terminal(null);
        }

        [TestMethod]
        public void TestConstructor_ValidTerminal_SetsTerminalModel()
        {
            var terminalViewModel = new Terminal(_terminalModelMoq.Object);
            Assert.AreEqual(_terminalModelMoq.Object, terminalViewModel.Model);
        }

        [TestMethod]
        public void TestConstructor_ValidTerminal_InitializesDataToTerminalData()
        {
            _terminalModelMoq.SetupGet(n => n.Data).Returns(3);
            var terminalViewModel = new Terminal(_terminalModelMoq.Object);
            Assert.AreEqual(_terminalModelMoq.Object.Data, terminalViewModel.Data);
        }

        [TestMethod]
        public void TestConstructor_ValidTerminal_InitializesNameToTerminalName()
        {
            _terminalModelMoq.SetupGet(n => n.Name).Returns("a");
            var terminalViewModel = new Terminal(_terminalModelMoq.Object);
            Assert.AreEqual(_terminalModelMoq.Object.Name, terminalViewModel.Name);
        }

        [TestMethod]
        public void TestConstructor_NorthTerminal_TerminalRotationSetTo0()
        {
            _terminalModelMoq.SetupGet(n => n.Direction).Returns(Direction.North);
            var terminalViewModel = new Terminal(_terminalModelMoq.Object);
            Assert.AreEqual(0, terminalViewModel.TerminalRotation);
        }

        [TestMethod]
        public void TestConstructor_EastTerminal_TerminalRotationSetTo90()
        {
            _terminalModelMoq.SetupGet(n => n.Direction).Returns(Direction.East);
            var terminalViewModel = new Terminal(_terminalModelMoq.Object);
            Assert.AreEqual(90, terminalViewModel.TerminalRotation);
        }

        [TestMethod]
        public void TestConstructor_SouthTerminal_TerminalRotationSetTo180()
        {
            _terminalModelMoq.SetupGet(n => n.Direction).Returns(Direction.South);
            var terminalViewModel = new Terminal(_terminalModelMoq.Object);
            Assert.AreEqual(180, terminalViewModel.TerminalRotation);
        }

        [TestMethod]
        public void TestConstructor_WestTerminal_TerminalRotationSetTo270()
        {
            _terminalModelMoq.SetupGet(n => n.Direction).Returns(Direction.West);
            var terminalViewModel = new Terminal(_terminalModelMoq.Object);
            Assert.AreEqual(270, terminalViewModel.TerminalRotation);
        }

        [TestMethod]
        public void TestChangingRealTerminalDirection_DirectionSetToWest_TerminalRotationSetTo270()
        {
            var realTerminal = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var terminalViewModel = new Terminal(realTerminal);
            realTerminal.Direction = Direction.West;
            Assert.AreEqual(270, terminalViewModel.TerminalRotation);
        }

        [TestMethod]
        public void TestSetData_TerminalDataSet()
        {
            var terminalViewModel = new Terminal(_terminalModelMoq.Object)
            {
                Data = 5
            };
            _terminalModelMoq.VerifySet(t => t.Data = 5);
        }

        [TestMethod]
        public void TestSetXRelativeToNode_TerminalXOffsetSet()
        {
            var terminalViewModel = new Terminal(_terminalModelMoq.Object)
            {
                XRelativeToNode = 5
            };
            _terminalModelMoq.VerifySet(t => t.OffsetX = 5);
        }

        [TestMethod]
        public void TestSetYRelativeToNode_TerminalYOffsetSet()
        {
            var terminalViewModel = new Terminal(_terminalModelMoq.Object)
            {
                YRelativeToNode = 6
            };
            _terminalModelMoq.VerifySet(t => t.OffsetY = 6);
        }

        [TestMethod]
        public void TestSetTerminalDirection_TerminalDirectionSet()
        {
            var terminalViewModel = new Terminal(_terminalModelMoq.Object);
            terminalViewModel.SetTerminalDirection(Direction.South);
            _terminalModelMoq.VerifySet(t => t.Direction = Direction.South);
        }

        [TestMethod]
        public void TestShowHighlightIfCompatibleType_TypesCompatible_HighlightVisibleSetToTrue()
        {
            var terminalViewModel = new Terminal(_terminalModelMoq.Object);
            Assert.IsFalse(terminalViewModel.HighlightVisible);
            terminalViewModel.ShowHighlightIfCompatibleType(typeof(int));
            Assert.IsTrue(terminalViewModel.HighlightVisible);
        }

        [TestMethod]
        public void TestShowHighlightIfCompatibleType_TypesNotCompatible_HighlightVisibleFalse()
        {
            var terminalViewModel = new Terminal(_terminalModelMoq.Object)
            {
                HighlightVisible = true
            };
            Assert.IsTrue(terminalViewModel.HighlightVisible);
            terminalViewModel.ShowHighlightIfCompatibleType(typeof(string));
            Assert.IsFalse(terminalViewModel.HighlightVisible);
        }

        [TestMethod]
        public void TestDisconnectTerminal_CallsDisconnectWiresOnModel()
        {
            var terminalViewModel = new Terminal(_terminalModelMoq.Object);
            terminalViewModel.DisconnectTerminal();
            _terminalModelMoq.Verify(t => t.DisconnectWires());
        }
    }
}
