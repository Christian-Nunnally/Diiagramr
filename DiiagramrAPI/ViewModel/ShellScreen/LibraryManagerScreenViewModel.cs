using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

        public ObservableCollection<string> Sources { get; set; }
        public ObservableCollection<string> LibraryNames { get; set; }
        public Dictionary<string, string> LibraryNameToPathMap { get; set; }

        public LibraryManagerScreenViewModel()
        {
            Sources = new BindableCollection<string>();
            LibraryNames = new BindableCollection<string>();
            LibraryNameToPathMap = new Dictionary<string, string>();
            Sources.Add("http://diiagramrlibraries.azurewebsites.net/nuget/Packages");
        }

        public void InstallLibrary()
        {
            if (string.IsNullOrEmpty(SelectedLibrary)) return;
            if (LibraryNameToPathMap.ContainsKey(SelectedLibrary))
            {
                string zipPath = "Plugins/" + SelectedLibrary + ".zip";
                string extractPath = "Plugins/" + SelectedLibrary;
                using (var client = new WebClient())
                {
                    client.DownloadFile(LibraryNameToPathMap[SelectedLibrary], zipPath);
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
                LibraryNameToPathMap.Add(libraryString, libraryPath);
                LibraryNames.Add(libraryString);
            }
        }

        public void Close()
        {
            RequestClose();
        }
    }
}
