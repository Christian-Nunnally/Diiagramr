using Stylet;
using System.ComponentModel;

namespace Diiagramr.ViewModel
{
    public class ViewModelBase : Screen
    {
        public new virtual event PropertyChangedEventHandler PropertyChanged;

        public ViewModelBase()
        {
            PropertyChanged += (sender, args) => OnPropertyChanged(args.PropertyName);
        }
    }
}
