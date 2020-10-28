using CSCore;
using CSCore.CoreAudioAPI;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.Streams;
using System;

namespace VisualDrop
{
    internal partial class SprectrumResultStream
    {
        private class FftResultStream : IDisposable
        {
            private readonly IFftResultObserver _resultConsumer;
            private readonly WasapiCapture _wasapiCapture;
            private readonly FftProvider _fftProvider;
            private readonly IWaveSource _waveSource;
            private readonly float[] _fft;
            private readonly byte[] _buffer;

            public FftResultStream(FftSize fftSize, WindowFunction windowFunction, MMDevice device, IFftResultObserver resultConsumer)
            {
                _resultConsumer = resultConsumer;
                _fft = new float[(int)fftSize];
                _wasapiCapture = CreateWaspiCapture(device);
                var sampleSource = CreateSampleSource(_wasapiCapture);
                _fftProvider = CreateFftProvider(sampleSource.WaveFormat.Channels, fftSize, windowFunction);
                _waveSource = CreateWaveSource(sampleSource, _fftProvider);
                _buffer = new byte[_waveSource.WaveFormat.BytesPerSecond / 2];
            }

            public void Start()
            {
                _wasapiCapture.Start();
            }

            public void Dispose()
            {
                _wasapiCapture?.Stop();
                _wasapiCapture?.Dispose();
                _waveSource?.Dispose();
            }

            private WasapiCapture CreateWaspiCapture(MMDevice device)
            {
                var wasapiCapture = new WasapiLoopbackCapture { Device = device };
                wasapiCapture.Initialize();
                return wasapiCapture;
            }

            private ISampleSource CreateSampleSource(WasapiCapture wasapiCapture)
            {
                var source = new SoundInSource(wasapiCapture) { FillWithZeros = false };
                source.DataAvailable += DataAvailableHandler;
                return source.ToSampleSource();
            }

            private FftProvider CreateFftProvider(int channels, FftSize fftSize, WindowFunction windowFunction)
                => new FftProvider(channels, fftSize) { WindowFunction = windowFunction };

            private IWaveSource CreateWaveSource(ISampleSource sampleSource, FftProvider fftProvider)
            {
                var sampleStream = new SingleBlockNotificationStream(sampleSource);
                sampleStream.SingleBlockRead += (s, a) => fftProvider.Add(a.Left, a.Right);
                return sampleStream.ToWaveSource(16);
            }

            private void DataAvailableHandler(object sender, DataAvailableEventArgs e)
            {
                ReadAllAvailableWaveSourceData();
                ProcessFftDataIfAvailable();
            }

            private void ReadAllAvailableWaveSourceData()
            {
                if (_buffer != null)
                {
                    int read = 1;
                    while (read > 0)
                    {
                        read = _waveSource.Read(_buffer, 0, _buffer.Length);
                    }
                }
            }

            private void ProcessFftDataIfAvailable()
            {
                if (_fftProvider != null)
                {
                    if (_fftProvider.GetFftData(_fft))
                    {
                        _resultConsumer.ObserveFftResult(_fft);
                    }
                }
            }
        }
    }
}