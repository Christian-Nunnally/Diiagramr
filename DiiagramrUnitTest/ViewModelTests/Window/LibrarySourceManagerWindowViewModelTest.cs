using DiiagramrAPI.Service.Plugins;
using DiiagramrAPI.Shell.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ViewModelTests.Window
{
    [TestClass]
    public class LibrarySourceManagerWindowViewModelTest
    {
        private Mock<ILibraryManager> _libraryManagerMoq;
        private LibrarySourceManagerWindowViewModel _librarySourceManagerWindowViewModel;

        [TestMethod]
        public void TestAddSource_NonEmptySource_AddsSource()
        {
            _librarySourceManagerWindowViewModel.SourceTextBoxText = "test";
            _librarySourceManagerWindowViewModel.AddSource();
            _libraryManagerMoq.Verify(l => l.AddSource("test"));
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _libraryManagerMoq = new Mock<ILibraryManager>();
            _librarySourceManagerWindowViewModel = new LibrarySourceManagerWindowViewModel(() => _libraryManagerMoq.Object);
        }

        [TestMethod]
        public void TestProperties_GetsResonableResults()
        {
            Assert.IsTrue(_librarySourceManagerWindowViewModel.MaxWidth > 10);
            Assert.IsTrue(_librarySourceManagerWindowViewModel.MaxWidth < 10000);
            Assert.IsTrue(_librarySourceManagerWindowViewModel.MaxHeight > 10);
            Assert.IsTrue(_librarySourceManagerWindowViewModel.MaxHeight < 10000);
            Assert.IsFalse(string.IsNullOrEmpty(_librarySourceManagerWindowViewModel.Title));
        }
    }
}