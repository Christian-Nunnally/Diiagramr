using DiiagramrAPI.Service.Plugins;
using DiiagramrAPI.Shell;
using DiiagramrAPI.Shell.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace DiiagramrUnitTests.ViewModelTests.Window
{
    [TestClass]
    public class LibraryManagerWindowViewModelTest
    {
        private Mock<ILibraryManager> _libraryManagerMoq;
        private LibraryManagerWindowViewModel _libraryManagerWindowViewModel;
        private Mock<LibrarySourceManagerWindowViewModel> _librarySourceManagerMoq;

        [TestInitialize]
        public void TestInitialize()
        {
            _libraryManagerMoq = new Mock<ILibraryManager>();
            _librarySourceManagerMoq = new Mock<LibrarySourceManagerWindowViewModel>(
                (Func<ILibraryManager>)(() => _libraryManagerMoq.Object));
            _libraryManagerWindowViewModel = new LibraryManagerWindowViewModel(() => _libraryManagerMoq.Object, () => _librarySourceManagerMoq.Object);
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

            Assert.IsTrue(openedWindow is LibrarySourceManagerWindowViewModel);
        }
    }
}