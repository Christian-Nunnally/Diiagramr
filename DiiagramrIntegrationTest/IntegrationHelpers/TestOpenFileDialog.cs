﻿using DiiagramrAPI.CustomControls;
using DiiagramrAPI.Service.Interfaces;
using System.Windows;

namespace DiiagramrIntegrationTest.IntegrationHelpers
{
    public class TestOpenFileDialog : IFileDialog, ITestImplementationOf<OpenFileDialog>
    {
        public string InitialDirectory { get; set; }
        public string Filter { get; set; }

        public string FileName
        {
            get => "testProj";
            set { }
        }

        public string ServiceBindingKey => "test";

        public MessageBoxResult ShowDialog()
        {
            return MessageBoxResult.OK;
        }

        public TestOpenFileDialog()
        {
            InitialDirectory = "c://";
        }
    }
}
