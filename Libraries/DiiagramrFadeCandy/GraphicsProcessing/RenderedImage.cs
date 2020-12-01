using DiiagramrAPI.Editor.Diagrams;
using DiiagramrFadeCandy.Utility;
using SharpDX.Mathematics.Interop;
using System;
using System.Runtime.Serialization;
using Bitmap = SharpDX.WIC.Bitmap;

namespace DiiagramrFadeCandy.GraphicsProcessing
{
    [Serializable]
    public class RenderedImage : IWireableType
    {
        private readonly object _bitmapLock = new object();

        [NonSerialized]
        private readonly Bitmap _bitmap;

        private int[] _intBuffer = new int[0];

        private byte[] _tempByteBuffer = new byte[0];

        private byte[] _preMapBuffer = new byte[0];

        public RenderedImage(Bitmap bitmap, int bitmapWidth, int bitmapHeight)
        {
            _bitmap = bitmap;
            BitmapWidth = bitmapWidth;
            BitmapHeight = bitmapHeight;
        }

        public RenderedImage()
        {
        }

        [field: NonSerialized]
        public event Action ImageUpdated;

        [DataMember]
        public int BitmapWidth { get; set; }

        [DataMember]
        public int BitmapHeight { get; set; }

        public int Width => _bitmap?.Size.Width ?? 0;

        public int Height => _bitmap?.Size.Height ?? 0;

        public void NotifyImageUpdated()
        {
            ImageUpdated?.Invoke();
        }

        public void CopyPixels(RawBox box, byte[] byteBuffer, Corner zeroPixelCorner, bool vertical, bool alternateStride)
        {
            if (IsBoxAValidRegion(box))
            {
                UpdateBufferSize(box);
                CopyWicPixelsToBuffer(box);
                CopyIntBufferToPreMapBuffer();
                PixelMapper.MapPixels(box.Width, box.Height, zeroPixelCorner, vertical, alternateStride, _preMapBuffer, byteBuffer);
            }
        }

        public System.Windows.Media.Imaging.BitmapSource GetBitmapSource()
        {
            if (_bitmap != null)
            {
                return BitmapConverter.GetBitmapSourceFromWicBitmap(_bitmap, BitmapWidth, BitmapHeight, _bitmapLock);
            }
            return null;
        }

        public System.Drawing.Color GetTypeColor()
        {
            return System.Drawing.Color.FromArgb(128, 128, 80);
        }

        private void CopyIntBufferToPreMapBuffer()
        {
            Buffer.BlockCopy(_intBuffer, 0, _tempByteBuffer, 0, _tempByteBuffer.Length);
            if (_preMapBuffer.Length != (_tempByteBuffer.Length / 4) * 3)
            {
                _preMapBuffer = new byte[(_tempByteBuffer.Length / 4) * 3];
            }
            for (int i = 0; i * 4 < _tempByteBuffer.Length; i++)
            {
                _preMapBuffer[i * 3 + 0] = _tempByteBuffer[i * 4 + 0];
                _preMapBuffer[i * 3 + 1] = _tempByteBuffer[i * 4 + 1];
                _preMapBuffer[i * 3 + 2] = _tempByteBuffer[i * 4 + 2];
            }
        }

        private void UpdateBufferSize(RawBox box)
        {
            var boxArea = box.Width * box.Height;
            if (_intBuffer.Length != boxArea)
            {
                _intBuffer = new int[boxArea];
                _tempByteBuffer = new byte[_intBuffer.Length * sizeof(int)];
            }
        }

        private bool IsBoxAValidRegion(RawBox box)
        {
            if (box.Width <= 0
                || box.Height <= 0
                || box.X + box.Width > BitmapWidth
                || box.Y + box.Height > BitmapHeight
                || box.X < 0
                || box.Y < 0)
            {
                return false;
            }
            return true;
        }

        private void CopyWicPixelsToBuffer(RawBox box)
        {
            if (_bitmap != null)
            {
                lock (_bitmapLock)
                {
                    _bitmap.CopyPixels(box, _intBuffer);
                }
            }
        }
    }
}