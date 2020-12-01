using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WicBitmap = SharpDX.WIC.Bitmap;

namespace DiiagramrFadeCandy.Utility
{
    public static class BitmapConverter
    {
        public static BitmapSource ConvertBitmapToSource(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var bitmapSource = BitmapSource.Create(
                bitmapData.Width,
                bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgra32,
                null,
                bitmapData.Scan0,
                bitmapData.Stride * bitmapData.Height,
                bitmapData.Stride);
            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }

        public static Bitmap CopyWicBitmapToBitmap(WicBitmap wicBitmap, int width, int height, object wicBitmapLock)
        {
            var pixelData = new byte[width * height * 4];
            lock (wicBitmapLock)
            {
                wicBitmap.CopyPixels(pixelData, width * 4);
            }
            var bitmap = new Bitmap(width, height);
            var lockRectangle = new Rectangle(0, 0, width, height);
            var pixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppPArgb;
            var lockFlags = ImageLockMode.WriteOnly;
            var bitmapData = bitmap.LockBits(lockRectangle, lockFlags, pixelFormat);
            Marshal.Copy(pixelData, 0, bitmapData.Scan0, pixelData.Length);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        public static BitmapSource GetBitmapSourceFromWicBitmap(WicBitmap wicBitmap, int width, int height, object wicBitmapLock)
        {
            Bitmap bitmap = CopyWicBitmapToBitmap(wicBitmap, width, height, wicBitmapLock);
            return ConvertBitmapToSource(bitmap);
        }
    }
}