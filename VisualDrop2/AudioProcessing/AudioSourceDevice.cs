using CSCore.CoreAudioAPI;

namespace VisualDrop
{
    internal class AudioSourceDevice
    {
        public static AudioSourceDevice CreateFromMMDevice(MMDevice mmDevice)
        {
            var device = new AudioSourceDevice
            {
                Name = mmDevice.FriendlyName
            };
            return device;
        }

        public AudioSourceDevice()
        {
        }

        public string Name { get; internal set; }
    }
}