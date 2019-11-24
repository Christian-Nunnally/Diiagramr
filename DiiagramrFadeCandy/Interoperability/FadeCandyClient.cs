using System;
using System.Net;
using System.Net.Sockets;

namespace DiiagramrFadeCandy
{
    public class FadeCandyClient : IDisposable
    {
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
        public bool _verbose;
        public bool _long_connection;
        public string _ip;
        public string Status;
        public int _port;
        public Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private int frameNumber;
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

            Debug(string.Format("{0}:{1}", _ip, _port));
        }

        private static void Debug(string message)
        {
            Console.WriteLine(message);
        }

        private bool EnsureConnected()
        {
            if (_socket.Connected)
            {
                Debug("Ensure Connected: already connected, doing nothing");
                return true;
            }
            else
            {
                try
                {
                    Debug("Ensure Connected: trying to connect...");
                    _socket.Ttl = 1;
                    IPAddress ip = IPAddress.Parse(_ip);
                    _socket.Connect(ip, _port);
                    Debug("Ensure Connected: ....success");
                    return true;
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                    Status = "Socket Error";
                    return false;
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine(e.Message);
                    Status = "Invalid Operation";
                    return false;
                }
            }
        }

        public void Dispose()
        {
            Debug("Disconnecting");
            if (_socket.Connected)
            {
                _socket.Dispose();
            }
            Status = "Disconnected";
        }

        private bool CanConnect()
        {
            bool success = EnsureConnected();
            if (!_long_connection)
            {
                Dispose();
            }
            return success;
        }

        public void PutPixels(LedChannelDriver[] drivers)
        {
            Debug("put pixels: connecting");
            bool is_connected = EnsureConnected();
            if (!is_connected)
            {
                Debug("Put pixels not connected. Ignoring these pixels.");
                Status = "Disconnected";
                return;
            }
            Status = "Connected";

            int bufferPosition = HeaderByteLength;
            foreach (var driver in drivers)
            {
                if (driver == null)
                {
                    continue;
                }

                var ledData = driver.GetLedData(frameNumber++);
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
            catch (Exception)
            {
            }
        }
    }
}
