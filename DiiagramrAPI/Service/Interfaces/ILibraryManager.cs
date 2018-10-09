using DiiagramrAPI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface ILibraryManager
    {
        ObservableCollection<string> Sources { get; }
        ObservableCollection<string> InstalledLibraryNames { get; }
        ObservableCollection<NodeLibrary> AvailableLibraries { get; }

        bool AddSource(string sourceUrl);

        bool RemoveSource(string sourceUrl);

        bool InstallLatestVersionOfLibrary(NodeLibrary libraryDescription);

        void LoadSources();

        IEnumerable<Type> GetSerializeableTypes();
    }
}