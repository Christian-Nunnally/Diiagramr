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
        private static SprectrumResultStream _instance;
        private readonly List<ISpectrumResultObserver> _resultSubscribers = new List<ISpectrumResultObserver>();
        private FftResultStream _fftResultStream;
        private MMDevice _lastEnabledDevice;
        private WindowFunction _windowFunction;
        private FftSize _fftSize;
        private int _fftBinCount;

        private SprectrumResultStream()
        {
            FftBinCount = 64;
            FftSize = FftSize.Fft4096;
            Gamma = 2f;
            WindowFunction = WindowFunctions.Hanning;
        }

        public static SprectrumResultStream Instance => _instance ?? (_instance = new SprectrumResultStream());

        public int FftBinCount
        {
            get => _fftBinCount;
            set
            {
                if (value < (int)_fftSize / 2 && value > 0)
                {
                    _fftBinCount = value;
                }
            }
        }

        public FftSize FftSize
        {
            get => _fftSize;
            set
            {
                _fftSize = value;
                Restart();
            }
        }

        public WindowFunction WindowFunction
        {
            get => _windowFunction;
            set
            {
                _windowFunction = value;
                Restart();
            }
        }

        public float Gamma { get; set; } = 2f;

        public MMDeviceCollection Devices => MMDeviceEnumerator.EnumerateDevices(DataFlow.Render, DeviceState.Active);

        public void Subscribe(ISpectrumResultObserver subscriber)
        {
            _resultSubscribers.Add(subscriber);
        }

        public void Unsubscribe(ISpectrumResultObserver subscriber)
        {
            _resultSubscribers.Remove(subscriber);
        }

        public void Enable(MMDevice device)
        {
            if (device is object)
            {
                _fftResultStream = new FftResultStream(FftSize, WindowFunction, device, this);
                _fftResultStream.Start();
                _lastEnabledDevice = device;
            }
        }

        public void Disable()
        {
            _fftResultStream?.Dispose();
        }

        public void Restart()
        {
            Disable();
            Enable(_lastEnabledDevice);
        }

        public void ObserveFftResult(float[] fft)
        {
            var bins = ComputeFftBins(fft);
            NotifySubscribers(bins);
        }

        private List<float> ComputeFftBins(float[] fft)
        {
            var fftIndex = 0;
            var fftResult = new List<float>(FftBinCount);
            float peak;
            for (float binIndex = 1; binIndex <= FftBinCount; binIndex++)
            {
                var nextBinEndIndex = GetNextBinEndIndex(binIndex);
                for (peak = 0; fftIndex < nextBinEndIndex; fftIndex++)
                {
                    if (peak < fft[fftIndex])
                    {
                        peak = fft[fftIndex];
                    }
                }
                fftResult.Add(peak);
            }
            return fftResult;
        }

        private double GetNextBinEndIndex(float binIndex) => (int)_fftSize / 2 * Math.Pow(binIndex / FftBinCount, Gamma);

        private void NotifySubscribers(List<float> fftResult) => _resultSubscribers.ForEach(x => x.ObserveSpectrumResults(fftResult));
    }
}