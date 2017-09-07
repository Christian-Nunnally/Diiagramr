using Castle.Core.Internal;
using Diiagramr.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ServiceTests
{
    [TestClass]
    public class ProjectFileServiceTests
    {
        private const string InvalidProjectName = "+";
        private const string ValidProjectName = "a";
        private Mock<IDirectoryService> _directoryServiceMoq;
        private ProjectFileService _projectFileService;
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
        public void ConstructorTest_CreatesProjectDirectory()
        {
            Assert.IsFalse(_projectFileService.ProjectDirectory.IsNullOrEmpty());
            _directoryServiceMoq.Verify(m => m.CreateDirectory(It.IsAny<string>()));
        }

        [TestMethod]
        public void ConstructorTest_ProjectNameNull()
        {
            Assert.IsNull(_projectFileService.ProjectName);
        }
    }
}
