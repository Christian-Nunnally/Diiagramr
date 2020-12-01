using DiiagramrAPI.Editor.Diagrams;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DiiagramrApplication.Editor
{
    /// <summary>
    /// Interaction logic for NodePreviewControl.xaml
    /// </summary>
    public partial class NodePreviewControl : UserControl
    {
        public static readonly DependencyProperty NodeToPreviewProperty =
            DependencyProperty.Register(
            "NodeToPreview", typeof(Node),
            typeof(NodePreviewControl),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsMeasure,
                new PropertyChangedCallback(OnNodeToPreviewChanged),
                new CoerceValueCallback(CoerceNodeToPreview)
            ));

        public static readonly DependencyProperty PreviewNodeScaleXProperty =
            DependencyProperty.Register(
            "PreviewNodeScaleX", typeof(double),
            typeof(NodePreviewControl));

        public static readonly DependencyProperty PreviewNodeScaleYProperty =
            DependencyProperty.Register(
            "PreviewNodeScaleY", typeof(double),
            typeof(NodePreviewControl));

        public NodePreviewControl()
        {
            InitializeComponent();
        }

        public double PreviewNodeScaleX
        {
            get { return (double)GetValue(PreviewNodeScaleXProperty); }
            set { SetValue(PreviewNodeScaleXProperty, value); }
        }

        public double PreviewNodeScaleY
        {
            get { return (double)GetValue(PreviewNodeScaleYProperty); }
            set { SetValue(PreviewNodeScaleYProperty, value); }
        }

        public Node NodeToPreview
        {
            get { return (Node)GetValue(NodeToPreviewProperty); }
            set { SetValue(NodeToPreviewProperty, value); }
        }

        private static object CoerceNodeToPreview(DependencyObject d, object baseValue)
        {
            return baseValue;
        }

        private static void OnNodeToPreviewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NodePreviewControl)d;
            control.PreviewNode(e.NewValue as Node);
        }

        private void PreviewNode(Node node)
        {
            if (node != null)
            {
                var totalNodeWidth = node.Width + Diagram.NodeBorderWidth * 2.0;
                var totalNodeHeight = node.Height + Diagram.NodeBorderWidth * 2.0;
                PreviewNodeScaleX = Width / totalNodeWidth;
                PreviewNodeScaleY = Height / totalNodeHeight;
                PreviewNodeScaleX = Math.Min(PreviewNodeScaleX, PreviewNodeScaleY);
                PreviewNodeScaleY = Math.Min(PreviewNodeScaleX, PreviewNodeScaleY);
            }
        }
    }
}