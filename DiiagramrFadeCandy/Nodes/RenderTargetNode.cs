using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Application;
using DiiagramrFadeCandy.GraphicsProcessing;
using DiiagramrFadeCandy.Utility;
using DiiagramrModel;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using System.Collections.Generic;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using BitmapSource = System.Windows.Media.Imaging.BitmapSource;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using WicBitmap = SharpDX.WIC.Bitmap;

namespace DiiagramrFadeCandy
{
    public class RenderTargetNode : Node, ILedDataProvider
    {
        public static readonly SharpDX.Direct2D1.Factory d2dFactory = new SharpDX.Direct2D1.Factory();
        private const int FrameDelay = 33;
        private static readonly ImagingFactory wicFactory = new ImagingFactory();
        private static readonly PixelFormat pixelFormat = new PixelFormat(Format.B8G8R8A8_UNorm_SRgb, AlphaMode.Unknown);
        private static readonly RenderTargetProperties renderTargetProperties = new RenderTargetProperties(RenderTargetType.Default, pixelFormat, 0, 0, RenderTargetUsage.None, FeatureLevel.Level_DEFAULT);
        private static readonly RawColor4 Black = new RawColor4(0, 0, 0, 1);
        private readonly object _bitmapLock = new object();
        private readonly BackgroundTask _backgroundRefreshTask;
        private bool _cleared;
        private int _bitmapWidth = 8;
        private int _bitmapHeight = 8;
        private System.Drawing.Size _bitmapSize = new System.Drawing.Size(8, 8);
        private WicBitmap _cachedBitmap;
        private WicRenderTarget _cachedRenderTarget;
        private int _lastRenderedFrame;

        public RenderTargetNode()
        {
            base.Width = 90;
            base.Height = 90;
            Name = "Render Target";
            ResizeEnabled = true;

            _backgroundRefreshTask = BackgroundTaskManager.Instance.CreateBackgroundTask(RenderFrameOnUIThread, FrameDelay);
            _backgroundRefreshTask.Start();
        }

        [NodeSetting]
        public int BitmapWidth
        {
            get => _bitmapWidth;
            set
            {
                _bitmapWidth = value;
                BitmapSize = new System.Drawing.Size(_bitmapWidth, _bitmapHeight);
            }
        }

        [NodeSetting]
        public int BitmapHeight
        {
            get => _bitmapHeight;
            set
            {
                _bitmapHeight = value;
                BitmapSize = new System.Drawing.Size(_bitmapWidth, _bitmapHeight);
            }
        }

        public System.Drawing.Size BitmapSize
        {
            get => _bitmapSize;

            set
            {
                _bitmapSize = value;
                CreateAndCacheBitmap();
            }
        }

        public WicRenderTarget RenderTarget => _cachedRenderTarget ?? CreateAndCacheRenderTarget();

        public BitmapSource BitmapImageSource { get; set; }

        public int WicImageWidth => WicBitmap.Size.Width;

        public int WicImageHeight => WicBitmap.Size.Height;

        [OutputTerminal(Direction.South)]
        [Help("A reference to the rendered image, to be sent out for display on another device, like an Arduino, FadeCandy, or for additional processing steps.")]
        public RenderedImage RenderedImage { get; set; }

        [InputTerminal(Direction.West)]
        [Help("Sets whether or not to clear the surface with black before each frame is drawn.")]
        public bool ClearBeforeFrame { get; set; }

        [InputTerminal(Direction.North, isCoalescing: true)]
        [Help("The list of effects to render on the surface.")]
        [NodeSetting]
        public List<GraphicEffect> Effects { get; set; } = new List<GraphicEffect>();

        [InputTerminal(Direction.West)]
        [Help("The width of the render surface, in pixels.")]
        public int ImageWidth
        {
            get => BitmapWidth;
            set
            {
                if (value > 0)
                {
                    BitmapWidth = value;
                }
            }
        }

        [InputTerminal(Direction.West)]
        [Help("The height of the render surface, in pixels.")]
        public int ImageHeight
        {
            get => BitmapHeight;
            set
            {
                if (value > 0)
                {
                    BitmapHeight = value;
                }
            }
        }

        private WicBitmap WicBitmap
        {
            get => _cachedBitmap ?? CreateAndCacheBitmap();

            set
            {
                _cachedBitmap = value;
                _cachedRenderTarget = null;
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

        private void RenderFrameOnUIThread()
        {
            View?.Dispatcher.Invoke(() =>
            {
                RenderFrame(_lastRenderedFrame + 1);
            });
        }

        private WicBitmap CreateAndCacheBitmap()
        {
            try
            {
                var bitmapWidth = _bitmapSize.Width < 1 ? 1 : (int)_bitmapSize.Width;
                var bitmapHeight = _bitmapSize.Height < 1 ? 1 : (int)_bitmapSize.Height;
                lock (_bitmapLock)
                {
                    WicBitmap = new WicBitmap(
                        wicFactory,
                        bitmapWidth,
                        bitmapHeight,
                        SharpDX.WIC.PixelFormat.Format32bppPBGRA,
                        BitmapCreateCacheOption.CacheOnLoad);
                }
                RenderedImage = new RenderedImage(WicBitmap, bitmapWidth, bitmapHeight);
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

            RenderedImage.NotifyImageUpdated();
            UpdateViewImageSource();
        }

        private void UpdateViewImageSource()
        {
            if (View != null)
            {
                BitmapImageSource = BitmapConverter.GetBitmapSourceFromWicBitmap(WicBitmap, BitmapWidth, BitmapHeight, _bitmapLock);
            }
        }

        private void DrawOnRenderTarget()
        {
            ClearFrame();
            DrawEffects();
        }

        private void DrawEffects()
        {
            foreach (var effect in Effects)
            {
                effect?.Draw(RenderTarget);
            }
        }

        private void ClearFrame()
        {
            if (ClearBeforeFrame || !_cleared)
            {
                _cleared = true;
                RenderTarget.Clear(Black);
            }
        }
    }
}