using DiiagramrAPI.Model;
using Stylet;

namespace DiiagramrAPI.ViewModel.Diagram
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
