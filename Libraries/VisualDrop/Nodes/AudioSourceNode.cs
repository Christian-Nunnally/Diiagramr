using CSCore.CoreAudioAPI;
using CSCore.DSP;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace VisualDrop
{
    [Help("Listens to a particular audio device driver for sound samples. After enough samples have been taken, applies a window function and does a FFT on the audio sample. Then calculates the maximum power level in a number of ranges. The number of ranges and the distribution of frequencies in each range can be configured. Uses the open source CSCore Audio API to compute the FFT and interact with WASAPI.")]
    public class AudioSourceNode : Node, IArrayConsumer
    {
        private SpectrumResultStream _spectrumResultStream;
        private FftResultStream _fftResultStream;
        private WindowFunctionType _windowFunction = WindowFunctionType.Hamming;
        private FftSize _fftSize = FftSize.Fft8192;
        private float _maxVolume = 0f;

        public AudioSourceNode()
        {
            Width = 180;
            Height = 60;
            Name = "Audio Source";
            Weight = 0;

            Sources.CollectionChanged += SourcesCollectionChanged;
            Sources.Add(new AudioDeviceInformation());

            InitializeAudioSpectrumToUIDispatcherStream();
            LoadAudioSources();
        }

        public float MasterVolumeBarWidth { get; set; } = 38;

        public ObservableCollection<AudioDeviceInformation> Sources { get; set; } = new ObservableCollection<AudioDeviceInformation>();

        public string DisplayedAudioSourceName { get; set; }

        public double SourceWidth { get; set; }

        public AudioDeviceInformation OpenAudioDevice { get; set; }

        [Help("When a device is enabled that has audio playing through it, this terminal will continuously output the frequency spectrum computed using an FFT.")]
        [OutputTerminal(Direction.South)]
        public float[] SpectrumOutput { get; set; } = new float[0];

        [NodeSetting]
        [InputTerminal(Direction.East)]
        [Help("Gamma = 1 results in even distribution over low and high frequencies.\n\nGamma > 1 results in more lower frequency bins than higher frequency bins.")]
        public float Gamma
        {
            get => _spectrumResultStream.Gamma;
            set => _spectrumResultStream.Gamma = value;
        }

        [NodeSetting]
        [InputTerminal(Direction.East)]
        [Help("The number of bins the FFT results are bucketed in to. The range of each bin is defined by the `Gamma` terminal")]
        public int BinCount
        {
            get => _spectrumResultStream.SpectrumBinCount;
            set => _spectrumResultStream.SpectrumBinCount = Math.Max(1, Math.Min((int)_fftSize, value));
        }

        [InputTerminal(Direction.West)]
        [Help("The window function to apply to the sampled data before computing the FFT.")]
        public WindowFunctionType WindowFunction
        {
            get => _windowFunction;
            set
            {
                _windowFunction = value;
                ResetFftResultStream(_fftResultStream?.Device);
            }
        }

        [InputTerminal(Direction.West)]
        [Help("How many sound samples to take before measuring the spectrum data with an FFT.")]
        public FftSize FftSize
        {
            get => _fftSize;
            set
            {
                _fftSize = value;
                ResetFftResultStream(_fftResultStream?.Device);
            }
        }

        /// <summary>
        /// Called when the mouse enters an audio source button.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void MouseEnterSourceButton(object sender, MouseEventArgs e)
        {
            if (TryGetAudioDeviceInformationFromSender(sender, out var audioDevice))
            {
                DisplayedAudioSourceName = audioDevice.DisplayName;
            }
        }

        /// <summary>
        /// Called when the mouse leaves an audio source button.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void MouseLeaveSourceButton(object sender, MouseEventArgs e)
        {
            if (TryGetAudioDeviceInformationFromSender(sender, out var _))
            {
                DisplayedAudioSourceName = OpenAudioDevice?.DisplayName ?? string.Empty;
            }
        }

        /// <summary>
        /// Called when a mouse button is pressed over an audio source button.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void MouseDownSourceButton(object sender, MouseEventArgs e)
        {
            if (TryGetAudioDeviceInformationFromSender(sender, out var audioDevice))
            {
                StartAudioDevice(audioDevice);
            }
        }

        /// <inheritdoc/>
        public void ConsumeArray(float[] spectrum)
        {
            SpectrumOutput = spectrum;
            var masterVolume = spectrum.Sum() / spectrum.Length;
            _maxVolume *= .99999f;
            if (masterVolume > _maxVolume)
            {
                _maxVolume = masterVolume;
            }
            MasterVolumeBarWidth = 38 - ((masterVolume / _maxVolume) * 38);
        }

        /// <inheritdoc/>
        protected override void DisposeExtraResources()
        {
            ResetFftResultStream();
        }

        private void InitializeAudioSpectrumToUIDispatcherStream()
        {
            var fftResultUIDispatcher = new ArrayResultDispatcher(DispatcherPriority.Render);
            fftResultUIDispatcher.Consumer = this;
            _spectrumResultStream = new SpectrumResultStream();
            _spectrumResultStream.Consumer = fftResultUIDispatcher;
        }

        private void LoadAudioSources()
        {
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += LoadSources;
            backgroundWorker.RunWorkerCompleted += SourcesLoaded;
            backgroundWorker.RunWorkerAsync();
        }

        private void SourcesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SourceWidth = Sources.Count == 0 ? 0 : Width / Sources.Count;
        }

        private void SourcesLoaded(object sender, RunWorkerCompletedEventArgs e)
        {
            var sources = (IEnumerable<AudioDeviceInformation>)e.Result;
            foreach (var source in sources)
            {
                if (!Sources.Any(x => x.Name == source.Name))
                {
                    Sources.Add(source);
                }
            }

            if (string.IsNullOrEmpty(OpenAudioDevice?.Name) && sources.Any())
            {
                StartAudioDevice(Sources.FirstOrDefault());
            }
        }

        private void LoadSources(object sender, DoWorkEventArgs e)
        {
            e.Result = FftResultStream.Devices.Select(x => new AudioDeviceInformation(x.FriendlyName, x));
        }

        private void StartAudioDevice(AudioDeviceInformation audioDevice)
        {
            if (OpenAudioDevice != null)
            {
                OpenAudioDevice.IsStreaming = false;
            }
            OpenAudioDevice = audioDevice;
            DisplayedAudioSourceName = audioDevice.DisplayName;
            ResetFftResultStream(audioDevice.Device);
            OpenAudioDevice.IsStreaming = true;
        }

        private void ResetFftResultStream(MMDevice device = null)
        {
            _fftResultStream?.Dispose();
            _fftResultStream = null;
            if (device is object)
            {
                _fftResultStream = new FftResultStream(_fftSize, _windowFunction.GetWindowFunctionFromType(), device);
                _fftResultStream.Start();
                _fftResultStream.Consumer = _spectrumResultStream;
            }
        }

        private bool TryGetAudioDeviceInformationFromSender(object sender, out AudioDeviceInformation audioDeviceInformation)
        {
            var frameworkElement = sender as FrameworkElement;
            return (audioDeviceInformation = frameworkElement?.DataContext as AudioDeviceInformation) is object;
        }
    }
}