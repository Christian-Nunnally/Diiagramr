using SharpDX.Mathematics.Interop;

namespace DiiagramrFadeCandy
{
    internal interface ILedDataProvider
    {
        int WicImageWidth { get; }

        int WicImageHeight { get; }

        void CopyPixels(RawBox box, int[] intBuffer);

        bool HasData();
    }
}