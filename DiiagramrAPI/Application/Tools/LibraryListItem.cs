using DiiagramrModel;
using PropertyChanged;

namespace DiiagramrAPI.Application.Tools
{
    /// <summary>
    /// Helper view model class for representing libraries in a list view.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class LibraryListItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="LibraryListItem"/>
        /// </summary>
        /// <param name="library">The <see cref="NodeLibrary"/> that this list item represents.</param>
        public LibraryListItem(NodeLibrary library)
        {
            Library = library;
        }

        /// <summary>
        /// The text displayed on the inline list view button.
        /// </summary>
        public string ButtonText { get; set; } = "Install";

        /// <summary>
        /// The library that this list item represents.
        /// </summary>
        public NodeLibrary Library { get; internal set; }

        /// <summary>
        /// The display name for the library item.
        /// </summary>
        public string LibraryDisplayName => Library?.Name;

        /// <inheritdoc/>
        public override string ToString()
        {
            return LibraryDisplayName;
        }
    }
}