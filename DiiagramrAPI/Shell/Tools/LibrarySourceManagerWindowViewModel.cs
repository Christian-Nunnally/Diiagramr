using DiiagramrAPI.Service.Interfaces;
using System;

namespace DiiagramrAPI.Shell.Tools
{
    public class LibrarySourceManagerWindowViewModel : AbstractShellWindow
    {
        public LibrarySourceManagerWindowViewModel(Func<ILibraryManager> libraryManagerFactory)
        {
            LibraryManager = libraryManagerFactory.Invoke();
            LibraryManager.AddSource("http://diiagramrlibraries.azurewebsites.net/nuget/Packages");
        }

        public ILibraryManager LibraryManager { get; }
        public override int MaxHeight => 400;
        public override int MaxWidth => 400;
        public string SelectedSource { get; set; }
        public string SourceTextBoxText { get; set; }
        public override string Title => "Library Source Manager";

        public void AddSource()
        {
            if (string.IsNullOrEmpty(SourceTextBoxText))
            {
                return;
            }

            LibraryManager.AddSource(SourceTextBoxText);
            SourceTextBoxText = "";
        }

        public void RemoveSelectedSource()
        {
            if (string.IsNullOrEmpty(SelectedSource))
            {
                return;
            }

            LibraryManager.RemoveSource(SelectedSource);
        }
    }
}
