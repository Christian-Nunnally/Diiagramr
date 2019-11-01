using Castle.Core.Internal;
using DiiagramrAPI.Diagram.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

namespace DiiagramrUnitTests.ModelTests
{
    [TestClass]
    public class DiagramModelTest
    {
        private DiagramModel _diagram;
        private Mock<NodeModel> _nodeMoq;

        [TestInitialize]
        public void SetupTests()
        {
            _diagram = new DiagramModel();
            _nodeMoq = new Mock<NodeModel>("");
            _diagram.AddNode(_nodeMoq.Object);
        }

        [TestMethod]
        public void TestAddNode_NewNode_AddsNodeToNodes()
        {
            var dia = new DiagramModel();
            Assert.IsTrue(dia.Nodes.IsNullOrEmpty());

            dia.AddNode(_nodeMoq.Object);

            Assert.AreEqual(_nodeMoq.Object, dia.Nodes.First());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddNode_NodeAlreadyAdded_ThrowsException()
        {
            _diagram.AddNode(_nodeMoq.Object);
        }

        [TestMethod]
        public void TestRemoveNode_NewNode_RemovedsNodeFromNodes()
        {
            Assert.IsFalse(_diagram.Nodes.IsNullOrEmpty());

            _diagram.RemoveNode(_nodeMoq.Object);

            Assert.IsTrue(_diagram.Nodes.IsNullOrEmpty());
        }

        [TestMethod]
        public void TestRemoveNode_NodeNotOnDiagram_NoOp()
        {
            var dia = new DiagramModel();
            dia.RemoveNode(_nodeMoq.Object);
        }
    }
}
