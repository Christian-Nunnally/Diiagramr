using DiiagramrAPI.Service.Commands;
using Stylet;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.ViewModel
{
    public class ContextMenuViewModel : Screen
    {
        public float MinimumWidth { get; set; } = 150;
        public float X { get; set; } = 0;
        public float Y { get; set; } = 22;
        public bool Visible { get; set; }
        public ObservableCollection<IDiiagramrCommand> Commands { get; set; } = new ObservableCollection<IDiiagramrCommand>();

        public event Action<DiiagramrCommand> ExecuteCommandHandler;

        public void ExecuteCommand(object sender, MouseEventArgs e)
        {
            var control = sender as FrameworkElement;
            if (control?.DataContext is DiiagramrCommand command)
            {
                if (ExecuteCommandHandler == null)
                {
                    throw new InvalidOperationException("No command handler asssigned for the context menu view model");
                }
                Visible = false;
                ExecuteCommandHandler?.Invoke(command);
            }
        }

        public void MouseLeft()
        {
            Visible = false;
        }
    }
}
