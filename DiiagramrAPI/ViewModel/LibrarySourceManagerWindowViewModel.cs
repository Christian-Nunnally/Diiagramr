using System;
using Diiagramr.View.ShellWindow;
using DiiagramrAPI.Service.Interfaces;

namespace DiiagramrAPI.ViewModel
{
    public class LibrarySourceManagerWindowViewModel : AbstractShellWindow
    {
        public LibrarySourceManagerWindowViewModel(Func<ILibraryManager> libraryManagerFactory)
        {
            LibraryManager = libraryManagerFactory.Invoke();
        }

        public ILibraryManager LibraryManager { get; set; }
        public string SelectedSource { get; set; }
        public string SourceTextBoxText { get; set; }

        public override int MaxWidth => 400;
        public override int MaxHeight => 400;
        public override string Title => "Library Source Manager";

        public void AddSource()
        {
            if (string.IsNullOrEmpty(SourceTextBoxText)) return;
            LibraryManager.AddSource(SourceTextBoxText);
            SourceTextBoxText = "";
        }

        public void RemoveSelectedSource()
        {
            if (string.IsNullOrEmpty(SelectedSource)) return;
            LibraryManager.RemoveSource(SelectedSource);
        }
    }
}