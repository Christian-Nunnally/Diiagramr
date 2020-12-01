using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System.Collections.Generic;
using System.ComponentModel;

namespace DiiagramrAPI.Service.Editor
{
    /// <summary>
    /// Interface for classes that are node factories.
    /// </summary>
    public interface INodeProvider : INotifyPropertyChanged, ISingletonService
    {
        /// <summary>
        /// Create a new instance of <see cref="Node"/>.
        /// </summary>
        /// <param name="typeFullName">The name of the type of node to create.</param>
        /// <returns>The new node.</returns>
        Node CreateNodeFromName(string typeFullName);

        /// <summary>
        /// Get all nodes creatable by this <see cref="INodeProvider"/>.
        /// </summary>
        /// <returns>A list of all nodes that this <see cref="INodeProvider"/> can create.</returns>
        IEnumerable<Node> GetRegisteredNodes();

        /// <summary>
        /// Create a new instance of <see cref="Node"/>.
        /// </summary>
        /// <param name="node">The model to create the node from.</param>
        /// <returns>The new node.</returns>
        Node CreateNodeFromModel(NodeModel node);

        /// <summary>
        /// Registers a new node type with this <see cref="INodeProvider"/> so that it can create the new type in the future.
        /// </summary>
        /// <param name="node">An instance of the new type of node.</param>
        /// <param name="library">The library that the new node is part of.</param>
        void RegisterNode(Node node, NodeLibrary library);
    }
}