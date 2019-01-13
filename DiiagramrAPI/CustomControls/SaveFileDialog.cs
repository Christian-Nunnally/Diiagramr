using System.Windows;
using System.Windows.Forms;

namespace DiiagramrAPI.CustomControls
{
    public class SaveFileDialog : IFileDialog
    {
        private System.Windows.Forms.SaveFileDialog _dialog;

        public SaveFileDialog()
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
                case DialogResult.None:
                    return MessageBoxResult.None;

                case DialogResult.OK:
                    return MessageBoxResult.OK;

                case DialogResult.Yes:
                    return MessageBoxResult.Yes;

                case DialogResult.No:
                    return MessageBoxResult.No;

                default: return MessageBoxResult.Cancel;
            }
        }
    }
}
