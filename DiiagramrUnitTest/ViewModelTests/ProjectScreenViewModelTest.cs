using System;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.ProjectScreen;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class ProjectScreenViewModelTest
    {
        private Mock<DiagramWellViewModel> _diagramWellViewModelMoq;
        private Mock<ProjectExplorerViewModel> _projectExplorerViewModelMoq;
        private Mock<IProjectManager> _projectManagerMoq;
        private ProjectScreenViewModel _projectScreenViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _projectManagerMoq = new Mock<IProjectManager>();
            _diagramWellViewModelMoq = new Mock<DiagramWellViewModel>(
                (Func<IProjectManager>) (() => _projectManagerMoq.Object));
            _projectExplorerViewModelMoq = new Mock<ProjectExplorerViewModel>(
                (Func<IProjectManager>) (() => _projectManagerMoq.Object));
            _projectScreenViewModel = new ProjectScreenViewModel(
                () => _projectExplorerViewModelMoq.Object,
                () => _diagramWellViewModelMoq.Object,
                () => _projectManagerMoq.Object);
        }

        [TestMethod]
        public void TestConstructor_SetsProjectExplorer()
        {
            Assert.AreEqual(_projectExplorerViewModelMoq.Object, _projectScreenViewModel.ProjectExplorerViewModel);
        }

        [TestMethod]
        public void TestConstructor_SetsDiagramWell()
        {
            Assert.AreEqual(_diagramWellViewModelMoq.Object, _projectScreenViewModel.DiagramWellViewModel);
        }
    }
}
