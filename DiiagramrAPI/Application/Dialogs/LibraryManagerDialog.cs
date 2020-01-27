using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Service.Plugins;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Application.Dialogs
{
    public class LibraryManagerDialog : Dialog
    {
        private readonly LibrarySourceManagerDialog _librarySourceManagerDialog;

        public LibraryManagerDialog(Func<ILibraryManager> libraryManagerFactory, Func<LibrarySourceManagerDialog> librarySourceManagerDialogFactory)
        {
            LibraryManager = libraryManagerFactory();
            _librarySourceManagerDialog = librarySourceManagerDialogFactory();

            CommandBarCommands.Add(new DialogCommandBarCommand("Open Plugin Directory", OpenPluginsDirectory));
            CommandBarCommands.Add(new DialogCommandBarCommand("Manage Plugin Sources", ViewSources));
        }

        public bool IsRestartRequired { get; set; }

        public ILibraryManager LibraryManager { get; set; }

        public override int MaxHeight => 350;

        public override int MaxWidth => 530;

        public override string Title { get; set; } = "Library Manager";

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
            OpenOtherDialog(_librarySourceManagerDialog);
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