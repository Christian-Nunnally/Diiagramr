using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Windows;
using Diiagramr.Model;
using Diiagramr.ViewModel;
using Diiagramr.ViewModel.Diagram;
using TechTalk.SpecFlow;

namespace ColorOrgan5UnitTests.Features.Steps
{
    [Binding]
    public class DiagramSteps : TechTalk.SpecFlow.Steps
    {
        public static DiagramWellViewModel DiagramWellViewModel;
        private DemoNodeViewModel _demoNode;
        private DiagramViewModel _diagramViewModel;

        [Given(@"I have a diagram")]
        public void GivenIHaveADiagram()
        {
            var nodeSelectorViewModel = new NodeSelectorViewModel(() => new List<AbstractNodeViewModel>());
            DiagramWellViewModel = new DiagramWellViewModel(() => nodeSelectorViewModel);
            _diagramViewModel = new DiagramViewModel(new EDiagram(), nodeSelectorViewModel);
            DiagramWellViewModel.ActiveItem = _diagramViewModel;
        }

        [Given(@"the diagram has (.*) nodes")]
        public void GivenTheDiagramHasNodes(int count)
        {
            while (_diagramViewModel.NodeViewModels.Count > count) _diagramViewModel.NodeViewModels.RemoveAt(0);
            var n = new DiagramNode("DemoNode");
            var nvm = new DemoNodeViewModel();
            nvm.InitializeWithNode(n);
            while (_diagramViewModel.NodeViewModels.Count < count) _diagramViewModel.NodeViewModels.Add(nvm);
        }


        [Given(@"a demo diagramNode is being inserted to the diagram")]
        public void GivenADemoNodeIsBeingInsertedToTheDiagram()
        {
            _demoNode = new DemoNodeViewModel();
            _demoNode.InitializeWithNode(new DiagramNode("TestNode"));
            _diagramViewModel.InsertingNodeViewModel = _demoNode;
        }

        [Given(@"no diagramNode is being inserted to the diagram")]
        public void GivenNoNodeIsBeingInsertedToTheDiagram()
        {
            _diagramViewModel.InsertingNodeViewModel = null;
        }

        [When(@"the user clicks the diagram")]
        public void WhenTheUserClicksTheDiagram()
        {
            _diagramViewModel.LeftMouseButtonDown(new Point(0, 0));
        }

        [Then(@"the diagram has (.*) diagramNode")]
        public void ThenTheDiagramHasNode(int count)
        {
            Assert.AreEqual(count, _diagramViewModel.Diagram.Nodes.Count);
        }

        [Then(@"the diagrams inserting diagramNode is null")]
        public void ThenTheDiagramsInsertingNodeIsNull()
        {
            Assert.IsNull(_diagramViewModel.InsertingNodeViewModel);
        }
    }
}