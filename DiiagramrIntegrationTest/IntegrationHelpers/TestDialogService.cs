using DiiagramrAPI.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiiagramrIntegrationTest.IntegrationHelpers
{
    public class TestDialogService : IDialogService
    {
        public Task<MessageBoxResult> Show(string prompt, string title, MessageBoxButton button)
        {
            return Task.FromResult(MessageBoxResult.Yes);
        }
    }
}
