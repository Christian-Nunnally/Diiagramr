using System.Windows.Forms;
using Castle.Core.Internal;
using Diiagramr.Model;
using Diiagramr.Service;
using Diiagramr.Service.Interfaces;
using Diiagramr.View.CustomControls;
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
        private Mock<IFileDialog> _testDialogMoq;


        [TestInitialize]
        public void TestInitialize()
        {
            _testDialogMoq = new Mock<IFileDialog>();
            _testDialogMoq.Setup(d => d.ShowDialog()).Returns(DialogResult.Cancel);
            _testDialogMoq.SetupAllProperties();
            _directoryServiceMoq = new Mock<IDirectoryService>();
            _directoryServiceMoq.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
            _directoryServiceMoq.Setup(m => m.GetCurrentDirectory()).Returns("testDirectory");
            _projectFileService =
                new ProjectFileService(_directoryServiceMoq.Object, _testDialogMoq.Object, _testDialogMoq.Object, new ProjectLoadSave());
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
            _projectFileService.SaveProject(new ProjectModel(), true);
            Assert.AreEqual(Directory, _testDialogMoq.Object.InitialDirectory);
        }

        [TestMethod]
        public void SaveAsProjectTest_SetsInitialFileName()
        {
            var proj = new ProjectModel
            {
                Name = "testProj"
            };
            _projectFileService.SaveProject(proj, true);
            Assert.AreEqual(proj.Name, _testDialogMoq.Object.FileName);
        }
    }
}
