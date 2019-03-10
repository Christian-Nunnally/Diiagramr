using DiiagramrAPI.Diagram;
using DiiagramrAPI.PluginNodeApi;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DiiagramrAPI.CustomControls
{
    public class NodeMoveThumb : Thumb
    {
        public NodeMoveThumb()
        {
            DragDelta += MoveThumb_DragDelta;
            PreviewMouseUp += OnPreviewMouseUp;
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!(DataContext is ContentPresenter contentPresenter))
            {
                return;
            }

            if (!(contentPresenter.Content is PluginNode node))
            {
                return;
            }

            node.X += e.HorizontalChange;
            node.Y += e.VerticalChange;
            node.Dragging = true;
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

            node.X = CoreUilities.RoundToNearest(node.X, DiagramViewModel.GridSnapInterval);
            node.Y = CoreUilities.RoundToNearest(node.Y, DiagramViewModel.GridSnapInterval);
        }
    }
}
