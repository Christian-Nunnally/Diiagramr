using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using System.Threading.Tasks;
using System.Windows;

namespace DiiagramrIntegrationTest.IntegrationHelpers
{
    public class FakeDialogService : IDialogService, ITestImplementationOf<DialogService>
    {
        public Task<MessageBoxResult> Show(string prompt, string title, MessageBoxButton button)
        {
            return Task.FromResult(MessageBoxResult.Yes);
        }
    }
}
