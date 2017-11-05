using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Stylet;
using System.Reflection;
using System.Threading.Tasks;

namespace DiiagramrAPI.ViewModel.ShellScreen
{
    public class LibraryManagerScreenViewModel : Screen
    {
        private const string PluginsDirectory = "Plugins\\";

        public string SelectedSource { get; set; }
        public string SelectedLibrary { get; set; }

        public ObservableCollection<string> Sources { get; set; }
        public ObservableCollection<string> InstalledLibraryNames { get; set; }
        public ObservableCollection<LibraryNameToPath> LibraryNameToPathMap { get; set; }

        public string SourceTextBoxText { get; set; }

        public bool SourcesVisible { get; set; }

        public LibraryManagerScreenViewModel()
        {
            Sources = new BindableCollection<string>();
            InstalledLibraryNames = new BindableCollection<string>();
            LibraryNameToPathMap = new BindableCollection<LibraryNameToPath>();

            AddSource("http://diiagramrlibraries.azurewebsites.net/nuget/Packages");
            UpdateInstalledLibraries();
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

        public void InstallSelectedLibrary()
        {
            if (string.IsNullOrEmpty(SelectedLibrary)) return;
            if (SelectedLibrary.Split(' ').Length != 3) return;
            InstallLibrary(SelectedLibrary.Split(' ')[0], SelectedLibrary.Split(' ')[2]);
        }

        /// <summary>
        /// Installs a library.
        /// </summary>
        /// <param name="libraryName">The name of the library you want to install.  Format "VisualDrop" or "visualdrop" (case insensitive).</param>
        /// <param name="libraryVersion">The version of the library you want to install.  Format = "v1.0.0"</param>
        /// <returns>True if the library was installed, false otherwise.</returns>
        public bool InstallLibrary(string libraryName, string libraryVersion)
        {
            var formattedLibraryName = FormatLibraryName(libraryName, libraryVersion);
            if (!LibraryPathMapContains(formattedLibraryName)) return false;

            var tmpDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\tmp";
            if (!Directory.Exists(tmpDir)) Directory.CreateDirectory(tmpDir);
            var zipPath = "tmp/" + SelectedLibrary + ".zip";
            var extractPath = "tmp/" + SelectedLibrary;
            var toPath = PluginsDirectory + SelectedLibrary;
            using (var client = new WebClient())
            {
                client.DownloadFile(LibraryPathMapGet(formattedLibraryName), zipPath);
            }

            try
            {
                if (!Directory.Exists(toPath))
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
                    Directory.Move(extractPath, toPath);
                    File.Delete(zipPath);
                }
            }
            catch (IOException)
            {
                return false;
            }

            UpdateInstalledLibraries();
            return true;
        }

        public string GetLibraryInstallDirectory(string libraryName, string libraryVersion)
        {
            var formattedLibraryName = FormatLibraryName(libraryName, libraryVersion);
            var libraryInstallDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + PluginsDirectory + formattedLibraryName;
            if (!Directory.Exists(libraryInstallDirectory)) return "";
            return libraryInstallDirectory;
        }

        private void AddSource(string sourceUrl)
        {
            if (!sourceUrl.StartsWith("http://")) return;
            Sources.Add(sourceUrl);
            Task.Run(() =>
            {
                var packagesString = DownloadPackagesStringFromSource(sourceUrl);
                var libraryPaths = GetLibraryPathsFromPackagesXml(packagesString);
                AddLibraryPathsToNameToPathMap(libraryPaths);
            });
        }

        private void RemoveSource(string sourceUrl)
        {
            if (!Sources.Contains(sourceUrl)) return;
            Sources.Remove(sourceUrl);
            Task.Run(() =>
            {
                var packagesString = DownloadPackagesStringFromSource(sourceUrl);
                var libraryPaths = GetLibraryPathsFromPackagesXml(packagesString);
                RemoveLibraryPathsFromNameToPathMap(libraryPaths);
            });
        }

        private void AddLibraryPathsToNameToPathMap(IEnumerable<string> libraryPaths)
        {
            foreach (var libraryPath in libraryPaths)
            {
                var libraryString = GetLibraryNameFromPath(libraryPath);
                LibraryPathMapAdd(libraryString, libraryPath);
            }
        }

        private void RemoveLibraryPathsFromNameToPathMap(IEnumerable<string> libraryPaths)
        {
            foreach (var libraryPath in libraryPaths)
            {
                var libraryString = GetLibraryNameFromPath(libraryPath);
                if (LibraryPathMapContains(libraryString)) LibraryPathMapRemove(libraryString);
            }
        }

        public void Close()
        {
            RequestClose();
            SourcesVisible = false;
        }

        public void ViewSources()
        {
            SourcesVisible = !SourcesVisible;
        }

        public void AddSource()
        {
            if (string.IsNullOrEmpty(SourceTextBoxText)) return;
            AddSource(SourceTextBoxText);
            SourceTextBoxText = "";
        }

        public void RemoveSelectedSource()
        {
            if (!Sources.Contains(SelectedSource)) return;
            RemoveSource(SelectedSource);
        }

        private bool LibraryPathMapContains(string name)
        {
            return LibraryNameToPathMap.Any(p => p.Name == name);
        }

        private void LibraryPathMapAdd(string name, string path)
        {
            if (LibraryPathMapContains(name)) return;
            var libraryNameToPath = new LibraryNameToPath();
            libraryNameToPath.Name = name;
            libraryNameToPath.Path = path;
            LibraryNameToPathMap.Add(libraryNameToPath);
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
