using DiiagramrAPI.Editor.Diagrams;
using Stylet;
using System.Windows;
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

            if (Items.Contains(diagram))
            {
                var index = Items.IndexOf(diagram);
                for (int i = Items.Count - 1; i > index; i--)
                {
                    Items.RemoveAt(i);
                }
            }

            ActivateItem(diagram);
        }

        public void ReadyDiagram(Diagram diagram)
        {
            diagram.ResetPanAndZoom();
            diagram.View.Focus();
            Keyboard.Focus(diagram.View);
        }

        public void NavigateToDiagramClicked(object sender, MouseButtonEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var dataContext = frameworkElement?.DataContext;
            var diagram = dataContext as Diagram;
            OpenDiagram(diagram);
        }
    }
}