namespace DiiagramrFadeCandy.GraphicsProcessing
{
    public static class PixelMapper
    {
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