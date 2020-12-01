using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
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
        private Direct3DEx _d3d;
        private DeviceEx _d3dDevice;
        private VertexDeclaration _vertexDecl;
        private VertexBuffer _vertices;

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
            if (_d3dDevice == null) return;
            _d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new RawColorBGRA(0, 128, 0, 255), 1.0f, 0);
            _d3dDevice.BeginScene();

            _d3dDevice.SetStreamSource(0, _vertices, 0, 20);
            _d3dDevice.VertexDeclaration = _vertexDecl;
            _d3dDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);

            _d3dDevice.EndScene();
            try
            {
                _d3dDevice.Present();
            }
            catch (SharpDXException e)
            {
            }
        }

        public void Initialize(int width, int height)
        {
            Uninitialize();

            _d3d = new Direct3DEx();
            IntPtr windowHandle = GetWpfWindowHandle();
            PresentParameters presentationParams = new PresentParameters(width, height);
            _d3dDevice = new DeviceEx(_d3d, DefaultAdapterNumber, DeviceType.Hardware, windowHandle, CreateFlags.HardwareVertexProcessing, presentationParams);
            RenderTarget = Surface.CreateRenderTarget(_d3dDevice, width, height, Format.A8R8G8B8, MultisampleType.None, 0, true);
            _d3dDevice.SetRenderTarget(0, RenderTarget);

            _vertices = new VertexBuffer(_d3dDevice, 3 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Default);
            _vertices.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                new Vertex() { Color = new RawColorBGRA(0, 0, 128, 255), Position = new Vector4(32.0f, 8.0f, 4.0f, 1.0f) },
                new Vertex() { Color = new RawColorBGRA(0, 128, 0, 255), Position = new Vector4(52f, 40.0f, 4.0f, 1.0f) },
                new Vertex() { Color = new RawColorBGRA(128, 0, 0, 255), Position = new Vector4(12f, 40.0f, 4.0f, 1.0f) }
            });
            _vertices.Unlock();

            var vertexElems = new[] {
                new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
                new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                VertexElement.VertexDeclarationEnd
            };
            _vertexDecl = new VertexDeclaration(_d3dDevice, vertexElems);
        }

        public void Uninitialize()
        {
            _d3d?.Dispose();
            _d3dDevice?.Dispose();
        }

        private static IntPtr GetWpfWindowHandle() => new WindowInteropHelper(new Window()).Handle;

        [StructLayout(LayoutKind.Sequential)]
        internal struct Vertex
        {
            public Vector4 Position;
            public RawColorBGRA Color;
        }
    }
}