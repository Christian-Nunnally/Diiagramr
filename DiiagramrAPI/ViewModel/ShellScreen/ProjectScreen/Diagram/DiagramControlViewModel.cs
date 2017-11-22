using DiiagramrAPI.Model;
using Stylet;

namespace DiiagramrAPI.ViewModel.Diagram
{
    public class DiagramControlViewModel : Screen
    {

        public DiagramControlViewModel(DiagramModel diagram)
        {
            Diagram = diagram;
            Play();
        }

        public bool PlayChecked { get; set; }

        public bool PauseChecked { get; set; }

        public bool StopChecked { get; set; }

        private DiagramModel Diagram { get; }

        public void Play()
        {
            PauseChecked = false;
            StopChecked = false;
            PlayChecked = true;
            Diagram.Play();
        }

        public void Pause()
        {
            PlayChecked = false;
            PauseChecked = true;
            StopChecked = false;
            Diagram.Pause();
        }

        public void Stop()
        {
            PlayChecked = false;
            PauseChecked = false;
            StopChecked = true;
            Diagram.Stop();
        }
    }
}