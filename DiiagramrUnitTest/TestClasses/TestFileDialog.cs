using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Diiagramr.View.CustomControls;

namespace DiiagramrUnitTests.TestClasses
{
    class TestFileDialog : IFileDialog
    {
        public DialogResult Result { get; set; }
        public string InitialDirectory { get; set; }
        public string Filter { get; set; }
        public string FileName { get; set; }

        public TestFileDialog()
        {
            Result = DialogResult.Cancel;
        }

        public DialogResult ShowDialog()
        {
            return Result;
        }
    }
}
