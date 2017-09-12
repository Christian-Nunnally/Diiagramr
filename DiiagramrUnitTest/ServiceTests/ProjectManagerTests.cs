using Diiagramr.Model;
using Diiagramr.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Windows.Forms;

namespace DiiagramrUnitTests.ServiceTests
{
    [TestClass]
    public class ProjectManagerTests
    {
        private ProjectManager _projectManager;
        private Mock<IProjectFileService> _projectFileServiceMoq;
        private bool _currentProjectChanged;

        [TestInitialize]
        public void TestInitialize()
        {
            _projectFileServiceMoq = new Mock<IProjectFileService>();
            _currentProjectChanged = false;
            _projectManager = new ProjectManager(_projectFileServiceMoq.Object);
            _projectManager.CurrentProjectChanged += () => _currentProjectChanged = true;
        }

        [TestMethod]
        public void CreateProjectTest_InitializeProject()
        {
            _projectManager.CreateProject();
            Assert.IsNotNull(_projectManager.CurrentProject);
        }

        [TestMethod]
        public void CreateProjectTest_ProjectDirty()
        {
            _projectManager.CreateProject();
            Assert.IsTrue(_projectManager.IsProjectDirty);
        }

        [TestMethod]
        public void CreateProjectTest_ProjectChanged()
        {
            _projectManager.CreateProject();
            Assert.IsTrue(_currentProjectChanged);
        }

        [TestMethod]
        public void CreateProjectTest_ConfirmSaveCanceled()
        {
            _projectFileServiceMoq.Setup(m => m.ConfirmProjectClose()).Returns(DialogResult.Cancel);
            _projectManager.CreateProject();
            _projectManager.CurrentProject.Name = "testProj";
            _projectManager.CreateProject();
            Assert.AreEqual("testProj", _projectManager.CurrentProject.Name);
        }

        [TestMethod]
        public void CreateProjectTest_ConfirmSaveNo()
        {
            _projectFileServiceMoq.Setup(m => m.ConfirmProjectClose()).Returns(DialogResult.No);
            _projectManager.CreateProject();
            _projectManager.CurrentProject.Name = "testProj";
            _projectManager.CreateProject();
            Assert.AreEqual("NewProject", _projectManager.CurrentProject.Name);
        }

        [TestMethod]
        public void CreateProjectTest_ConfirmSaveYes()
        {
            _projectFileServiceMoq.Setup(m => m.ConfirmProjectClose()).Returns(DialogResult.Yes);
            _projectFileServiceMoq.Setup(m => m.SaveProject(It.IsAny<Project>(), false)).Returns(true);
            _projectManager.CreateProject();
            _projectManager.CurrentProject.Name = "testProj";
            _projectManager.CreateProject();
            Assert.AreEqual("NewProject", _projectManager.CurrentProject.Name);
        }

        [TestMethod]
        public void SaveProjectTest_ProjectSavedNotDirty()
        {
            _projectManager.CreateProject();
            _projectFileServiceMoq.Setup(m => m.SaveProject(_projectManager.CurrentProject, false)).Returns(true);
            _projectManager.SaveProject();
            Assert.IsFalse(_projectManager.IsProjectDirty);
        }

        [TestMethod]
        public void SaveProjectTest_ProjectNotSavedDirty()
        {
            _projectManager.CreateProject();
            _projectFileServiceMoq.Setup(m => m.SaveProject(_projectManager.CurrentProject, false)).Returns(false);
            _projectManager.SaveProject();
            Assert.IsTrue(_projectManager.IsProjectDirty);
        }

        [TestMethod]
        public void LoadProjectTest_CurrentProjectSet()
        {
            _projectFileServiceMoq.Setup(m => m.LoadProject()).Returns(new Project());
            _projectManager.LoadProject();
            Assert.IsNotNull(_projectManager.CurrentProject);
        }

        [TestMethod]
        public void LoadProjectTest_ProjectChanged()
        {
            _projectFileServiceMoq.Setup(m => m.LoadProject()).Returns(new Project());
            _projectManager.LoadProject();
            Assert.IsTrue(_currentProjectChanged);
        }

        [TestMethod]
        public void LoadProjectTest_ConfirmSaveCanceled()
        {
            _projectFileServiceMoq.Setup(m => m.ConfirmProjectClose()).Returns(DialogResult.Cancel);

            _projectManager.CreateProject();
            _projectManager.CurrentProject.Name = "testProj";
            _projectManager.LoadProject();
            Assert.AreEqual("testProj", _projectManager.CurrentProject.Name);
        }

        [TestMethod]
        public void LoadProjectTest_ConfirmSaveNo()
        {
            _projectFileServiceMoq.Setup(m => m.ConfirmProjectClose()).Returns(DialogResult.No);
            _projectFileServiceMoq.Setup(m => m.LoadProject()).Returns(new Project());

            _projectManager.CreateProject();
            _projectManager.CurrentProject.Name = "testProj";
            _projectManager.LoadProject();
            Assert.AreEqual("NewProject", _projectManager.CurrentProject.Name);
        }

        [TestMethod]
        public void LoadProjectTest_ConfirmSaveYes()
        {
            _projectFileServiceMoq.Setup(m => m.ConfirmProjectClose()).Returns(DialogResult.Yes);
            _projectFileServiceMoq.Setup(m => m.SaveProject(It.IsAny<Project>(), false)).Returns(true);
            _projectFileServiceMoq.Setup(m => m.LoadProject()).Returns(new Project());

            _projectManager.CreateProject();
            _projectManager.CurrentProject.Name = "testProj";
            _projectManager.LoadProject();
            Assert.AreEqual("NewProject", _projectManager.CurrentProject.Name);
        }

        [TestMethod]
        public void CloseProjectTest_ProjectNotDirty()
        {
            Assert.IsTrue(_projectManager.CloseProject());
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException), "Project does not exist")]
        public void CreateDiagramTest_NoProjectException()
        {
            _projectManager.CreateDiagram();
        }

        [TestMethod]
        public void CreateDiagramTest_DiagramAdded()
        {
            _projectManager.CreateProject();
            _projectManager.CreateDiagram();
            Assert.IsNotNull(_projectManager.CurrentDiagrams[0]);
        }

        [TestMethod]
        public void CreateDiagramTest_ProjectDirty()
        {
            _projectManager.CreateProject();
            _projectManager.IsProjectDirty = false;
            _projectManager.CreateDiagram();
            Assert.IsTrue(_projectManager.IsProjectDirty);
        }

        [TestMethod]
        public void CreateDiagramTest_UniqueNames()
        {
            _projectManager.CreateProject();
            _projectManager.CreateDiagram();
            _projectManager.CreateDiagram();
            Assert.AreNotEqual(_projectManager.CurrentDiagrams[0].Name, _projectManager.CurrentDiagrams[1].Name);
        }

        [TestMethod]
        public void CreateDiagramTest_ProjectChanged()
        {
            _projectManager.CreateProject();
            _currentProjectChanged = false;
            _projectManager.CreateDiagram();
            Assert.IsTrue(_currentProjectChanged);
        }

        [TestMethod]
        public void DeleteDiagramTest_DiagramDeleted()
        {
            _projectManager.CreateProject();
            _projectManager.CreateDiagram();
            _projectManager.DeleteDiagram(_projectManager.CurrentDiagrams[0]);
            Assert.AreEqual(_projectManager.CurrentDiagrams.Count, 0);
        }

        [TestMethod]
        public void DeleteDiagramTest_ProjectChanged()
        {
            _projectManager.CreateProject();
            _projectManager.CreateDiagram();
            _currentProjectChanged = false;
            _projectManager.DeleteDiagram(_projectManager.CurrentDiagrams[0]);
            Assert.IsTrue(_currentProjectChanged);
        }

        [TestMethod]
        public void DeleteDiagramTest_ProjectDirty()
        {
            _projectManager.CreateProject();
            _projectManager.CreateDiagram();
            _projectManager.IsProjectDirty = false;
            _projectManager.DeleteDiagram(_projectManager.CurrentDiagrams[0]);
            Assert.IsTrue(_projectManager.IsProjectDirty);
        }
    }
}
