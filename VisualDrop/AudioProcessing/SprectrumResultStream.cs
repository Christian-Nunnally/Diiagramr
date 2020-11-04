using CSCore.CoreAudioAPI;
using CSCore.DSP;
using System;
using System.Collections.Generic;
using VisualDrop.AudioProcessing;

namespace VisualDrop
{
    /// <summary>
    /// Wraps around CScore to provide WASAPI audio capture and a FFT implementation.
    /// </summary>
    internal partial class SprectrumResultStream : ISpectrumResultNotifier, IFftResultObserver
    {
        private readonly List<ISpectrumResultObserver> _observers = new List<ISpectrumResultObserver>();
        private int _fftBinCount = 64;

        public SprectrumResultStream()
        {
            FftBinCount = 64;
            FftSize = FftSize.Fft4096;
            Gamma = 2f;
        }

        public int FftBinCount
        {
            get => _fftBinCount;
            set
            {
                if (value < (int)FftSize / 2 && value > 0)
                {
                    _fftBinCount = value;
                }
            }
        }

        public FftSize FftSize { get; set; }

        public float Gamma { get; set; } = 2f;

        public float Rotation { get; set; } = 0f;

        public MMDeviceCollection Devices => MMDeviceEnumerator.EnumerateDevices(DataFlow.Render, DeviceState.Active);

        public void Subscribe(ISpectrumResultObserver observer)
        {
            _observers.Add(observer);
        }

        public void Unsubscribe(ISpectrumResultObserver observer)
        {
            _observers.Remove(observer);
        }

        public void ObserveFftResult(float[] fft)
        {
            var bins = ComputeFftBins(fft);
            NotifySubscribers(bins);
        }

        private List<float> ComputeFftBins(float[] fft)
        {
            var fftIndex = 0;
            var binEndIndex = 0;
            var fftResult = new List<float>(FftBinCount);
            float peak;
            for (float binIndex = 1; binIndex <= FftBinCount; binIndex++)
            {
                binEndIndex = (int)Math.Max(binEndIndex + 1, GetNextBinEndIndex(binIndex));
                for (peak = 0; fftIndex < binEndIndex && fftIndex < fft.Length; fftIndex++)
                {
                    var value = fft[fftIndex] * (1 + (Rotation * binIndex));
                    if (peak < value)
                    {
                        peak = value;
                    }
                }
                fftResult.Add(peak);
            }
            return fftResult;
        }

        private double GetNextBinEndIndex(float binIndex) => (int)FftSize / 2 * Math.Pow(binIndex / FftBinCount, Gamma);

        private void NotifySubscribers(List<float> fftResult) => _observers.ForEach(x => x.ObserveSpectrumResults(fftResult));
    }
}