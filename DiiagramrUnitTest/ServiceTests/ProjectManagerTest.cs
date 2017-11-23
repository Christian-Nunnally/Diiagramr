using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using DiiagramrAPI.Model;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.Diagram;
using DiiagramrAPI.ViewModel.ShellScreen;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ServiceTests
{
    [TestClass]
    public class ProjectManagerTest
    {
        private bool _currentProjectChanged;
        private Mock<DiagramViewModelFactory> _diagramViewModelFactoryMoq;
        private Mock<ILibraryManager> _libraryManagerMoq;
        private Mock<IProvideNodes> _nodeProviderMoq;
        private Mock<NodeSelectorViewModel> _nodeSelectorViewModelMoq;
        private Mock<IProjectFileService> _projectFileServiceMoq;
        private ProjectManager _projectManager;

        [TestInitialize]
        public void TestInitialize()
        {
            var pluginLoader = new Mock<IPluginLoader>();
            _projectFileServiceMoq = new Mock<IProjectFileService>();
            _nodeProviderMoq = new Mock<IProvideNodes>();
            _nodeSelectorViewModelMoq = new Mock<NodeSelectorViewModel>((Func<IProvideNodes>) (() => _nodeProviderMoq.Object));
            _libraryManagerMoq = new Mock<ILibraryManager>();
            _diagramViewModelFactoryMoq = new Mock<DiagramViewModelFactory>((Func<IProvideNodes>) (() => _nodeProviderMoq.Object), (Func<NodeSelectorViewModel>) (() => _nodeSelectorViewModelMoq.Object));
            _currentProjectChanged = false;

            _projectManager = new ProjectManager(
                () => _projectFileServiceMoq.Object,
                () => _nodeProviderMoq.Object,
                () => _libraryManagerMoq.Object,
                () => _diagramViewModelFactoryMoq.Object);

            _projectManager.CurrentProjectChanged += () => _currentProjectChanged = true;
        }

        [TestMethod]
        public void CreateProjectTest_InitializeProject()
        {
            _projectManager.CreateProject();
            Assert.IsNotNull(_projectManager.CurrentProject);
        }

        [TestMethod]
        public void CreateProjectTest_ProjectNotDirty()
        {
            _projectManager.CreateProject();
            Assert.IsFalse(_projectManager.IsProjectDirty);
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
            _projectFileServiceMoq.Setup(m => m.SaveProject(It.IsAny<ProjectModel>(), false)).Returns(true);
            _projectManager.CreateProject();
            _projectManager.CurrentProject.Name = "testProj";
            _projectManager.CreateProject();
            Assert.AreEqual("NewProject", _projectManager.CurrentProject.Name);
        }

        [TestMethod]
        public void CreateProjectTest_DiagramViewModelsCleared()
        {
            var diagramViewModel = new Mock<DiagramViewModel>(new Mock<DiagramModel>().Object, _nodeProviderMoq.Object, _nodeSelectorViewModelMoq.Object).Object;
            _projectManager.DiagramViewModels.Add(diagramViewModel);
            _projectManager.CreateProject();
            Assert.AreEqual(0, _projectManager.DiagramViewModels.Count);
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
            // Change name to dirty project.
            _projectManager.CurrentProject.Name = "bob";
            _projectFileServiceMoq.Setup(m => m.SaveProject(_projectManager.CurrentProject, false)).Returns(false);
            _projectManager.SaveProject();
            Assert.IsTrue(_projectManager.IsProjectDirty);
        }

        [TestMethod]
        public void LoadProjectTest_CurrentProjectSet()
        {
            _projectFileServiceMoq.Setup(m => m.LoadProject()).Returns(new ProjectModel());
            _projectManager.LoadProject();
            Assert.IsNotNull(_projectManager.CurrentProject);
        }

        [TestMethod]
        public void LoadProjectTest_ProjectChanged()
        {
            _projectFileServiceMoq.Setup(m => m.LoadProject()).Returns(new ProjectModel());
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
            _projectFileServiceMoq.Setup(m => m.LoadProject()).Returns(new ProjectModel());

            _projectManager.CreateProject();
            _projectManager.CurrentProject.Name = "testProj";
            _projectManager.LoadProject();
            Assert.AreEqual("NewProject", _projectManager.CurrentProject.Name);
        }

        [TestMethod]
        public void LoadProjectTest_ConfirmSaveYes()
        {
            _projectFileServiceMoq.Setup(m => m.ConfirmProjectClose()).Returns(DialogResult.Yes);
            _projectFileServiceMoq.Setup(m => m.SaveProject(It.IsAny<ProjectModel>(), false)).Returns(true);
            _projectFileServiceMoq.Setup(m => m.LoadProject()).Returns(new ProjectModel());

            _projectManager.CreateProject();
            _projectManager.CurrentProject.Name = "testProj";
            _projectManager.LoadProject();
            Assert.AreEqual("NewProject", _projectManager.CurrentProject.Name);
        }

        [TestMethod]
        public void LoadProjectTest_ViewModelCreatedForExistingDiagram()
        {
            var projectMoq = new Mock<ProjectModel>();
            var diagramMoq = new Mock<DiagramModel>();
            var diagrams = new ObservableCollection<DiagramModel>();
            var nodes = new List<NodeModel>();
            var dependencyMoq = new Mock<DependencyModel>();
            diagramMoq.SetupGet(d => d.Nodes).Returns(nodes);
            diagrams.Add(diagramMoq.Object);
            projectMoq.SetupGet(p => p.Diagrams).Returns(diagrams);
            _projectFileServiceMoq.Setup(m => m.LoadProject()).Returns(projectMoq.Object);

            _projectManager.LoadProject();
            Assert.AreEqual(diagramMoq.Object, _projectManager.DiagramViewModels.First().Diagram);
        }

        [TestMethod]
        public void CloseProjectTest_ProjectNotDirty()
        {
            Assert.IsTrue(_projectManager.CloseProject());
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException), "ProjectModel does not exist")]
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
        public void CreateDiagramTest_DiagramViewModelAdded()
        {
            _projectManager.CreateProject();
            _projectManager.CreateDiagram();
            Assert.AreEqual(_projectManager.CurrentDiagrams.First(), _projectManager.DiagramViewModels.First().Diagram);
        }

        [TestMethod]
        public void CreateDiagramTest_ProjectDirty()
        {
            _projectManager.CreateProject();
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
        public void CreateDiagramTest_FromExistingDiagramModel_DiagramAdded()
        {
            var diagramMoq = new Mock<DiagramModel>();
            _projectManager.CreateProject();
            _projectManager.CreateDiagram(diagramMoq.Object);
            Assert.IsNotNull(_projectManager.CurrentDiagrams[0]);
        }

        [TestMethod]
        public void CreateDiagramTest_FromExistingDiagramModel_DiagramViewModelAdded()
        {
            var diagramMoq = new Mock<DiagramModel>();
            _projectManager.CreateProject();
            _projectManager.CreateDiagram(diagramMoq.Object);
            Assert.AreEqual(_projectManager.CurrentDiagrams.First(), _projectManager.DiagramViewModels.First().Diagram);
        }

        [TestMethod]
        public void CreateDiagramTest_FromExistingDiagramModel_ProjectDirty()
        {
            var diagramMoq = new Mock<DiagramModel>();
            _projectManager.CreateProject();
            _projectManager.CreateDiagram(diagramMoq.Object);
            Assert.IsTrue(_projectManager.IsProjectDirty);
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
        public void DeleteDiagramTest_DiagramViewModelRemoved()
        {
            _projectManager.CreateProject();
            _projectManager.CreateDiagram();
            _projectManager.DeleteDiagram(_projectManager.CurrentDiagrams[0]);
            Assert.AreEqual(0, _projectManager.DiagramViewModels.Count);
        }

        [TestMethod]
        public void DeleteDiagramTest_RemoveDiagramOnProjectCalled()
        {
            var projectMoq = new Mock<ProjectModel>();
            projectMoq.SetupGet(p => p.Diagrams).Returns(new ObservableCollection<DiagramModel>());
            _projectManager.CurrentProject = projectMoq.Object;
            _projectManager.DeleteDiagram(new DiagramModel());
            projectMoq.Verify(model => model.RemoveDiagram(It.IsAny<DiagramModel>()));
        }

        [TestMethod]
        public void CreateDiagramTest_AddDiagramOnProjectCalled()
        {
            var projectMoq = new Mock<ProjectModel>();
            projectMoq.SetupGet(p => p.Diagrams).Returns(new ObservableCollection<DiagramModel>());
            _projectManager.CurrentProject = projectMoq.Object;
            _projectManager.CreateDiagram(new DiagramModel());
            projectMoq.Verify(model => model.AddDiagram(It.IsAny<DiagramModel>()));
        }

        [TestMethod]
        public void DeleteDiagramTest_ProjectDirty()
        {
            _projectManager.CreateProject();
            _projectManager.CreateDiagram();
            _projectManager.DeleteDiagram(_projectManager.CurrentDiagrams[0]);
            Assert.IsTrue(_projectManager.IsProjectDirty);
        }
    }
}