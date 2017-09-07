using Diiagramr.Model;
using Diiagramr.Service;
using Diiagramr.ViewModel;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class DiagramWellViewModelTest
    {
        private DiagramWellViewModel _diagramWellViewModel;
        private Mock<IProjectManager> _projectManagerMoq;
        private Mock<IProvideNodes> _nodeProviderMoq;
        private Mock<NodeSelectorViewModel> _nodeSelectorMoq;

        [TestInitialize]
        public void TestInitialize()
        {
            MockedViewModelFactories.CreateSingletonMoqs();
            _projectManagerMoq = MockedViewModelFactories.CreateMoqProjectManager();
            _nodeProviderMoq = MockedViewModelFactories.CreateMoqNodeProvider();
            _nodeSelectorMoq = MockedViewModelFactories.CreateMoqNodeSelectorViewModel();

            Func<IProjectManager> projectManagerFactory = () => _projectManagerMoq.Object;
            Func<IProvideNodes> nodeProviderFactory = () => _nodeProviderMoq.Object;
            Func<NodeSelectorViewModel> nodeSelectorFactory = () => _nodeSelectorMoq.Object;

            _diagramWellViewModel = new DiagramWellViewModel(projectManagerFactory, nodeProviderFactory, nodeSelectorFactory);
        }

        [TestMethod]
        public void TestCloseActiveDiagram_NoOpenDiagrams_DoesNotThrowException()
        {
            _diagramWellViewModel.CloseActiveDiagram();
        }

        [TestMethod]
        public void TestCloseActiveDiagram_OneOpenDiagram_DoesNotThrowException()
        {
            var diagramMoq = new Mock<EDiagram>();
            diagramMoq.SetupAllProperties();
            diagramMoq.Object.IsOpen = true;
            _diagramWellViewModel.ActiveItem = new Mock<DiagramViewModel>(diagramMoq.Object, _nodeProviderMoq.Object, _nodeSelectorMoq.Object).Object;

            _diagramWellViewModel.CloseActiveDiagram();

            Assert.IsFalse(diagramMoq.Object.IsOpen);
        }

        [TestMethod]
        public void TestOpenDiagram_DiagramIsOpenSetToTrue_DiagramBecomesActiveItem()
        {
            var diagram = new EDiagram();
            var project = new Project("TestProject");

            _projectManagerMoq.SetupAllProperties();
            _projectManagerMoq.Object.CurrentProject = project;
            _projectManagerMoq.SetupGet(m => m.CurrentDiagrams).Returns(project.Diagrams);
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);
            project.Diagrams.Add(diagram);

            Assert.IsNull(_diagramWellViewModel.ActiveItem);
            diagram.IsOpen = true;
            Assert.AreEqual(diagram, _diagramWellViewModel.ActiveItem.Diagram);
        }
    }
}
