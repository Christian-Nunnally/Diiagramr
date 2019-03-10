using DiiagramrAPI.Service.Interfaces;
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

        [TestInitialize]
        public void TestInitialize()
        {
            _libraryManagerMoq = new Mock<ILibraryManager>();
            _librarySourceManagerWindowViewModel = new LibrarySourceManagerWindowViewModel(() => _libraryManagerMoq.Object);
        }

        [TestMethod]
        public void TestAddSource_EmptySource_DoesNotAddSource()
        {
            _libraryManagerMoq.Invocations.Clear();
            _librarySourceManagerWindowViewModel.AddSource();
            _libraryManagerMoq.Verify(l => l.AddSource(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void TestAddSource_NonEmptySource_AddsSource()
        {
            _librarySourceManagerWindowViewModel.SourceTextBoxText = "test";
            _librarySourceManagerWindowViewModel.AddSource();
            _libraryManagerMoq.Verify(l => l.AddSource("test"));
        }

        [TestMethod]
        public void TestRemoveSelectedSource_NoSourceSelected_DoesNotRemoveSource()
        {
            _librarySourceManagerWindowViewModel.RemoveSelectedSource();
            _libraryManagerMoq.Verify(l => l.RemoveSource(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void TestRemoveSelected_SourceSelected_RemovesSelectedSource()
        {
            _librarySourceManagerWindowViewModel.SelectedSource = "test";
            _librarySourceManagerWindowViewModel.RemoveSelectedSource();
            _libraryManagerMoq.Verify(l => l.RemoveSource("test"));
        }

        [TestMethod]
        public void TestAddSource_NonEmptySource_ClearsSourceTextBox()
        {
            _librarySourceManagerWindowViewModel.SourceTextBoxText = "test";
            _librarySourceManagerWindowViewModel.AddSource();
            Assert.AreEqual("", _librarySourceManagerWindowViewModel.SourceTextBoxText);
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
