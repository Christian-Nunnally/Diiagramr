using DiiagramrAPI.Application;
using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Service.Plugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace DiiagramrUnitTests.ViewModelTests.Window
{
    [TestClass]
    public class LibraryManagerWindowViewModelTest
    {
        private Mock<ILibraryManager> _libraryManagerMoq;
        private LibraryManagerWindow _libraryManagerWindowViewModel;
        private Mock<LibrarySourceManagerWindow> _librarySourceManagerMoq;

        [TestInitialize]
        public void TestInitialize()
        {
            _libraryManagerMoq = new Mock<ILibraryManager>();
            _librarySourceManagerMoq = new Mock<LibrarySourceManagerWindow>(
                (Func<ILibraryManager>)(() => _libraryManagerMoq.Object));
            _libraryManagerWindowViewModel = new LibraryManagerWindow(() => _libraryManagerMoq.Object, () => _librarySourceManagerMoq.Object);
        }

        [TestMethod]
        public void TestProperties_GetsResonableResults()
        {
            Assert.IsTrue(_libraryManagerWindowViewModel.MaxWidth > 10);
            Assert.IsTrue(_libraryManagerWindowViewModel.MaxWidth < 10000);
            Assert.IsTrue(_libraryManagerWindowViewModel.MaxHeight > 10);
            Assert.IsTrue(_libraryManagerWindowViewModel.MaxHeight < 10000);
            Assert.IsFalse(string.IsNullOrEmpty(_libraryManagerWindowViewModel.Title));
        }

        [TestMethod]
        public void TestViewSources_InvokesEventToOpenSourcesManagerWindow()
        {
            AbstractShellWindow openedWindow = null;
            _libraryManagerWindowViewModel.OpenWindow += w => openedWindow = w;

            _libraryManagerWindowViewModel.ViewSources();

            Assert.IsTrue(openedWindow is LibrarySourceManagerWindow);
        }
    }
}