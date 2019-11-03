using System;

namespace DiiagramrAPI.Editor.Diagrams
{
    /// <summary>
    /// Class that provides an API that is as simple as possible to make creating nodes easy.
    /// </summary>
    public class NodeSetup
    {
        private readonly Node _nodeViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeSetup" /> class.
        /// </summary>
        /// <param name="nodeViewModel">The node view model.</param>
        public NodeSetup(Node nodeViewModel)
        {
            _nodeViewModel = nodeViewModel ?? throw new ArgumentNullException(nameof(nodeViewModel));
        }

        /// <summary>
        /// Allows the node to be resized via an adorner that appears when the node is selected.
        /// </summary>
        public void EnableResize()
        {
            _nodeViewModel.ResizeEnabled = true;
        }

        /// <summary>
        /// Sets the name of a node, this is what displays above the node on the diagram.
        /// </summary>
        /// <param name="name"></param>
        public void NodeName(string name)
        {
            _nodeViewModel.Name = name;
        }

        /// <summary>
        /// Sets the initial node geometry.
        /// </summary>
        /// <param name="width">The width of the node.</param>
        /// <param name="height">The height of the node.</param>
        public void NodeSize(int width, int height)
        {
            if (Math.Abs(_nodeViewModel.Model.Width) < 0.01)
            {
                _nodeViewModel.Width = width;
            }

            if (Math.Abs(_nodeViewModel.Model.Height) < 0.01)
            {
                _nodeViewModel.Height = height;
            }

            _nodeViewModel.MinimumHeight = height;
            _nodeViewModel.MinimumWidth = width;
        }

        /// <summary>
        /// Sets the weight of the node in the node selector (where it gets inserted in the list).
        /// </summary>
        /// <param name="weight">Usually between 0 and 1, 1 being at the bottom of the list and 0 being at the top of the list.</param>
        public void NodeWeight(float weight)
        {
            _nodeViewModel.Weight = weight;
        }
    }
}