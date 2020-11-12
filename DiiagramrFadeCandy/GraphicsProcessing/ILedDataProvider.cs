using SharpDX.Mathematics.Interop;

namespace DiiagramrFadeCandy
{
    internal interface ILedDataProvider
    {
        void CopyPixels(RawBox box, int[] intBuffer);

        int WicImageWidth { get; }
        int WicImageHeight { get; }

        bool HasData();
    }
}
