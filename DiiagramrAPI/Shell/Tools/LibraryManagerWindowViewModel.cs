using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.Service.Interfaces;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiiagramrAPI.Shell.Tools
{
    public class LibraryManagerWindowViewModel : AbstractShellWindow
    {
        private readonly LibrarySourceManagerWindowViewModel _librarySourceManagerViewModel;

        public LibraryManagerWindowViewModel(Func<ILibraryManager> libraryManagerFactory, Func<LibrarySourceManagerWindowViewModel> librarySourceManagerWindowViewModelFactory)
        {
            LibraryManager = libraryManagerFactory.Invoke();
            _librarySourceManagerViewModel = librarySourceManagerWindowViewModelFactory.Invoke();
        }

        public ILibraryManager LibraryManager { get; set; }
        public override int MaxHeight => 400;
        public override int MaxWidth => 400;
        public override string Title => "Library Manager";

        public async void InstallPressed(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                if (border.DataContext is NodeLibrary library)
                {
                    await LibraryManager.InstallLatestVersionOfLibraryAsync(library);
                }
            }
        }

        public void ViewSources()
        {
            OpenOtherWindow(_librarySourceManagerViewModel);
        }

        public void OpenPluginsDirectory()
        {
            System.Diagnostics.Process.Start("explorer.exe", "Plugins");
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            LibraryManager.LoadSourcesAsync();
        }
    }
}
