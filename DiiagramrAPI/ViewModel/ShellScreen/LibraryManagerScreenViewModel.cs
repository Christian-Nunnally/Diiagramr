using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Stylet;

namespace DiiagramrAPI.ViewModel.ShellScreen
{
    public class LibraryManagerScreenViewModel : Screen
    {
        public bool Visible { get; set; }

        public string SelectedSource { get; set; }
        public string SelectedLibrary { get; set; }
        public string SelectedInstalledLibrary { get; set; }

        public ObservableCollection<string> Sources { get; set; }
        public ObservableCollection<string> LibraryNames { get; set; }
        public ObservableCollection<string> InstalledLibraryNames { get; set; }
        public Dictionary<string, string> LibraryNameToPathMap { get; set; }

        public LibraryManagerScreenViewModel()
        {
            Sources = new BindableCollection<string>();
            LibraryNames = new BindableCollection<string>();
            InstalledLibraryNames = new BindableCollection<string>();
            LibraryNameToPathMap = new Dictionary<string, string>();
            Sources.Add("http://diiagramrlibraries.azurewebsites.net/nuget/Packages");
            SelectedSource = Sources.First();
            SourceSelectionChanged();

            UpdateInstalledLibraries();
        }

        // TODO: Refactor into a directory watcher.
        public void UpdateInstalledLibraries()
        {
            InstalledLibraryNames.Clear();
            string path = "Plugins/";
            foreach (string s in Directory.GetDirectories(path))
            {
                var directoryName = s.Remove(0, path.Length);
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
            var file = "Plugins/" + SelectedInstalledLibrary;
            DeleteDirectory(file);
            UpdateInstalledLibraries();
        }

        public void DeleteDirectory(string targetDir)
        {
            File.SetAttributes(targetDir, FileAttributes.Normal);

            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }

        public void InstallLibrary(string libraryName, string version)
        {
            var lowercaseName = libraryName.ToLower() + " - " + version.ToLower();
            if (LibraryNameToPathMap.ContainsKey(lowercaseName))
            {
                string zipPath = "Plugins/" + lowercaseName + ".zip";
                string extractPath = "Plugins/" + lowercaseName;
                using (var client = new WebClient())
                {
                    client.DownloadFile(LibraryNameToPathMap[lowercaseName], zipPath);
                }

                try
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
                }
                catch (IOException)
                {
                }
                File.Delete(zipPath);
            }
            UpdateInstalledLibraries();
        }

        public void SourceSelectionChanged()
        {
            LibraryNames.Clear();
            string packages;
            using (var client = new WebClient())
            {
                packages = client.DownloadString(SelectedSource);
            }

            var descendants = XElement.Parse(packages).Descendants("{http://www.w3.org/2005/Atom}content");

            foreach (var descendant in descendants)
            {
                var libraryPath = descendant.LastAttribute.Value;
                var sl = libraryPath.Split('/');
                var libraryName = sl[sl.Length - 2];
                var libraryVersion = sl[sl.Length - 1];
                var libraryString = libraryName + " - " + libraryVersion;
                if (!LibraryNameToPathMap.ContainsKey(libraryString))
                {
                    LibraryNameToPathMap.Add(libraryString, libraryPath);
                    LibraryNames.Add(libraryString);
                }
            }
        }

        public void Close()
        {
            RequestClose();
        }
    }
}
