using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;

namespace DiiagramrAPI.CustomControls
{
    public class NodeMoveThumb : Thumb
    {
        public NodeMoveThumb()
        {
            DragDelta += MoveThumb_DragDelta;
            PreviewMouseUp += OnPreviewMouseUp;
        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (!(DataContext is ContentPresenter contentPresenter)) return;
            if (!(contentPresenter.Content is PluginNode node)) return;
            node.Dragging = false;

            if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl)) return;

            node.X = RoundToNearest(node.X, DiagramViewModel.GridSnapInterval);
            node.Y = RoundToNearest(node.Y, DiagramViewModel.GridSnapInterval);
        }

        private static double RoundToNearest(double value, double multiple)
        {
            var rem = value % multiple;
            var result = value - rem;
            if (rem > multiple / 2.0)
                result += multiple;
            return result;
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!(DataContext is ContentPresenter contentPresenter)) return;
            if (!(contentPresenter.Content is PluginNode node)) return;

            node.X += e.HorizontalChange;
            node.Y += e.VerticalChange;
            node.Dragging = true;
        }
    }
}