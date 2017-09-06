﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Diiagramr.Service;
using Moq;
using Diiagramr.Model;

namespace ColorOrgan5UnitTests.ServiceTests
{
    [TestClass]
    public class ProjectManagerTests
    {
        private ProjectManager _projectManager;
        private Mock<IProjectFileService> _projectFileServiceMoq;
        private bool _currentProjectChanged;

        [TestInitialize]
        public void TestInitialize()
        {
            _projectFileServiceMoq = new Mock<IProjectFileService>();
            _currentProjectChanged = false;
            _projectManager = new ProjectManager(_projectFileServiceMoq.Object);
            _projectManager.CurrentProjectChanged += () => _currentProjectChanged = true;
        }

        [TestMethod]
        public void CreateProjectTest_InitializeProject()
        {
            _projectManager.CreateProject();
            Assert.IsNotNull(_projectManager.CurrentProject);
        }

        [TestMethod]
        public void CreateProjectTest_ProjectDirty()
        {
            _projectManager.CreateProject();
            Assert.IsTrue(_projectManager.IsProjectDirty);
        }

        [TestMethod]
        public void CreateProjectTest_ProjectChanged()
        {
            _projectManager.CreateProject();
            Assert.IsTrue(_currentProjectChanged);
        }

        [TestMethod]
        public void CreateProjectTest_ProjectFileProjectNameSetNull()
        {
            _projectFileServiceMoq.SetupSet(m => m.ProjectName = "test");
            _projectManager.CreateProject();
            _projectFileServiceMoq.VerifySet(m => m.ProjectName = null);
        }

        [TestMethod]
        public void SaveProjectTest_ProjectSavedNotDirty()
        {
            _projectManager.CreateProject();
            _projectFileServiceMoq.Setup(m => m.SaveProject(_projectManager.CurrentProject, false)).Returns(true);
            _projectManager.SaveProject();
            Assert.IsFalse(_projectManager.IsProjectDirty);
        }

        [TestMethod]
        public void SaveProjectTest_ProjectNotSavedDirty()
        {
            _projectManager.CreateProject();
            _projectFileServiceMoq.Setup(m => m.SaveProject(_projectManager.CurrentProject, false)).Returns(false);
            _projectManager.SaveProject();
            Assert.IsTrue(_projectManager.IsProjectDirty);
        }

        [TestMethod]
        public void LoadProjectTest_CurrentProjectSet()
        {
            _projectFileServiceMoq.Setup(m => m.LoadProject()).Returns(new Project(""));
            _projectManager.LoadProject();
            Assert.IsNotNull(_projectManager.CurrentProject);
        }

        [TestMethod]
        public void LoadProjectTest_ProjectChanged()
        {
            _projectFileServiceMoq.Setup(m => m.LoadProject()).Returns(new Project(""));
            _projectManager.LoadProject();
            Assert.IsTrue(_currentProjectChanged);
        }
    }
}
