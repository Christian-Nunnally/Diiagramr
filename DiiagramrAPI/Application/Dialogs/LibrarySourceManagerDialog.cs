using DiiagramrAPI.Service.Plugins;
using System;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Application.Dialogs
{
    /// <summary>
    /// The dialog for managing library sources, which are just URL's pointing to nuget repositories.
    /// </summary>
    public class LibrarySourceManagerDialog : Dialog
    {
        /// <summary>
        /// Creates a new instance of <see cref="LibrarySourceManagerDialog"/>.
        /// </summary>
        /// <param name="libraryManagerFactory">A factory that creates an instance of <see cref="ILibraryManager"/>.</param>
        public LibrarySourceManagerDialog(Func<ILibraryManager> libraryManagerFactory)
        {
            LibraryManager = libraryManagerFactory.Invoke();
            LibraryManager.AddSource("http://diiagramrlibraries.azurewebsites.net/nuget/Packages");
        }

        /// <summary>
        /// The library manager that actaully manages the libraries.
        /// </summary>
        public ILibraryManager LibraryManager { get; }

        /// <inheritdoc/>
        public override int MaxHeight => 400;

        /// <inheritdoc/>
        public override int MaxWidth => 550;

        /// <summary>
        /// The currently selected source.
        /// </summary>
        public string SelectedSource { get; set; }

        /// <summary>
        /// The text in the source textbox that users that enter new sources in to.
        /// </summary>
        public string SourceTextBoxText { get; set; } = "http://";

        /// <inheritdoc/>
        public override string Title { get; set; } = "Library Source Manager";

        /// <summary>
        /// Adds a new source to the current <see cref="ILibraryManager"/>'s sources.
        /// </summary>
        public void AddSource()
        {
            if (string.IsNullOrEmpty(SourceTextBoxText))
            {
                return;
            }

            LibraryManager.AddSource(SourceTextBoxText);
            SourceTextBoxText = "http://";
        }

        /// <summary>
        /// Occurs when the user presses the remove button on a source list item.
        /// </summary>
        /// <param name="sender">The source list item that was clicked.</param>
        /// <param name="e">The event args.</param>
        public void RemoveSource(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement frameworkElement && frameworkElement.DataContext is string source)
            {
                LibraryManager.RemoveSource(source);
            }
        }
    }
}