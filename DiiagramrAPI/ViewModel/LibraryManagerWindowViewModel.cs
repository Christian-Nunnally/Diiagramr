using System;
using System.Threading.Tasks;
using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;

namespace DiiagramrAPI.ViewModel
{
    public class LibraryManagerWindowViewModel : AbstractShellWindow
    {
        private readonly LibrarySourceManagerWindowViewModel _librarySourceManagerViewModel;
        public ILibraryManager LibraryManager { get; set; }

        public LibraryManagerWindowViewModel(Func<ILibraryManager> libraryManagerFactory, Func<LibrarySourceManagerWindowViewModel> librarySourceManagerWindowViewModelFactory)
        {
            LibraryManager = libraryManagerFactory.Invoke();
            _librarySourceManagerViewModel = librarySourceManagerWindowViewModelFactory.Invoke();
        }

        public string SelectedLibrary { get; set; }

        public override int MaxWidth => 400;
        public override int MaxHeight => 400;
        public override string Title => "Library Manager";

        public void ViewSources()
        {
            OpenOtherWindow(_librarySourceManagerViewModel);
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            LibraryManager.LoadSourcesAsync();
        }

        public async Task InstallSelectedLibrary()
        {
            if (string.IsNullOrEmpty(SelectedLibrary)) return;
            if (SelectedLibrary.Split(' ').Length != 3) return;
            var selectedLibraryMajorVersion = int.Parse(SelectedLibrary.Split(' ')[2].Substring(0, 1));
            var selectedLibraryName = SelectedLibrary.Split(' ')[0];
            var selectedLibrary = new NodeLibrary(selectedLibraryName, "", selectedLibraryMajorVersion, 0, 0);
            await LibraryManager.InstallLatestVersionOfLibraryAsync(selectedLibrary);
        }
    }
}