using DiiagramrAPI.Service.Interfaces;
using System.Windows;

namespace DiiagramrAPI.CustomControls
{
    public interface IFileDialog : IService, IKeyedService
    {
        string FileName { get; set; }
        string Filter { get; set; }
        string InitialDirectory { get; set; }

        MessageBoxResult ShowDialog();
    }
}
