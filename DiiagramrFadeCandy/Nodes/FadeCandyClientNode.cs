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
        public void SetPin0LedData(byte[] pin0Data)
        {
            if (pin0Data != null)
            {
                _ledData[0] = pin0Data;
            }
        }

        [InputTerminal(Direction.North)]
        public void SetPin1LedData(byte[] pin1Data)
        {
            if (pin1Data != null)
            {
                _ledData[1] = pin1Data;
            }
        }

        [InputTerminal(Direction.North)]
        public void SetPin2LedData(byte[] pin2Data)
        {
            if (pin2Data != null)
            {
                _ledData[2] = pin2Data;
            }
        }

        [InputTerminal(Direction.North)]
        public void SetPin3LedData(byte[] pin3Data)
        {
            if (pin3Data != null)
            {
                _ledData[3] = pin3Data;
            }
        }

        [InputTerminal(Direction.North)]
        public void SetPin4LedData(byte[] pin4Data)
        {
            if (pin4Data != null)
            {
                _ledData[4] = pin4Data;
            }
        }

        [InputTerminal(Direction.North)]
        public void SetPin5LedData(byte[] pin5Data)
        {
            if (pin5Data != null)
            {
                _ledData[5] = pin5Data;
            }
        }

        [InputTerminal(Direction.North)]
        public void SetPin6LedData(byte[] pin6Data)
        {
            if (pin6Data != null)
            {
                _ledData[6] = pin6Data;
            }
        }

        [InputTerminal(Direction.North)]
        public void SetPin7LedData(byte[] pin7Data)
        {
            if (pin7Data != null)
            {
                _ledData[7] = pin7Data;
            }
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