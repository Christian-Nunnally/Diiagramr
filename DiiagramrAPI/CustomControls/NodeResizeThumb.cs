using DiiagramrAPI.Diagram;
using DiiagramrAPI.PluginNodeApi;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DiiagramrAPI.CustomControls
{
    public class NodeResizeThumb : Thumb
    {
        public NodeResizeThumb()
        {
            DragDelta += ResizeThumb_DragDelta;
            PreviewMouseUp += OnPreviewMouseUp;
        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (!(DataContext is ContentPresenter contentPresenter))
            {
                return;
            }

            if (!(contentPresenter.Content is PluginNode node))
            {
                return;
            }

            node.Dragging = false;

            if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                return;
            }

            node.Width = CoreUilities.RoundToNearest(node.Width, DiagramViewModel.GridSnapInterval);
            node.Height = CoreUilities.RoundToNearest(node.Height, DiagramViewModel.GridSnapInterval);
            node.Dragging = false;
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!(DataContext is ContentPresenter contentPresenter))
            {
                return;
            }

            if (!(contentPresenter.Content is PluginNode node))
            {
                return;
            }

            node.Width += e.HorizontalChange;
            node.Height += e.VerticalChange;
            node.Dragging = true;
        }
    }
}
