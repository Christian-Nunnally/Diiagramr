using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;

namespace DiiagramrAPI.Service
{
    public class LibraryManager : ILibraryManager
    {
        private const string PluginsDirectory = "Plugins\\";
        private readonly IPluginLoader _pluginLoader;

        public LibraryManager(Func<IPluginLoader> pluginLoaderFactory)
        {
            _pluginLoader = pluginLoaderFactory.Invoke();

            AddSource("http://diiagramrlibraries.azurewebsites.net/nuget/Packages");
            UpdateInstalledLibraries();
        }

        public ObservableCollection<string> Sources { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> InstalledLibraryNames { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<LibraryNameToPath> LibraryNameToPathMap { get; set; } = new ObservableCollection<LibraryNameToPath>();

        public bool AddSource(string sourceUrl)
        {
            if (!sourceUrl.StartsWith("http://")) return false;
            Sources.Add(sourceUrl);
            Task.Run(() =>
            {
                var packagesString = DownloadPackagesStringFromSource(sourceUrl);
                var libraryPaths = GetLibraryPathsFromPackagesXml(packagesString);
                AddLibraryPathsToNameToPathMap(libraryPaths);
            });
            return true;
        }

        public bool RemoveSource(string sourceUrl)
        {
            if (!Sources.Contains(sourceUrl)) return false;
            Sources.Remove(sourceUrl);
            Task.Run(() =>
            {
                var packagesString = DownloadPackagesStringFromSource(sourceUrl);
                var libraryPaths = GetLibraryPathsFromPackagesXml(packagesString);
                RemoveLibraryPathsFromNameToPathMap(libraryPaths);
            });
            return true;
        }

        public bool InstallLibrary(string libraryName, string libraryVersion)
        {
            var formattedLibraryName = FormatLibraryName(libraryName, libraryVersion);
            if (!LibraryPathMapContains(formattedLibraryName)) return false;

            var absPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var tmpDir = absPath + "\\tmp";
            if (!Directory.Exists(tmpDir)) Directory.CreateDirectory(tmpDir);
            var zipPath = "tmp/" + formattedLibraryName + ".zip";
            var extractPath = "tmp/" + formattedLibraryName;
            var toPath = PluginsDirectory + formattedLibraryName;
            using (var client = new WebClient())
            {
                client.DownloadFile(LibraryPathMapGet(formattedLibraryName), zipPath);
            }

            try
            {
                if (!Directory.Exists(toPath))
                {
                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                    Directory.Move(extractPath, toPath);
                    File.Delete(zipPath);
                }
            }
            catch (IOException)
            {
                return false;
            }

            _pluginLoader.AddPluginFromDirectory(absPath + "/" + toPath, new DependencyModel(libraryName, libraryVersion));
            UpdateInstalledLibraries();
            return true;
        }

        private void RemoveLibraryPathsFromNameToPathMap(IEnumerable<string> libraryPaths)
        {
            foreach (var libraryPath in libraryPaths)
            {
                var libraryString = GetLibraryNameFromPath(libraryPath);
                if (LibraryPathMapContains(libraryString)) LibraryPathMapRemove(libraryString);
            }
        }

        private void AddLibraryPathsToNameToPathMap(IEnumerable<string> libraryPaths)
        {
            foreach (var libraryPath in libraryPaths)
            {
                var libraryString = GetLibraryNameFromPath(libraryPath);
                LibraryPathMapAdd(libraryString, libraryPath);
            }
        }

        private void LibraryPathMapAdd(string name, string path)
        {
            if (LibraryPathMapContains(name)) return;
            var libraryNameToPath = new LibraryNameToPath();
            libraryNameToPath.Name = name;
            libraryNameToPath.Path = path;
            LibraryNameToPathMap.Add(libraryNameToPath);
        }

        private bool LibraryPathMapContains(string name)
        {
            return LibraryNameToPathMap.Any(p => p.Name == name);
        }

        private string LibraryPathMapGet(string name)
        {
            return LibraryNameToPathMap.First(p => p.Name == name).Path;
        }

        private void LibraryPathMapRemove(string name)
        {
            if (!LibraryPathMapContains(name)) return;
            var itemToRemove = LibraryNameToPathMap.First(p => p.Name == name);
            LibraryNameToPathMap.Remove(itemToRemove);
        }

        private void UpdateInstalledLibraries()
        {
            InstalledLibraryNames.Clear();
            string[] directories;

            try
            {
                directories = Directory.GetDirectories(PluginsDirectory);
            }
            catch (IOException e)
            {
                Console.WriteLine("Can't find plugins directory!");
                Console.WriteLine(e);
                return;
            }

            foreach (var directory in directories)
            {
                var directoryName = directory.Remove(0, PluginsDirectory.Length);
                InstalledLibraryNames.Add(directoryName);
            }
        }

        #region Static Helper Methods

        private static string GetLibraryNameFromPath(string libraryPath)
        {
            var sl = libraryPath.Split('/');
            var libraryName = sl[sl.Length - 2];
            var libraryVersion = sl[sl.Length - 1];
            return FormatLibraryName(libraryName, libraryVersion);
        }

        private static string FormatLibraryName(string name, string version) => name.ToLower() + " - " + version.ToLower();

        private static IEnumerable<string> GetLibraryPathsFromPackagesXml(string packagesXml)
        {
            const string searchString = "{http://www.w3.org/2005/Atom}content";
            var xmlElement = XElement.Parse(packagesXml);
            return xmlElement.Descendants(searchString).Select(descendant => descendant.LastAttribute.Value).ToList();
        }

        private static string DownloadPackagesStringFromSource(string uriSource)
        {
            using (var client = new WebClient())
            {
                try
                {
                    return client.DownloadString(uriSource);
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        #endregion

    }

    public class LibraryNameToPath
    {
        public string Name;
        public string Path;

        public override string ToString()
        {
            return Name;
        }
    }
}