using DiiagramrAPI.Service.Interfaces;
using System;
using System.Windows;
using System.Windows.Input;

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
        public override int MaxWidth => 550;
        public string SelectedSource { get; set; }
        public string SourceTextBoxText { get; set; } = "http://";
        public override string Title => "Library Source Manager";

        public void AddSource()
        {
            if (string.IsNullOrEmpty(SourceTextBoxText))
            {
                return;
            }

            LibraryManager.AddSource(SourceTextBoxText);
            SourceTextBoxText = "http://";
        }

        public void RemoveSource(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement frameworkElement)
            {
                if (frameworkElement.DataContext is string source)
                {
                    if (LibraryManager.RemoveSource(source))
                    {

                    }
                }
            }
        }
    }
}
