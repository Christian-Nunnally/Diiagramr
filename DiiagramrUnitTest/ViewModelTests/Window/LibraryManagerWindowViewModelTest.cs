using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace DiiagramrUnitTests.ViewModelTests.Window
{
    [TestClass]
    public class LibraryManagerWindowViewModelTest
    {
        private Mock<ILibraryManager> _libraryManagerMoq;
        private Mock<LibrarySourceManagerWindowViewModel> _librarySourceManagerMoq;
        private LibraryManagerWindowViewModel _libraryManagerWindowViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _libraryManagerMoq = new Mock<ILibraryManager>();
            _librarySourceManagerMoq = new Mock<LibrarySourceManagerWindowViewModel>(
                (Func<ILibraryManager>)(() => _libraryManagerMoq.Object));
            _libraryManagerWindowViewModel = new LibraryManagerWindowViewModel(() => _libraryManagerMoq.Object, () => _librarySourceManagerMoq.Object);
        }

        [TestMethod]
        public void TestViewSources_InvokesEventToOpenSourcesManagerWindow()
        {
            AbstractShellWindow openedWindow = null;
            _libraryManagerWindowViewModel.OpenWindow += w => openedWindow = w;

            _libraryManagerWindowViewModel.ViewSources();

            Assert.IsTrue(openedWindow is LibrarySourceManagerWindowViewModel);
        }

        [TestMethod]
        public void TestInstallLibrary_SelectedLibraryNullOrEmpty_DoesNothing()
        {
            Assert.IsTrue(string.IsNullOrEmpty(_libraryManagerWindowViewModel.SelectedLibrary));
            _libraryManagerWindowViewModel.InstallSelectedLibrary().Wait();
            _libraryManagerMoq.Verify(m => m.InstallLatestVersionOfLibraryAsync(It.IsAny<NodeLibrary>()), Times.Never);
        }

        [TestMethod]
        public void TestInstallLibrary_InvalidFormat_DoesNothing()
        {
            _libraryManagerWindowViewModel.SelectedLibrary = "a 1.0v";
            _libraryManagerWindowViewModel.InstallSelectedLibrary().Wait();
            _libraryManagerMoq.Verify(m => m.InstallLatestVersionOfLibraryAsync(It.IsAny<NodeLibrary>()), Times.Never);
        }

        [TestMethod]
        public void TestInstallLibrary_ValidFormat_ParsesSelectedLibraryAndCallsInstallLibrary()
        {
            _libraryManagerWindowViewModel.SelectedLibrary = "a - 1.0";
            _libraryManagerWindowViewModel.InstallSelectedLibrary().Wait();
            _libraryManagerMoq.Verify(m => m.InstallLatestVersionOfLibraryAsync(It.IsAny<NodeLibrary>()), Times.Once);
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
    }
}
