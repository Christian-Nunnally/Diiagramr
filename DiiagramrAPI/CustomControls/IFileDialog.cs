﻿using System.Windows;

namespace DiiagramrAPI.CustomControls
{
    public interface IFileDialog
    {
        string InitialDirectory { get; set; }

        string Filter { get; set; }

        string FileName { get; set; }

        MessageBoxResult ShowDialog();
    }
}
