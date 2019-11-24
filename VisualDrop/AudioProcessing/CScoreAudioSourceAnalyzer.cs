using CSCore;
using CSCore.CoreAudioAPI;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace VisualDrop
{
    /// <summary>
    /// Wraps around CScore to provide WASAPI audio capture and a FFT implementation.
    /// </summary>
    internal class CScoreAudioSourceAnalyzer : IAudioSourceAnalyzer
    {
        private const int MillisecondsBetweenData = 20;
        private static readonly FftSize FftSize = FftSize.Fft4096;
        private static CScoreAudioSourceAnalyzer _instance;
        private readonly DispatcherTimer _audioDataReceivedTimer = new DispatcherTimer(DispatcherPriority.Render);
        private readonly float[] _fft = new float[(int)FftSize];
        private readonly List<byte> _spectrumdata = new List<byte>();

        private WasapiCapture _wasapiCapture;
        private BasicSpectrumProvider _spectrumProvider;
        private IWaveSource _waveSource;
        private int _lines = 512;
        // Data is read into this buffer so that the BlockRead event is fired.
        private byte[] _dummyBuffer;

        public event Action<List<byte>> AudioDataReceived;

        private CScoreAudioSourceAnalyzer()
        {
            InitializeAudioDataReceivedTimer();
        }

        private void InitializeAudioDataReceivedTimer()
        {
            _audioDataReceivedTimer.Tick += _audioDataReceivedTimer_Tick;
            _audioDataReceivedTimer.Interval = TimeSpan.FromMilliseconds(MillisecondsBetweenData);
            _audioDataReceivedTimer.IsEnabled = false;
        }

        public static CScoreAudioSourceAnalyzer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CScoreAudioSourceAnalyzer();
                }
                return _instance;
            }
        }

        public int FFTBinCount
        {
            get => _lines;
            set
            {
                if (value < (int)FftSize.Fft4096 && value > 0)
                {
                    _lines = value;
                }
            }
        }

        public void Enable(AudioSourceDevice device)
        {
            var mmDevice = MMDeviceEnumerator.EnumerateDevices(DataFlow.Render, DeviceState.Active).FirstOrDefault(d => d.FriendlyName == device.Name);
            if (mmDevice == null)
            {
                return;
            }

            InitializeWaspiCapture(mmDevice);
            SetupSoundSource();
            EnableOutput();
        }

        private void SetupSoundSource()
        {
            var soundInSource = new SoundInSource(_wasapiCapture) { FillWithZeros = false };
            SetupSampleSource(soundInSource);
            _dummyBuffer = new byte[_waveSource.WaveFormat.BytesPerSecond / 2];
            soundInSource.DataAvailable += DataAvailableHandler;
        }

        private void EnableOutput()
        {
            _wasapiCapture.Start();
            _audioDataReceivedTimer.IsEnabled = true;
        }

        public void Disable()
        {
            DisposeOfWasapiCapture();
            DisposeOfWaveSource();
        }

        private void DisposeOfWaveSource()
        {
            if (_waveSource != null)
            {
                _waveSource.Dispose();
                _waveSource = null;
            }
        }

        private void DisposeOfWasapiCapture()
        {
            if (_wasapiCapture != null)
            {
                _wasapiCapture.Stop();
                _wasapiCapture.Dispose();
                _wasapiCapture = null;
                _audioDataReceivedTimer.IsEnabled = false;
            }
        }

        private void InitializeWaspiCapture(MMDevice device)
        {
            _wasapiCapture = new WasapiLoopbackCapture
            {
                Device = device
            };
            _wasapiCapture.Initialize();
        }

        private void _audioDataReceivedTimer_Tick(object sender, EventArgs e)
        {
            if (_spectrumProvider != null)
            {
                if (_spectrumProvider.GetFftData(_fft, this))
                {
                    ProcessFFTResults(_fft);
                }
            }
        }

        private void ProcessFFTResults(float[] fft)
        {
            int x;
            var b0 = 0;

            for (x = 0; x < FFTBinCount; x++)
            {
                float peak = 0;
                var b1 = (int)Math.Pow(2, x * 10.0 / (FFTBinCount - 1));
                b1 = Math.Min(b1, 1023);

                if (b1 <= b0)
                {
                    b1 = b0 + 1;
                }

                for (; b0 < b1; b0++)
                {
                    if (peak < fft[1 + b0])
                    {
                        peak = fft[1 + b0];
                    }
                }

                var bytePeak = ScalePeakToByte(peak);
                _spectrumdata.Add(bytePeak);
            }

            AudioDataReceived?.Invoke(_spectrumdata);
            _spectrumdata.Clear();
        }

        private byte ScalePeakToByte(float peak)
        {
            var scaledValue = (int)(Math.Sqrt(peak) * 3 * 255 - 4);
            scaledValue = Math.Max(scaledValue, 0);
            scaledValue = Math.Min(scaledValue, 255);
            return (byte)scaledValue;
        }

        public IEnumerable<AudioSourceDevice> GetDevices()
        {
            var devices = new List<AudioSourceDevice>();
            var mmDevices = MMDeviceEnumerator.EnumerateDevices(DataFlow.Render, DeviceState.Active);
            foreach (var mmDevice in mmDevices)
            {
                devices.Add(AudioSourceDevice.CreateFromMMDevice(mmDevice));
            }
            return devices;
        }

        private void SetupSampleSource(SoundInSource soundInSource)
        {
            var sampleSource = soundInSource.ToSampleSource();
            SetupSampleSource(sampleSource);
        }

        private void SetupSampleSource(ISampleSource sampleSource)
        {
            _spectrumProvider = new BasicSpectrumProvider(sampleSource.WaveFormat.Channels, sampleSource.WaveFormat.SampleRate, FftSize);
            var blockDataNotificationStream = new SingleBlockNotificationStream(sampleSource);
            blockDataNotificationStream.SingleBlockRead += (s, a) => _spectrumProvider.Add(a.Left, a.Right);
            _waveSource = blockDataNotificationStream.ToWaveSource(16);
        }

        // This doesn't do anything meaningful. For some reason it's required that the DataAvailable 
        // event be handled in order for the SingleBlockNotificationStream to fire the BlockRead event.
        private void DataAvailableHandler(object sender, DataAvailableEventArgs e)
        {
            int read = 1;
            while (read > 0)
            {
                read = _waveSource.Read(_dummyBuffer, 0, _dummyBuffer.Length);
            }
        }
    }
}