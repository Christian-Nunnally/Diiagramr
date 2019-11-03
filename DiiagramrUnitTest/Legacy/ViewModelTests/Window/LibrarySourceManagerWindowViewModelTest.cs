using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Service.Plugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.Legacy.ViewModelTests.Window
{
    [TestClass]
    public class LibrarySourceManagerWindowViewModelTest
    {
        private Mock<ILibraryManager> _libraryManagerMoq;
        private LibrarySourceManagerWindow _librarySourceManagerWindowViewModel;

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
            _librarySourceManagerWindowViewModel = new LibrarySourceManagerWindow(() => _libraryManagerMoq.Object);
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