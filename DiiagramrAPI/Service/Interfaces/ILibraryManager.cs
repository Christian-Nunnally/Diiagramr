using DiiagramrAPI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface ILibraryManager : IService
    {
        ObservableCollection<NodeLibrary> AvailableLibraries { get; }
        ObservableCollection<string> InstalledLibraryNames { get; }
        ObservableCollection<string> Sources { get; }

        bool AddSource(string sourceUrl);

        IEnumerable<Type> GetSerializeableTypes();

        Task<bool> InstallLatestVersionOfLibraryAsync(NodeLibrary libraryDescription);

        Task LoadSourcesAsync();

        bool RemoveSource(string sourceUrl);
    }
}
