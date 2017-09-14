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
            var nodeMoq = new Mock<DiagramNode>("");
            Assert.IsTrue(diagram.Nodes.IsNullOrEmpty());

            diagram.AddNode(nodeMoq.Object);

            Assert.AreEqual(nodeMoq.Object, diagram.Nodes.First());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddNode_NodeAlreadyAdded_ThrowsException()
        {
            var diagram = new DiagramModel();
            var nodeMoq = new Mock<DiagramNode>("");

            diagram.AddNode(nodeMoq.Object);
            diagram.AddNode(nodeMoq.Object);
        }

        [TestMethod]
        public void TestPreSave_HasNode_CallsPreSaveOnNode()
        {
            var diagram = new DiagramModel();
            var nodeMoq = new Mock<DiagramNode>("");
            diagram.AddNode(nodeMoq.Object);

            diagram.PreSave();

            nodeMoq.Verify(d => d.PreSave());
        }
    }
}
