using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Service.IO;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DiiagramrAPI.Service.Plugins
{
    public class LibraryManager : ILibraryManager
    {
        private const string PluginsDirectory = "Plugins\\";
        private readonly IDirectoryService _directoryService;
        private readonly IPluginLoader _pluginLoader;
        private readonly bool _sourcesLoaded = false;
        private readonly IFetchWebResource _webResourceFetcher;
        private bool _shouldSourcesBeLoaded = false;

        public LibraryManager(
            Func<IPluginLoader> pluginLoaderFactory,
            Func<IDirectoryService> directoryServiceFactory,
            Func<IFetchWebResource> webResourceFetcher)
        {
            _pluginLoader = pluginLoaderFactory();
            _directoryService = directoryServiceFactory();
            _webResourceFetcher = webResourceFetcher();

            UpdateInstalledLibraries();
        }

        public ObservableCollection<NodeLibrary> AvailableLibraries { get; } = new ObservableCollection<NodeLibrary>();

        public ObservableCollection<LibraryListItem> AvailableLibraryItems { get; } = new ObservableCollection<LibraryListItem>();

        public ObservableCollection<LibraryListItem> InstalledLibraryItems { get; } = new ObservableCollection<LibraryListItem>();

        public ObservableCollection<string> Sources { get; } = new ObservableCollection<string>();

        public bool AddSource(string sourceUrl)
        {
            if (!sourceUrl.StartsWith("http://"))
            {
                return false;
            }

            Sources.Add(sourceUrl);
            return true;
        }

        public async Task<bool> InstallLatestVersionOfLibraryAsync(LibraryListItem libraryListItem)
        {
            if (IsLibraryInstalled(libraryListItem.Library))
            {
                return true;
            }

            libraryListItem.ButtonText = "Installing";
            await LoadSourcesAsync();

            if (!TryGetLibraryWithNameAndVersion(libraryListItem.Library, out var library))
            {
                return false;
            }

            var absPath = _directoryService.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            CreateTemporaryDirectory(absPath);

            var zipPath = "tmp/" + library + ".zip";
            var extractPath = "tmp/" + library;
            var toPath = PluginsDirectory + library;

            await _webResourceFetcher.DownloadFileAsync(library.DownloadPath, zipPath);
            ExtractDllFromZip(zipPath, extractPath, toPath);
            library.PathOnDisk = toPath;
            _pluginLoader.AddPluginFromDirectory(absPath + "/" + toPath, library);
            UpdateInstalledLibraries();
            InstalledLibraryItems.Add(new LibraryListItem(library) { ButtonText = "Uninstall" });
            libraryListItem.ButtonText = "Installed";
            return true;
        }

        public async Task LoadSourcesAsync()
        {
            _shouldSourcesBeLoaded = true;
            if (_sourcesLoaded)
            {
                return;
            }

            foreach (var source in Sources)
            {
                await LoadSourceAsync(source);
            }
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
            RemoveAvailableLibrariesFromPaths(libraryPaths);

            return true;
        }

        public void UninstallLibrary(LibraryListItem libraryListItem)
        {
            if (libraryListItem.Library != null)
            {
                if (TryGetLibraryWithNameAndVersion(libraryListItem.Library, out NodeLibrary library))
                {
                    if (!string.IsNullOrEmpty(library.PathOnDisk))
                    {
                        _directoryService.Delete(library.PathOnDisk, true);
                        InstalledLibraryItems.Remove(libraryListItem);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(libraryListItem?.Library?.PathOnDisk))
                    {
                        _directoryService.Delete(libraryListItem.Library.PathOnDisk, true);
                        InstalledLibraryItems.Remove(libraryListItem);
                    }
                }
            }
        }

        private static NodeLibrary CreateLibraryFromPath(string libraryPath)
        {
            var splitPath = libraryPath.Split('/');
            var libraryName = splitPath[^2];
            var libraryVersion = splitPath[^1];
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

        private void AddLibraryToAvailableIfNewest(NodeLibrary library)
        {
            if (TryGetLibraryWithNameAndVersion(library, out var otherLibrary))
            {
                if (!library.IsNewerVersionThan(otherLibrary))
                {
                    return;
                }

                AvailableLibraries.Remove(otherLibrary);
                AvailableLibraryItems.Remove(AvailableLibraryItems.FirstOrDefault(x => x.Library == otherLibrary));
                AvailableLibraries.Add(library);
                AvailableLibraryItems.Add(new LibraryListItem(library));
            }
            else
            {
                AvailableLibraries.Add(library);
                AvailableLibraryItems.Add(new LibraryListItem(library));
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

        private void CreateTemporaryDirectory(string absPath)
        {
            var tmpDir = absPath + "\\tmp";

            if (!_directoryService.Exists(tmpDir))
            {
                _directoryService.CreateDirectory(tmpDir);
            }
        }

        private void ExtractDllFromZip(string zipPath, string extractPath, string toPath)
        {
            if (!_directoryService.Exists(toPath))
            {
                _directoryService.ExtractToDirectory(zipPath, extractPath);
                _directoryService.Move(extractPath, toPath);
                _directoryService.Delete(zipPath, false);
            }
        }

        private bool IsLibraryInstalled(NodeLibrary libraryDescription)
        {
            return InstalledLibraryItems.Any(i => i.LibraryDisplayName == libraryDescription.ToString());
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

        private void RemoveAvailableLibrariesFromPaths(IEnumerable<string> libraryPaths)
        {
            foreach (var libraryPath in libraryPaths)
            {
                var library = CreateLibraryFromPath(libraryPath);
                while (TryGetLibraryWithNameAndVersion(library, out var otherLibrary))
                {
                    AvailableLibraries.Remove(otherLibrary);
                    AvailableLibraryItems.Remove(AvailableLibraryItems.FirstOrDefault(x => x.Library == otherLibrary));
                }
            }
        }

        private bool TryGetLibraryWithNameAndVersion(NodeLibrary library, out NodeLibrary otherLibrary)
        {
            otherLibrary = null;
            if (library.Name == null)
            {
                return false;
            }

            bool SameName(NodeLibrary l) => l.Name == library.Name.ToLower();
            bool SameMajorVersion(NodeLibrary l) => l.MajorVersion == library.MajorVersion;
            bool SameMinorVersion(NodeLibrary l) => l.MinorVersion == library.MinorVersion;
            bool SamePatch(NodeLibrary l) => l.Patch == library.Patch;
            otherLibrary = AvailableLibraries.FirstOrDefault(l =>
                   SameName(l)
                && SameMajorVersion(l)
                && SameMinorVersion(l)
                && SamePatch(l));
            return otherLibrary != null;
        }

        private void UpdateInstalledLibraries()
        {
            InstalledLibraryItems.Clear();

            var directories = _directoryService.GetDirectories(PluginsDirectory);
            foreach (var directory in directories)
            {
                var directoryName = directory.Remove(0, PluginsDirectory.Length);
                var matchingLibrary = AvailableLibraries.FirstOrDefault(x => x.PathOnDisk == directoryName);
                var libraryListItem = new LibraryListItem(new NodeLibrary() { PathOnDisk = directory, Name = directoryName }) { ButtonText = "Uninstall" };
                InstalledLibraryItems.Add(libraryListItem);
            }
        }
    }
}