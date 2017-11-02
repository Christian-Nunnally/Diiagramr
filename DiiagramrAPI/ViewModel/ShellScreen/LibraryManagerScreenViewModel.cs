using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Stylet;
using System.Reflection;

namespace DiiagramrAPI.ViewModel.ShellScreen
{
    public class LibraryManagerScreenViewModel : Screen
    {
        private const string PluginsDirectory = "Plugins/";

        public string SelectedSource { get; set; }
        public string SelectedLibrary { get; set; }
        public string SelectedInstalledLibrary { get; set; }

        public ObservableCollection<string> Sources { get; set; }
        public ObservableCollection<string> LibraryNames { get; set; }
        public ObservableCollection<string> InstalledLibraryNames { get; set; }
        public Dictionary<string, string> LibraryNameToPathMap { get; set; }

        public string SourceTextBoxText { get; set; }

        public bool SourcesVisible { get; set; }
        public bool InvalidSource { get; set; }

        private List<string> uninstalledLibraries = new List<string>();

        public LibraryManagerScreenViewModel()
        {
            Sources = new BindableCollection<string>();
            LibraryNames = new BindableCollection<string>();
            InstalledLibraryNames = new BindableCollection<string>();
            LibraryNameToPathMap = new Dictionary<string, string>();

            LoadDefaultSource();
        }

        private void LoadDefaultSource()
        {
            Sources.Add("http://diiagramrlibraries.azurewebsites.net/nuget/Packages");
            SelectedSource = Sources.First();
            SourceSelectionChanged();

            UpdateInstalledLibraries();
        }

        private void UpdateInstalledLibraries()
        {
            InstalledLibraryNames.Clear();
            foreach (var directory in Directory.GetDirectories(PluginsDirectory))
            {
                var directoryName = directory.Remove(0, PluginsDirectory.Length);
                if (uninstalledLibraries.Contains(directoryName)) continue;
                InstalledLibraryNames.Add(directoryName);
            }
        }

        public void InstallSelectedLibrary()
        {
            if (string.IsNullOrEmpty(SelectedLibrary)) return;
            if (SelectedLibrary.Split(' ').Length != 3) return;
            InstallLibrary(SelectedLibrary.Split(' ')[0], SelectedLibrary.Split(' ')[2]);
        }

        public void UninstallSelectedLibrary()
        {
            var file = PluginsDirectory + SelectedInstalledLibrary;
            uninstalledLibraries.Add(SelectedInstalledLibrary);
            //DeleteDirectory(file);
            UpdateInstalledLibraries();
        }

        private static void DeleteDirectory(string targetDir)
        {
            File.SetAttributes(targetDir, FileAttributes.Normal);

            var files = Directory.GetFiles(targetDir);
            var dirs = Directory.GetDirectories(targetDir);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }

        /// <summary>
        /// Installs a library.
        /// </summary>
        /// <param name="libraryName">The name of the library you want to install.  Format "VisualDrop" or "visualdrop" (case insensitive).</param>
        /// <param name="version">The version of the library you want to install.  Format = "v1.0.0"</param>
        /// <returns>True if the library was able to be installed, false if otherwise.</returns>
        public bool InstallLibrary(string libraryName, string version)
        {
            var lowercaseName = libraryName.ToLower() + " - " + version.ToLower();
            if (uninstalledLibraries.Contains(lowercaseName))
            {
                uninstalledLibraries.Remove(lowercaseName);
            }

            if (LibraryNameToPathMap.ContainsKey(lowercaseName))
            {
                var tmpDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\tmp";
                if (!Directory.Exists(tmpDir)) Directory.CreateDirectory(tmpDir);
                var zipPath = "tmp/" + SelectedLibrary + ".zip";
                var extractPath = "tmp/" + SelectedLibrary;
                var toPath = PluginsDirectory + SelectedLibrary;
                using (var client = new WebClient())
                {
                    client.DownloadFile(LibraryNameToPathMap[lowercaseName], zipPath);
                }

                try
                {
                    if (!Directory.Exists(toPath))
                    {
                        System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
                        Directory.Move(extractPath, toPath);
                    }
                }
                catch (IOException)
                {
                }
                File.Delete(zipPath);
                UpdateInstalledLibraries();
                return true;
            }
            return false;
        }

        public void SourceSelectionChanged()
        {
            LibraryNames.Clear();
            LibraryNameToPathMap.Clear();
            string packages;

            if (!SelectedSource.StartsWith("http://"))
            {
                InvalidSource = true;
                return;
            }

            using (var client = new WebClient())
            {
                try
                {
                    packages = client.DownloadString(SelectedSource);
                }
                catch (Exception)
                {
                    InvalidSource = true;
                    return;
                }
            }
            InvalidSource = false;

            const string searchString = "{http://www.w3.org/2005/Atom}content";
            foreach (var descendant in XElement.Parse(packages).Descendants(searchString))
            {
                var libraryPath = descendant.LastAttribute.Value;
                var sl = libraryPath.Split('/');
                var libraryName = sl[sl.Length - 2];
                var libraryVersion = sl[sl.Length - 1];
                var libraryString = libraryName + " - " + libraryVersion;
                if (LibraryNameToPathMap.ContainsKey(libraryString)) continue;
                LibraryNameToPathMap.Add(libraryString, libraryPath);
                LibraryNames.Add(libraryString);
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
            Sources.Add(SourceTextBoxText);
            SourceTextBoxText = "";
        }

        public void RemoveSelectedSource()
        {
            if (string.IsNullOrEmpty(SelectedSource)) return;
            if (!Sources.Contains(SelectedSource)) return;
            Sources.Remove(SelectedSource);
        }
    }
}
