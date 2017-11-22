using System;
using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.ShellScreen;
using DiiagramrAPI.ViewModel.ShellScreen.ProjectScreen;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class ShellViewModelTest
    {
        private Mock<DiagramWellViewModel> _diagramWellViewModelMoq;
        private Mock<LibraryManagerScreenViewModel> _libraryManagerViewModelMoq;
        private Mock<IProvideNodes> _nodeProviderMoq;
        private Mock<NodeSelectorViewModel> _nodeSelectorViewModelMoq;
        private Mock<IPluginLoader> _pluginLoaderMoq;
        private Mock<ProjectExplorerViewModel> _projectExplorerViewModelMoq;
        private Mock<IProjectManager> _projectManagerMoq;
        private Mock<ProjectScreenViewModel> _projectScreenViewModelMoq;
        private ShellViewModel _shellViewModel;
        private Mock<StartScreenViewModel> _startScreenViewModelMoq;

        [TestInitialize]
        public void TestInitialize()
        {
            _projectManagerMoq = new Mock<IProjectManager>();
            _nodeProviderMoq = new Mock<IProvideNodes>();
            _pluginLoaderMoq = new Mock<IPluginLoader>();
            _nodeSelectorViewModelMoq = new Mock<NodeSelectorViewModel>(
                (Func<IProvideNodes>) (() => _nodeProviderMoq.Object));
            _diagramWellViewModelMoq = new Mock<DiagramWellViewModel>((Func<IProjectManager>) (() => _projectManagerMoq.Object));
            _projectExplorerViewModelMoq = new Mock<ProjectExplorerViewModel>(
                (Func<IProjectManager>) (() => _projectManagerMoq.Object));
            _projectScreenViewModelMoq = new Mock<ProjectScreenViewModel>(
                (Func<ProjectExplorerViewModel>) (() => _projectExplorerViewModelMoq.Object),
                (Func<DiagramWellViewModel>) (() => _diagramWellViewModelMoq.Object),
                (Func<IProjectManager>) (() => _projectManagerMoq.Object));
            _libraryManagerViewModelMoq = new Mock<LibraryManagerScreenViewModel>(
                (Func<IPluginLoader>) (() => _pluginLoaderMoq.Object));
            _startScreenViewModelMoq = new Mock<StartScreenViewModel>(
                (Func<IProjectManager>) (() => _projectManagerMoq.Object));
            _shellViewModel = new ShellViewModel(
                () => _projectScreenViewModelMoq.Object,
                () => _libraryManagerViewModelMoq.Object,
                () => _startScreenViewModelMoq.Object,
                () => _projectManagerMoq.Object);
        }

        [TestMethod]
        public void TestRequestClose_CallsCloseProject()
        {
            _shellViewModel.RequestClose();
            _projectManagerMoq.Verify(m => m.CloseProject(), Times.Once);
        }

        [TestMethod]
        public void TestCreateProject_CallsCreateProject()
        {
            _shellViewModel.CreateProject();
            _projectManagerMoq.Verify(m => m.CreateProject(), Times.Once);
        }

        [TestMethod]
        public void TestLoadProject_CallsLoadProject()
        {
            _shellViewModel.LoadProject();
            _projectManagerMoq.Verify(m => m.LoadProject(), Times.Once);
        }

        [TestMethod]
        public void TestSaveProject_CallsSaveProject()
        {
            _shellViewModel.SaveProject();
            _projectManagerMoq.Verify(m => m.SaveProject(), Times.Once);
        }

        [TestMethod]
        public void TestSaveAsProject_CallsSaveAsProject()
        {
            _shellViewModel.SaveAsProject();
            _projectManagerMoq.Verify(m => m.SaveAsProject(), Times.Once);
        }

        [TestMethod]
        public void TestSaveAndClose_CallsSaveProject()
        {
            _shellViewModel.SaveAndClose();
            _projectManagerMoq.Verify(m => m.SaveProject(), Times.Once);
        }

        [TestMethod]
        public void TestSaveAndClose_CallsCloseProject()
        {
            _shellViewModel.SaveAndClose();
            _projectManagerMoq.Verify(m => m.CloseProject(), Times.Once);
        }

        [TestMethod]
        public void TestClose_CallsCloseProject()
        {
            _shellViewModel.Close();
            _projectManagerMoq.Verify(m => m.CloseProject(), Times.Once);
        }

        [TestMethod]
        public void TestCanSaveProject_ProjectChangedToNull_CanSaveProjectIsFalse()
        {
            _projectManagerMoq.SetupProperty(m => m.CurrentProject);
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.IsFalse(_shellViewModel.CanSaveProject);
        }

        [TestMethod]
        public void TestCanSaveProject_ProjectChangedToNotNull_CanSaveProjectIsTrue()
        {
            _projectManagerMoq.SetupProperty(m => m.CurrentProject);
            _projectManagerMoq.Object.CurrentProject = new ProjectModel();
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.IsTrue(_shellViewModel.CanSaveProject);
        }

        [TestMethod]
        public void TestCanSaveAsProject_ProjectChangedToNull_CanSaveProjectIsFalse()
        {
            _projectManagerMoq.SetupProperty(m => m.CurrentProject);
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.IsFalse(_shellViewModel.CanSaveAsProject);
        }

        [TestMethod]
        public void TestCanSaveAsProject_ProjectChangedToNotNull_CanSaveProjectIsTrue()
        {
            _projectManagerMoq.SetupProperty(m => m.CurrentProject);
            _projectManagerMoq.Object.CurrentProject = new ProjectModel();
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.IsTrue(_shellViewModel.CanSaveAsProject);
        }
    }
}