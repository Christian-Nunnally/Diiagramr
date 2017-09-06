using Diiagramr.Service;
using Diiagramr.ViewModel;
using Diiagramr.ViewModel.Diagram;
using Moq;
using System;
using System.Collections.Generic;

namespace ColorOrgan5UnitTests.ViewModelTests
{
    internal static class MockedViewModelFactories
    {
        public static Mock<IProjectManager> CreateMoqProjectManager()
        {
            return new Mock<IProjectManager>();
        }

        private static Func<IProjectManager> CreateProjectManagerFactory()
        {
            return () => CreateMoqProjectManager().Object;
        }

        public static Mock<NodeSelectorViewModel> CreateMoqNodeSelectorViewModel()
        {
            Func<IEnumerable<AbstractNodeViewModel>> emptyNodeListFactory = () => new List<AbstractNodeViewModel>();
            return new Mock<NodeSelectorViewModel>(emptyNodeListFactory);
        }

        private static Func<NodeSelectorViewModel> CreateNodeSelectorFactory()
        {
            return () => CreateMoqNodeSelectorViewModel().Object;
        }

        public static Mock<ProjectExplorerViewModel> CreateMoqProjectExplorer()
        {
            return new Mock<ProjectExplorerViewModel>(CreateProjectManagerFactory());
        }

        public static Mock<DiagramWellViewModel> CreateMoqDiagramWell()
        {
            return new Mock<DiagramWellViewModel>(CreateProjectManagerFactory(), CreateNodeSelectorFactory());
        }
    }
}
