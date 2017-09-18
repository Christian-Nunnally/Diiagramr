﻿using System.Windows.Controls;
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
            if (!(DataContext is ContentPresenter contentPresenter)) return;
            if (!(contentPresenter.Content is AbstractNodeViewModel node)) return;
            node.X += e.HorizontalChange;
            node.Y += e.VerticalChange;
        }
    }
}