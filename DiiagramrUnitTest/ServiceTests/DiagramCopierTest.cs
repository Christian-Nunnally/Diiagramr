using DiiagramrAPI.Model;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace DiiagramrUnitTests.ServiceTests
{
    [TestClass]
    public class DiagramCopierTest
    {
        [TestMethod]
        public void TestCopy_CopyEmptyDiagram_CopiedDiagramIsDifferent()
        {
            var projectManagerMoq = new Mock<IProjectManager>();
            projectManagerMoq.Verify(x => x.GetSerializeableTypes(), Times.Once);
            var copier = new DiagramCopier(projectManagerMoq.Object);
            var diagram = new DiagramModel
            {
                Name = "d"
            };
            var copiedDiagram = copier.Copy(diagram);
            Assert.AreNotEqual(diagram, copiedDiagram);
        }

        [TestMethod]
        public void TestCopy_CopyEmptyDiagram_NameCopied()
        {
            var projectManagerMoq = new Mock<IProjectManager>();
            projectManagerMoq.Verify(x => x.GetSerializeableTypes(), Times.Once);
            var copier = new DiagramCopier(projectManagerMoq.Object);
            var diagram = new DiagramModel
            {
                Name = "d"
            };
            var copiedDiagram = copier.Copy(diagram);
            Assert.AreEqual(diagram.Name, copiedDiagram.Name);
        }

        [TestMethod]
        public void TestCopy_CopyDiagramWithOneNode_NodeCopiedWithFullName()
        {
            var projectManagerMoq = new Mock<IProjectManager>();
            projectManagerMoq.Verify(x => x.GetSerializeableTypes(), Times.Once);
            var copier = new DiagramCopier(projectManagerMoq.Object);
            var diagram = new DiagramModel();
            var node = new NodeModel("Node")
            {
                NodeTypeFullName = "test"
            };
            diagram.AddNode(node);
            var copiedDiagram = copier.Copy(diagram);
            Assert.AreEqual(diagram.Nodes.First().NodeTypeFullName, copiedDiagram.Nodes.First().NodeTypeFullName);
        }
    }
}
