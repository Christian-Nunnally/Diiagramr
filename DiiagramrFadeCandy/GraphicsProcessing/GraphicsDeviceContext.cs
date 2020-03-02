using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;
using System;
using System.Windows;
using System.Windows.Interop;

namespace DiiagramrFadeCandy.GraphicsProcessing
{
    /// <summary>
    /// Wraps a <see cref="Device"/> for interoperability with WPF.
    /// </summary>
    public class GraphicsDeviceContext
    {
        private const int DefaultAdapterNumber = 0;
        private Direct3D _d3d;
        private Device _d3dDevice;

        public GraphicsDeviceContext(int width, int height)
        {
            if (Application.Current.MainWindow != null)
            {
                Initialize(width, height);
            }
        }

        public Surface RenderTarget { get; private set; }

        public void Render()
        {
            _d3dDevice?.BeginScene();
            _d3dDevice?.Clear(ClearFlags.All, new RawColorBGRA(128, 0, 0, 255), 0, 0);
            _d3dDevice?.EndScene();
        }

        public void Initialize(int width, int height)
        {
            Uninitialize();

            _d3d = new Direct3D();
            IntPtr windowHandle = GetWpfWindowHandle();
            PresentParameters presentationParams = new PresentParameters(1, 1);
            _d3dDevice = new Device(_d3d, DefaultAdapterNumber, DeviceType.Hardware, windowHandle, CreateFlags.HardwareVertexProcessing, presentationParams);
            RenderTarget = Surface.CreateRenderTarget(_d3dDevice, width, height, Format.A8R8G8B8, MultisampleType.None, 0, true);
            _d3dDevice.SetRenderTarget(0, RenderTarget);
        }

        public void Uninitialize()
        {
            _d3d?.Dispose();
            _d3dDevice?.Dispose();
        }

        private static IntPtr GetWpfWindowHandle()
        {
            return new WindowInteropHelper(Application.Current.MainWindow).Handle;
        }
    }
}