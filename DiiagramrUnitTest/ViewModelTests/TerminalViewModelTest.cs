using System;
using Diiagramr.Model;
using Diiagramr.PluginNodeApi;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
            new TerminalViewModel(null);
        }

        [TestMethod]
        public void TestConstructor_ValidTerminal_SetsTerminalModel()
        {
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            Assert.AreEqual(_terminalModelMoq.Object, terminalViewModel.TerminalModel);
        }

        [TestMethod]
        public void TestConstructor_ValidTerminal_InitializesDataToTerminalData()
        {
            _terminalModelMoq.SetupGet(n => n.Data).Returns(3);
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            Assert.AreEqual(_terminalModelMoq.Object.Data, terminalViewModel.Data);
        }

        [TestMethod]
        public void TestConstructor_ValidTerminal_InitializesNameToTerminalName()
        {
            _terminalModelMoq.SetupGet(n => n.Name).Returns("a");
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            Assert.AreEqual(_terminalModelMoq.Object.Name, terminalViewModel.Name);
        }

        [TestMethod]
        public void TestConstructor_NorthTerminal_TerminalRotationSetTo0()
        {
            _terminalModelMoq.SetupGet(n => n.Direction).Returns(Direction.North);
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            Assert.AreEqual(0, terminalViewModel.TerminalRotation);
        }

        [TestMethod]
        public void TestConstructor_EastTerminal_TerminalRotationSetTo90()
        {
            _terminalModelMoq.SetupGet(n => n.Direction).Returns(Direction.East);
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            Assert.AreEqual(90, terminalViewModel.TerminalRotation);
        }

        [TestMethod]
        public void TestConstructor_SouthTerminal_TerminalRotationSetTo180()
        {
            _terminalModelMoq.SetupGet(n => n.Direction).Returns(Direction.South);
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            Assert.AreEqual(180, terminalViewModel.TerminalRotation);
        }

        [TestMethod]
        public void TestConstructor_WestTerminal_TerminalRotationSetTo270()
        {
            _terminalModelMoq.SetupGet(n => n.Direction).Returns(Direction.West);
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            Assert.AreEqual(270, terminalViewModel.TerminalRotation);
        }

        [TestMethod]
        public void TestSetData_TerminalDataSet()
        {
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            terminalViewModel.Data = 5;
            _terminalModelMoq.VerifySet(t => t.Data = 5);
        }

        [TestMethod]
        public void TestSetXRelativeToNode_TerminalXOffsetSet()
        {
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            terminalViewModel.XRelativeToNode = 5;
            _terminalModelMoq.VerifySet(t => t.OffsetX = 5);
        }

        [TestMethod]
        public void TestSetYRelativeToNode_TerminalYOffsetSet()
        {
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            terminalViewModel.YRelativeToNode = 6;
            _terminalModelMoq.VerifySet(t => t.OffsetY = 6);
        }

        [TestMethod]
        public void TestDropObject_DroppingOppositeTerminalModel_ConnectedWireSetBetweenTerminals()
        {
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            var droppingTerminalMoq = new Mock<TerminalModel>(string.Empty, typeof(int), Direction.North, TerminalKind.Output, 0);

            terminalViewModel.DropObject(droppingTerminalMoq.Object);

            _terminalModelMoq.VerifySet(t => t.ConnectedWire = It.IsAny<WireModel>());
            droppingTerminalMoq.VerifySet(t => t.ConnectedWire = It.IsAny<WireModel>());
        }

        [TestMethod]
        public void TestWireToTerminal_TerminalSameKind_ReturnsFalse()
        {
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            var droppingTerminalMoq = new Mock<TerminalModel>(string.Empty, typeof(int), Direction.North, TerminalKind.Input, 0);
            Assert.IsFalse(terminalViewModel.WireToTerminal(droppingTerminalMoq.Object));
        }

        [TestMethod]
        public void TestWireToTerminal_TerminalNull_ReturnsFalse()
        {
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            Assert.IsFalse(terminalViewModel.WireToTerminal(null));
        }

        [TestMethod]
        public void TestWireToTerminal_TerminalValid_WiresTerminals()
        {
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            var droppingTerminalMoq = new Mock<TerminalModel>(string.Empty, typeof(int), Direction.North, TerminalKind.Output, 0);

            terminalViewModel.WireToTerminal(droppingTerminalMoq.Object);

            _terminalModelMoq.VerifySet(t => t.ConnectedWire = It.IsAny<WireModel>());
            droppingTerminalMoq.VerifySet(t => t.ConnectedWire = It.IsAny<WireModel>());
        }

        [TestMethod]
        public void TestSetTerminalDirection_TerminalDirectionSet()
        {
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            terminalViewModel.SetTerminalDirection(Direction.South);
            _terminalModelMoq.VerifySet(t => t.Direction = Direction.South);
        }

        [TestMethod]
        public void TestMouseEntered_TitleVisibleSetToTrue()
        {
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            Assert.IsFalse(terminalViewModel.TitleVisible);
            terminalViewModel.MouseEntered(null, null);
            Assert.IsTrue(terminalViewModel.TitleVisible);
        }

        [TestMethod]
        public void TestMouseLeft_TitleVisibleSetToFalse()
        {
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            terminalViewModel.TitleVisible = true;
            Assert.IsTrue(terminalViewModel.TitleVisible);
            terminalViewModel.MouseLeft(null, null);
            Assert.IsFalse(terminalViewModel.TitleVisible);
        }

        [TestMethod]
        public void TestShowLabelIfCompatibleType_TypesCompatible_TitleVisibleSetToTrue()
        {
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            Assert.IsFalse(terminalViewModel.TitleVisible);
            terminalViewModel.ShowLabelIfCompatibleType(typeof(int));
            Assert.IsTrue(terminalViewModel.TitleVisible);
        }

        [TestMethod]
        public void TestShowLabelIfCompatibleType_TypesNotCompatible_TitleVisibleSetToFalse()
        {
            var terminalViewModel = new TerminalViewModel(_terminalModelMoq.Object);
            terminalViewModel.TitleVisible = true;
            Assert.IsTrue(terminalViewModel.TitleVisible);
            terminalViewModel.ShowLabelIfCompatibleType(typeof(string));
            Assert.IsFalse(terminalViewModel.TitleVisible);
        }
    }
}
