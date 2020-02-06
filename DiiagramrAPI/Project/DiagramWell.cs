using DiiagramrAPI.Editor.Diagrams;
using Stylet;
using System.Windows.Input;

namespace DiiagramrAPI.Project
{
    public class DiagramWell : Conductor<Diagram>.Collection.OneActive
    {
        public void OpenDiagram(Diagram diagram)
        {
            if (diagram == null)
            {
                return;
            }
            diagram.ExecuteWhenViewLoaded(() => ReadyDiagram(diagram));
            ActivateItem(diagram);
        }

        public void ReadyDiagram(Diagram diagram)
        {
            diagram.View.Focus();
            Keyboard.Focus(diagram.View);
        }
    }
}