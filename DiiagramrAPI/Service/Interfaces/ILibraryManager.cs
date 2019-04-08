using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.Shell.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface ILibraryManager : IService
    {
        ObservableCollection<NodeLibrary> AvailableLibraries { get; }
        ObservableCollection<LibraryListItem> InstalledLibraryItems { get; }
        ObservableCollection<LibraryListItem> AvailableLibraryItems { get; }
        ObservableCollection<string> Sources { get; }
        IEnumerable<Type> GetSerializeableTypes();

        Task<bool> InstallLatestVersionOfLibraryAsync(LibraryListItem libraryListItem);
        void UninstallLibrary(LibraryListItem libraryListItem);

        Task LoadSourcesAsync();
        bool AddSource(string sourceUrl);
        bool RemoveSource(string sourceUrl);
    }
}
