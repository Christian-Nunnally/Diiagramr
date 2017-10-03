using System.Windows.Forms;
using Diiagramr.View.CustomControls;

namespace DiiagramrIntegrationTest.IntegrationHelpers
{
    public class TestFileDialog : IFileDialog
    {
        public string InitialDirectory { get; set; }
        public string Filter { get; set; }

        public string FileName
        {
            get { return "testProj"; }
            set { }
        }

        public DialogResult ShowDialog()
        {
            return DialogResult.OK;
        }

        public TestFileDialog()
        {
            InitialDirectory = "c://";
        }
    }
}
