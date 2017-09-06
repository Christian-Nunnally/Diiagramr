﻿using Diiagramr.Model;
using Diiagramr.Service;
using Diiagramr.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace ColorOrgan5UnitTests.ViewModelTests
{
    [TestClass]
    public class ShellViewModelTests
    {
        private Mock<IProjectManager> _projectManagerMoq;
        private Mock<DiagramWellViewModel> _diagramWellViewModelMoq;
        private Mock<ProjectExplorerViewModel> _projectExplorerViewModelMoq;
        private ShellViewModel _shellViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _projectManagerMoq = MockedViewModelFactories.CreateMoqProjectManager();
            _diagramWellViewModelMoq = MockedViewModelFactories.CreateMoqDiagramWell();
            _projectExplorerViewModelMoq = MockedViewModelFactories.CreateMoqProjectExplorer();

            Func<IProjectManager> projectManagerFactory = () => _projectManagerMoq.Object;
            Func<DiagramWellViewModel> diagramWellFactory = () => _diagramWellViewModelMoq.Object;
            Func<ProjectExplorerViewModel> projectExplorerFactory = () => _projectExplorerViewModelMoq.Object;

            _shellViewModel = new ShellViewModel(projectExplorerFactory, diagramWellFactory, projectManagerFactory);
        }

        [TestMethod]
        public void TestRequestClose_CallsCloseProject()
        {
            _shellViewModel.RequestClose();
            _projectManagerMoq.Verify(m => m.CloseProject(), Times.Once);
        }

        [TestMethod]
        public void TestCreateProject_CallsCreateProject()
        {
            _shellViewModel.CreateProject();
            _projectManagerMoq.Verify(m => m.CreateProject(), Times.Once);
        }

        [TestMethod]
        public void TestLoadProject_CallsLoadProject()
        {
            _shellViewModel.LoadProject();
            _projectManagerMoq.Verify(m => m.LoadProject(), Times.Once);
        }

        [TestMethod]
        public void TestSaveProject_CallsSaveProject()
        {
            _shellViewModel.SaveProject();
            _projectManagerMoq.Verify(m => m.SaveProject(), Times.Once);
        }

        [TestMethod]
        public void TestSaveAndClose_CallsSaveProject()
        {
            _shellViewModel.SaveAndClose();
            _projectManagerMoq.Verify(m => m.SaveProject(), Times.Once);
        }

        [TestMethod]
        public void TestSaveAndClose_CallsCloseProject()
        {
            _shellViewModel.SaveAndClose();
            _projectManagerMoq.Verify(m => m.CloseProject(), Times.Once);
        }

        [TestMethod]
        public void TestClose_CallsCloseProject()
        {
            _shellViewModel.Close();
            _projectManagerMoq.Verify(m => m.CloseProject(), Times.Once);
        }

        [TestMethod]
        public void TestCanSaveProject_ProjectChangedToNull_CanSaveProjectIsFalse()
        {
            _projectManagerMoq.SetupProperty(m => m.CurrentProject);
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.IsFalse(_shellViewModel.CanSaveProject);
        }

        [TestMethod]
        public void TestCanSaveProject_ProjectChangedToNotNull_CanSaveProjectIsTrue()
        {
            _projectManagerMoq.SetupProperty(m => m.CurrentProject);
            _projectManagerMoq.Object.CurrentProject = new Project("");
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.IsTrue(_shellViewModel.CanSaveProject);
        }

        [TestMethod]
        public void TestCanSaveAsProject_ProjectChangedToNull_CanSaveProjectIsFalse()
        {
            _projectManagerMoq.SetupProperty(m => m.CurrentProject);
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.IsFalse(_shellViewModel.CanSaveAsProject);
        }

        [TestMethod]
        public void TestCanSaveAsProject_ProjectChangedToNotNull_CanSaveProjectIsTrue()
        {
            _projectManagerMoq.SetupProperty(m => m.CurrentProject);
            _projectManagerMoq.Object.CurrentProject = new Project("");
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.IsTrue(_shellViewModel.CanSaveAsProject);
        }
    }
}