using Castle.Core.Internal;
using DiiagramrAPI.CustomControls;
using DiiagramrAPI.Model;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Windows.Forms;

namespace DiiagramrUnitTests.ServiceTests
{
    [TestClass]
    public class ProjectFileServiceTest
    {
        private Mock<IDirectoryService> _directoryServiceMoq;
        private ProjectFileService _projectFileService;
        private const string Directory = "test";
        private Mock<IFileDialog> _testDialogMoq;
        private Mock<IProjectLoadSave> _projectLoadSaveMoq;
        private Mock<ProjectModel> _projectMoq;

        [TestInitialize]
        public void TestInitialize()
        {
            _projectMoq = new Mock<ProjectModel>();
            _projectLoadSaveMoq = new Mock<IProjectLoadSave>();
            _testDialogMoq = new Mock<IFileDialog>();
            _testDialogMoq.Setup(d => d.ShowDialog()).Returns(DialogResult.Cancel);
            _testDialogMoq.SetupAllProperties();
            _directoryServiceMoq = new Mock<IDirectoryService>();
            _directoryServiceMoq.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
            _directoryServiceMoq.Setup(m => m.GetCurrentDirectory()).Returns("testDirectory");
            _projectFileService = new ProjectFileService(
                _directoryServiceMoq.Object,
                _testDialogMoq.Object,
                _testDialogMoq.Object,
                _projectLoadSaveMoq.Object)
            {
                ProjectDirectory = Directory
            };
        }

        [TestMethod]
        public void ConstructorTest_CreatesProjectDirectory()
        {
            Assert.IsFalse(_projectFileService.ProjectDirectory.IsNullOrEmpty());
            _directoryServiceMoq.Verify(m => m.CreateDirectory(It.IsAny<string>()));
        }

        [TestMethod]
        public void SaveProjectTest_SetsInitialDirectory()
        {
            _projectFileService.SaveProject(new Mock<ProjectModel>().Object, true);
            Assert.AreEqual(Directory, _testDialogMoq.Object.InitialDirectory);
        }

        [TestMethod]
        public void SaveProjectTest_SetsInitialFileName()
        {
            var project = new ProjectModel
            {
                Name = "testProj"
            };
            _projectFileService.SaveProject(project, true);
            Assert.AreEqual(project.Name, _testDialogMoq.Object.FileName);
        }

        [TestMethod]
        public void SaveProjectTest_SaveAsWithOkDialogResult_ProjectSaved()
        {
            string fakeFileName = "Dir\\OtherDir\\ActualProject.xml";
            _testDialogMoq.Reset();
            _testDialogMoq.SetupGet(d => d.FileName).Returns(fakeFileName);
            _testDialogMoq.Setup(f => f.ShowDialog()).Returns(DialogResult.OK);
            _projectFileService.SaveProject(_projectMoq.Object, true);

            _projectLoadSaveMoq.Verify(l => l.Save(_projectMoq.Object, fakeFileName));
        }

        [TestMethod]
        public void SaveProjectTest_SaveAsFalseWithNewName_CallsSaveOnProjectLoadSave()
        {
            _projectMoq.SetupGet(p => p.Name).Returns("Project");
            _projectFileService.SaveProject(_projectMoq.Object, false);

            _projectLoadSaveMoq.Verify(p => p.Save(_projectMoq.Object, It.Is<string>(s => s.EndsWith(".xml"))));
        }

        [TestMethod]
        public void LoadProjectTest_DirectoryPathMocked_ProjectNameSetToDirectoryPath()
        {
            const string fakeFileName = "Dir\\OtherDir\\ActualProject.xml";
            _testDialogMoq.Reset();
            _testDialogMoq.SetupGet(d => d.FileName).Returns(fakeFileName);
            _testDialogMoq.Setup(f => f.ShowDialog()).Returns(DialogResult.OK);
            _projectLoadSaveMoq.Setup(s => s.Open(fakeFileName)).Returns(_projectMoq.Object);
            _projectFileService.LoadProject();

            _projectMoq.VerifySet(p => p.Name = "ActualProject");
        }

        [TestMethod]
        public void LoadProjectTest_OpenFileDialogCanceled_NullProjectReturned()
        {
            _testDialogMoq.Setup(f => f.ShowDialog()).Returns(DialogResult.Cancel);
            _projectLoadSaveMoq.Setup(s => s.Open(It.IsAny<string>())).Returns(_projectMoq.Object);
            Assert.IsNull(_projectFileService.LoadProject());
        }

        [TestMethod]
        public void LoadProjectTest_OpenFileDialogOk_ProjectReturned()
        {
            _testDialogMoq.Setup(f => f.ShowDialog()).Returns(DialogResult.OK);
            _projectLoadSaveMoq.Setup(s => s.Open(It.IsAny<string>())).Returns(_projectMoq.Object);
            Assert.IsNotNull(_projectFileService.LoadProject());
        }
    }
}
