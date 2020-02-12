using PropertyChanged;
using System;
using System.Windows;
using System.Xml.Serialization;

namespace VisualDrop
{
    [AddINotifyPropertyChangedInterface]
    [Serializable]
    public class AudioDeviceInformation
    {
        private string _name;

        public string Name
        {
            get => _name;

            set
            {
                _name = value;
                SetDisplayName();
                SetIcon();
            }
        }

        public string DisplayName { get; set; } = string.Empty;

        [XmlIgnore]
        public FrameworkElement Icon { get; set; }

        public bool Running { get; set; }

        public AudioDeviceInformation Copy() => new AudioDeviceInformation() { Name = Name };

        private void SetDisplayName()
        {
            if (_name.Contains("("))
            {
                DisplayName = _name.Substring(0, _name.LastIndexOf('('));
            }
            if (DisplayName.Contains(" - "))
            {
                DisplayName = DisplayName.Substring(DisplayName.IndexOf(" - ") + 3);
            }
        }

        private void SetIcon()
        {
            var icons = new ResourceDictionary
            {
                Source = new Uri("/VisualDrop;component/Themes/Icons.xaml", UriKind.RelativeOrAbsolute)
            };

            if (Name.ToLower().Contains("headphones"))
            {
                Icon = icons["HeadphoneIcon"] as FrameworkElement;
            }
            else if (Name.ToLower().Contains("speakers"))
            {
                Icon = icons["LoudSpeakerIcon"] as FrameworkElement;
            }
            else if (Name.ToLower().Contains("headset"))
            {
                Icon = icons["HeadsetIcon"] as FrameworkElement;
            }
            else if (Name.Contains("none"))
            {
                Icon = icons["MuteSpeakerIcon"] as FrameworkElement;
            }
            else
            {
                Icon = icons["LoudSpeakerIcon"] as FrameworkElement;
            }
        }
    }
}