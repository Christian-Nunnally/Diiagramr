using System;
using System.Collections.Generic;

namespace VisualDrop
{
    internal interface IAudioSourceAnalyzer
    {
        int FFTBinCount { get; set; }

        void Enable(AudioSourceDevice deviceName);
        void Disable();

        IEnumerable<AudioSourceDevice> GetDevices();

        event Action<List<byte>> AudioDataReceived;
    }
}