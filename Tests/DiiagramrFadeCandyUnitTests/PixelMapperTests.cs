using DiiagramrFadeCandy.GraphicsProcessing;
using Xunit;

namespace DiiagramrFadeCandyUnitTests
{
    public class PixelMapperTests
    {
        [Theory]
        [InlineData(3, 2, Corner.NorthWest, false, false, new byte[] { 0, 1, 2, 3, 4, 5 })]
        [InlineData(3, 2, Corner.NorthWest, true, false, new byte[] { 0, 3, 1, 4, 2, 5 })]
        [InlineData(3, 2, Corner.NorthWest, false, true, new byte[] { 0, 1, 2, 5, 4, 3 })]
        [InlineData(3, 2, Corner.NorthWest, true, true, new byte[] { 0, 3, 4, 1, 2, 5 })]

        [InlineData(3, 2, Corner.NorthEast, false, false, new byte[] { 2, 1, 0, 5, 4, 3 })]
        [InlineData(3, 2, Corner.NorthEast, true, false, new byte[] { 2, 5, 1, 4, 0, 3 })]
        [InlineData(3, 2, Corner.NorthEast, false, true, new byte[] { 2, 1, 0, 3, 4, 5 })]
        [InlineData(3, 2, Corner.NorthEast, true, true, new byte[] { 2, 5, 4, 1, 0, 3 })]

        [InlineData(3, 2, Corner.SouthWest, false, false, new byte[] { 3, 4, 5, 0, 1, 2 })]
        [InlineData(3, 2, Corner.SouthWest, true, false, new byte[] { 3, 0, 4, 1, 5, 2 })]
        [InlineData(3, 2, Corner.SouthWest, false, true, new byte[] { 3, 4, 5, 2, 1, 0 })]
        [InlineData(3, 2, Corner.SouthWest, true, true, new byte[] { 3, 0, 1, 4, 5, 2 })]

        [InlineData(3, 2, Corner.SouthEast, false, false, new byte[] { 5, 4, 3, 2, 1, 0 })]
        [InlineData(3, 2, Corner.SouthEast, true, false, new byte[] { 5, 2, 4, 1, 3, 0 })]
        [InlineData(3, 2, Corner.SouthEast, false, true, new byte[] { 5, 4, 3, 0, 1, 2 })]
        [InlineData(3, 2, Corner.SouthEast, true, true, new byte[] { 5, 2, 1, 4, 3, 0 })]
        public void MapPixels_ExpectedMapping(int width, int height, Corner zeroPixelCorner, bool vertical, bool alternateStride, byte[] expectedOutput)
        {
            var input = ExtendArray(new byte[] { 0, 1, 2, 3, 4, 5 });
            expectedOutput = ExtendArray(expectedOutput);
            var output = new byte[expectedOutput.Length];

            PixelMapper.MapPixels(width, height, zeroPixelCorner, vertical, alternateStride, input, output);

            for (int i = 0; i < expectedOutput.Length; i++)
            {
                Assert.True(expectedOutput[i] == output[i], $"Expected: [{string.Join(", ", expectedOutput)}] Actual: [{string.Join(", ", output)}]");
            }
        }

        private byte[] ExtendArray(byte[] input)
        {
            var result = new byte[input.Length * 3];
            for (int i = 0; i < input.Length; i++)
            {
                result[(i * 3) + 0] = input[i];
                result[(i * 3) + 1] = input[i];
                result[(i * 3) + 2] = input[i];
            }
            return result;
        }
    }
}