using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using DiiagramrAPI.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRemoveNode_NodeNotOnDiagram_ThrowsException()
        {
            var dia = new DiagramModel();
            dia.RemoveNode(_nodeMoq.Object);
        }

        [TestMethod]
        public void TestSemanticsChanged_NodeAdded_SematicsChangedInvoked()
        {
            var semanticsChanged = false;
            _diagram.SemanticsChanged += () => semanticsChanged = true;
            var nodeMoq2 = new Mock<NodeModel>("");
            _diagram.AddNode(nodeMoq2.Object);

            Assert.IsTrue(semanticsChanged);
        }

        [TestMethod]
        public void TestSemanticsChanged_NodeRemoved_SematicsChangedInvoked()
        {
            var semanticsChanged = false;
            _diagram.SemanticsChanged += () => semanticsChanged = true;
            var nodeMoq2 = new Mock<NodeModel>("");
            _diagram.AddNode(nodeMoq2.Object);
            semanticsChanged = false;
            _diagram.RemoveNode(nodeMoq2.Object);

            Assert.IsTrue(semanticsChanged);
        }

        [TestMethod]
        public void TestSemanticsChanged_NodeSemanticsChanged_SematicsChangedInvoked()
        {
            var semanticsChanged = false;
            _diagram.SemanticsChanged += () => semanticsChanged = true;
            var nodeMoq2 = new Mock<NodeModel>("");
            _diagram.AddNode(nodeMoq2.Object);

            nodeMoq2.Raise(n => n.SemanticsChanged += null);

            Assert.IsTrue(semanticsChanged);
        }

        [TestMethod]
        public void TestPlay_CallsEnableTerminals()
        {
            _diagram.Play();
            _nodeMoq.Verify(m => m.EnableTerminals(), Times.Once);
        }

        [TestMethod]
        public void TestPause_CallsDisableTerminals()
        {
            _diagram.Pause();
            _nodeMoq.Verify(m => m.DisableTerminals(), Times.Once);
        }

        [TestMethod]
        public void TestStop_CallsResetTerminals()
        {
            _diagram.Stop();
            _nodeMoq.Verify(m => m.ResetTerminals(), Times.Once);
        }
    }
}
