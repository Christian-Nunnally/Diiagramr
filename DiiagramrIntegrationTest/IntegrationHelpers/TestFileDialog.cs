using DiiagramrAPI.CustomControls;
using System.Windows;

namespace DiiagramrIntegrationTest.IntegrationHelpers
{
    public class TestFileDialog : IFileDialog
    {
        public string InitialDirectory { get; set; }
        public string Filter { get; set; }

        public string FileName
        {
            get => "testProj";
            set { }
        }

        public MessageBoxResult ShowDialog()
        {
            return MessageBoxResult.OK;
        }

        public TestFileDialog()
        {
            InitialDirectory = "c://";
        }
    }
}
