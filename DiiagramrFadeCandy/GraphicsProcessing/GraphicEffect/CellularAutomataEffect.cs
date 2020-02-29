using SharpDX.Direct2D1;
using System;
using System.Runtime.Serialization;

namespace DiiagramrFadeCandy
{
    [Serializable]
    public class CellularAutomataEffect : GraphicEffect
    {
        private readonly Random _random = new Random();
        private readonly uint[] intPixelData;
        private int _colorChangeMode = 3;

        public CellularAutomataEffect()
        {
            intPixelData = new uint[Width * Height];
            Randomize();
        }

        [DataMember]
        public float[] SpectrumData { get; set; }

        [DataMember]
        public Color Color { get; set; }

        [DataMember]
        public int Width { get; set; } = 128;

        [DataMember]
        public int Height { get; set; } = 128;

        [DataMember]
        public float BarWidth { get; set; } = 1f;

        public override void Draw(RenderTarget target)
        {
            Step();
            Bitmap backBufferBitmap = new Bitmap(target, new SharpDX.Size2(Width, Height), new BitmapProperties(target.PixelFormat));
            backBufferBitmap.CopyFromMemory(intPixelData, Width * 4);
            target.DrawBitmap(backBufferBitmap, 1.0f, BitmapInterpolationMode.Linear);
        }

        public void Randomize()
        {
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    intPixelData[row * Width + col] = GetRandomColor();
                }
            }
            _colorChangeMode = _random.Next(10);
        }

        private uint GetRandomColor()
        {
            byte[] bytes = new byte[4];
            _random.NextBytes(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        private void Step()
        {
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    if (_random.NextDouble() > 0.5)
                    {
                        var currentIndex = row * Width + col;
                        var currentColor = intPixelData[currentIndex];
                        var newIndex = PickRandomCardinalIndex(currentIndex);
                        if (ShouldChangeColor(intPixelData[newIndex], currentColor))
                        {
                            intPixelData[newIndex] = currentColor;
                        }
                    }
                }
            }
        }

        private bool ShouldChangeColor(uint oldColor, uint newColor)
        {
            return _colorChangeMode switch
            {
                0 => oldColor < newColor,
                1 => oldColor > newColor,
                2 => (oldColor & 255) > (newColor & 255),
                3 => (oldColor & 255) < (newColor & 255),
                4 => (oldColor & 65280) > (newColor & 65280),
                5 => (oldColor & 65280) < (newColor & 65280),
                6 => (oldColor & 16711680) > (newColor & 16711680),
                7 => (oldColor & 16711680) < (newColor & 16711680),
                8 => (oldColor & 4278190080) > (newColor & 4278190080),
                9 => (oldColor & 4278190080) < (newColor & 4278190080),
                _ => true,
            };
        }

        private int PickRandomCardinalIndex(int startingIndex)
        {
            return (_random.Next(0, 4)) switch
            {
                0 => (startingIndex + Width) % intPixelData.Length,
                1 => (startingIndex + intPixelData.Length - Width) % intPixelData.Length,
                2 => (startingIndex + 1) % intPixelData.Length,
                3 => (startingIndex + intPixelData.Length - 1) % intPixelData.Length,
                _ => throw new InvalidOperationException(),
            };
        }
    }
}