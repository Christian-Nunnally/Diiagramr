using DiiagramrAPI.Service.Interfaces;
using System.Windows;

namespace DiiagramrAPI.CustomControls
{
    public interface IFileDialog : IDiiagramrService, IKeyedDiiagramrService
    {
        string InitialDirectory { get; set; }

        string Filter { get; set; }

        string FileName { get; set; }

        MessageBoxResult ShowDialog();
    }
}
