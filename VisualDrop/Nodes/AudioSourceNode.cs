using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using VisualDrop.AudioProcessing;

namespace VisualDrop
{
    [Help("Listens to a particular audio device driver, then does an FFT on the audio stream and outputs the results of the FFT as an array of bytes. Uses the open source CSCore Audio API.")]
    public class AudioSourceNode : Node, ISpectrumResultObserver
    {
        private const int DefaultLines = 8;
        private int _lines;

        public AudioSourceNode()
        {
            Width = 120;
            Height = 60;
            Name = "Audio Source";
            Weight = 0;

            Sources.CollectionChanged += SourcesCollectionChanged;
            Sources.Add(new AudioDeviceInformation("None"));
            AudioSourceAnalyzer = SprectrumResultStream.Instance;
            var fftResultUIDispatcher = new FFTResultUIDispatcher();
            AudioSourceAnalyzer.Subscribe(fftResultUIDispatcher);
            fftResultUIDispatcher.Subscribe(this);

            GetUpdatedDeviceListWorker.DoWork += LoadSources;
            GetUpdatedDeviceListWorker.RunWorkerCompleted += SourcesLoaded;

            Lines = DefaultLines;
            GetUpdatedDeviceListWorker.RunWorkerAsync();
            FFTOutput = new float[0];
        }

        public ObservableCollection<AudioDeviceInformation> Sources { get; set; } = new ObservableCollection<AudioDeviceInformation>();

        public string DisplayedAudioSourceName { get; set; }

        public double SourceWidth { get; set; }

        [Help("If a device is enabled that has audio playing through it, this terminal will continuously output the FFT result of that audio stream.")]
        [OutputTerminal(Direction.South)]
        public float[] FFTOutput { get; set; }

        public AudioDeviceInformation OpenAudioDevice { get; set; }

        public AudioDeviceInformation OpenAudioDeviceCopy { get; set; }

        public bool Enabled { get; set; }

        public bool NotEnabled => !Enabled;

        public string ToggleEnableButtonText => Enabled ? "Enabled" : "Enable";

        public SolidColorBrush EnabledIndicatorColor => Enabled ? Brushes.LightGreen : Brushes.LightCoral;

        public int Lines
        {
            get => _lines;

            set
            {
                _lines = value;
                if (AudioSourceAnalyzer != null)
                {
                    AudioSourceAnalyzer.FftBinCount = _lines;
                }
            }
        }

        public bool IsMouseWithin { get; private set; }

        private SprectrumResultStream AudioSourceAnalyzer { get; set; }

        private BackgroundWorker GetUpdatedDeviceListWorker { get; set; } = new BackgroundWorker();

        [Help("Sets the size of the array resulting from the FFT.")]
        [InputTerminal(Direction.West)]
        public void SetLines(int lines)
        {
            if (lines <= 0)
            {
                Lines = DefaultLines;
                return;
            }
            Lines = lines;
        }

        public void AddLine()
        {
            Lines++;
        }

        public void RemoveLine()
        {
            if (Lines > 0)
            {
                Lines--;
            }
        }

        public void MouseEnterSourceButton(object sender, MouseEventArgs e)
        {
            var audioDevice = GetAudioDeviceInformationFromSender(sender);
            if (audioDevice != null)
            {
                DisplayedAudioSourceName = audioDevice.DisplayName;
            }
        }

        public void MouseLeaveSourceButton(object sender, MouseEventArgs e)
        {
            var audioDevice = GetAudioDeviceInformationFromSender(sender);
            if (audioDevice != null)
            {
                DisplayedAudioSourceName = OpenAudioDevice?.DisplayName ?? string.Empty;
            }
        }

        public void MouseDownSourceButton(object sender, MouseEventArgs e)
        {
            var audioDevice = GetAudioDeviceInformationFromSender(sender);
            if (audioDevice != null)
            {
                StartAudioDevice(audioDevice);
            }
        }

        public void ObserveSpectrumResults(List<float> fft)
        {
            FFTOutput = fft.ToArray();
        }

        protected override void MouseEnteredNode()
        {
            IsMouseWithin = true;
        }

        protected override void MouseLeftNode()
        {
            IsMouseWithin = false;
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

            if (!string.IsNullOrEmpty(OpenAudioDevice?.Name)
                && sources.Any(x => x.Name == OpenAudioDevice.Name))
            {
                return;
            }

            StartAudioDevice(Sources.FirstOrDefault());
        }

        private void LoadSources(object sender, DoWorkEventArgs e)
        {
            e.Result = AudioSourceAnalyzer.Devices.Select(x => new AudioDeviceInformation(x.FriendlyName, x));
        }

        private void StartAudioDevice(AudioDeviceInformation audioDevice)
        {
            if (OpenAudioDevice != null)
            {
                OpenAudioDevice.Running = false;
            }
            OpenAudioDevice = audioDevice;
            OpenAudioDeviceCopy = OpenAudioDevice.Copy();
            DisplayedAudioSourceName = audioDevice.DisplayName;
            if (audioDevice.Name == "none")
            {
                AudioSourceAnalyzer.Disable();
            }
            else
            {
                AudioSourceAnalyzer.Disable();
                AudioSourceAnalyzer.Enable(audioDevice.Device);
            }
            OpenAudioDevice.Running = true;
        }

        private AudioDeviceInformation GetAudioDeviceInformationFromSender(object sender)
        {
            return sender is FrameworkElement frameworkElement
                ? frameworkElement.DataContext as AudioDeviceInformation
                : null;
        }
    }
}