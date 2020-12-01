namespace DiiagramrFadeCandy.GraphicsProcessing
{
    /// <summary>
    /// Maps an array of pixels to another array of pixels respecting that first array
    /// contains a 2d image and the resulting array should contain the same 2d image
    /// correctly flipped or rotated for display on a device that supports OPC (Open Pixel Control).
    /// </summary>
    /// <remarks>
    /// The code in the class is quite complex, there are some unit tests that validate its correctness: PixelMapperTests.
    /// </remarks>
    public static class PixelMapper
    {
        /// <summary>
        /// Maps the pixels from one image to another accounting for the given flip/rotate/stride parameters.
        /// </summary>
        /// <param name="width">The width of the area to copy.</param>
        /// <param name="height">The height of the area to copy.</param>
        /// <param name="zeroPixelCorner">The direction the first pixel of the image should be copied to. Generally the position of led #0.</param>
        /// <param name="vertical">Whether the pixels should be mapped horozontially across the output array or vertically. This changes depending on whether your LED's are phsysically wired so that led #1 is to the left/right or up/down from led #0.</param>
        /// <param name="alternateStride">Whether the direction rows are layed out in the resulting array should alternate. If your leds are wired so that the eletricity flows in alternate directions each row of pixels, you want to enable this.</param>
        /// <param name="fromBuffer">The pixel data buffer to copy from. Each pixel is three elements in the buffer (r,g,b).</param>
        /// <param name="toBuffer">The pixel data buffer to copy to. Each pixel is three elements in the buffer (r,g,b).</param>
        public static void MapPixels(int width, int height, Corner zeroPixelCorner, bool vertical, bool alternateStride, byte[] fromBuffer, byte[] toBuffer)
        {
            if (fromBuffer.Length != toBuffer.Length)
            {
                return;
            }

            switch (zeroPixelCorner)
            {
                case Corner.NorthWest:
                    MapToNorthWestCorner(width, height, vertical, alternateStride, fromBuffer, toBuffer);
                    break;

                case Corner.NorthEast:
                    MapToNorthEastCorner(width, height, vertical, alternateStride, fromBuffer, toBuffer);
                    break;

                case Corner.SouthEast:
                    MapToSouthEastCorner(width, height, vertical, alternateStride, fromBuffer, toBuffer);
                    break;

                case Corner.SouthWest:
                    MapToSouthWestCorner(width, height, vertical, alternateStride, fromBuffer, toBuffer);
                    break;
            }
        }

        private static void MapToNorthWestCorner(int width, int height, bool vertical, bool alternateStride, byte[] fromBuffer, byte[] toBuffer)
        {
            var toIndex = 0;
            if (vertical)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int fromIndex = alternateStride && x % 2 == 1
                            ? (x + ((height - y - 1) * width)) * 3
                            : (x + y * width) * 3;
                        CopyFromBufferToBuffer(fromBuffer, toBuffer, toIndex, fromIndex);
                        toIndex += 3;
                    }
                }
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int fromIndex = alternateStride && y % 2 == 1
                            ? ((width - x - 1) + (y * width)) * 3
                            : (x + y * width) * 3;
                        CopyFromBufferToBuffer(fromBuffer, toBuffer, toIndex, fromIndex);
                        toIndex += 3;
                    }
                }
            }
        }

        private static void MapToNorthEastCorner(int width, int height, bool vertical, bool alternateStride, byte[] fromBuffer, byte[] toBuffer)
        {
            var toIndex = 0;
            if (vertical)
            {
                for (int x = width - 1; x >= 0; x--)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int fromIndex = alternateStride && x % 2 == 1
                            ? (x + ((height - y - 1) * width)) * 3
                            : (x + y * width) * 3;
                        CopyFromBufferToBuffer(fromBuffer, toBuffer, toIndex, fromIndex);
                        toIndex += 3;
                    }
                }
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = width - 1; x >= 0; x--)
                    {
                        int fromIndex = alternateStride && y % 2 == 1
                            ? ((width - x - 1) + (y * width)) * 3
                            : (x + y * width) * 3;
                        CopyFromBufferToBuffer(fromBuffer, toBuffer, toIndex, fromIndex);
                        toIndex += 3;
                    }
                }
            }
        }

        private static void MapToSouthEastCorner(int width, int height, bool vertical, bool alternateStride, byte[] fromBuffer, byte[] toBuffer)
        {
            var toIndex = 0;
            if (vertical)
            {
                for (int x = width - 1; x >= 0; x--)
                {
                    for (int y = height - 1; y >= 0; y--)
                    {
                        int fromIndex = alternateStride && x % 2 == 1
                            ? (x + ((height - y - 1) * width)) * 3
                            : (x + y * width) * 3;
                        CopyFromBufferToBuffer(fromBuffer, toBuffer, toIndex, fromIndex);
                        toIndex += 3;
                    }
                }
            }
            else
            {
                for (int y = height - 1; y >= 0; y--)
                {
                    for (int x = width - 1; x >= 0; x--)
                    {
                        int fromIndex = alternateStride && y % 2 == 0
                            ? ((width - x - 1) + (y * width)) * 3
                            : (x + y * width) * 3;
                        CopyFromBufferToBuffer(fromBuffer, toBuffer, toIndex, fromIndex);
                        toIndex += 3;
                    }
                }
            }
        }

        private static void MapToSouthWestCorner(int width, int height, bool vertical, bool alternateStride, byte[] fromBuffer, byte[] toBuffer)
        {
            var toIndex = 0;
            if (vertical)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = height - 1; y >= 0; y--)
                    {
                        int fromIndex = alternateStride && x % 2 == 1
                            ? (x + ((height - y - 1) * width)) * 3
                            : (x + y * width) * 3;
                        CopyFromBufferToBuffer(fromBuffer, toBuffer, toIndex, fromIndex);
                        toIndex += 3;
                    }
                }
            }
            else
            {
                for (int y = height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int fromIndex = alternateStride && y % 2 == 0
                            ? ((width - x - 1) + (y * width)) * 3
                            : (x + y * width) * 3;
                        CopyFromBufferToBuffer(fromBuffer, toBuffer, toIndex, fromIndex);
                        toIndex += 3;
                    }
                }
            }
        }

        private static void CopyFromBufferToBuffer(byte[] fromBuffer, byte[] toBuffer, int toIndex, int fromIndex)
        {
            toBuffer[toIndex + 0] = fromBuffer[fromIndex + 0];
            toBuffer[toIndex + 1] = fromBuffer[fromIndex + 1];
            toBuffer[toIndex + 2] = fromBuffer[fromIndex + 2];
        }
    }
}