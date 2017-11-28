﻿using System.Collections.ObjectModel;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface ILibraryManager
    {
        ObservableCollection<string> Sources { get; }
        ObservableCollection<string> InstalledLibraryNames { get; }
        ObservableCollection<LibraryNameToPath> LibraryNameToPathMap { get; }

        bool AddSource(string sourceUrl);

        bool RemoveSource(string sourceUrl);

        bool InstallLibrary(string libraryName, string libraryVersion);
    }
}