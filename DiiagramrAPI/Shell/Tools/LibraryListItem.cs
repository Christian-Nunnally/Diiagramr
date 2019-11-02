using DiiagramrModel;
using PropertyChanged;

namespace DiiagramrAPI.Shell.Tools
{
    [AddINotifyPropertyChangedInterface]
    public class LibraryListItem
    {
        public string ButtonText { get; set; } = "Install";
        public string LibraryDisplayName { get; }
        public NodeLibrary Library { get; internal set; }

        public LibraryListItem(NodeLibrary library)
        {
            Library = library;
            LibraryDisplayName = library.Name;
        }

        public override string ToString()
        {
            return LibraryDisplayName;
        }
    }
}
