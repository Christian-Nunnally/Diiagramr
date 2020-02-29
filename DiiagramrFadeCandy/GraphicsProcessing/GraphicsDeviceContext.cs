using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using d2 = SharpDX.Direct2D1;
using d3d = SharpDX.Direct3D11;

using dxgi = SharpDX.DXGI;

using wic = SharpDX.WIC;

namespace DiiagramrFadeCandy.GraphicsProcessing
{
    public class GraphicsDeviceContext
    {
        private d2.DeviceContext _d2dContext;
        private d3d.Device _defaultDevice;
        private d3d.Device1 _d3dDevice;
        private dxgi.Device _dxgiDevice;
        private d2.Device _d2dDevice;
        private d2.Bitmap1 _d2dRenderTarget;

        public GraphicsDeviceContext(int width, int height)
        {
            Initialize(width, height);
        }

        public void Render()
        {
            _d2dContext?.BeginDraw();
            _d2dContext?.DrawLine(new RawVector2(0, 0), new RawVector2(100, 100), new d2.SolidColorBrush(_d2dContext, new RawColor4(1.0f, 0.0f, 0.0f, 1.0f), null), 2);
            _d2dContext?.EndDraw();
        }

        public void Initialize(int width, int height)
        {
            Uninitialize();

            var deviceCreationFlags = d3d.DeviceCreationFlags.VideoSupport | d3d.DeviceCreationFlags.BgraSupport;
            _defaultDevice = new d3d.Device(SharpDX.Direct3D.DriverType.Hardware, deviceCreationFlags);
            _d3dDevice = _defaultDevice.QueryInterface<d3d.Device1>(); // get a reference to the Direct3D 11.1 device
            _dxgiDevice = _d3dDevice.QueryInterface<dxgi.Device>(); // get a reference to DXGI device
            _d2dDevice = new d2.Device(_dxgiDevice); // initialize the D2D device

            //Create an empty offscreen surface. Use SystemMemory to allow for surface copying.
            SharpDX.Direct3D9.Surface imageSurface = SharpDX.Direct3D9.Surface.CreateOffscreenPlain(_d3dDevice, width, height, SharpDX.Direct3D9.Format.A8R8G8B8, Pool.SystemMemory);

            //Fill the surface with the image data.
            Surface.FromFile(imageSurface, "dx.jpg", Filter.None, 0);

            _d2dContext = new d2.DeviceContext(_d2dDevice, d2.DeviceContextOptions.None);

            // specify a pixel format that is supported by both D2D and WIC
            var d2PixelFormat = new d2.PixelFormat(dxgi.Format.R8G8B8A8_UNorm, d2.AlphaMode.Premultiplied);
            // if in D2D was specified an R-G-B-A format - use the same for wic
            var wicPixelFormat = wic.PixelFormat.Format32bppPRGBA;
            var d2dBitmapProperties = new d2.BitmapProperties1(d2PixelFormat, 96, 96, d2.BitmapOptions.Target | d2.BitmapOptions.CannotDraw);
            _d2dRenderTarget = new d2.Bitmap1(_d2dContext, new Size2(width, height), d2dBitmapProperties);
            _d2dContext.Target = _d2dRenderTarget;
        }

        public void Uninitialize()
        {
            _d2dRenderTarget?.Dispose();
            _d2dContext?.Dispose();
            _d2dDevice?.Dispose();
            _dxgiDevice?.Dispose();
            _d3dDevice?.Dispose();
            _defaultDevice?.Dispose();
        }
    }
}