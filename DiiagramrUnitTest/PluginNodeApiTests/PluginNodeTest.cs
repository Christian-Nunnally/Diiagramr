using System;
using Diiagramr.Model;
using Diiagramr.PluginNodeApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.PluginNodeApiTests
{
    [TestClass]
    public class PluginNodeTest
    {
        [TestMethod]
        public void TestLoadNodeVariables_SetsActualProperty()
        {
            var nodeMoq = new Mock<NodeModel>("");
            nodeMoq.Setup(n => n.GetVariable("PublicProperty")).Returns(5);
            var testPluginNode = new TestPluginNode();
            testPluginNode.NodeModel = nodeMoq.Object;

            testPluginNode.LoadNodeVariables();

            Assert.AreEqual(5, testPluginNode.PublicProperty);
        }

        [TestMethod]
        public void TestSaveNodeVariables_FindsImplementingPublicProperty()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode();
            testPluginNode.NodeModel = nodeMoq.Object;

            testPluginNode.SaveNodeVariables();

            nodeMoq.Verify(n => n.SetVariable("PublicProperty", 0));
        }

        [TestMethod]
        public void TestSaveNodeVariables_DoesntSavePublicPropertyFromPluginNode()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode();
            testPluginNode.NodeModel = nodeMoq.Object;

            testPluginNode.SaveNodeVariables();

            nodeMoq.Verify(n => n.SetVariable("Width", 0), Times.Never);
        }

        [TestMethod]
        public void TestSaveNodeVariables_DoesntSaveImplementingPrivateProperty()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode();
            testPluginNode.NodeModel = nodeMoq.Object;

            testPluginNode.SaveNodeVariables();

            nodeMoq.Verify(n => n.SetVariable("PrivateProperty", 0), Times.Never);
        }

        [TestMethod]
        public void TestSaveNodeVariables_DoesntSaveImplementingPropertyWithPrivateSetter()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode();
            testPluginNode.NodeModel = nodeMoq.Object;

            testPluginNode.SaveNodeVariables();

            nodeMoq.Verify(n => n.SetVariable("PrivateSetter", 0), Times.Never);
        }

        [TestMethod]
        public void TestSaveNodeVariables_DoesntSaveImplementingPropertyWithPrivateGetter()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode();
            testPluginNode.NodeModel = nodeMoq.Object;

            testPluginNode.SaveNodeVariables();

            nodeMoq.Verify(n => n.SetVariable("PrivateGetter", 0), Times.Never);
        }

        [TestMethod]
        public void TestLoadNodeVariables_FindsImplementingPublicProperty()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode();
            testPluginNode.NodeModel = nodeMoq.Object;

            testPluginNode.LoadNodeVariables();

            nodeMoq.Verify(n => n.GetVariable("PublicProperty"));
        }

        [TestMethod]
        public void TestLoadNodeVariables_DoesntSavePublicPropertyFromPluginNode()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode();
            testPluginNode.NodeModel = nodeMoq.Object;

            testPluginNode.LoadNodeVariables();

            nodeMoq.Verify(n => n.GetVariable("Width"), Times.Never);
        }

        [TestMethod]
        public void TestLoadNodeVariables_DoesntSaveImplementingPrivateProperty()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode();
            testPluginNode.NodeModel = nodeMoq.Object;

            testPluginNode.LoadNodeVariables();

            nodeMoq.Verify(n => n.GetVariable("PrivateProperty"), Times.Never);
        }

        [TestMethod]
        public void TestLoadNodeVariables_DoesntSaveImplementingPropertyWithPrivateSetter()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode();
            testPluginNode.NodeModel = nodeMoq.Object;

            testPluginNode.LoadNodeVariables();

            nodeMoq.Verify(n => n.GetVariable("PrivateSetter"), Times.Never);
        }

        [TestMethod]
        public void TestLoadNodeVariables_DoesntLoadImplementingPropertyWithPrivateGetter()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode();
            testPluginNode.NodeModel = nodeMoq.Object;

            testPluginNode.LoadNodeVariables();

            nodeMoq.Verify(n => n.GetVariable("PrivateGetter"), Times.Never);
        }
    }

    class TestPluginNode : PluginNode
    {
        public override string Name => "Test Node";

        public int PublicProperty { get; set; }

        private int PrivateProperty { get; set; }

        public int PrivateSetter { get; private set; }

        public int PrivateGetter { private get; set; }

        public override void SetupNode(NodeSetup setup)
        {
            throw new NotImplementedException();
        }
    }
}
