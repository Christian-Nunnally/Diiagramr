using System;
using System.Linq;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
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
        private LibraryManager _libraryManager;

        [TestInitialize]
        public void TestInitialize()
        {
            _pluginLoaderMoq = new Mock<IPluginLoader>();
            _libraryManager = new LibraryManager(() => _pluginLoaderMoq.Object);
        }

        [TestMethod]
        public void TestConstructor_DefaultSourceAdded()
        {
            Assert.AreEqual("http://diiagramrlibraries.azurewebsites.net/nuget/Packages", _libraryManager.Sources.First());
        }

        [TestMethod]
        public void TestLibraryNameToPathToString_ReturnsLibraryName()
        {
            var libraryNameToPath = new LibraryNameToPath();
            libraryNameToPath.Name = "test";
            Assert.AreEqual("test", libraryNameToPath.ToString());
        }

        [TestMethod]
        public void TestAddSource_SourceDoesntStartWithHttp_ReturnsFalse()
        {
            Assert.IsFalse(_libraryManager.AddSource(""));
        }
    }
}
