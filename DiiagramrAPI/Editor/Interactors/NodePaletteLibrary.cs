using DiiagramrAPI.Editor.Diagrams;
using PropertyChanged;
using System.Collections.Generic;
using System.Windows.Media;

namespace DiiagramrAPI.Editor.Interactors
{
    [AddINotifyPropertyChangedInterface]
    public class NodePaletteCategory
    {
        public NodePaletteCategory(string name)
        {
            Name = name;
            Nodes = new List<Node>();
        }

        public Brush BackgroundBrush { get; private set; } = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

        public bool IsCategoryMenuExpanded { get; set; }

        public string Name { get; }

        public virtual List<Node> Nodes { get; }

        public bool NodesLoaded { get; set; }

        public virtual void SelectCategoryItem()
        {
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
            IsCategoryMenuExpanded = true;
        }

        public virtual void UnselectCategoryItem()
        {
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            IsCategoryMenuExpanded = false;
        }
    }
}