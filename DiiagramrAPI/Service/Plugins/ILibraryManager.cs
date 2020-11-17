using DiiagramrAPI.Application.Tools;
using DiiagramrModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DiiagramrAPI.Service.Plugins
{
    /// <summary>
    /// Interface for managing libraries of nodes.
    /// </summary>
    public interface ILibraryManager : ISingletonService
    {
        /// <summary>
        /// Gets the list of libraries that are available.
        /// </summary>
        ObservableCollection<NodeLibrary> AvailableLibraries { get; }

        /// <summary>
        /// Gets the list of library view model items that are available.
        /// </summary>
        ObservableCollection<LibraryListItem> AvailableLibraryItems { get; }

        /// <summary>
        /// Gets the list of installed library view model items.
        /// </summary>
        ObservableCollection<LibraryListItem> InstalledLibraryItems { get; }

        /// <summary>
        /// Gets the list of sources that libraries can be downloaded from.
        /// </summary>
        ObservableCollection<string> Sources { get; }

        /// <summary>
        /// Add a source that libraries can be downloaded from.
        /// </summary>
        /// <param name="sourceUrl">The url to the plugin source.</param>
        /// <returns>True if the new source was added.</returns>
        bool AddSource(string sourceUrl);

        /// <summary>
        /// Install the latest available version of the given library list item.
        /// </summary>
        /// <param name="libraryListItem">The library to download.</param>
        /// <returns>True if the library was downloaded.</returns>
        Task<bool> InstallLatestVersionOfLibraryAsync(LibraryListItem libraryListItem);

        /// <summary>
        /// Load libraries from sources.
        /// </summary>
        /// <returns>A task representing the work.</returns>
        Task LoadSourcesAsync();

        /// <summary>
        /// Remove a source from the list of available sources.
        /// </summary>
        /// <param name="sourceUrl">The source to remove.</param>
        /// <returns>True if the source was removed.</returns>
        bool RemoveSource(string sourceUrl);

        /// <summary>
        /// Uninstall the given library.
        /// </summary>
        /// <param name="libraryListItem">The library to uninstall.</param>
        void UninstallLibrary(LibraryListItem libraryListItem);
    }
}