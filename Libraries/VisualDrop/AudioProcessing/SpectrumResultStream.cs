using System;

namespace VisualDrop
{
    /// <summary>
    /// Wraps around CScore to provide WASAPI audio capture and a FFT implementation.
    /// </summary>
    public class SpectrumResultStream : IArrayProducer, IArrayConsumer
    {
        private int _spectrumBinCount = 64;

        public SpectrumResultStream()
        {
            SpectrumBinCount = 64;
            Gamma = 2f;
        }

        public int SpectrumBinCount
        {
            get => _spectrumBinCount;
            set
            {
                if (value > 0)
                {
                    _spectrumBinCount = value;
                }
            }
        }

        public float Gamma { get; set; } = 1f;

        public IArrayConsumer Consumer { get; set; }

        public bool ShowFullFft { get; set; } = false;

        public void ConsumeArray(float[] fft)
        {
            var bins = ComputeSpectrumBins(fft);
            Consumer.ConsumeArray(bins);
        }

        private float[] ComputeSpectrumBins(float[] fft)
        {
            var fftStartIndexForBin = 0;
            var fftEndIndexForBin = 0;
            SpectrumBinCount = Math.Min(fft.Length, SpectrumBinCount);
            var result = new float[SpectrumBinCount];
            for (int binIndex = 0; binIndex < result.Length; binIndex++)
            {
                var nextBinEndIndex = GetNextBinEndIndex(binIndex, SpectrumBinCount, fft.Length);
                fftStartIndexForBin += fftEndIndexForBin - fftStartIndexForBin;
                fftEndIndexForBin = Math.Max(fftEndIndexForBin + 1, nextBinEndIndex);
                result[binIndex] = FindMaxValueInArray(fft, fftStartIndexForBin, fftEndIndexForBin);
            }
            return result;
        }

        private float FindMaxValueInArray(float[] array, int startIndex, int endIndex)
        {
            float max = 0;
            for (int i = startIndex; i < endIndex && i < array.Length; i++)
            {
                max = Math.Max(max, array[i]);
            }
            return max;
        }

        private int GetNextBinEndIndex(float binIndex, float binCount, int fftLength)
        {
            var spectrumBins = ShowFullFft ? fftLength : fftLength / 2;
            return (int)(spectrumBins * Math.Pow(binIndex / binCount, Gamma));
        }
    }
}