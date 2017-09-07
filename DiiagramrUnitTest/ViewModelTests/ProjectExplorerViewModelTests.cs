using Diiagramr.Model;
using Diiagramr.Service;
using Diiagramr.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stylet;
using System;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class ProjectExplorerViewModelTests
    {
        private Mock<IProjectManager> _projectManagerMoq;
        private ProjectExplorerViewModel _projectExplorerViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            MockedViewModelFactories.CreateSingletonMoqs();
            _projectManagerMoq = MockedViewModelFactories.CreateMoqProjectManager();

            Func<IProjectManager> projectManagerFactory = () => _projectManagerMoq.Object;

            _projectExplorerViewModel = new ProjectExplorerViewModel(projectManagerFactory);
        }

        [TestMethod]
        public void TestCanDeleteDiagram_SelectedDiagramNull_ReturnsFalse()
        {
            Assert.IsFalse(_projectExplorerViewModel.CanDeleteDiagram);
        }

        [TestMethod]
        public void TestCanDeleteDiagram_SelectedDiagramNotNull_ReturnsTrue()
        {
            _projectExplorerViewModel.SelectedDiagram = new EDiagram();
            Assert.IsTrue(_projectExplorerViewModel.CanDeleteDiagram);
        }

        [TestMethod]
        public void TestCreateDiagram_CallsCreateDiagram()
        {
            _projectExplorerViewModel.CreateDiagram();
            _projectManagerMoq.Verify(m => m.CreateDiagram(), Times.Once);
        }

        [TestMethod]
        public void TestDeleteDiagram_CallsDeleteDiagramWithSelectedDiagram()
        {
            var diagram = new EDiagram();
            _projectManagerMoq.SetupGet(m => m.CurrentDiagrams).Returns(new BindableCollection<EDiagram> { diagram });
            _projectExplorerViewModel.SelectedDiagram = diagram;
            _projectExplorerViewModel.DeleteDiagram();
            _projectManagerMoq.Verify(m => m.DeleteDiagram(It.Is<EDiagram>(d => d == diagram)), Times.Once);
        }

        [TestMethod]
        public void TestProjectChanged_ProjectNull_IsAddDiagramButtonIsFalse()
        {
            _projectManagerMoq.SetupProperty(m => m.CurrentProject);
            _projectManagerMoq.Object.CurrentProject = null;
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.IsFalse(_projectExplorerViewModel.IsAddDiagramButtonVisible);
        }

        [TestMethod]
        public void TestProjectChanged_ProjectNotNull_IsAddDiagramButtonIsTrue()
        {
            _projectManagerMoq.SetupProperty(m => m.CurrentProject);
            _projectManagerMoq.Object.CurrentProject = new Project("");
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.IsTrue(_projectExplorerViewModel.IsAddDiagramButtonVisible);
        }

        [TestMethod]
        public void TestProjectChanged_ProjectNotNull_ProjectSet()
        {
            var project = new Project("");
            _projectManagerMoq.SetupProperty(m => m.CurrentProject);
            _projectManagerMoq.Object.CurrentProject = project;
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.AreEqual(project, _projectExplorerViewModel.Project);
        }
    }
}
