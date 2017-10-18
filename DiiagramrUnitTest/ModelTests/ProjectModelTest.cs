using System;
using DiiagramrAPI.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ModelTests
{
    [TestClass]
    public class ProjectModelTest
    {
        [TestMethod]
        public void TestConstructor_ConstructsDiagramCollection()
        {
            var project = new ProjectModel();
            Assert.IsNotNull(project.Diagrams);
        }

        [TestMethod]
        public void TestConstructor_SetsNameToNewProject()
        {
            var project = new ProjectModel();
            Assert.AreEqual("NewProject", project.Name);
        }
    }
}
