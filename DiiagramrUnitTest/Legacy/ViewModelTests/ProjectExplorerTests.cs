using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Diiagramr.Service;
using Diiagramr.ViewModel;
using Moq;

namespace ColorOrgan5UnitTests.ViewModelTests
{
    [TestClass]
    public class ProjectExplorerTests
    {
        private Mock<IDirectoryService> _directoryServiceMoq;
        private ProjectFileService _projectFileService;
        private DiagramWellViewModel diagramWell;
        private const string Directory = "test";
        private const string OldDirectory = "test\\oldproject";
        private const string NewDirectory = "test\\newproject";
        private const string OldName = "oldproject";
        private const string NewName = "newproject";

        [TestInitialize]
        public void TestInitialize()
        {
            _directoryServiceMoq = new Mock<IDirectoryService>();
            _directoryServiceMoq.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
            _directoryServiceMoq.Setup(m => m.GetCurrentDirectory()).Returns("testDirectory");
            _projectFileService = new ProjectFileService(_directoryServiceMoq.Object);
            _projectFileService.ProjectDirectory = Directory;

        }

        [TestMethod]
        public void TestChangeProjectName()
        {
            Assert.IsTrue(RenameProject(NewName));
        }
    }
}
