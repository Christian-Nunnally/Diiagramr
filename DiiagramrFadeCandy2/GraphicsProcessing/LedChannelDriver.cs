using DiiagramrAPI.Editor.Diagrams;
using PropertyChanged;
using SharpDX.Mathematics.Interop;
using System;
using System.Runtime.Serialization;

namespace DiiagramrFadeCandy
{
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class LedChannelDriver : IWireableType
    {
        // The FadeCandy limits the number of leds per channel to 64.
        private const int MaxChannelSize = 64;

        private const int NumberOfLeds = 64;

        private readonly byte[] _messageByteBuffer = new byte[NumberOfLeds * 3];

        private int[] _intBuffer = new int[0];

        private byte[] _intermediateByteBuffer = new byte[0];

        private RawBox _box = new RawBox();

        private int _boxArea;

        [DataMember]
        public string Name { get; set; }

        public bool IsSelected { get; set; }
        public bool AlternateStrideDirection { get; set; } = false;

        [DataMember]
        public int X
        {
            get => Box.X;
            set => Box = new RawBox(value, Box.Y, Box.Width, Box.Height);
        }

        public string XText
        {
            get => X.ToString();

            set
            {
                if (int.TryParse(value, out int result))
                {
                    X = result;
                }
            }
        }

        [DataMember]
        public int Y
        {
            get => Box.Y;
            set => Box = new RawBox(Box.X, value, Box.Width, Box.Height);
        }

        public string YText
        {
            get => Y.ToString();

            set
            {
                if (int.TryParse(value, out int result))
                {
                    Y = result;
                }
            }
        }

        [DataMember]
        public int Width
        {
            get => Box.Width;
            set => Box = new RawBox(Box.X, Box.Y, value, Box.Height);
        }

        public string WidthText
        {
            get => Width.ToString();

            set
            {
                if (int.TryParse(value, out int result))
                {
                    Width = result;
                }
            }
        }

        [DataMember]
        public int Height
        {
            get => Box.Height;
            set => Box = new RawBox(Box.X, Box.Y, Box.Width, value);
        }

        public string HeightText
        {
            get => Height.ToString();

            set
            {
                if (int.TryParse(value, out int result))
                {
                    Height = result;
                }
            }
        }

        public RawBox Box
        {
            get => _box;

            set
            {
                _box = value;
                _boxArea = _box.Width * _box.Height;
                _intBuffer = new int[Math.Min(_boxArea, MaxChannelSize)];
                _intermediateByteBuffer = new byte[_intBuffer.Length * sizeof(int)];
            }
        }

        internal ILedDataProvider ImageDataProvider { get; set; }

        public byte[] GetLedData()
        {
            if (!ImageDataProvider?.HasData() ?? true)
            {
                return _messageByteBuffer;
            }

            if (IsLedBoxOutsideOfFrame())
            {
                return _messageByteBuffer;
            }

            if (Box.X < 0)
            {
                X = 0;
            }

            if (Box.Y < 0)
            {
                Y = 0;
            }

            if (ImageDataProvider.ImageWidth < Box.X + Box.Width)
            {
                Width = ImageDataProvider.ImageWidth - Box.X;
            }

            if (ImageDataProvider.ImageHeight < Box.Y + Box.Height)
            {
                Height = ImageDataProvider.ImageHeight - Box.Y;
            }

            if (_intBuffer.Length <= 0 || _intBuffer.Length > MaxChannelSize)
            {
                return _messageByteBuffer;
            }

            if (Box.Width * Box.Height > NumberOfLeds)
            {
                return _messageByteBuffer;
            }

            ImageDataProvider.CopyPixels(Box, _intBuffer);
            Buffer.BlockCopy(_intBuffer, 0, _intermediateByteBuffer, 0, _intermediateByteBuffer.Length);
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    var pixelIndex = row * Width + col;
                    var invertedRowPixelIndex = (row * Width) + (Width - col - 1);
                    var oddRow = row % 2 == 0;
                    var copyFromPixelIndex = AlternateStrideDirection
                        ? oddRow ? pixelIndex : invertedRowPixelIndex
                        : pixelIndex;
                    CopyIntermediateBufferToMessageBuffer(pixelIndex, copyFromPixelIndex);
                }
            }

            return _messageByteBuffer;
        }

        public System.Drawing.Color GetTypeColor()
        {
            return System.Drawing.Color.FromArgb(255, 85, 128, 85);
        }

        private bool IsLedBoxOutsideOfFrame()
        {
            return ImageDataProvider.ImageWidth < Box.X || ImageDataProvider.ImageHeight < Box.Y || Box.X + Box.Width < 0 || Box.Y + Box.Height < 0;
        }

        private void CopyIntermediateBufferToMessageBuffer(int toPixelIndex, int fromPixelIndex)
        {
            var toPixelColorIndex = toPixelIndex * 3;
            var fromPixelColorIndex = fromPixelIndex * 4;
            _messageByteBuffer[toPixelColorIndex + 0] = _intermediateByteBuffer[fromPixelColorIndex + 2];
            _messageByteBuffer[toPixelColorIndex + 1] = _intermediateByteBuffer[fromPixelColorIndex + 1];
            _messageByteBuffer[toPixelColorIndex + 2] = _intermediateByteBuffer[fromPixelColorIndex + 0];
        }
    }
}