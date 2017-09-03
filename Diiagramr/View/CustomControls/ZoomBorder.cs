using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PropertyChanged;

namespace Diiagramr.View.CustomControls
{
    [AddINotifyPropertyChangedInterface]
    public class ZoomBorder : Border
    {
        private UIElement _child;
        private Point _origin;
        private Point _start;


        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(
            "Zoom", typeof(double), typeof(ZoomBorder), new PropertyMetadata(default(double)));

        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }


        public static readonly DependencyProperty PanXProperty = DependencyProperty.Register(
            "PanX", typeof(double), typeof(ZoomBorder), new PropertyMetadata(default(double)));

        public double PanX
        {
            get { return (double)GetValue(PanXProperty); }
            set { SetValue(PanXProperty, value); }
        }

        public static readonly DependencyProperty PanYProperty = DependencyProperty.Register(
            "PanY", typeof(double), typeof(ZoomBorder), new PropertyMetadata(default(double)));

        public double PanY
        {
            get { return (double)GetValue(PanYProperty); }
            set { SetValue(PanYProperty, value); }
        }

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if ((value != null) && !Equals(value, Child))
                    Initialize(value);
                base.Child = value;
            }
        }

        private static TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
                .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
                .Children.First(tr => tr is ScaleTransform);
        }

        private void Initialize(UIElement element)
        {
            _child = element;
            if (_child == null) return;
            var group = new TransformGroup();
            var st = new ScaleTransform();
            group.Children.Add(st);
            var tt = new TranslateTransform();
            group.Children.Add(tt);
            _child.RenderTransform = group;
            _child.RenderTransformOrigin = new Point(0.0, 0.0);
            Zoom = 1.0;

            MouseWheel += child_MouseWheel;
            MouseLeftButtonDown += child_MouseLeftButtonDown;
            MouseLeftButtonUp += child_MouseLeftButtonUp;
            MouseMove += child_MouseMove;
            PreviewMouseRightButtonDown += child_PreviewMouseRightButtonDown;
        }

        private void Reset()
        {
            if (_child == null) return;
            // reset zoom
            var st = GetScaleTransform(_child);
            st.ScaleX = 1.0;
            st.ScaleY = 1.0;
            Zoom = 1.0;

            // reset pan
            var tt = GetTranslateTransform(_child);
            tt.X = 0.0;
            tt.Y = 0.0;
            PanX = tt.X;
            PanY = tt.Y;
        }

        #region Child Events

        private void child_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_child == null) return;
            var st = GetScaleTransform(_child);
            var tt = GetTranslateTransform(_child);

            var zoom = e.Delta > 0 ? .2 : -.2;
            if (!(e.Delta > 0) && ((st.ScaleX < .4) || (st.ScaleY < .4)))
                return;

            var relative = e.GetPosition(_child);

            var abosuluteX = relative.X * st.ScaleX + tt.X;
            var abosuluteY = relative.Y * st.ScaleY + tt.Y;

            st.ScaleX += zoom;
            st.ScaleY += zoom;
            Zoom = st.ScaleX;

            tt.X = abosuluteX - relative.X * st.ScaleX;
            tt.Y = abosuluteY - relative.Y * st.ScaleY;
            PanX = tt.X;
            PanY = tt.Y;
            e.Handled = false;

        }

        private void child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_child == null) return;
            var tt = GetTranslateTransform(_child);
            _start = e.GetPosition(this);
            _origin = new Point(tt.X, tt.Y);
            Cursor = Cursors.Hand;
            _child.CaptureMouse();
            e.Handled = false;

        }

        private void child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_child == null) return;
            _child.ReleaseMouseCapture();
            Cursor = Cursors.Arrow;
            e.Handled = false;

        }

        private void child_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;

        }

        private void child_MouseMove(object sender, MouseEventArgs e)
        {
            if (_child == null) return;
            if (_child.IsMouseCaptured)
            {
                var tt = GetTranslateTransform(_child);
                var v = _start - e.GetPosition(this);
                tt.X = _origin.X - v.X;
                tt.Y = _origin.Y - v.Y;
                PanX = tt.X;
                PanY = tt.Y;
            }
            e.Handled = false;
        }

        #endregion
    }
}