using Castle.Core.Internal;
using Diiagramr.Model;
using Diiagramr.Service;
using Diiagramr.View.CustomControls;
using DiiagramrUnitTests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ServiceTests
{
    [TestClass]
    public class ProjectFileServiceTest
    {
        private Mock<IDirectoryService> _directoryServiceMoq;
        private ProjectFileService _projectFileService;
        private const string Directory = "test";
        private IFileDialog _testDialog;


        [TestInitialize]
        public void TestInitialize()
        {
            _testDialog = new TestFileDialog();
            _directoryServiceMoq = new Mock<IDirectoryService>();
            _directoryServiceMoq.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
            _directoryServiceMoq.Setup(m => m.GetCurrentDirectory()).Returns("testDirectory");
            _projectFileService =
                new ProjectFileService(_directoryServiceMoq.Object, _testDialog, _testDialog);
            _projectFileService.ProjectDirectory = Directory;
        }

        [TestMethod]
        public void ConstructorTest_CreatesProjectDirectory()
        {
            Assert.IsFalse(_projectFileService.ProjectDirectory.IsNullOrEmpty());
            _directoryServiceMoq.Verify(m => m.CreateDirectory(It.IsAny<string>()));
        }

        [TestMethod]
        public void SaveAsProjectTest_SetsInitialDirectory()
        {
            _projectFileService.SaveProject(new Project(), true);
            Assert.AreEqual(Directory, _testDialog.InitialDirectory);
        }

        [TestMethod]
        public void SaveAsProjectTest_SetsInitialFileName()
        {
            var proj = new Project
            {
                Name = "testProj"
            };
            _projectFileService.SaveProject(proj, true);
            Assert.AreEqual(proj.Name, _testDialog.FileName);
        }
    }
}
