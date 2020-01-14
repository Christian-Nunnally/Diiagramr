using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Application;
using DiiagramrModel;
using SharpDX.Mathematics.Interop;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrFadeCandy
{
    public class FadeCandyNode : Node
    {
        public LedChannelDriver[] _ledDrivers = new LedChannelDriver[NumberOfDrivers];
        private const int NumberOfDrivers = 8;
        private static bool FadeCandyConnected;
        private FadeCandyClient _fadeCandyClient;

        public FadeCandyNode()
        {
            Width = 180;
            Height = 180;
            Name = "Fade Candy";

            Drivers.CollectionChanged += Drivers_CollectionChanged;
            InitializeDriversToDefaults();
        }

        public bool ConnectButtonVisible { get; set; } = true;
        public ObservableCollection<LedChannelDriver> Drivers { get; set; } = new ObservableCollection<LedChannelDriver>();

        [OutputTerminal(Direction.North)]
        public LedChannelDriver Driver1 { get; set; } = CreateDefaultDriver(0);

        [OutputTerminal(Direction.North)]
        public LedChannelDriver Driver2 { get; set; } = CreateDefaultDriver(1);

        [OutputTerminal(Direction.North)]
        public LedChannelDriver Driver3 { get; set; } = CreateDefaultDriver(2);

        [OutputTerminal(Direction.North)]
        public LedChannelDriver Driver4 { get; set; } = CreateDefaultDriver(3);

        [OutputTerminal(Direction.North)]
        public LedChannelDriver Driver5 { get; set; } = CreateDefaultDriver(4);

        [OutputTerminal(Direction.North)]
        public LedChannelDriver Driver6 { get; set; } = CreateDefaultDriver(5);

        [OutputTerminal(Direction.North)]
        public LedChannelDriver Driver7 { get; set; } = CreateDefaultDriver(6);

        [OutputTerminal(Direction.North)]
        public LedChannelDriver Driver8 { get; set; } = CreateDefaultDriver(7);

        public LedChannelDriver SelectedDriver { get; set; }

        public bool IsDriverSelected => SelectedDriver != null;

        public double DriverButtonWidthOnView { get; set; }

        public string ServerStatusString { get; set; }

        public void ConnectFadeCandy()
        {
            ConnectButtonVisible = false;
            OnPropertyChanged(nameof(ConnectButtonVisible));

            OpenOrRestartFadeCandyServer();

            _fadeCandyClient = new FadeCandyClient("127.0.0.1", 7890, false, false);
            if (FadeCandyConnected)
            {
                return;
            }

            FadeCandyConnected = true;
            var backgroundTaskStreamingToFadeCandyServer = BackgroundTaskManager.Instance.CreateBackgroundTask(PushPixelsToFadeCandyServer, 33);
            backgroundTaskStreamingToFadeCandyServer.Start();
        }

        public void SelectDriver(LedChannelDriver driver)
        {
            if (driver == null)
            {
                SelectedDriver.IsSelected = false;
                SelectedDriver = null;
                return;
            }
            else if (driver == SelectedDriver)
            {
                SelectDriver(null);
                return;
            }
            if (SelectedDriver != null)
            {
                SelectedDriver.IsSelected = false;
            }
            SelectedDriver = driver;
            SelectedDriver.IsSelected = true;
        }

        public void MouseEnterSourceButton(object sender, MouseEventArgs e)
        {
        }

        public void MouseLeaveSourceButton(object sender, MouseEventArgs e)
        {
        }

        public void MouseDownSourceButton(object sender, MouseEventArgs e)
        {
            var ledDriver = GetLedChannelDriverFromSender(sender);
            SelectDriver(ledDriver);
        }

        private static LedChannelDriver CreateDefaultDriver(int pinNumber) => new LedChannelDriver
        {
            Box = new RawBox(0, 0, 8, 8),
            Name = "pin " + pinNumber
        };

        private void PushPixelsToFadeCandyServer()
        {
            _fadeCandyClient.PutPixels(_ledDrivers);
            ServerStatusString = _fadeCandyClient.Status;
        }

        private void InitializeDriversToDefaults()
        {
            Drivers.Add(Driver1);
            Drivers.Add(Driver2);
            Drivers.Add(Driver3);
            Drivers.Add(Driver4);
            Drivers.Add(Driver5);
            Drivers.Add(Driver6);
            Drivers.Add(Driver7);
            Drivers.Add(Driver8);
            _ledDrivers[0] = Driver1;
            _ledDrivers[1] = Driver2;
            _ledDrivers[2] = Driver3;
            _ledDrivers[3] = Driver4;
            _ledDrivers[4] = Driver5;
            _ledDrivers[5] = Driver6;
            _ledDrivers[6] = Driver7;
            _ledDrivers[7] = Driver8;
        }

        private void Drivers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DriverButtonWidthOnView = Drivers.Count == 0 ? 0 : ((Width - 10) / Drivers.Count) - 2;
        }

        private void OpenOrRestartFadeCandyServer()
        {
            KillProcess("fcserver");
            StartProcess("fcserver.exe");
        }

        private void KillProcess(string processName)
        {
            var processes = Process.GetProcesses().Where(p => p.ProcessName.Contains(processName));
            foreach (var process in processes)
            {
                process.Kill();
            }
        }

        private void StartProcess(string processName)
        {
            var enviromentPath = System.Environment.GetEnvironmentVariable("PATH");

            var paths = enviromentPath.Split(';');
            var exePath = paths.Select(x => Path.Combine(x, processName))
                               .Where(x => File.Exists(x))
                               .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(exePath) == false)
            {
                StartProcessInBackground(exePath);
            }
        }

        private void StartProcessInBackground(string exePath)
        {
            var startInfo = new ProcessStartInfo(exePath)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            var process = new Process
            {
                StartInfo = startInfo
            };
            process.Start();
        }

        private LedChannelDriver GetLedChannelDriverFromSender(object sender)
        {
            return sender is FrameworkElement frameworkElement
                ? frameworkElement.DataContext as LedChannelDriver
                : null;
        }
    }
}