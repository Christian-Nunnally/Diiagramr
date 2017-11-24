using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.ViewModel;

namespace DiiagramrAPI.CustomControls
{
    public class NodeResizeThumb : Thumb
    {
        public NodeResizeThumb()
        {
            DragDelta += ResizeThumb_DragDelta;
            //PreviewMouseUp += OnPreviewMouseUp;
        }

        //        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        //        {
        //            if (!(DataContext is ContentPresenter contentPresenter)) return;
        //            if (!(contentPresenter.Content is PluginNode node)) return;
        //            node.Dragging = false;
        //
        //            if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl)) return;
        //
        //            node.X = RoundToNearest((int)node.X, DiagramConstants.GridSnapInterval) - DiagramConstants.NodeBorderWidth + 1;
        //            node.Y = RoundToNearest((int)node.Y, DiagramConstants.GridSnapInterval) - DiagramConstants.NodeBorderWidth + 1;
        //        }
        //
        //        private static int RoundToNearest(int value, int multiple)
        //        {
        //            var rem = value % multiple;
        //            var result = value - rem;
        //            if (rem > (multiple / 2))
        //                result += multiple;
        //            return result;
        //        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!(DataContext is ContentPresenter contentPresenter)) return;
            if (!(contentPresenter.Content is PluginNode node)) return;

            node.Width += e.HorizontalChange;
            node.Height += e.VerticalChange;
            //node.Dragging = true;
        }
    }
}