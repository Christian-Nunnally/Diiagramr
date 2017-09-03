using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Diiagramr.ViewModel.Diagram;

namespace Diiagramr.View.CustomControls
{
    public class NodeMoveThumb : Thumb
    {
        public NodeMoveThumb()
        {
            DragDelta += MoveThumb_DragDelta;
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AbstractNodeViewModel item = null;
            var contentPresenter = DataContext as ContentPresenter;

            if (contentPresenter != null)
                item = contentPresenter.Content as AbstractNodeViewModel;

            if (item == null) return;
            item.X += e.HorizontalChange;
            item.Y += e.VerticalChange;
        }
    }
}