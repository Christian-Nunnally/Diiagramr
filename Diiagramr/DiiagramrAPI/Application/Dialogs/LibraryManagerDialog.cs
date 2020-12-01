using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Service.Plugins;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Application.Dialogs
{
    /// <summary>
    /// The dialog for managing libraries.
    /// </summary>
    public class LibraryManagerDialog : Dialog
    {
        private readonly LibrarySourceManagerDialog _librarySourceManagerDialog;

        /// <summary>
        /// Creates a new insteace of <see cref="LibraryManagerDialog"/>.
        /// </summary>
        /// <param name="libraryManagerFactory">A factory that creates an <see cref="ILibraryManager"/> instance.</param>
        /// <param name="librarySourceManagerDialogFactory">A factory that creates an <see cref="LibrarySourceManagerDialog"/> instance.</param>
        public LibraryManagerDialog(Func<ILibraryManager> libraryManagerFactory, Func<LibrarySourceManagerDialog> librarySourceManagerDialogFactory)
        {
            LibraryManager = libraryManagerFactory();
            _librarySourceManagerDialog = librarySourceManagerDialogFactory();

            CommandBarCommands.Add(new DialogCommandBarCommand("Open Plugin Directory", OpenPluginsDirectory));
            CommandBarCommands.Add(new DialogCommandBarCommand("Manage Plugin Sources", ViewSources));
        }

        /// <summary>
        /// Gets or sets whether to show the user a label telling them that restarting the application is required.
        /// </summary>
        public bool IsRestartRequired { get; set; }

        /// <summary>
        /// The library manager instance that actually manages the libraries.
        /// </summary>
        public ILibraryManager LibraryManager { get; set; }

        /// <inheritdoc/>
        public override int MaxHeight => 350;

        /// <inheritdoc/>
        public override int MaxWidth => 530;

        /// <inheritdoc/>
        public override string Title { get; set; } = "Library Manager";

        /// <summary>
        /// Occurs when the user presses the install button for a library.
        /// </summary>
        /// <param name="sender">The library list item containing the install button.</param>
        /// <param name="e">The event arguments.</param>
        public async void InstallPressed(object sender, MouseEventArgs e)
        {
            var libraryListItem = GetLibraryListItemFromSender(sender);
            if (libraryListItem == null)
            {
                return;
            }

            await InstallLibraryFromListItemAsync(libraryListItem);
        }

        /// <summary>
        /// Occurs when the user presses the uninstall button for a library.
        /// </summary>
        /// <param name="sender">The library list item containing the uninstall button.</param>
        /// <param name="e">The event arguments.</param>
        public void UninstallPressed(object sender, MouseEventArgs e)
        {
            var libraryListItem = GetLibraryListItemFromSender(sender);
            if (libraryListItem == null)
            {
                return;
            }

            UninstallLibraryFromListItem(libraryListItem);
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            LibraryManager.LoadSourcesAsync();
        }

        private void OpenPluginsDirectory()
        {
            System.Diagnostics.Process.Start("explorer.exe", "Plugins");
        }

        private void ViewSources()
        {
            OpenDialog(_librarySourceManagerDialog);
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