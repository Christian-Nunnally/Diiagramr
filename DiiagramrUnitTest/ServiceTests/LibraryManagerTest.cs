using DiiagramrAPI.Diagram.Interoperability;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ServiceTests
{
    [TestClass]
    public class LibraryManagerTest
    {
        private Mock<IPluginLoader> _pluginLoaderMoq;
        private Mock<IDirectoryService> _directoryServiceMoq;
        private Mock<IFetchWebResource> _webResourceFetcherMoq;
        private LibraryManager _libraryManager;

        [TestInitialize]
        public void TestInitialize()
        {
            _pluginLoaderMoq = new Mock<IPluginLoader>();
            _directoryServiceMoq = new Mock<IDirectoryService>();
            _webResourceFetcherMoq = new Mock<IFetchWebResource>();
            _libraryManager = new LibraryManager(
                () => _pluginLoaderMoq.Object,
                () => _directoryServiceMoq.Object,
                () => _webResourceFetcherMoq.Object);
        }

        [TestMethod]
        public void TestNodeLibraryToString_ReturnsProperlyFormattedLibraryName()
        {
            var libraryNameToPath = new NodeLibrary("test", "", 1, 0, 0);
            Assert.AreEqual("test - 1.0.0", libraryNameToPath.ToString());
        }

        [TestMethod]
        public void TestAddSource_SourceDoesntStartWithHttp_ReturnsFalse()
        {
            Assert.IsFalse(_libraryManager.AddSource(""));
        }
    }
}
