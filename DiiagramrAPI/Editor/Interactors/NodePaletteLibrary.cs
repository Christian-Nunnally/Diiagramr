using PropertyChanged;
using System.Collections.Generic;
using System.Windows.Media;

namespace DiiagramrAPI.Editor.Interactors
{
    [AddINotifyPropertyChangedInterface]
    public class NodePaletteLibrary
    {
        public NodePaletteLibrary(string name)
        {
            Name = name;
            Nodes = new List<Node>();
        }

        public bool IsLibraryMenuExpanded { get; set; }
        public Brush BackgroundBrush { get; private set; } = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        public string Name { get; }
        public virtual List<Node> Nodes { get; }
        public bool NodesLoaded { get; set; }

        public virtual void Select()
        {
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
            IsLibraryMenuExpanded = true;
        }

        public virtual void Unselect()
        {
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            IsLibraryMenuExpanded = false;
        }
    }
}
