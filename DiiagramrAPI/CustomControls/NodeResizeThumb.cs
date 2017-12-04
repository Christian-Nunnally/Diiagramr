using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using DiiagramrAPI.PluginNodeApi;

namespace DiiagramrAPI.CustomControls
{
    public class NodeResizeThumb : Thumb
    {
        public NodeResizeThumb()
        {
            DragDelta += ResizeThumb_DragDelta;
        }
        
        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!(DataContext is ContentPresenter contentPresenter)) return;
            if (!(contentPresenter.Content is PluginNode node)) return;

            node.Width += e.HorizontalChange;
            node.Height += e.VerticalChange;
        }
    }
}