using DiiagramrAPI.Service.Interfaces;
using System.Threading.Tasks;
using System.Windows;

namespace DiiagramrAPI.Service
{
    public class DialogService : IDialogService
    {
        public Task<MessageBoxResult> Show(string prompt, string title, MessageBoxButton button)
        {
            return Task.FromResult(MessageBox.Show(prompt, title, button));
        }
    }
}