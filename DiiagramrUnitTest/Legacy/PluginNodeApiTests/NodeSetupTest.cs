using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace DiiagramrUnitTests.Legacy.PluginNodeApiTests
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
    }
}