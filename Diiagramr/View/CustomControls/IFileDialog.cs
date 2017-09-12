using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diiagramr.View.CustomControls
{
    public interface IFileDialog
    {
        string InitialDirectory { get; set; }

        string Filter { get; set; }

        string FileName { get; set; }

        DialogResult ShowDialog();
    }
}
