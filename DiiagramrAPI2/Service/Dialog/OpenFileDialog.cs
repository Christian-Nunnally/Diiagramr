using System;
using System.Windows;

namespace DiiagramrAPI.Service.Dialog
{
    public sealed class OpenFileDialog : IFileDialog, IDisposable
    {
        // private readonly System.Windows.Forms.OpenFileDialog _dialog;

        public OpenFileDialog()
        {
            // throw new NotSupportedException("Not yet supported in .net core version");
            // _dialog = new System.Windows.Forms.OpenFileDialog();
        }

        public string FileName { get; set; }

        // public string Filter { get => _dialog.Filter; set => _dialog.Filter = value; }
        public string Filter { get; set; }

        // public string InitialDirectory { get => _dialog.InitialDirectory; set => _dialog.InitialDirectory = value; }
        public string InitialDirectory { get; set; }

        public string ServiceBindingKey => "open";

        public void Dispose()
        {
            // _dialog?.Dispose();
        }

        public MessageBoxResult ShowDialog()
        {
            throw new NotSupportedException("Not yet supported in .net core version");

            //var result = _dialog.ShowDialog();
            //switch (result)
            //{
            //    case System.Windows.Forms.DialogResult.None:
            //        return MessageBoxResult.None;

            //    case System.Windows.Forms.DialogResult.OK:
            //        return MessageBoxResult.OK;

            //    case System.Windows.Forms.DialogResult.Yes:
            //        return MessageBoxResult.Yes;

            //    case System.Windows.Forms.DialogResult.No:
            //        return MessageBoxResult.No;

            //    default: return MessageBoxResult.Cancel;
            //}
        }
    }
}