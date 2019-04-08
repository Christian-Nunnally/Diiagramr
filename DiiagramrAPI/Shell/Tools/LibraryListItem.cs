using DiiagramrAPI.Diagram.Model;
using PropertyChanged;

namespace DiiagramrAPI.Shell.Tools
{
    [AddINotifyPropertyChangedInterface]
    public class LibraryListItem
    {
        public string ButtonText { get; set; } = "Install";
        public string LibraryDisplayName { get; }
        public NodeLibrary Library { get; internal set; }

        // TODO: Just pass in the library and get the name from that.
        public LibraryListItem(NodeLibrary library, string libraryDisplayName)
        {
            Library = library;
            LibraryDisplayName = libraryDisplayName;
        }

        public override string ToString()
        {
            return LibraryDisplayName;
        }
    }
}
