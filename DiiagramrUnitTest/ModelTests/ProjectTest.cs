using System;
using Diiagramr.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ModelTests
{
    [TestClass]
    public class ProjectTest
    {
        [TestMethod]
        public void TestConstructor_ConstructsDiagramCollection()
        {
            var project = new Project();
            Assert.IsNotNull(project.Diagrams);
        }

        [TestMethod]
        public void TestConstructor_SetsNameToNewProject()
        {
            var project = new Project();
            Assert.AreEqual("NewProject", project.Name);
        }

        [TestMethod]
        public void TestPreSave_HasDiagram_CallsPreSaveOnDiagram()
        {
            var project = new Project();
            var diagramMoq = new Mock<DiagramModel>();
            project.Diagrams.Add(diagramMoq.Object);

            project.PreSave();

            diagramMoq.Verify(d => d.PreSave());
        }
    }
}
