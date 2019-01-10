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
            _dialog = new System.Windows.Forms.SaveFileDialog();
        }

        public string InitialDirectory { get => _dialog.InitialDirectory; set => _dialog.InitialDirectory = value; }
        public string Filter { get => _dialog.Filter; set => _dialog.Filter = value; }
        public string FileName { get => _dialog.FileName; set => _dialog.FileName = value; }

        public string ServiceBindingKey => "save";

        public void Dispose()
        {
            _dialog?.Dispose();
        }

        public MessageBoxResult ShowDialog()
        {
            var result = _dialog.ShowDialog();
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
