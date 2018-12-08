using DiiagramrAPI.Model;
using DiiagramrAPI.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DiiagramrUnitTests.ServiceTests
{
    [TestClass]
    public class DiagramCopierTest
    {
        [TestMethod]
        public void TestCopy_CopyEmptyDiagram_CopiedDiagramIsDifferent()
        {
            var copier = new DiagramCopier();
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
            var copier = new DiagramCopier();
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
            var copier = new DiagramCopier();
            var diagram = new DiagramModel();
            var node = new NodeModel("Node")
            {
                Name = "test"
            };
            diagram.AddNode(node);
            var copiedDiagram = copier.Copy(diagram);
            Assert.AreEqual(diagram.Nodes.First().Name, copiedDiagram.Nodes.First().Name);
        }
    }
}
