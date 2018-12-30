using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DiiagramrAPI.Service
{
    public class LibraryManager : ILibraryManager
    {
        private const string PluginsDirectory = "Plugins\\";
        private readonly IDirectoryService _directoryService;
        private readonly IPluginLoader _pluginLoader;
        private readonly IFetchWebResource _webResourceFetcher;
        private bool _shouldSourcesBeLoaded = false;
        private readonly bool _sourcesLoaded = false;

        public LibraryManager(
            Func<IPluginLoader> pluginLoaderFactory,
            Func<IDirectoryService> directoryServiceFactory,
            Func<IFetchWebResource> webResourceFetcher)
        {
            _pluginLoader = pluginLoaderFactory.Invoke();
            _directoryService = directoryServiceFactory.Invoke();
            _webResourceFetcher = webResourceFetcher.Invoke();

            UpdateInstalledLibraries();
        }

        public ObservableCollection<string> Sources { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> InstalledLibraryNames { get; } = new ObservableCollection<string>();
        public ObservableCollection<NodeLibrary> AvailableLibraries { get; } = new ObservableCollection<NodeLibrary>();

        public bool AddSource(string sourceUrl)
        {
            if (!sourceUrl.StartsWith("http://"))
            {
                return false;
            }

            Sources.Add(sourceUrl);
            return true;
        }

        public IEnumerable<Type> GetSerializeableTypes()
        {
            return _pluginLoader.SerializeableTypes;
        }

        public async Task LoadSourcesAsync()
        {
            _shouldSourcesBeLoaded = true;
            if (_sourcesLoaded)
            {
                return;
            }
            foreach(var source in Sources)
            {
                await LoadSourceAsync(source);
            }
        }

        private async Task LoadSourceAsync(string sourceUrl)
        {
            if (!_shouldSourcesBeLoaded)
            {
                return;
            }

            var packagesString = await _webResourceFetcher.DownloadStringAsync(sourceUrl);
            if (packagesString == null)
            {
                return;
            }
            var libraryPaths = GetLibraryPathsFromPackagesXml(packagesString);
            AddToAvailableLibrariesFromPaths(libraryPaths);
        }

        public bool RemoveSource(string sourceUrl)
        {
            if (!Sources.Contains(sourceUrl))
            {
                return false;
            }

            Sources.Remove(sourceUrl);

            var packagesString = Task.Run(() => _webResourceFetcher.DownloadStringAsync(sourceUrl)).Result;
            var libraryPaths = GetLibraryPathsFromPackagesXml(packagesString);
            RemoveFromAvailableLibrariesFromPaths(libraryPaths);

            return true;
        }

        public async Task<bool> InstallLatestVersionOfLibraryAsync(NodeLibrary libraryDescription)
        {
            if (InstalledLibraryNames.Any(s => s == libraryDescription.ToString()))
            {
                return true;
            }

            await LoadSourcesAsync();

            if (!TryGetLibraryWithNameAndMajorVersion(libraryDescription, out var library))
            {
                return false;
            }

            var absPath = _directoryService.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var tmpDir = absPath + "\\tmp";

            if (!_directoryService.Exists(tmpDir))
            {
                _directoryService.CreateDirectory(tmpDir);
            }

            var zipPath = "tmp/" + library + ".zip";
            var extractPath = "tmp/" + library;
            var toPath = PluginsDirectory + library;

            Task.Run(() => _webResourceFetcher.DownloadFileAsync(library.DownloadPath, zipPath)).Wait();

            if (!_directoryService.Exists(toPath))
            {
                _directoryService.ExtractToDirectory(zipPath, extractPath);
                _directoryService.Move(extractPath, toPath);
                _directoryService.Delete(zipPath, false);
            }

            _pluginLoader.AddPluginFromDirectory(absPath + "/" + toPath, library);
            UpdateInstalledLibraries();
            return true;
        }

        private void RemoveFromAvailableLibrariesFromPaths(IEnumerable<string> libraryPaths)
        {
            foreach (var libraryPath in libraryPaths)
            {
                var library = CreateLibraryFromPath(libraryPath);
                if (TryGetLibraryWithNameAndMajorVersion(library, out var otherLibrary))
                {
                    AvailableLibraries.Remove(otherLibrary);
                }
            }
        }

        private void AddToAvailableLibrariesFromPaths(IEnumerable<string> libraryPaths)
        {
            foreach (var libraryPath in libraryPaths)
            {
                var library = CreateLibraryFromPath(libraryPath);
                AddLibraryToAvailableIfNewest(library);
            }
        }

        private void AddLibraryToAvailableIfNewest(NodeLibrary library)
        {
            if (TryGetLibraryWithNameAndMajorVersion(library, out var otherLibrary))
            {
                if (!library.IsNewerVersionThan(otherLibrary))
                {
                    return;
                }

                AvailableLibraries.Remove(otherLibrary);
                AvailableLibraries.Add(library);
            }
            else
            {
                AvailableLibraries.Add(library);
            }
        }

        private bool TryGetLibraryWithNameAndMajorVersion(NodeLibrary library, out NodeLibrary otherLibrary)
        {
            otherLibrary = null;
            if (library.Name == null)
            {
                return false;
            }

            bool SameName(NodeLibrary l) => l.Name == library.Name.ToLower();
            bool SameMajorVersion(NodeLibrary l) => l.MajorVersion == library.MajorVersion;
            otherLibrary = AvailableLibraries.FirstOrDefault(l => SameName(l) && SameMajorVersion(l));
            return otherLibrary != null;
        }

        private void UpdateInstalledLibraries()
        {
            InstalledLibraryNames.Clear();

            var directories = _directoryService.GetDirectories(PluginsDirectory);
            foreach (var directory in directories)
            {
                var directoryName = directory.Remove(0, PluginsDirectory.Length);
                InstalledLibraryNames.Add(directoryName);
            }
        }

        #region Static Helper Methods

        private static NodeLibrary CreateLibraryFromPath(string libraryPath)
        {
            var splitPath = libraryPath.Split('/');
            var libraryName = splitPath[splitPath.Length - 2];
            var libraryVersion = splitPath[splitPath.Length - 1];
            var splitVersion = libraryVersion.Split('.');
            var majorVersion = int.Parse(splitVersion[0]);
            var minorVersion = int.Parse(splitVersion[1]);
            var patch = int.Parse(splitVersion[2]);
            return new NodeLibrary(libraryName, libraryPath, majorVersion, minorVersion, patch);
        }

        private static IEnumerable<string> GetLibraryPathsFromPackagesXml(string packagesXml)
        {
            const string searchString = "{http://www.w3.org/2005/Atom}content";
            var xmlElement = XElement.Parse(packagesXml);
            return xmlElement.Descendants(searchString).Select(descendant => descendant.LastAttribute.Value).ToList();
        }

        #endregion
    }
}