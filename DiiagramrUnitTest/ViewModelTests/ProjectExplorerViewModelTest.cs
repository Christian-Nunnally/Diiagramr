using DiiagramrAPI.Project;
using DiiagramrModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stylet;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class ProjectExplorerViewModelTest
    {
        private ProjectExplorer _projectExplorerViewModel;
        private Mock<IProjectManager> _projectManagerMoq;

        [TestMethod]
        public void TestCanDeleteDiagram_SelectedDiagramNotNull_ReturnsTrue()
        {
            _projectExplorerViewModel.SelectedDiagram = new DiagramModel();
            Assert.IsTrue(_projectExplorerViewModel.CanDeleteDiagram);
        }

        [TestMethod]
        public void TestCanDeleteDiagram_SelectedDiagramNull_ReturnsFalse()
        {
            Assert.IsFalse(_projectExplorerViewModel.CanDeleteDiagram);
        }

        [TestMethod]
        public void TestCopyDiagram_DiagramSelected_NewDiagramAdded()
        {
            _projectExplorerViewModel.SelectedDiagram = new DiagramModel();

            _projectExplorerViewModel.CopyDiagram();

            _projectManagerMoq.Verify(p => p.CreateDiagram(It.IsAny<DiagramModel>()));
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
            var diagram = new DiagramModel();
            _projectManagerMoq.SetupGet(m => m.CurrentDiagrams).Returns(new BindableCollection<DiagramModel> { diagram });
            _projectExplorerViewModel.SelectedDiagram = diagram;
            _projectExplorerViewModel.DeleteDiagram();
            _projectManagerMoq.Verify(m => m.DeleteDiagram(It.Is<DiagramModel>(d => d == diagram)), Times.Once);
        }

        [TestMethod]
        public void TestDeleteDiagram_SelectedDiagramNull_NoExceptions()
        {
            _projectExplorerViewModel.DeleteDiagram();
        }

        [TestMethod]
        public void TestDiagramProjectItemMouseUp_DiagramsClosed()
        {
            var diagramMoq = new Mock<DiagramModel>();
            _projectExplorerViewModel.Diagrams.Add(diagramMoq.Object);
            _projectExplorerViewModel.DiagramProjectItemMouseUp();

            diagramMoq.VerifySet(d => d.IsOpen = false);
        }

        [TestMethod]
        public void TestDiagramProjectItemMouseUp_SingleClick_SelectedDiagramOpen()
        {
            var diagramMoq = new Mock<DiagramModel>();
            _projectExplorerViewModel.SelectedDiagram = diagramMoq.Object;

            _projectExplorerViewModel.DiagramProjectItemMouseUp();

            diagramMoq.VerifySet(d => d.IsOpen = true);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _projectManagerMoq = new Mock<IProjectManager>();
            _projectExplorerViewModel = new ProjectExplorer(() => _projectManagerMoq.Object);
        }

        [TestMethod]
        public void TestProjectChanged_ProjectNotNull_IsAddDiagramButtonIsTrue()
        {
            _projectManagerMoq.SetupProperty(m => m.CurrentProject);
            _projectManagerMoq.Object.CurrentProject = new ProjectModel();
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.IsTrue(_projectExplorerViewModel.IsAddDiagramButtonVisible);
        }

        [TestMethod]
        public void TestProjectChanged_ProjectNotNull_ProjectSet()
        {
            var project = new ProjectModel();
            _projectManagerMoq.SetupProperty(m => m.CurrentProject);
            _projectManagerMoq.Object.CurrentProject = project;
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.AreEqual(project, _projectExplorerViewModel.Project);
        }

        [TestMethod]
        public void TestProjectChanged_ProjectNull_IsAddDiagramButtonIsFalse()
        {
            _projectManagerMoq.SetupProperty(m => m.CurrentProject);
            _projectManagerMoq.Object.CurrentProject = null;
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.IsFalse(_projectExplorerViewModel.IsAddDiagramButtonVisible);
        }
    }
}