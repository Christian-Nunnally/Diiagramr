using System;
using System.Windows;

namespace DiiagramrAPI.CustomControls
{
    public class SaveFileDialog : IFileDialog
    {
        public string InitialDirectory { get; set; }
        public string Filter { get; set; }
        public string FileName { get; set; }

        public MessageBoxResult ShowDialog()
        {
            var dialog = new System.Windows.Forms.SaveFileDialog
            {
                InitialDirectory = InitialDirectory,
                Filter = Filter,
                FileName = FileName
            };
            var result = dialog.ShowDialog();
            dialog.Dispose();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.None:
                    return MessageBoxResult.None;
                case System.Windows.Forms.DialogResult.OK:
                    return MessageBoxResult.OK;
                case System.Windows.Forms.DialogResult.Yes:
                    return MessageBoxResult.Yes;
                case System.Windows.Forms.DialogResult.No:
                    return MessageBoxResult.No;
                default: return MessageBoxResult.Cancel;
            }
        }
    }
}
