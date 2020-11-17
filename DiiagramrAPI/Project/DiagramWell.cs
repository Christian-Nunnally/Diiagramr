using DiiagramrAPI.Editor.Diagrams;
using Stylet;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Project
{
    /// <summary>
    /// Manages all open <see cref="Diagram"/>s in the application.
    /// </summary>
    public class DiagramWell : Conductor<Diagram>.Collection.OneActive
    {
        /// <summary>
        /// Opens a diagram for the user to see.
        /// </summary>
        /// <param name="diagram">The diagram to open.</param>
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

        /// <summary>
        /// Close all open diagrams.
        /// </summary>
        public void CloseAllDiagrams()
        {
            Items.Clear();
        }

        /// <summary>
        /// Occurs when the user clicks on the name of a diagram in the navigation area.
        /// </summary>
        /// <param name="sender">The diagram visual that was clicked.</param>
        /// <param name="e">The event arguments.</param>
        public void NavigateToDiagramClicked(object sender, MouseButtonEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var dataContext = frameworkElement?.DataContext;
            var diagram = dataContext as Diagram;
            OpenDiagram(diagram);
        }

        private void ReadyDiagram(Diagram diagram)
        {
            diagram.ResetPanAndZoom();
            diagram.View.Focus();
            Keyboard.Focus(diagram.View);
        }
    }
}