using System;
using System.Linq;
using Castle.Core.Internal;
using Diiagramr.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ModelTests
{
    [TestClass]
    public class DiagramModelTest
    {
        [TestMethod]
        public void TestAddNode_NewNode_AddsNodeToNodes()
        {
            var diagram = new DiagramModel();
            var nodeMoq = new Mock<NodeModel>("");
            Assert.IsTrue(diagram.Nodes.IsNullOrEmpty());

            diagram.AddNode(nodeMoq.Object);

            Assert.AreEqual(nodeMoq.Object, diagram.Nodes.First());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddNode_NodeAlreadyAdded_ThrowsException()
        {
            var diagram = new DiagramModel();
            var nodeMoq = new Mock<NodeModel>("");

            diagram.AddNode(nodeMoq.Object);
            diagram.AddNode(nodeMoq.Object);
        }

        [TestMethod]
        public void TestRemoveNode_NewNode_RemovedsNodeFromNodes()
        {
            var diagram = new DiagramModel();
            var nodeMoq = new Mock<NodeModel>("");
            diagram.AddNode(nodeMoq.Object);
            Assert.IsFalse(diagram.Nodes.IsNullOrEmpty());

            diagram.RemoveNode(nodeMoq.Object);

            Assert.IsTrue(diagram.Nodes.IsNullOrEmpty());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRemoveNode_NodeNotOnDiagram_ThrowsException()
        {
            var diagram = new DiagramModel();
            var nodeMoq = new Mock<NodeModel>("");

            diagram.RemoveNode(nodeMoq.Object);
        }

        [TestMethod]
        public void TestPreSave_HasNode_CallsPreSaveOnNode()
        {
            var diagram = new DiagramModel();
            var nodeMoq = new Mock<NodeModel>("");
            diagram.AddNode(nodeMoq.Object);

            diagram.PreSave();

            nodeMoq.Verify(d => d.PreSave());
        }

        [TestMethod]
        public void TestSemanticsChanged_NodeAdded_SematicsChangedInvoked()
        {
            var diagram = new DiagramModel();
            var semanticsChanged = false;
            diagram.SemanticsChanged += () => semanticsChanged = true;
            var nodeMoq = new Mock<NodeModel>("");
            diagram.AddNode(nodeMoq.Object);

            Assert.IsTrue(semanticsChanged);
        }

        [TestMethod]
        public void TestSemanticsChanged_NodeRemoved_SematicsChangedInvoked()
        {
            var diagram = new DiagramModel();
            var semanticsChanged = false;
            diagram.SemanticsChanged += () => semanticsChanged = true;
            var nodeMoq = new Mock<NodeModel>("");
            diagram.AddNode(nodeMoq.Object);
            semanticsChanged = false;
            diagram.RemoveNode(nodeMoq.Object);

            Assert.IsTrue(semanticsChanged);
        }

        [TestMethod]
        public void TestSemanticsChanged_NodeSemanticsChanged_SematicsChangedInvoked()
        {
            var diagram = new DiagramModel();
            var semanticsChanged = false;
            diagram.SemanticsChanged += () => semanticsChanged = true;
            var nodeMoq = new Mock<NodeModel>("");
            diagram.AddNode(nodeMoq.Object);

            nodeMoq.Raise(n => n.SemanticsChanged += null);

            Assert.IsTrue(semanticsChanged);
        }
    }
}
