using DiiagramrAPI.Editor.Diagrams;
using PropertyChanged;
using System.Collections.Generic;
using System.Windows.Media;

namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// A helper view model class representing a category of node in the palette.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class NodePaletteCategory
    {
        /// <summary>
        /// Creates a new instance of <see cref="NodePaletteCategory"/>.
        /// </summary>
        /// <param name="name">The name of the category.</param>
        public NodePaletteCategory(string name)
        {
            Name = name;
            Nodes = new List<Node>();
        }

        /// <summary>
        /// The color to draw this category list item as.
        /// </summary>
        public Brush BackgroundBrush { get; private set; } = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

        /// <summary>
        /// Gets or sets whether the categrory is showing its children.
        /// </summary>
        public bool IsCategoryMenuExpanded { get; set; }

        /// <summary>
        /// Gets the name of the category.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the nodes contained within the category.
        /// </summary>
        public virtual List<Node> Nodes { get; }

        /// <summary>
        /// Gets or sets whether the nodes in this category have been loaded yet.
        /// </summary>
        public bool NodesLoaded { get; set; }

        /// <summary>
        /// Selects this category item.
        /// </summary>
        public virtual void SelectCategoryItem()
        {
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
            IsCategoryMenuExpanded = true;
        }

        /// <summary>
        /// Unselects this category item.
        /// </summary>
        public virtual void UnselectCategoryItem()
        {
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            IsCategoryMenuExpanded = false;
        }
    }
}