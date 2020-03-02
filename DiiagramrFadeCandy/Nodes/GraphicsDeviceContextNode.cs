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
        private readonly GraphicsDeviceContext _deviceContext;
        private int _width = 8;
        private int _height = 8;

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
            if (D3DImage.IsFrontBufferAvailable)
            {
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
        }

        private void OnRendering(object sender, EventArgs e)
        {
            UpdateScene();
        }

        private void UpdateScene()
        {
            if (D3DImage.IsFrontBufferAvailable && _deviceContext.RenderTarget != null)
            {
                _deviceContext.Render();
                D3DImage.Lock();
                D3DImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _deviceContext.RenderTarget.NativePointer);
                D3DImage.AddDirtyRect(new Int32Rect(0, 0, D3DImage.PixelWidth, D3DImage.PixelHeight));
                D3DImage.Unlock();
            }
        }
    }
}