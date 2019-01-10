using System.Threading.Tasks;
using System.Windows;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IDialogService : IDiiagramrService
    {
        Task<MessageBoxResult> Show(string prompt, string title, MessageBoxButton button);
    }
}
