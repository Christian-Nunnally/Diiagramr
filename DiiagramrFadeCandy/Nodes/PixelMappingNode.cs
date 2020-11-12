using DiiagramrAPI.Editor.Diagrams;
using DiiagramrFadeCandy.GraphicsProcessing;
using DiiagramrModel;
using SharpDX.Mathematics.Interop;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DiiagramrFadeCandy.Nodes
{
    public class PixelMappingNode : Node
    {
        private const int NumberOfLeds = 64;

        private RawBox _box = new RawBox();
        private RenderedImage renderedImage;
        private byte[] _frontBuffer;
        private byte[] _backBuffer;
        private bool _onFrontBuffer;
        private Point _lastMouseDownLocation;
        private int _originalVisualRectangleTopCanvasPosition;
        private int _originalVisualRectangleLeftCanvasPosition;
        private int _originalVisualRectangleWidth;
        private int _originalVisualRectangleHeight;
        private bool _isMouseDown;
        private PixelRegionDragMode _pixelRegionDragMode;
        private int _visualRectangleTopCanvasPosition = 0;
        private int _visualRectangleLeftCanvasPosition = 0;
        private int _visualRectangleWidth = 8;
        private int _visualRectangleHeight = 8;

        public PixelMappingNode()
        {
            Name = "Pixel Mapping";
            Width = 90;
            Height = 90;
            ResizeEnabled = true;
            PropertyChanged += PropertyChangedHandler;
        }

        private enum PixelRegionDragMode
        {
            Move,
            MoveTop,
            MoveBottom,
            MoveLeft,
            MoveRight
        }

        public int CanvasWidth { get; set; }

        public int CanvasHeight { get; set; }

        [NodeSetting]
        [InputTerminal(Direction.North)]
        public bool IsStrideOrientationVertical { get; set; }

        public int VisualRectangleTopCanvasPosition
        {
            get => _visualRectangleTopCanvasPosition;
            set
            {
                var pixelHeight = Height / RenderedImage?.Height ?? 1;
                var temp = value - (value % (int)pixelHeight);
                SetViewPortY = (int)(temp / pixelHeight);
            }
        }

        public int VisualRectangleLeftCanvasPosition
        {
            get => _visualRectangleLeftCanvasPosition;
            set
            {
                var pixelWidth = Width / RenderedImage?.Width ?? 1;
                var temp = value - (value % (int)pixelWidth);
                SetViewPortX = (int)(temp / pixelWidth);
            }
        }

        public int VisualRectangleWidth
        {
            get => _visualRectangleWidth;
            set
            {
                var pixelWidth = Width / RenderedImage?.Width ?? CachedPixelWidth;
                CachedPixelWidth = pixelWidth > 0 ? pixelWidth : CachedPixelWidth;
                var nonZeroPixelWidth = pixelWidth == 0 ? 1 : pixelWidth;
                var temp = value + nonZeroPixelWidth - (value % (int)nonZeroPixelWidth);
                SetViewPortWidth = (int)(temp / nonZeroPixelWidth);
            }
        }

        public int VisualRectangleHeight
        {
            get => _visualRectangleHeight;
            set
            {
                var pixelHeight = Height / RenderedImage?.Height ?? CachedPixelHeight;
                CachedPixelHeight = pixelHeight > 0 ? pixelHeight : CachedPixelHeight;
                var nonZeroPixelHeight = pixelHeight == 0 ? 1 : pixelHeight;
                var temp = value + nonZeroPixelHeight - (value % (int)nonZeroPixelHeight);
                SetViewPortHeight = (int)(temp / nonZeroPixelHeight);
            }
        }

        [NodeSetting]
        public double CachedPixelHeight { get; set; }

        [NodeSetting]
        public double CachedPixelWidth { get; set; }

        public string RegionPositionText => $"{Box.X}x{Box.Y}";

        public string RegionSizeText => $"{Box.Width}x{Box.Height}";

        [field: NonSerialized]
        public BitmapSource BitmapImageSource { get; set; }

        [NodeSetting]
        public RawBox Box
        {
            get => _box;

            set
            {
                _box = value;
                MappedRegionChanged();
            }
        }

        public RenderedImage RenderedImage
        {
            get => renderedImage;
            set
            {
                if (renderedImage != null)
                {
                    renderedImage.ImageUpdated -= ImageUpdatedHandler;
                }
                renderedImage = value;
                if (renderedImage != null)
                {
                    renderedImage.ImageUpdated += ImageUpdatedHandler;
                }
                MappedRegionChanged();
            }
        }

        public byte[] MappedPixelData { get; set; } = new byte[NumberOfLeds * 3];

        [OutputTerminal(Direction.South)]
        public byte[] PixelData { get; set; }

        public bool IsMouseOverMappingRegionRectangle { get; set; }

        [NodeSetting]
        public Corner ZeroPixelCorner { get; set; }

        public HorizontalAlignment ZeroPixelIndicatorHorizontalAlignment { get; set; } = HorizontalAlignment.Center;

        public VerticalAlignment ZeroPixelIndicatorVerticalAlignment { get; set; } = VerticalAlignment.Top;

        [NodeSetting]
        public bool ShouldAlternateStrideDirection { get; set; }

        [NodeSetting]
        public int CompassArrowIconRotation { get; set; }

        [InputTerminal(Direction.East)]
        public int SetViewPortX
        {
            get => Box.X;
            set
            {
                if (value >= 0 && value < (RenderedImage?.Width ?? 0))
                {
                    Box = new RawBox(value, Box.Y, Box.Width, Box.Height);
                }
            }
        }

        [InputTerminal(Direction.East)]
        public int SetViewPortY
        {
            get => Box.Y;
            set
            {
                if (value >= 0 && value < (RenderedImage?.Height ?? 0))
                {
                    Box = new RawBox(Box.X, value, Box.Width, Box.Height);
                }
            }
        }

        [InputTerminal(Direction.West)]
        public int SetViewPortWidth
        {
            get => Box.Width;
            set
            {
                if (value >= 0)
                {
                    Box = new RawBox(Box.X, Box.Y, value, Box.Height);
                }
            }
        }

        [InputTerminal(Direction.West)]
        public int SetViewPortHeight
        {
            get => Box.Height;
            set
            {
                if (value >= 0)
                {
                    Box = new RawBox(Box.X, Box.Y, Box.Width, value);
                }
            }
        }

        [InputTerminal(Direction.North)]
        public RenderedImage SetImageSource
        {
            get => RenderedImage;
            set
            {
                if (value == null)
                {
                    return;
                }
                RenderedImage = value;
            }
        }

        private bool IsMouseOverNode { get; set; }

        public void PixelMapRegionMouseDown(object sender, MouseButtonEventArgs e)
        {
            _lastMouseDownLocation = e.GetPosition(View);
            _originalVisualRectangleTopCanvasPosition = VisualRectangleTopCanvasPosition;
            _originalVisualRectangleLeftCanvasPosition = VisualRectangleLeftCanvasPosition;
            _originalVisualRectangleWidth = VisualRectangleWidth;
            _originalVisualRectangleHeight = VisualRectangleHeight;
            _isMouseDown = true;
            SetDragMode(sender, e);

            NotifyOfPropertyChange(nameof(RegionSizeText));
            Mouse.Capture((IInputElement)sender);
            UpdateUI();
        }

        public void PixelMapRegionMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            Mouse.Capture(null);
        }

        public void AlternateStrideDirectionButtonHandler()
        {
            ShouldAlternateStrideDirection = !ShouldAlternateStrideDirection;
        }

        public void ToggleStrideOrientationButtonHandler()
        {
            IsStrideOrientationVertical = !IsStrideOrientationVertical;
            UpdateUI();
        }

        public void PixelMapRegionMouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                var newMousePosition = e.GetPosition(View);
                var differenceBetweenOriginalPoint = new Point(newMousePosition.X - _lastMouseDownLocation.X, newMousePosition.Y - _lastMouseDownLocation.Y);
                switch (_pixelRegionDragMode)
                {
                    case PixelRegionDragMode.Move:
                        VisualRectangleLeftCanvasPosition = _originalVisualRectangleLeftCanvasPosition + (int)differenceBetweenOriginalPoint.X;
                        VisualRectangleTopCanvasPosition = _originalVisualRectangleTopCanvasPosition + (int)differenceBetweenOriginalPoint.Y;
                        break;

                    case PixelRegionDragMode.MoveTop:
                        VisualRectangleTopCanvasPosition = _originalVisualRectangleTopCanvasPosition + (int)differenceBetweenOriginalPoint.Y;
                        VisualRectangleHeight = _originalVisualRectangleHeight + (_originalVisualRectangleTopCanvasPosition - VisualRectangleTopCanvasPosition);
                        break;

                    case PixelRegionDragMode.MoveBottom:
                        VisualRectangleHeight = _originalVisualRectangleHeight + (int)differenceBetweenOriginalPoint.Y;
                        break;

                    case PixelRegionDragMode.MoveLeft:
                        VisualRectangleLeftCanvasPosition = _originalVisualRectangleLeftCanvasPosition + (int)differenceBetweenOriginalPoint.X;
                        VisualRectangleWidth = _originalVisualRectangleWidth + (_originalVisualRectangleLeftCanvasPosition - VisualRectangleLeftCanvasPosition);
                        break;

                    case PixelRegionDragMode.MoveRight:
                        VisualRectangleWidth = _originalVisualRectangleWidth + (int)differenceBetweenOriginalPoint.X;
                        break;
                }
            }
        }

        public void HandleMapRegionMouseEnter()
        {
            IsMouseOverMappingRegionRectangle = true;
            Mouse.SetCursor(Cursors.Wait);
        }

        public void HandleMapRegionMouseLeave()
        {
            IsMouseOverMappingRegionRectangle = false;
            Mouse.SetCursor(Cursors.Arrow);
        }

        public void ChangeZeroPixelButtonMouseDownHandler()
        {
            if (ZeroPixelCorner == Corner.NorthWest)
            {
                ZeroPixelCorner = Corner.NorthEast;
            }
            else if (ZeroPixelCorner == Corner.NorthEast)
            {
                ZeroPixelCorner = Corner.SouthEast;
            }
            else if (ZeroPixelCorner == Corner.SouthEast)
            {
                ZeroPixelCorner = Corner.SouthWest;
            }
            else
            {
                ZeroPixelCorner = Corner.NorthWest;
            }
            UpdateUI();
        }

        protected override void MouseEnteredNode()
        {
            IsMouseOverNode = true;
        }

        protected override void MouseLeftNode()
        {
            IsMouseOverNode = false;
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Width) || e.PropertyName == nameof(Height))
            {
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            switch (ZeroPixelCorner)
            {
                case Corner.NorthWest:
                    ZeroPixelIndicatorHorizontalAlignment = HorizontalAlignment.Left;
                    ZeroPixelIndicatorVerticalAlignment = VerticalAlignment.Top;
                    CompassArrowIconRotation = IsStrideOrientationVertical ? 180 : 90;
                    break;

                case Corner.NorthEast:
                    ZeroPixelIndicatorHorizontalAlignment = HorizontalAlignment.Left;
                    ZeroPixelIndicatorVerticalAlignment = VerticalAlignment.Bottom;
                    CompassArrowIconRotation = IsStrideOrientationVertical ? 0 : 90;
                    break;

                case Corner.SouthEast:
                    ZeroPixelIndicatorHorizontalAlignment = HorizontalAlignment.Right;
                    ZeroPixelIndicatorVerticalAlignment = VerticalAlignment.Bottom;
                    CompassArrowIconRotation = IsStrideOrientationVertical ? 0 : 270;
                    break;

                case Corner.SouthWest:
                    ZeroPixelIndicatorHorizontalAlignment = HorizontalAlignment.Right;
                    ZeroPixelIndicatorVerticalAlignment = VerticalAlignment.Top;
                    CompassArrowIconRotation = IsStrideOrientationVertical ? 180 : 270;
                    break;
            }
        }

        private void MappedRegionChanged()
        {
            UpdatePixelData();

            // TODO: Calculate based on image control size, not node size.
            var pixelWidth = Width / RenderedImage?.Width ?? CachedPixelWidth;
            var pixelHeight = Height / RenderedImage?.Height ?? CachedPixelHeight;
            _visualRectangleWidth = (int)(Box.Width * pixelWidth);
            _visualRectangleHeight = (int)(Box.Height * pixelHeight);
            _visualRectangleLeftCanvasPosition = (int)(Box.X * pixelWidth);
            _visualRectangleTopCanvasPosition = (int)(Box.Y * pixelHeight);

            NotifyOfPropertyChange(nameof(VisualRectangleLeftCanvasPosition));
            NotifyOfPropertyChange(nameof(VisualRectangleTopCanvasPosition));
            NotifyOfPropertyChange(nameof(VisualRectangleWidth));
            NotifyOfPropertyChange(nameof(VisualRectangleHeight));
            NotifyOfPropertyChange(nameof(RegionSizeText));
            NotifyOfPropertyChange(nameof(RegionPositionText));
        }

        private void SetDragMode(object sender, MouseButtonEventArgs e)
        {
            const int dragBorderThickness = 5;
            var mousePositionRelativeToSender = e.GetPosition((IInputElement)sender);
            if (mousePositionRelativeToSender.X < dragBorderThickness)
            {
                _pixelRegionDragMode = PixelRegionDragMode.MoveLeft;
            }
            else if (mousePositionRelativeToSender.X > VisualRectangleWidth - dragBorderThickness)
            {
                _pixelRegionDragMode = PixelRegionDragMode.MoveRight;
            }
            else if (mousePositionRelativeToSender.Y < dragBorderThickness)
            {
                _pixelRegionDragMode = PixelRegionDragMode.MoveTop;
            }
            else if (mousePositionRelativeToSender.Y > VisualRectangleHeight - dragBorderThickness)
            {
                _pixelRegionDragMode = PixelRegionDragMode.MoveBottom;
            }
            else
            {
                _pixelRegionDragMode = PixelRegionDragMode.Move;
            }
        }

        private void UpdatePixelData()
        {
            // TODO: Also I think wires send updates even when data doesnt change?
            // TODO: might not be needed because technically the pointer changes through the wire even if the wire does not pass an update
            if (_frontBuffer == null || PixelData.Length != Box.Width * Box.Height * 3 && Box.Width * Box.Height * 3 > 0)
            {
                _frontBuffer = new byte[Box.Width * Box.Height * 3];
                _backBuffer = new byte[Box.Width * Box.Height * 3];
            }
            if (_onFrontBuffer)
            {
                RenderedImage?.CopyPixels(Box, _frontBuffer, ZeroPixelCorner, IsStrideOrientationVertical, ShouldAlternateStrideDirection);
                PixelData = _frontBuffer;
            }
            else
            {
                RenderedImage?.CopyPixels(Box, _backBuffer, ZeroPixelCorner, IsStrideOrientationVertical, ShouldAlternateStrideDirection);
                PixelData = _backBuffer;
            }
            _onFrontBuffer = !_onFrontBuffer;
        }

        private void ImageUpdatedHandler()
        {
            UpdatePixelData();
            if (IsMouseOverNode)
            {
                BitmapImageSource = RenderedImage.GetBitmapSource();
            }
        }
    }
}