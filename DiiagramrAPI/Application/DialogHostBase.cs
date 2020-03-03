using System.Windows;
using System.Windows.Input;
using static DiiagramrAPI.Application.Dialog;

namespace DiiagramrAPI.Application
{
    public abstract class DialogHostBase : ViewModel
    {
        public abstract Dialog ActiveDialog { get; set; }

        public abstract void OpenDialog(Dialog dialog);

        public abstract void CloseDialog();

        public void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        public void CommandBarActionClickedHandler(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is DialogCommandBarCommand command)
            {
                command.Action();
            }
        }
    }
}