using Diiagramr.Model;
using Diiagramr.ViewModel;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Windows;
using TechTalk.SpecFlow;

// ReSharper disable UnusedMember.Global

namespace ColorOrgan5UnitTests.Features.Steps
{
    [Binding]
    public class ProjectSteps : TechTalk.SpecFlow.Steps
    {

        private readonly DiagramWellViewModel _diagramWell = new DiagramWellViewModel(() => new NodeSelectorViewModel(() => new List<AbstractNodeViewModel>()));

        private EDiagram _eDiagram;

        private bool _deleteResult;
        private ProjectExplorerViewModel _projectExplorer;
        private bool _renameRequestedResult;

        [Given(@"a project is open")]
        public void GivenAProjectIsOpen()
        {
            Given("the user launches the application");
            When("the new project button is pressed");
            _projectExplorer = ShellSteps.Shell.ProjectExplorerViewModel;
        }

        [When(@"the project has (.*) diagrams")]
        public void WhenTheProjectHasDiagrams(int numberOfDiagrams)
        {
            while (_projectExplorer.CurrentProject.Diagrams.Count < numberOfDiagrams)
                _projectExplorer.CurrentProject.Diagrams.Add(new EDiagram());
        }

        [When(@"the new _eDiagram button is pressed")]
        public void WhenTheNewDiagramButtonIsPressed()
        {
            _projectExplorer.CreateDiagram();
        }

        [Then(@"the project has (.*) diagrams")]
        public void ThenTheProjectHasDiagrams(int numberOfDiagrams)
        {
            Assert.AreEqual(numberOfDiagrams, _projectExplorer.CurrentProject.Diagrams.Count);
        }

        [Then(@"the _eDiagram is named '(.*)'")]
        public void ThenTheDiagramIsNamed(string name)
        {
            Assert.AreEqual(name, _projectExplorer.CurrentProject.Diagrams[0].Name);
        }

        [Then(@"the second _eDiagram is named '(.*)'")]
        public void ThenTheSecondDiagramIsNamed(string name)
        {
            Assert.AreEqual(name, _projectExplorer.CurrentProject.Diagrams[1].Name);
        }

        [When(@"that project is selected")]
        public void GivenThatProjectIsSelected()
        {
            _projectExplorer.SelectedProject = _projectExplorer.CurrentProject;
        }

        [When(@"there is another project with the name '(.*)'")]
        public void GivenThereIsAnotherProjectWithTheName(string name)
        {
            ShellSteps.FileSystemServiceMoq.Setup(m => m.IsProjectNameValid(name)).Returns(false);
        }

        [When(@"the user selects a project")]
        public void WhenTheUserSelectsAProject()
        {
            _projectExplorer.SelectedItemChanged(null, new RoutedPropertyChangedEventArgs<object>(null, _projectExplorer.CurrentProject));
        }

        [When(@"the user tries to rename the selected item to '(.*)'")]
        public void WhenTheUserTriesToRenameTheSelectedItemTo(string newName)
        {
            var oldName = _projectExplorer.CurrentProject.Name;
            ShellSteps.FileSystemServiceMoq.Setup(m => m.MoveProject(oldName, newName)).Returns(true);
            _renameRequestedResult = _projectExplorer.RenameRequested(newName);
        }

        [Then(@"that project is selected in the view model")]
        public void ThenThatProjectIsSelectedInTheViewModel()
        {
            Assert.AreEqual(_projectExplorer.CurrentProject, _projectExplorer.SelectedProject);
        }

        [Then(@"the project name is changed to '(.*)'")]
        public void ThenTheProjectNameIsChangedTo(string name)
        {
            Assert.AreEqual(_projectExplorer.CurrentProject.Name, name);
            Assert.IsTrue(_renameRequestedResult);
        }

        [Then(@"the project name is not changed")]
        public void ThenTheProjectNameIsNotChanged()
        {
            Assert.IsFalse(_renameRequestedResult);
        }

        [When(@"the user selects the (.*) _eDiagram")]
        public void WhenTheUserSelectsTheDiagram(int diagramIndex)
        {
            _projectExplorer.SelectedItemChanged(null, new RoutedPropertyChangedEventArgs<object>(null, _projectExplorer.CurrentProject.Diagrams[diagramIndex]));
        }

        [Then(@"the (.*) _eDiagram is selected")]
        public void ThenTheDiagramIsSelected(int diagramIndex)
        {
            Assert.AreEqual(_projectExplorer.CurrentProject.Diagrams[diagramIndex], _projectExplorer.SelectedDiagram);
        }

        [Then(@"the (.*) _eDiagram name is '(.*)'")]
        public void ThenTheDiagramNameIs(int diagramIndex, string name)
        {
            Assert.AreEqual(name, _projectExplorer.CurrentProject.Diagrams[diagramIndex].Name);
        }

        [Then(@"the (.*) _eDiagram name is not '(.*)'")]
        public void ThenTheDiagramNameIsNot(int diagramIndex, string name)
        {
            Assert.AreNotEqual(name, _projectExplorer.CurrentProject.Diagrams[diagramIndex].Name);
        }

        [When(@"there is a _eDiagram '(.*)'")]
        public void WhenThereIsADiagram(string diagramName)
        {
            var d = new EDiagram();
            d.Name = diagramName;
            _projectExplorer.CurrentProject.Diagrams.Add(d);
        }

        [When(@"_eDiagram '(.*)' is selected")]
        public void WhenDiagramIsSelected(string diagramName)
        {
            foreach (var diagram in _projectExplorer.CurrentProject.Diagrams)
                if (diagram.Name.Equals(diagramName))
                    _projectExplorer.SelectDiagram(diagram);
        }

        [Then(@"the _eDiagram well should open the selected _eDiagram")]
        public void ThenTheDiagramWellShouldOpenTheSelectedDiagram()
        {
            ShellSteps.DiagramWellViewModelMoq.Verify(m => m.OpenDiagram(ShellSteps.Shell.ProjectExplorerViewModel.SelectedDiagram));
        }

        [When(@"the user presses the delete _eDiagram button")]
        public void WhenTheUserPressesTheDeleteDiagramButton()
        {
            _deleteResult = _projectExplorer.DeleteRequested();
            Assert.IsTrue(_deleteResult);
        }

        [Given(@"there is a _eDiagram")]
        public void GivenThereIsADiagram()
        {
            _eDiagram = new EDiagram();
        }

        [When(@"the _eDiagram is opened in the _eDiagram well twice")]
        public void WhenTheDiagramIsOpenedInTheDiagramWellTwice()
        {
            _diagramWell.OpenDiagram(_eDiagram);
            _diagramWell.OpenDiagram(_eDiagram);
        }

        [Then(@"the _eDiagram well only has one _eDiagram open")]
        public void ThenTheDiagramWellOnlyHasOneDiagramOpen()
        {
            Assert.AreEqual(1, _diagramWell.Items.Count);
        }
    }
}