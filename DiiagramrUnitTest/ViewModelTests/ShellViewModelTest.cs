using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Commands;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.ProjectScreen;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class ShellViewModelTest
    {
        private Mock<DiagramWellViewModel> _diagramWellViewModelMoq;
        private Mock<ILibraryManager> _libraryManagerMoq;
        private Mock<LibraryManagerWindowViewModel> _libraryManagerViewModelMoq;
        private Mock<LibrarySourceManagerWindowViewModel> _librarySourceManagerViewModelMoq;
        private Mock<IProvideNodes> _nodeProviderMoq;
        private Mock<NodeSelectorViewModel> _nodeSelectorViewModelMoq;
        private Mock<IPluginLoader> _pluginLoaderMoq;
        private Mock<ProjectExplorerViewModel> _projectExplorerViewModelMoq;
        private Mock<IProjectManager> _projectManagerMoq;
        private Mock<ProjectScreenViewModel> _projectScreenViewModelMoq;
        private ShellViewModel _shellViewModel;
        private Mock<StartScreenViewModel> _startScreenViewModelMoq;
        private Mock<ContextMenuViewModel> _contextMenuViewModelMoq;

        [TestInitialize]
        public void TestInitialize()
        {
            _projectManagerMoq = new Mock<IProjectManager>();
            _nodeProviderMoq = new Mock<IProvideNodes>();
            _pluginLoaderMoq = new Mock<IPluginLoader>();
            _libraryManagerMoq = new Mock<ILibraryManager>();
            _contextMenuViewModelMoq = new Mock<ContextMenuViewModel>();
            _nodeSelectorViewModelMoq = new Mock<NodeSelectorViewModel>(
                (Func<IProvideNodes>)(() => _nodeProviderMoq.Object));
            _diagramWellViewModelMoq = new Mock<DiagramWellViewModel>((Func<IProjectManager>)(() => _projectManagerMoq.Object));
            _projectExplorerViewModelMoq = new Mock<ProjectExplorerViewModel>(
                (Func<IProjectManager>)(() => _projectManagerMoq.Object));
            _projectScreenViewModelMoq = new Mock<ProjectScreenViewModel>(
                (Func<ProjectExplorerViewModel>)(() => _projectExplorerViewModelMoq.Object),
                (Func<DiagramWellViewModel>)(() => _diagramWellViewModelMoq.Object),
                (Func<IProjectManager>)(() => _projectManagerMoq.Object));
            _librarySourceManagerViewModelMoq = new Mock<LibrarySourceManagerWindowViewModel>(
                (Func<ILibraryManager>)(() => _libraryManagerMoq.Object));
            _libraryManagerViewModelMoq = new Mock<LibraryManagerWindowViewModel>(
                (Func<ILibraryManager>)(() => _libraryManagerMoq.Object),
                (Func<LibrarySourceManagerWindowViewModel>)(() => _librarySourceManagerViewModelMoq.Object));
            _startScreenViewModelMoq = new Mock<StartScreenViewModel>(
                (Func<IProjectManager>)(() => _projectManagerMoq.Object));
            _shellViewModel = new ShellViewModel(
                () => _projectScreenViewModelMoq.Object,
                () => _libraryManagerViewModelMoq.Object,
                () => _startScreenViewModelMoq.Object,
                () => _projectManagerMoq.Object,
                () => Enumerable.Empty<IDiiagramrCommand>(),
                () => _contextMenuViewModelMoq.Object);
        }

        [TestMethod]
        public void TestRequestClose_CallsCloseProject()
        {
            _shellViewModel.RequestClose();
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
