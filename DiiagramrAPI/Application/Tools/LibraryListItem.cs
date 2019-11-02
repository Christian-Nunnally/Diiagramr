using DiiagramrModel;
using PropertyChanged;

namespace DiiagramrAPI.Application.Tools
{
    [AddINotifyPropertyChangedInterface]
    public class LibraryListItem
    {
        public LibraryListItem(NodeLibrary library)
        {
            Library = library;
            LibraryDisplayName = library.Name;
        }

        public string ButtonText { get; set; } = "Install";
        public NodeLibrary Library { get; internal set; }
        public string LibraryDisplayName { get; }

        public override string ToString()
        {
            return LibraryDisplayName;
        }
    }
}