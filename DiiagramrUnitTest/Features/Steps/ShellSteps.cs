using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Windows;
using Diiagramr.Model;
using Diiagramr.Service;
using Diiagramr.ViewModel;
using Diiagramr.ViewModel.Diagram;
using TechTalk.SpecFlow;
// ReSharper disable UnusedMember.Global

namespace ColorOrgan5UnitTests.Features.Steps
{
    [Binding]
    public class ShellSteps : TechTalk.SpecFlow.Steps
    {
        public static Mock<IProjectFileService> FileSystemServiceMoq;
        public static Mock<DiagramWellViewModel> DiagramWellViewModelMoq;
        public static ShellViewModel Shell;

        [Given(@"the user launches the application")]
        public void GivenTheUserLaunchsTheApplication()
        {
            FileSystemServiceMoq = new Mock<IProjectFileService>();
            FileSystemServiceMoq.Setup(m => m.IsProjectNameValid(It.IsAny<string>())).Returns(true);
            FileSystemServiceMoq.Setup(m => m.GetSavedProjectNames()).Returns(_projectNamesInProjectDirectory);
            FileSystemServiceMoq.Setup(m => m.CreateProject(It.IsAny<string>())).Returns(new Project("p"));

            DiagramWellViewModelMoq = new Mock<DiagramWellViewModel>(new Func<NodeSelectorViewModel>(() => new NodeSelectorViewModel(() => new AbstractNodeViewModel[0])));

            Shell = new ShellViewModel(() => FileSystemServiceMoq.Object, () => DiagramWellViewModelMoq.Object);
        }


        [When(@"the new project button is pressed")]
        public void WhenTheNewProjectButtonIsPressed()
        {
            Shell.CreateProject();
        }

        [Then(@"a new project is created")]
        public void ThenANewProjectIsCreated()
        {
            FileSystemServiceMoq.Verify(m => m.CreateProject(It.IsNotIn("")));
        }

        [When(@"they press the close button")]
        public void WhenTheyPressTheCloseButton()
        {
            Shell.Close();
        }

        [Then(@"the application closes")]
        public void ThenTheApplicationCloses()
        {
            Assert.IsTrue(Shell.IsClosed);
        }

        [Then(@"the application prompts the user to save")]
        public void ThenTheApplicationPromptsTheUserToSave()
        {
            Assert.AreEqual(Visibility.Visible, Shell.SavePromptVisible);
        }

        [When(@"the project is in a dirty state")]
        public void GivenTheProjectIsInADirtyState()
        {
            Shell.ProjectSaved = false;
        }

        [When(@"the project is in a clean state")]
        public void GivenTheProjectIsInACleanState()
        {
            Shell.ProjectSaved = true;
        }

        [When(@"the user presses the dont save before closing button")]
        public void WhenTheUserPressesTheDontSaveBeforeClosingButton()
        {
            Shell.DoNotSaveAndClose();
        }

        [When(@"the user presses the save before closing button")]
        public void WhenTheUserPressesTheSaveBeforeClosingButton()
        {
            Shell.SaveAndClose();
        }

        [Then(@"the project is saved")]
        public void ThenTheProjectIsSaved()
        {
            FileSystemServiceMoq.Verify(m => m.SaveProject(It.IsAny<Project>()));
        }

        [Then(@"the project is closed")]
        public void ThenTheProjectIsClosed()
        {
            Assert.IsNull(Shell.ProjectExplorerViewModel?.CurrentProject);
        }

        [When(@"the user presses the cancel button")]
        public void WhenTheUserPressesTheCancelButton()
        {
            Shell.CancelClose();
        }

        [Then(@"the project is not closed")]
        public void ThenTheProjectIsNotClosed()
        {
            Assert.IsNotNull(Shell.ProjectExplorerViewModel.CurrentProject);
        }

        [When(@"the project is saved")]
        public void WhenTheProjectIsSaved()
        {
            Shell.SaveProject();
        }

        [Then(@"the project saved indicator is true")]
        public void ThenTheProjectSavedIndicatorIsTrue()
        {
            Assert.AreEqual(true, Shell.ProjectSaved);
        }

        [Then(@"the save prompt is no longer visible")]
        public void ThenTheSavePromptIsNoLongerVisible()
        {
            Assert.AreEqual(Visibility.Hidden, Shell.SavePromptVisible);
        }

        private readonly IList<string> _projectNamesInProjectDirectory = new List<string>();

        [Given(@"there is a project in the project directory named '(.*)'")]
        public void GivenThereIsAProjectInTheProjectDirectoryNamed(string projectName)
        {
            _projectNamesInProjectDirectory.Add("\\" + projectName);
            FileSystemServiceMoq.Setup(m => m.GetSavedProjectNames()).Returns(_projectNamesInProjectDirectory);
            FileSystemServiceMoq.Setup(m => m.LoadProject(projectName)).Returns(new Project(projectName));
        }

        [Then(@"there is a '(.*)' button in the open menu")]
        public void ThenThereIsAButtonInTheOpenMenu(string buttonHeader)
        {
            Assert.IsTrue(Shell.ProjectNamesInProjectDirectory.Contains(buttonHeader));
        }

        [Given(@"there is no open project")]
        public void GivenThereIsNoOpenProject()
        {
            Shell.ProjectExplorerViewModel = null;
        }

        [When(@"the user loads project '(.*)'")]
        public void WhenTheUserLoadsProject(string projectName)
        {
            Shell.LoadProject(projectName);
        }

        [Then(@"the project load will be requested for project '(.*)'")]
        public void ThenTheProjectLoadWillBeRequested(string projectName)
        {
            FileSystemServiceMoq.Verify(m => m.LoadProject(projectName));
        }

        [Then(@"the project will be loaded")]
        public void ThenTheProjectWillBeLoaded()
        {
            Assert.IsNotNull(Shell.ProjectExplorerViewModel);
        }

        [When(@"the open button is pressed")]
        public void WhenTheOpenButtonIsPressed()
        {
            Shell.UpdateProjectNamesInProjectDirectory();
        }

        [When(@"the project is changed")]
        public void WhenTheProjectIsChanged()
        {
            FileSystemServiceMoq.Setup(m => m.MoveProject(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            Shell.ProjectExplorerViewModel.SelectedProject = Shell.ProjectExplorerViewModel.CurrentProject;
            Shell.ProjectExplorerViewModel.RenameRequested("newprojectname");
        }

        [Then(@"the project saved flag is false")]
        public void ThenTheProjectSavedFlagIsFalse()
        {
            Assert.IsFalse(Shell.ProjectSaved);
        }

    }
}
