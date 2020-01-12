using DiiagramrAPI.Application.Tools;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DiiagramrAPI.Service.Plugins
{
    public interface ILibraryManager : IService
    {
        ObservableCollection<NodeLibrary> AvailableLibraries { get; }

        ObservableCollection<LibraryListItem> AvailableLibraryItems { get; }

        ObservableCollection<LibraryListItem> InstalledLibraryItems { get; }

        ObservableCollection<string> Sources { get; }

        bool AddSource(string sourceUrl);

        IEnumerable<Type> GetSerializeableTypes();

        Task<bool> InstallLatestVersionOfLibraryAsync(LibraryListItem libraryListItem);

        Task LoadSourcesAsync();

        bool RemoveSource(string sourceUrl);

        void UninstallLibrary(LibraryListItem libraryListItem);
    }
}