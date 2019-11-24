using SharpDX.Mathematics.Interop;

namespace DiiagramrFadeCandy
{
    internal interface ILedDataProvider
    {
        void CopyPixels(RawBox box, int[] intBuffer);

        int ImageWidth { get; }
        int ImageHeight { get; }

        bool HasData();
    }
}
