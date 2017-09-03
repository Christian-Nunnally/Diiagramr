using Castle.Core.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Diiagramr.Service;

namespace ColorOrgan5UnitTests.ServiceTests
{
    [TestClass]
    public class ProjectFileServiceTests
    {
        private const string InvalidProjectName = "+";
        private const string ValidProjectName = "a";
        private Mock<IDirectoryService> _directoryServiceMoq;
        private ProjectFileService _projectFileService;

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
        public void TestConstructorCreatesProjectDirectory()
        {
            Assert.IsFalse(_projectFileService.ProjectDirectory.IsNullOrEmpty());
            _directoryServiceMoq.Verify(m => m.CreateDirectory(It.IsAny<string>()));
        }

        [TestMethod]
        public void TestCreateProjectWithInvalidNameDoesNotCreateProject()
        {
            _directoryServiceMoq.ResetCalls();
            Assert.IsNull(_projectFileService.CreateProject(InvalidProjectName));
            _directoryServiceMoq.Verify(m => m.CreateDirectory(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void TestCreateProjectWithValidNameCreatesNewProject()
        {
            Assert.IsNotNull(_projectFileService.CreateProject(ValidProjectName));
            _directoryServiceMoq.Verify(m => m.CreateDirectory(It.IsAny<string>()));
        }

        [TestMethod]
        public void TestIsProjectNameValidWithInvalidNames()
        {
            Assert.IsFalse(_projectFileService.IsProjectNameValid(""));
            Assert.IsFalse(_projectFileService.IsProjectNameValid(" "));
            Assert.IsFalse(_projectFileService.IsProjectNameValid("!"));
            Assert.IsFalse(_projectFileService.IsProjectNameValid("a!"));
            Assert.IsFalse(_projectFileService.IsProjectNameValid("a "));
            Assert.IsFalse(_projectFileService.IsProjectNameValid(" a"));
            Assert.IsFalse(_projectFileService.IsProjectNameValid(InvalidProjectName));
        }

        [TestMethod]
        public void TestIsProjectNameValidWithValidNames()
        {
            Assert.IsTrue(_projectFileService.IsProjectNameValid("a"));
            Assert.IsTrue(_projectFileService.IsProjectNameValid("1"));
            Assert.IsTrue(_projectFileService.IsProjectNameValid("a1"));
            Assert.IsTrue(_projectFileService.IsProjectNameValid("1a"));
            Assert.IsTrue(_projectFileService.IsProjectNameValid("a a"));
            Assert.IsTrue(_projectFileService.IsProjectNameValid(ValidProjectName));
        }

        [TestMethod]
        public void TestGetSavedProjectNames()
        {
            _directoryServiceMoq.Setup(m => m.GetDirectories(It.IsAny<string>())).Returns(new List<string> { "a", "b", "c" });
            var savedProjectNames = _projectFileService.GetSavedProjectNames();
            Assert.IsTrue(savedProjectNames.Contains("a"));
            Assert.IsTrue(savedProjectNames.Contains("b"));
            Assert.IsTrue(savedProjectNames.Contains("c"));
            Assert.IsFalse(savedProjectNames.Contains("d"));
        }

        private const string Directory = "test";
        private const string OldDirectory = "test\\oldproject";
        private const string NewDirectory = "test\\newproject";
        private const string OldName = "oldproject";
        private const string NewName = "newproject";

        [TestMethod]
        public void TestMoveProjectProjectDoesNotExistReturnsFalse()
        {
            _directoryServiceMoq.Setup(m => m.Exists(OldDirectory)).Returns(false);
            Assert.IsFalse(_projectFileService.MoveProject(OldName, NewName));
            _directoryServiceMoq.Verify(m => m.Move(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _directoryServiceMoq.Verify(m => m.Delete(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void TestMoveProjectNewProjectNameNotValidReturnsFalse()
        {
            _directoryServiceMoq.Setup(m => m.Exists(NewDirectory)).Returns(true);
            Assert.IsFalse(_projectFileService.MoveProject(OldName, NewName));
            _directoryServiceMoq.Verify(m => m.Move(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _directoryServiceMoq.Verify(m => m.Delete(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void TestMoveProjectValidProjectSourceAndDestinationReturnsTrue()
        {
            _directoryServiceMoq.Setup(m => m.Exists(OldDirectory)).Returns(true);
            _directoryServiceMoq.Setup(m => m.Exists(NewDirectory)).Returns(false);
            Assert.IsTrue(_projectFileService.MoveProject(OldName, NewName));
            _directoryServiceMoq.Verify(m => m.Move(OldDirectory, NewDirectory));
            _directoryServiceMoq.Verify(m => m.Delete(OldDirectory, true));
        }
    }
}
