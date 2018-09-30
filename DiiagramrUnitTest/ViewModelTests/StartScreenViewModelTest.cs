using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.ObjectModel;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class StartScreenViewModelTest
    {
        private Mock<IProjectManager> _projectManagerMoq;
        private StartScreenViewModel _startScreenViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _projectManagerMoq = new Mock<IProjectManager>();
            _startScreenViewModel = new StartScreenViewModel(() => _projectManagerMoq.Object);
        }

        [TestMethod]
        public void TestLoadProject_CallsLoadProjectOnProjectManager()
        {
            _startScreenViewModel.LoadProject();
            _projectManagerMoq.Verify(m => m.LoadProject(false));
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
        public void TestCreateProject_OpensOnlyDiagram()
        {
            var diagramMoq = new Mock<DiagramModel>();
            var diagrams = new ObservableCollection<DiagramModel> { diagramMoq.Object };
            _projectManagerMoq.SetupGet(m => m.CurrentDiagrams).Returns(diagrams);
            _startScreenViewModel.NewProject();
            diagramMoq.VerifySet(d => d.IsOpen = true);
        }
    }
}
