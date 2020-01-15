using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace VisualDrop
{
    public class AudioSourceNode : Node
    {
        private const int DefaultLines = 4;
        private int _lines;

        public AudioSourceNode()
        {
            Width = 120;
            Height = 60;
            Name = "Audio Source";
            Weight = 0;

            Sources.CollectionChanged += Sources_CollectionChanged;
            Sources.Add(new AudioDeviceInformation() { DisplayName = "None", Name = "none" });
            AudioSourceAnalyzer = CScoreAudioSourceAnalyzer.Instance;
            GetUpdatedDeviceListWorker.DoWork += LoadSources;
            GetUpdatedDeviceListWorker.RunWorkerCompleted += SourcesLoaded;

            Lines = DefaultLines;
            AudioSourceAnalyzer.AudioDataReceived += AudioSourceAnalyzerOnAudioDataReceived;

            GetUpdatedDeviceListWorker.RunWorkerAsync();
        }

        public ObservableCollection<AudioDeviceInformation> Sources { get; set; } = new ObservableCollection<AudioDeviceInformation>();

        public string DisplayedAudioSourceName { get; set; }

        public double SourceWidth { get; set; }

        [OutputTerminal(Direction.South)]
        public byte[] FFTOutput { get; set; }

        public AudioDeviceInformation RunningAudioDevice { get; set; }

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
                    AudioSourceAnalyzer.FFTBinCount = _lines;
                }
            }
        }

        private IAudioSourceAnalyzer AudioSourceAnalyzer { get; set; }

        private BackgroundWorker GetUpdatedDeviceListWorker { get; set; } = new BackgroundWorker();

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
                DisplayedAudioSourceName = RunningAudioDevice?.DisplayName ?? string.Empty;
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

        private void Sources_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SourceWidth = Sources.Count == 0 ? 0 : Width / Sources.Count;
        }

        private void SourcesLoaded(object sender, RunWorkerCompletedEventArgs e)
        {
            var sources = (IEnumerable<AudioSourceDevice>)e.Result;
            foreach (var source in sources)
            {
                if (!Sources.Any(x => x.Name == source.Name))
                {
                    Sources.Add(new AudioDeviceInformation
                    {
                        Name = source.Name
                    });
                }
            }

            if (!string.IsNullOrEmpty(RunningAudioDevice?.Name)
                && sources.Any(x => x.Name == RunningAudioDevice.Name))
            {
                return;
            }

            StartAudioDevice(Sources.FirstOrDefault());
        }

        private void LoadSources(object sender, DoWorkEventArgs e)
        {
            e.Result = AudioSourceAnalyzer.GetDevices();
        }

        private void AudioSourceAnalyzerOnAudioDataReceived(List<byte> bytes)
        {
            FFTOutput = bytes.ToArray();
        }

        private void StartAudioDevice(AudioDeviceInformation audioDevice)
        {
            if (RunningAudioDevice != null)
            {
                RunningAudioDevice.Running = false;
            }
            RunningAudioDevice = audioDevice;
            DisplayedAudioSourceName = audioDevice.DisplayName;
            if (audioDevice.Name == "none")
            {
                AudioSourceAnalyzer.Disable();
            }
            else
            {
                AudioSourceAnalyzer.Disable();
                AudioSourceAnalyzer.Enable(new AudioSourceDevice() { Name = audioDevice.Name });
            }
            RunningAudioDevice.Running = true;
        }

        private AudioDeviceInformation GetAudioDeviceInformationFromSender(object sender)
        {
            return sender is FrameworkElement frameworkElement
                ? frameworkElement.DataContext as AudioDeviceInformation
                : null;
        }
    }
}