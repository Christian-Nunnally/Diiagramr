using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using Diiagramr.Model;
using Diiagramr.Service;
using Stylet;

namespace Diiagramr.ViewModel.Diagram
{
    public class DiagramControlViewModel : Screen
    {
        public bool PlayChecked { get; set; }

        public bool PauseChecked { get; set; }

        private DiagramModel diagram { get; set; }

        public DiagramControlViewModel(DiagramModel dia)
        {
            diagram = dia;
            Play();
        }

        public void Play()
        {
            PauseChecked = false;
            PlayChecked = true;
            diagram.Play();
        }

        public void Pause()
        {
            PlayChecked = false;
            PauseChecked = true;
            diagram.Pause();
        }

        public void Stop()
        {
            PlayChecked = false;
            PauseChecked = false;
            diagram.Stop();
        }
    }
}
