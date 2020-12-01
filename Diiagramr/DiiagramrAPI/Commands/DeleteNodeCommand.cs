using DiiagramrAPI.Application.Commands;
using DiiagramrAPI.Editor.Diagrams;
using System;

namespace DiiagramrAPI.Commands
{
    /// <summary>
    /// Undoable command that deletes a single node.
    /// </summary>
    public class DeleteNodeCommand : IReversableCommand
    {
        private readonly Diagram _diagram;

        /// <summary>
        /// Creates a new instance of <see cref="DeleteNodeCommand"/>.
        /// </summary>
        /// <param name="diagram">The diagram to delete the node from.</param>
        public DeleteNodeCommand(Diagram diagram)
        {
            _diagram = diagram;
        }

        /// <inheritdoc/>
        public Action Execute(object parameter)
        {
            if (parameter is Node node)
            {
                _diagram.RemoveNode(node);
                return () => _diagram.AddNode(node);
            }

            return () => { };
        }
    }
}