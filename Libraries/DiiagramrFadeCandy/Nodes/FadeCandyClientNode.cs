using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Application;
using DiiagramrModel;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DiiagramrFadeCandy
{
    public class FadeCandyClientNode : Node
    {
        public LedChannelDriver[] _ledDrivers = new LedChannelDriver[NumberOfDrivers];
        private const int NumberOfDrivers = 8;
        private readonly byte[][] _ledData = new byte[8][];
        private BackgroundTask _backgroundTaskStreamingToFadeCandyServer;
        private FadeCandyClient _fadeCandyClient;
        private Process _fadeCandyServerProcess;

        public FadeCandyClientNode()
        {
            Width = 180;
            Height = 180;
            Name = "FadeCandy Client";
            InitializeLedData();
        }

        public ObservableCollection<string> NodeStatusOutput { get; set; } = new ObservableCollection<string>();

        public bool ConnectButtonVisible { get; set; } = true;

        [InputTerminal(Direction.North)]
        public byte[] SetPin0LedData
        {
            get => _ledData[0];
            set => _ledData[0] = value ?? new byte[0];
        }

        [InputTerminal(Direction.North)]
        public byte[] SetPin1LedData
        {
            get => _ledData[1];
            set => _ledData[1] = value ?? new byte[0];
        }

        [InputTerminal(Direction.North)]
        public byte[] SetPin2LedData
        {
            get => _ledData[2];
            set => _ledData[2] = value ?? new byte[0];
        }

        [InputTerminal(Direction.North)]
        public byte[] SetPin3LedData
        {
            get => _ledData[3];
            set => _ledData[3] = value ?? new byte[0];
        }

        [InputTerminal(Direction.North)]
        public byte[] SetPin4LedData
        {
            get => _ledData[4];
            set => _ledData[4] = value ?? new byte[0];
        }

        [InputTerminal(Direction.North)]
        public byte[] SetPin5LedData
        {
            get => _ledData[5];
            set => _ledData[5] = value ?? new byte[0];
        }

        [InputTerminal(Direction.North)]
        public byte[] SetPin6LedData
        {
            get => _ledData[6];
            set => _ledData[6] = value ?? new byte[0];
        }

        [InputTerminal(Direction.North)]
        public byte[] SetPin7LedData
        {
            get => _ledData[7];
            set => _ledData[7] = value ?? new byte[0];
        }

        public void ConnectFadeCandy()
        {
            StartAndConnectToServer(useNoWindow: false);
        }

        public void ConnectFadeCandyBackground()
        {
            StartAndConnectToServer(useNoWindow: true);
        }

        private void StartAndConnectToServer(bool useNoWindow)
        {
            ConnectButtonVisible = false;
            StopStreamingDataToFadeCandyServer();

            RestartFadeCandyServer(useNoWindow);
            InitializeNewFadeCandyClient();

            StartStreamingDataToFadeCandyServer();
        }

        private void StartStreamingDataToFadeCandyServer()
        {
            _backgroundTaskStreamingToFadeCandyServer = BackgroundTaskManager.Instance.CreateBackgroundTask(PushPixelsToFadeCandyServer, 33);
            _backgroundTaskStreamingToFadeCandyServer.Start();
        }

        private void StopStreamingDataToFadeCandyServer()
        {
            _backgroundTaskStreamingToFadeCandyServer?.Cancel();
            _backgroundTaskStreamingToFadeCandyServer = null;
        }

        private void InitializeLedData()
        {
            for (int i = 0; i < _ledData.Length; i++)
            {
                _ledData[i] = new byte[64 * 3];
            }
        }

        private void InitializeNewFadeCandyClient()
        {
            if (_fadeCandyClient != null)
            {
                _fadeCandyClient.StatusUpdated -= ClientStatusUpdatedHandler;
                _fadeCandyClient.Dispose();
            }
            _fadeCandyClient = new FadeCandyClient("127.0.0.1", 7890, false, false);
            _fadeCandyClient.StatusUpdated += ClientStatusUpdatedHandler;
        }

        private void ClientStatusUpdatedHandler(string newStatus)
        {
            AddOutputLine(newStatus);
        }

        private void PushPixelsToFadeCandyServer()
        {
            _fadeCandyClient.PutPixels(_ledData);
        }

        private void RestartFadeCandyServer(bool createNoWindow)
        {
            KillProcess("fcserver");
            var fcserverPath = FindProcessPathFromPathEnvironmentVariable("fcserver.exe");
            StartProcess(fcserverPath, createNoWindow);
        }

        private void KillProcess(string processName)
        {
            var processes = Process.GetProcesses().Where(p => p.ProcessName.Contains(processName));
            foreach (var process in processes)
            {
                process.Kill();
            }
        }

        private string FindProcessPathFromPathEnvironmentVariable(string processName)
        {
            var enviromentPath = Environment.GetEnvironmentVariable("PATH");

            var paths = enviromentPath.Split(';');
            return paths.Select(x => Path.Combine(x, processName))
                               .Where(x => File.Exists(x))
                               .FirstOrDefault();
        }

        private void StartProcess(string exePath, bool createNoWindow = true)
        {
            if (_fadeCandyServerProcess != null)
            {
                _fadeCandyServerProcess.Exited -= FadeCandyServerProcessExited;
            }
            _fadeCandyServerProcess = new Process
            {
                StartInfo = new ProcessStartInfo(exePath)
                {
                    CreateNoWindow = createNoWindow,
                    UseShellExecute = false,
                }
            };
            _fadeCandyServerProcess.EnableRaisingEvents = true;
            _fadeCandyServerProcess.Start();
            _fadeCandyServerProcess.Exited += FadeCandyServerProcessExited;
            AddOutputLine("fcserver.exe process started");
        }

        private void FadeCandyServerProcessExited(object sender, EventArgs e)
        {
            StopStreamingDataToFadeCandyServer();
            AddOutputLine("fcserver.exe process exited");
            ConnectButtonVisible = true;
        }

        private void AddOutputLine(string line)
        {
            View?.Dispatcher.Invoke(() =>
            {
                NodeStatusOutput.Add(line);
                if (NodeStatusOutput.Count > 10)
                {
                    NodeStatusOutput.RemoveAt(10);
                }
            });
        }
    }
}