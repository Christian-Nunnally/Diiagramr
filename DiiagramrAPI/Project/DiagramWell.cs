using DiiagramrAPI.Editor.Diagrams;
using Stylet;

namespace DiiagramrAPI.Project
{
    public class DiagramWell : Conductor<Diagram>.StackNavigation, IDiagramViewer
    {
        public void OpenDiagram(Diagram diagram)
        {
            if (diagram == null)
            {
                return;
            }
            diagram.ExecuteWhenViewLoaded(() => diagram.ResetPanAndZoom());
            ActiveItem = diagram;
        }
    }
}