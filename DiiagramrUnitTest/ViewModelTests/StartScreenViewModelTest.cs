using DiiagramrAPI.Project;
using DiiagramrAPI.Shell;
using DiiagramrModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.ObjectModel;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class StartScreenViewModelTest
    {
        private Mock<IProjectFileService> _projectFileServiceMoq;
        private Mock<IProjectManager> _projectManagerMoq;
        private StartScreenViewModel _startScreenViewModel;

        [TestMethod]
        public void TestCreateProject_CallsCreateDiagramOnProjectManager()
        {
            var diagramMoq = new Mock<DiagramModel>();
            var diagrams = new ObservableCollection<DiagramModel> { diagramMoq.Object };
            _projectManagerMoq.SetupGet(m => m.CurrentDiagrams).Returns(diagrams);
            _startScreenViewModel.NewProject();
            _projectManagerMoq.Verify(m => m.CreateDiagram());
        }

        [TestMethod]
        public void TestCreateProject_CallsCreateProjectOnProjectManager()
        {
            var diagramMoq = new Mock<DiagramModel>();
            var diagrams = new ObservableCollection<DiagramModel> { diagramMoq.Object };
            _projectManagerMoq.SetupGet(m => m.CurrentDiagrams).Returns(diagrams);
            _startScreenViewModel.NewProject();
            _projectManagerMoq.Verify(m => m.CreateProject());
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _projectManagerMoq = new Mock<IProjectManager>();
            _projectFileServiceMoq = new Mock<IProjectFileService>();
            _startScreenViewModel = new StartScreenViewModel(() => _projectManagerMoq.Object, () => _projectFileServiceMoq.Object);
        }

        [TestMethod]
        public void TestLoadProject_CallsLoadProjectOnProjectManager()
        {
            _startScreenViewModel.LoadProject();
            _projectManagerMoq.Verify(m => m.LoadProject(It.IsAny<ProjectModel>(), true));
        }
    }
}