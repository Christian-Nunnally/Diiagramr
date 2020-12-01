using System;
using System.Net;
using System.Net.Sockets;

namespace DiiagramrFadeCandy
{
    public class FadeCandyClient : IDisposable
    {
        public bool _verbose;
        public bool _long_connection;
        public string _ip;
        public string _status;
        public int _port;
        public Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public Action<string> StatusUpdated;
        private const int LedsPerDevice = 64;
        private const int NumberOfDevices = 8;
        private const int HeaderByteLength = 4;
        private const int BytesPerLed = 3;
        private const int TotalNumberOfLeds = NumberOfDevices * LedsPerDevice;
        private const int BytesPerPacket = TotalNumberOfLeds * BytesPerLed;
        private const byte LengthHighByte = BytesPerPacket / 256;
        private const byte LengthLowByte = BytesPerPacket % 256;
        private const byte Channel = 0;
        private const byte Command = 0;
        private readonly byte[] _messageByteBuffer = new byte[TotalNumberOfLeds * BytesPerLed + HeaderByteLength];

        public FadeCandyClient(string ip, int port, bool long_connecton = true, bool verbose = false)
        {
            _ip = ip;
            _port = port;
            _long_connection = long_connecton;
            _verbose = verbose;

            _messageByteBuffer[0] = Channel;
            _messageByteBuffer[1] = Command;
            _messageByteBuffer[2] = LengthHighByte;
            _messageByteBuffer[3] = LengthLowByte;
        }

        public void Dispose()
        {
            if (_socket.Connected)
            {
                _socket.Dispose();
            }
            _status = "Disconnected";
        }

        public bool CanConnect()
        {
            bool success = EnsureConnected();
            if (!_long_connection)
            {
                Dispose();
            }
            return success;
        }

        public void PutPixels(byte[][] ledPixelData)
        {
            if (ledPixelData.Length != 8)
            {
                throw new NotImplementedException("Fade candy only has 8 pins");
            }

            bool is_connected = EnsureConnected();
            if (!is_connected)
            {
                return;
            }

            int bufferPosition = HeaderByteLength;
            foreach (var ledData in ledPixelData)
            {
                if (ledData.Length != LedsPerDevice * BytesPerLed)
                {
                    bufferPosition += LedsPerDevice * BytesPerLed;
                    continue;
                }

                for (int i = 0; i < LedsPerDevice * BytesPerLed;)
                {
                    _messageByteBuffer[bufferPosition++] = ledData[i++];
                    _messageByteBuffer[bufferPosition++] = ledData[i++];
                    _messageByteBuffer[bufferPosition++] = ledData[i++];
                }
            }

            try
            {
                _socket.Send(_messageByteBuffer);
            }
            catch (SocketException)
            {
                UpdateStatus("Socket closed");
            }
        }

        private void UpdateStatus(string newStatus)
        {
            _status = newStatus;
            StatusUpdated?.Invoke(newStatus);
        }

        private bool EnsureConnected()
        {
            if (_socket.Connected)
            {
                return true;
            }
            else
            {
                try
                {
                    _socket.Ttl = 1;
                    IPAddress ip = IPAddress.Parse(_ip);
                    _socket.Connect(ip, _port);
                    UpdateStatus("Connected to fcserver.exe");
                    return true;
                }
                catch (SocketException e)
                {
                    UpdateStatus("Disconnected from fcserver.exe");
                    UpdateStatus("Error: " + e.Message);
                    return false;
                }
                catch (InvalidOperationException e)
                {
                    UpdateStatus("Disconnected from fcserver.exe");
                    UpdateStatus("Error: " + e.Message);
                    return false;
                }
            }
        }
    }
}