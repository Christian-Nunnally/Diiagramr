using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diiagramr.View.CustomControls
{
    class OpenFileDialog : IFileDialog
    {
        private System.Windows.Forms.OpenFileDialog _dialog;

        public OpenFileDialog()
        {
            _dialog = new System.Windows.Forms.OpenFileDialog();
        }

        public string InitialDirectory { get => _dialog.InitialDirectory; set => _dialog.InitialDirectory = value; }
        public string Filter { get => _dialog.Filter; set => _dialog.Filter = value; }
        public string FileName { get => _dialog.FileName; set => _dialog.FileName = value; }

        public DialogResult ShowDialog()
        {
            return _dialog.ShowDialog();
        }
    }
}
