using DiiagramrAPI.Diagram;
using DiiagramrAPI.Diagram.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace DiiagramrUnitTests.PluginNodeApiTests
{
    [TestClass]
    public class NodeSetupTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructor_NullArguments_ThrowsException()
        {
            new NodeSetup(null);
        }

        [TestMethod]
        public void TestNodeSize_SetsNodeWidthAndHeight()
        {
            var nodeViewModelMoq = new Mock<Node>();
            nodeViewModelMoq.SetupGet(n => n.Model).Returns(new Mock<NodeModel>("").Object);
            var nodeSetup = new NodeSetup(nodeViewModelMoq.Object);
            nodeSetup.NodeSize(30, 40);

            nodeViewModelMoq.VerifySet(n => n.Width = 30);
            nodeViewModelMoq.VerifySet(n => n.Height = 40);
        }

        [TestMethod]
        public void TestNodeName_SetsNodeName()
        {
            var nodeViewModelMoq = new Mock<Node>();
            var nodeSetup = new NodeSetup(nodeViewModelMoq.Object);
            nodeSetup.NodeName("bob");

            nodeViewModelMoq.VerifySet(n => n.Name = "bob");
        }

        [TestMethod]
        public void TestEnableResize_SetsResizeEnabledToTrue()
        {
            var nodeViewModelMoq = new Mock<Node>();
            var nodeSetup = new NodeSetup(nodeViewModelMoq.Object);
            nodeSetup.EnableResize();

            nodeViewModelMoq.VerifySet(n => n.ResizeEnabled = true);
        }

        [TestMethod]
        public void TestInputTerminal_AddsInputTerminalViewModelToNode()
        {
            var nodeViewModelMoq = new Mock<Node>();
            nodeViewModelMoq.SetupGet(n => n.Model).Returns(new Mock<NodeModel>(string.Empty).Object);
            nodeViewModelMoq.SetupGet(n => n.Terminals).Returns(new List<Terminal>());
            var nodeSetup = new NodeSetup(nodeViewModelMoq.Object);
            nodeSetup.InputTerminal<int>(string.Empty, Direction.East);

            nodeViewModelMoq.Verify(n => n.AddTerminalViewModel(It.IsAny<InputTerminal>()));
        }

        [TestMethod]
        public void TestInputTerminal_DirectionSouth_AddedTerminalHasSouthDirection()
        {
            var nodeViewModelMoq = new Mock<Node>();
            nodeViewModelMoq.SetupGet(n => n.Model).Returns(new Mock<NodeModel>(string.Empty).Object);
            nodeViewModelMoq.SetupGet(n => n.Terminals).Returns(new List<Terminal>());
            var nodeSetup = new NodeSetup(nodeViewModelMoq.Object);
            nodeSetup.InputTerminal<int>(string.Empty, Direction.South);

            nodeViewModelMoq.Verify(n => n.AddTerminalViewModel(It.Is<Terminal>(tvm => tvm.Model.Direction == Direction.South)));
        }

        [TestMethod]
        public void TestInputTerminal_TerminalName_AddedTerminalHasNameSet()
        {
            var nodeViewModelMoq = new Mock<Node>();
            nodeViewModelMoq.SetupGet(n => n.Model).Returns(new Mock<NodeModel>(string.Empty).Object);
            nodeViewModelMoq.SetupGet(n => n.Terminals).Returns(new List<Terminal>());
            var nodeSetup = new NodeSetup(nodeViewModelMoq.Object);
            nodeSetup.InputTerminal<int>("lala", Direction.South);

            nodeViewModelMoq.Verify(n => n.AddTerminalViewModel(It.Is<Terminal>(tvm => tvm.Model.Name == "lala" && tvm.Name == "lala")));
        }

        [TestMethod]
        public void TestInputTerminal_DoesNotReturnNull()
        {
            var nodeViewModelMoq = new Mock<Node>();
            nodeViewModelMoq.SetupGet(n => n.Model).Returns(new Mock<NodeModel>(string.Empty).Object);
            nodeViewModelMoq.SetupGet(n => n.Terminals).Returns(new List<Terminal>());
            var nodeSetup = new NodeSetup(nodeViewModelMoq.Object);
            Assert.IsNotNull(nodeSetup.InputTerminal<int>("lala", Direction.South));
        }

        [TestMethod]
        public void TestOutputTerminal_AddsInputTerminalViewModelToNode()
        {
            var nodeViewModelMoq = new Mock<Node>();
            nodeViewModelMoq.SetupGet(n => n.Model).Returns(new Mock<NodeModel>(string.Empty).Object);
            nodeViewModelMoq.SetupGet(n => n.Terminals).Returns(new List<Terminal>());
            var nodeSetup = new NodeSetup(nodeViewModelMoq.Object);
            nodeSetup.OutputTerminal<int>(string.Empty, Direction.East);

            nodeViewModelMoq.Verify(n => n.AddTerminalViewModel(It.IsAny<OutputTerminal>()));
        }

        [TestMethod]
        public void TestOutputTerminal_DirectionSouth_AddedTerminalHasSouthDirection()
        {
            var nodeViewModelMoq = new Mock<Node>();
            nodeViewModelMoq.SetupGet(n => n.Model).Returns(new Mock<NodeModel>(string.Empty).Object);
            nodeViewModelMoq.SetupGet(n => n.Terminals).Returns(new List<Terminal>());
            var nodeSetup = new NodeSetup(nodeViewModelMoq.Object);
            nodeSetup.OutputTerminal<int>(string.Empty, Direction.South);

            nodeViewModelMoq.Verify(n => n.AddTerminalViewModel(It.Is<Terminal>(tvm => tvm.Model.Direction == Direction.South)));
        }

        [TestMethod]
        public void TestOutputTerminal_TerminalName_AddedTerminalHasNameSet()
        {
            var nodeViewModelMoq = new Mock<Node>();
            nodeViewModelMoq.SetupGet(n => n.Model).Returns(new Mock<NodeModel>(string.Empty).Object);
            nodeViewModelMoq.SetupGet(n => n.Terminals).Returns(new List<Terminal>());
            var nodeSetup = new NodeSetup(nodeViewModelMoq.Object);
            nodeSetup.OutputTerminal<int>("lala", Direction.South);

            nodeViewModelMoq.Verify(n => n.AddTerminalViewModel(It.Is<Terminal>(tvm => tvm.Model.Name == "lala" && tvm.Name == "lala")));
        }

        [TestMethod]
        public void TestOutputTerminal_DoesNotReturnNull()
        {
            var nodeViewModelMoq = new Mock<Node>();
            nodeViewModelMoq.SetupGet(n => n.Model).Returns(new Mock<NodeModel>(string.Empty).Object);
            nodeViewModelMoq.SetupGet(n => n.Terminals).Returns(new List<Terminal>());
            var nodeSetup = new NodeSetup(nodeViewModelMoq.Object);
            Assert.IsNotNull(nodeSetup.OutputTerminal<int>("lala", Direction.South));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCreateClientTerminal_ViewModelNotOnNode_ThrowsException()
        {
            var nodeViewModelMoq = new Mock<Node>();
            var terminalViewModelMoq = new Mock<Terminal>(new Mock<TerminalModel>().Object);
            terminalViewModelMoq.SetupGet(model => model.Model).Returns(new Mock<TerminalModel>().Object);
            nodeViewModelMoq.SetupGet(n => n.Terminals).Returns(new List<Terminal>());
            var nodeSetup = new NodeSetup(nodeViewModelMoq.Object);

            nodeSetup.CreateClientTerminal<int>(terminalViewModelMoq.Object);
        }

        [TestMethod]
        public void TestCreateClientTerminal_ViewModelOnNode_ReturnsTerminal()
        {
            var nodeViewModelMoq = new Mock<Node>();
            var terminalViewModelMoq = new Mock<Terminal>(new Mock<TerminalModel>().Object);
            terminalViewModelMoq.SetupGet(model => model.Model).Returns(new Mock<TerminalModel>().Object);
            nodeViewModelMoq.SetupGet(n => n.Terminals).Returns(new List<Terminal> { terminalViewModelMoq.Object });
            var nodeSetup = new NodeSetup(nodeViewModelMoq.Object);

            Assert.IsNotNull(nodeSetup.CreateClientTerminal<int>(terminalViewModelMoq.Object));
        }
    }
}
