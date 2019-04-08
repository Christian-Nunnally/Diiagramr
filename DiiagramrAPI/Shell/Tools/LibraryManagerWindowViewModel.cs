using DiiagramrAPI.Service.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows;
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

        public bool IsRestartRequired { get; set; }
        public ILibraryManager LibraryManager { get; set; }
        public override int MaxHeight => 400;
        public override int MaxWidth => 550;
        public override string Title => "Library Manager";

        public async void InstallPressed(object sender, MouseEventArgs e)
        {
            var libraryListItem = GetLibraryListItemFromSender(sender);
            if (libraryListItem == null)
            {
                return;
            }
            await InstallLibraryFromListItemAsync(libraryListItem);
        }

        public void OpenPluginsDirectory()
        {
            System.Diagnostics.Process.Start("explorer.exe", "Plugins");
        }

        public void UninstallPressed(object sender, MouseEventArgs e)
        {
            var libraryListItem = GetLibraryListItemFromSender(sender);
            if (libraryListItem == null)
            {
                return;
            }
            UninstallLibraryFromListItem(libraryListItem);
        }

        public void ViewSources()
        {
            OpenOtherWindow(_librarySourceManagerViewModel);
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            LibraryManager.LoadSourcesAsync();
        }

        private LibraryListItem GetLibraryListItemFromSender(object sender)
        {
            if (sender is FrameworkElement frameworkElement)
            {
                return frameworkElement.DataContext as LibraryListItem;
            }
            return null;
        }

        private async Task InstallLibraryFromListItemAsync(LibraryListItem libraryListItem)
        {
            if (libraryListItem.ButtonText != "Install")
            {
                return;
            }
            await LibraryManager.InstallLatestVersionOfLibraryAsync(libraryListItem);
            IsRestartRequired = true;
        }

        private void UninstallLibraryFromListItem(LibraryListItem libraryListItem)
        {
            if (libraryListItem.ButtonText != "Uninstall")
            {
                return;
            }
            LibraryManager.UninstallLibrary(libraryListItem);
            IsRestartRequired = true;
        }
    }
}
