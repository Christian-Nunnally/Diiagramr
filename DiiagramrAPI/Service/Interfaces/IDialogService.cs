using System.Threading.Tasks;
using System.Windows;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IDialogService : IService
    {
        Task<MessageBoxResult> Show(string prompt, string title, MessageBoxButton button);
    }
}