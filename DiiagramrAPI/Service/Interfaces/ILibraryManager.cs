using DiiagramrAPI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface ILibraryManager : IDiiagramrService
    {
        ObservableCollection<string> Sources { get; }
        ObservableCollection<string> InstalledLibraryNames { get; }
        ObservableCollection<NodeLibrary> AvailableLibraries { get; }

        bool AddSource(string sourceUrl);

        bool RemoveSource(string sourceUrl);

        Task<bool> InstallLatestVersionOfLibraryAsync(NodeLibrary libraryDescription);

        Task LoadSourcesAsync();

        IEnumerable<Type> GetSerializeableTypes();
    }
}