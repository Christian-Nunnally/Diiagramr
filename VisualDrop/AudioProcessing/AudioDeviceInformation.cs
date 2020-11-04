using CSCore.CoreAudioAPI;
using PropertyChanged;
using System;
using System.Windows;

namespace VisualDrop
{
    [AddINotifyPropertyChangedInterface]
    [Serializable]
    public class AudioDeviceInformation
    {
        public AudioDeviceInformation() : this("None")
        {
        }

        public AudioDeviceInformation(string name, MMDevice device = null)
        {
            Name = name;
            Device = device;
        }

        public string Name { get; set; }

        // TODO: get rid of as this is not serializable.
        public MMDevice Device { get; }

        public string DisplayName
        {
            get
            {
                if (Name.Contains("("))
                {
                    return Name.Substring(0, Name.LastIndexOf('('));
                }
                if (Name.Contains(" - "))
                {
                    return Name.Substring(Name.IndexOf(" - ") + 3);
                }
                return Name;
            }
        }

        public FrameworkElement Icon
        {
            get
            {
                var icons = new ResourceDictionary
                {
                    Source = new Uri("/VisualDrop;component/Themes/Icons.xaml", UriKind.RelativeOrAbsolute)
                };
                var lowerName = Name.ToLower();

                if (lowerName.Contains("headphones"))
                {
                    return icons["HeadphoneIcon"] as FrameworkElement;
                }
                else if (lowerName.Contains("speakers"))
                {
                    return icons["LoudSpeakerIcon"] as FrameworkElement;
                }
                else if (lowerName.Contains("headset"))
                {
                    return icons["HeadsetIcon"] as FrameworkElement;
                }
                else if (lowerName.Contains("none"))
                {
                    return icons["MuteSpeakerIcon"] as FrameworkElement;
                }
                return icons["LoudSpeakerIcon"] as FrameworkElement;
            }
        }

        public bool IsStreaming { get; set; }
    }
}