using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using System;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Bitmap = System.Drawing.Bitmap;
using BitmapSource = System.Windows.Media.Imaging.BitmapSource;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using Rectangle = System.Drawing.Rectangle;
using WicBitmap = SharpDX.WIC.Bitmap;

namespace DiiagramrFadeCandy
{
    public class LedMatrixViewModel : Node, ILedDataProvider
    {
        private const int FrameDelay = 33;
        private static readonly ImagingFactory wicFactory = new ImagingFactory();
        private static readonly SharpDX.Direct2D1.Factory d2dFactory = new SharpDX.Direct2D1.Factory();
        private static readonly PixelFormat pixelFormat = new PixelFormat(Format.B8G8R8A8_UNorm_SRgb, AlphaMode.Unknown);
        private static readonly RenderTargetProperties renderTargetProperties = new RenderTargetProperties(RenderTargetType.Default, pixelFormat, 0, 0, RenderTargetUsage.None, FeatureLevel.Level_DEFAULT);
        private static readonly bool ClearBeforeFrame = true;
        private static readonly RawColor4 Black = new RawColor4(0, 0, 0, 1);
        private readonly object _bitmapLock = new object();

        private int _bitmapWidth = 8;
        private int _bitmapHeight = 8;
        private Size _bitmapSize = new Size(8, 8);
        private WicBitmap _cachedBitmap;
        private WicRenderTarget _cachedRenderTarget;
        private int _lastRenderedFrame;

        public LedMatrixViewModel()
        {
            Width = 60;
            Height = 60;
            Name = "Led Matrix";
            ResizeEnabled = true;

            // TODO: Don't just always update the frame.
            // TODO: Clean up resources when application is closed.
            new Thread(() =>
            {
                while (true)
                {
                    if (View != null)
                    {
                        Thread.Sleep(FrameDelay);
                        View.Dispatcher.Invoke(() =>
                        {
                            RenderFrame(_lastRenderedFrame + 1);
                        });
                    }
                }
            }).Start();
        }

        public ObservableCollection<LedChannelDriver> Drivers { get; set; } = new ObservableCollection<LedChannelDriver>();

        public ObservableCollection<GraphicEffect> Effects { get; set; } = new ObservableCollection<GraphicEffect>();

        [NodeSetting]
        public int BitmapWidth
        {
            get => _bitmapWidth;

            set
            {
                _bitmapWidth = value;
                BitmapSize = new Size(_bitmapWidth, _bitmapHeight);
            }
        }

        [NodeSetting]
        public int BitmapHeight
        {
            get => _bitmapHeight;

            set
            {
                _bitmapHeight = value;
                BitmapSize = new Size(_bitmapWidth, _bitmapHeight);
            }
        }

        public Size BitmapSize
        {
            get => _bitmapSize;

            set
            {
                _bitmapSize = value;
                CreateAndCacheBitmap();
            }
        }

        public WicRenderTarget RenderTarget => _cachedRenderTarget ?? CreateAndCacheRenderTarget();

        public LedChannelDriver SelectedDriver { get; private set; }

        public BitmapSource BitmapImageSource { get; set; }

        public bool IsDriverSelected => SelectedDriver != null;

        public int ImageWidth => WicBitmap.Size.Width;

        public int ImageHeight => WicBitmap.Size.Height;

        private WicBitmap WicBitmap
        {
            get => _cachedBitmap ?? CreateAndCacheBitmap();

            set
            {
                _cachedBitmap = value;
                _cachedRenderTarget = null;
            }
        }

        [InputTerminal("Add Effect", Direction.North)]
        public void SetEffect(GraphicEffect data)
        {
            if (data == null)
            {
                return;
            }

            Effects.Clear();
            foreach (var effect in NodeModel.Terminals.SelectMany(t => t.ConnectedWires).Select(w => w.SourceTerminal.Data).OfType<GraphicEffect>())
            {
                Effects.Add(effect);
            }
        }

        [InputTerminal("Width", Direction.West)]
        public void BitmapWidthTerminalDataChanged(int width)
        {
            if (width != 0)
            {
                BitmapWidth = width;
            }
        }

        [InputTerminal("Height", Direction.West)]
        public void BitmapHeightTerminalDataChanged(int height)
        {
            if (height != 0)
            {
                BitmapHeight = height;
            }
        }

        [InputTerminal("Driver", Direction.West)]
        public void LedDriverTerminalDataChanged(LedChannelDriver data)
        {
            if (data == null)
            {
                return;
            }
            foreach (var driver in Drivers)
            {
                driver.ImageDataProvider = null;
            }
            Drivers.Clear();
            foreach (var driver in NodeModel.Terminals.SelectMany(t => t.ConnectedWires).Select(w => w.SourceTerminal.Data).OfType<LedChannelDriver>())
            {
                Drivers.Add(driver);
                driver.ImageDataProvider = this;
            }
        }

        public void CopyPixels(RawBox box, int[] intBuffer)
        {
            if (WicBitmap != null)
            {
                lock (_bitmapLock)
                {
                    WicBitmap.CopyPixels(box, intBuffer);
                }
            }
        }

        public bool HasData()
        {
            return WicBitmap != null;
        }

        private WicBitmap CreateAndCacheBitmap()
        {
            try
            {
                lock (_bitmapLock)
                {
                    WicBitmap = new WicBitmap(
                        wicFactory,
                        _bitmapSize.Width < 1 ? 1 : (int)_bitmapSize.Width,
                        _bitmapSize.Height < 1 ? 1 : (int)_bitmapSize.Height,
                        SharpDX.WIC.PixelFormat.Format32bppBGR,
                        BitmapCreateCacheOption.CacheOnLoad);
                }
                foreach (var driver in Drivers)
                {
                    driver.ImageDataProvider = this;
                }
                return WicBitmap;
            }
            catch (SharpDXException)
            {
                return null;
            }
        }

        private WicRenderTarget CreateAndCacheRenderTarget()
        {
            _cachedRenderTarget = new WicRenderTarget(d2dFactory, WicBitmap, renderTargetProperties);
            return _cachedRenderTarget;
        }

        private void RenderFrame(int frameNumber)
        {
            if (_lastRenderedFrame == frameNumber)
            {
                return;
            }
            _lastRenderedFrame = frameNumber;

            lock (_bitmapLock)
            {
                RenderTarget.BeginDraw();
                DrawOnRenderTarget();
                RenderTarget.EndDraw();
            }

            UpdateViewImageSource();
        }

        private void UpdateViewImageSource()
        {
            if (View != null)
            {
                var bitmap = CopyWicBitmapToBitmap();
                View.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                {
                    BitmapImageSource = ConvertBitmapToSource(bitmap);
                }));
            }
        }

        private void DrawOnRenderTarget()
        {
            ClearFrame();
            DrawEffects();

            var selectedDriver = Drivers.FirstOrDefault(d => d.IsSelected);
            if (selectedDriver != null)
            {
                var brush = new SharpDX.Direct2D1.SolidColorBrush(RenderTarget, new RawColor4(64, 64, 64, 128));
                var left = selectedDriver.X + 0.2f;
                var top = selectedDriver.Y + 0.2f;
                var right = selectedDriver.X + selectedDriver.Width - .4f;
                var bottom = selectedDriver.Y + selectedDriver.Height - .4f;
                var rectangle = new RawRectangleF(left, top, right, bottom);
                RenderTarget.DrawRectangle(rectangle, brush, 0.3f);
            }
        }

        private void DrawEffects()
        {
            foreach (var effect in Effects)
            {
                effect.Draw(RenderTarget);
            }
        }

        private void ClearFrame()
        {
            if (ClearBeforeFrame)
            {
                RenderTarget.Clear(Black);
            }
        }

        private BitmapSource ConvertBitmapToSource(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgra32, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);
            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }

        private Bitmap CopyWicBitmapToBitmap()
        {
            var pixelData = new byte[BitmapWidth * BitmapHeight * 4];
            lock (_bitmapLock)
            {
                WicBitmap.CopyPixels(pixelData, BitmapWidth * 4);
            }
            var bitmap = new Bitmap(BitmapWidth, BitmapHeight);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, BitmapWidth, BitmapHeight), ImageLockMode.WriteOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            Marshal.Copy(pixelData, 0, bitmapData.Scan0, pixelData.Length);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }
    }
}