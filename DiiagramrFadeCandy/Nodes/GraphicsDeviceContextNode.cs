using DiiagramrAPI.Editor.Diagrams;
using DiiagramrFadeCandy.GraphicsProcessing;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace DiiagramrFadeCandy.Nodes
{
    public class GraphicsDeviceContextNode : Node
    {
        private const int FrameDelay = 33;
        private readonly GraphicsDeviceContext _deviceContext;
        private int _width = 8;
        private int _height = 8;
        private IntPtr _scene;

        public GraphicsDeviceContextNode()
        {
            _deviceContext = new GraphicsDeviceContext(_width, _height);

            Width = 90;
            Height = 90;
            Name = "Graphics Device Context";

            D3DImage = new D3DImage(96, 96);
            D3DImage.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;

            BeginRenderingScene();
        }

        public D3DImage D3DImage { get; set; }

        /// <summary>
        /// Creates a <see cref="SharpDX.Direct3D11.Texture2D"/> from a WIC <see cref="SharpDX.WIC.BitmapSource"/>
        /// </summary>
        /// <param name="device">The Direct3D11 device</param>
        /// <param name="bitmapSource">The WIC bitmap source</param>
        /// <returns>A Texture2D</returns>
        public static SharpDX.Direct3D11.Texture2D CreateTexture2DFromBitmap(SharpDX.Direct3D11.Device device, SharpDX.WIC.BitmapSource bitmapSource)
        {
            // Allocate DataStream to receive the WIC image pixels
            int stride = bitmapSource.Size.Width * 4;
            using (var buffer = new SharpDX.DataStream(bitmapSource.Size.Height * stride, true, true))
            {
                // Copy the content of the WIC to the buffer
                bitmapSource.CopyPixels(stride, buffer);
                return new SharpDX.Direct3D11.Texture2D(device, new SharpDX.Direct3D11.Texture2DDescription()
                {
                    Width = bitmapSource.Size.Width,
                    Height = bitmapSource.Size.Height,
                    ArraySize = 1,
                    BindFlags = SharpDX.Direct3D11.BindFlags.ShaderResource,
                    Usage = SharpDX.Direct3D11.ResourceUsage.Immutable,
                    CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                    Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                    MipLevels = 1,
                    OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.None,
                    SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                }, new SharpDX.DataRectangle(buffer.DataPointer, stride));
            }
        }

        [InputTerminal(DiiagramrModel.Direction.West)]
        public void SetWidth(int width)
        {
            _width = width;
            _deviceContext.Initialize(_width, _height);
        }

        [InputTerminal(DiiagramrModel.Direction.West)]
        public void SetHeight(int height)
        {
            _height = height;
            _deviceContext.Initialize(_width, _height);
        }

        private void BeginRenderingScene()
        {
            return;
            if (D3DImage.IsFrontBufferAvailable)
            {
                //_scene = _deviceContext.GetRenderSurfacePointer();
                D3DImage.Lock();
                D3DImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _scene);
                D3DImage.Unlock();
                CompositionTarget.Rendering += OnRendering;
            }
        }

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (D3DImage.IsFrontBufferAvailable)
            {
                BeginRenderingScene();
            }
            else
            {
                StopRenderingScene();
            }
        }

        private void StopRenderingScene()
        {
            CompositionTarget.Rendering -= OnRendering;
            _deviceContext.Uninitialize();
            _scene = IntPtr.Zero;
        }

        private void OnRendering(object sender, EventArgs e)
        {
            UpdateScene();
        }

        private void UpdateScene()
        {
            if (D3DImage.IsFrontBufferAvailable && _scene != IntPtr.Zero)
            {
                D3DImage.Lock();
                _deviceContext.Render();
                D3DImage.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
                D3DImage.Unlock();
            }
        }
    }
}