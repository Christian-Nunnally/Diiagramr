using CSCore;
using CSCore.CoreAudioAPI;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.Streams;
using System;
using VisualDrop.AudioProcessing;

namespace VisualDrop
{
    public class FftResultStream : IDisposable, IArrayProducer
    {
        public readonly MMDevice Device;
        public readonly int FftSize;
        private readonly WasapiCapture _wasapiCapture;
        private readonly CustomFftProvider _fftProvider;
        private readonly IWaveSource _waveSource;
        private readonly float[] _fft;
        private readonly byte[] _buffer;

        public FftResultStream(FftSize fftSize, WindowFunction windowFunction, MMDevice device)
        {
            FftSize = (int)fftSize;
            Device = device;
            _fft = new float[(int)fftSize];
            _wasapiCapture = CreateWaspiCapture();
            var sampleSource = CreateSampleSource(_wasapiCapture);
            _fftProvider = new CustomFftProvider(sampleSource.WaveFormat.Channels, fftSize, windowFunction);
            _waveSource = CreateWaveSource(sampleSource, _fftProvider);
            _buffer = new byte[_waveSource.WaveFormat.BytesPerSecond / 2];
        }

        public static MMDeviceCollection Devices => MMDeviceEnumerator.EnumerateDevices(DataFlow.Render, DeviceState.Active);

        public IArrayConsumer Consumer { get; set; }

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

        private WasapiCapture CreateWaspiCapture()
        {
            var wasapiCapture = new WasapiLoopbackCapture { Device = Device };
            wasapiCapture.Initialize();
            return wasapiCapture;
        }

        private ISampleSource CreateSampleSource(WasapiCapture wasapiCapture)
        {
            var source = new SoundInSource(wasapiCapture) { FillWithZeros = false };
            source.DataAvailable += DataAvailableHandler;
            return source.ToSampleSource();
        }

        private IWaveSource CreateWaveSource(ISampleSource sampleSource, CustomFftProvider fftProvider)
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
                    Consumer.ConsumeArray(_fft);
                }
            }
        }
    }
}