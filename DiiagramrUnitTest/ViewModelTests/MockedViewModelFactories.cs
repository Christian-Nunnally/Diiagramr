using Diiagramr.Service;
using Diiagramr.ViewModel;
using Moq;
using System;

namespace DiiagramrUnitTests.ViewModelTests
{
    internal static class MockedViewModelFactories
    {
        private static Mock<IProjectManager> _staticProjectManagerMoq;
        private static Mock<IProvideNodes> _staticNodeProviderMoq;

        public static void CreateSingletonMoqs()
        {
            _staticProjectManagerMoq = new Mock<IProjectManager>();
            _staticNodeProviderMoq = new Mock<IProvideNodes>();
        }

        public static Mock<IProjectManager> CreateMoqProjectManager()
        {
            if (_staticProjectManagerMoq == null) throw new InvalidOperationException("Must call CreateSingletonMoqs before getting singleton moqs");
            return _staticProjectManagerMoq;
        }

        public static Mock<IProvideNodes> CreateMoqNodeProvider()
        {
            if (_staticNodeProviderMoq == null) throw new InvalidOperationException("Must call CreateSingletonMoqs before getting singleton moqs");
            return _staticNodeProviderMoq;
        }

        private static Func<IProjectManager> CreateProjectManagerFactory()
        {
            return () => CreateMoqProjectManager().Object;
        }

        private static Func<IProvideNodes> CreateNodeProviderFactory()
        {
            return () => CreateMoqNodeProvider().Object;
        }

        public static Mock<NodeSelectorViewModel> CreateMoqNodeSelectorViewModel()
        {
            return new Mock<NodeSelectorViewModel>(CreateNodeProviderFactory());
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
            return new Mock<DiagramWellViewModel>(CreateProjectManagerFactory(), CreateNodeProviderFactory(), CreateNodeSelectorFactory());
        }
    }
}
