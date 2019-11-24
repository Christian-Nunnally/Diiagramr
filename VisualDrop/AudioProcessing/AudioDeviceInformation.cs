using PropertyChanged;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
            if (Name.Contains("headphones") || Name.Contains("Headphones"))
            {
                Icon = new BitmapImage(new Uri("pack://application:,,,/VisualDrop;component/Resources/headphoneicon.png"));
            }
            else if (Name.Contains("speakers") || Name.Contains("Speakers"))
            {
                Icon = new BitmapImage(new Uri("pack://application:,,,/VisualDrop;component/Resources/speakericon.png"));
            }
            else if (Name.Contains("none"))
            {
                Icon = new BitmapImage(new Uri("pack://application:,,,/VisualDrop;component/Resources/noneicon.png"));
            }
            else
            {
                Icon = new BitmapImage(new Uri("pack://application:,,,/VisualDrop;component/Resources/questionicon.png"));
            }
        }

        public string DisplayName { get; set; }

        [XmlIgnore]
        public ImageSource Icon { get; set; }

        public bool Running { get; set; }
    }
}
