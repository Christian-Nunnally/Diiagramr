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
        private int _width = 64;
        private int _height = 64;
        private bool _backBufferSet = false;
        private bool _renderingScene = false;

        public GraphicsDeviceContextNode()
        {
            _deviceContext = new GraphicsDeviceContext(_width, _height);
            Width = 90;
            Height = 90;
            Name = "Graphics Device Context";
            ResizeEnabled = true;
            D3DImage = new D3DImage(96, 96);
            D3DImage.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;

            BeginRenderingScene();
        }

        public D3DImage D3DImage { get; set; }

        [InputTerminal(DiiagramrModel.Direction.West)]
        public int SetWidth
        {
            get => _width;
            set
            {
                _width = value;
                _deviceContext.Initialize(_width, _height);
            }
        }

        [InputTerminal(DiiagramrModel.Direction.West)]
        public int SetHeight
        {
            get => _height;
            set
            {
                _height = value;
                _deviceContext.Initialize(_width, _height);
            }
        }

        private void BeginRenderingScene()
        {
            if (D3DImage.IsFrontBufferAvailable && !_renderingScene)
            {
                CompositionTarget.Rendering += OnRendering;
                _renderingScene = true;
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
            _renderingScene = false;
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
                if (D3DImage.TryLock(new Duration(TimeSpan.FromMilliseconds(250))))
                {
                    if (!_backBufferSet)
                    {
                        D3DImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _deviceContext.RenderTarget.NativePointer);
                        _backBufferSet = true;
                    }
                    D3DImage.AddDirtyRect(new Int32Rect(0, 0, D3DImage.PixelWidth, D3DImage.PixelHeight));
                    D3DImage.Unlock();
                }
            }
        }
    }
}