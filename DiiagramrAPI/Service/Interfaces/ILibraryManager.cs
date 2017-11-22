using System.Collections.ObjectModel;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface ILibraryManager
    {
        ObservableCollection<string> Sources { get; set; }
        ObservableCollection<string> InstalledLibraryNames { get; set; }
        ObservableCollection<LibraryNameToPath> LibraryNameToPathMap { get; set; }

        bool AddSource(string sourceUrl);

        bool RemoveSource(string sourceUrl);

        bool InstallLibrary(string libraryName, string libraryVersion);
    }
}