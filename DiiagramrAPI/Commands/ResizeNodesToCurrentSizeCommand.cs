using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Editor.Diagrams;
using System.Collections.Generic;
using System.Windows;

namespace DiiagramrAPI.Commands
{
    /// <summary>
    /// An undoable command that sets a list of node to whatever thier current size is.
    /// </summary>
    public class ResizeNodesToCurrentSizeCommand : TransactingCommand
    {
        private readonly IEnumerable<Node> _nodes;
        private readonly Dictionary<Node, Size> _nodeToSizeMap = new Dictionary<Node, Size>();

        /// <summary>
        /// Creates a new instance of <see cref="ResizeNodesToCurrentSizeCommand"/>.
        /// </summary>
        /// <param name="nodes">The list of nodes to resize.</param>
        public ResizeNodesToCurrentSizeCommand(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                _nodeToSizeMap[node] = new Size(node.Width, node.Height);
            }

            _nodes = nodes;
        }

        /// <inheritdoc/>
        protected override void Execute(ITransactor transactor, object parameter)
        {
            foreach (var node in _nodes)
            {
                if (_nodeToSizeMap.ContainsKey(node))
                {
                    transactor.Transact(new ResizeNodeCommand(_nodeToSizeMap[node]), node);
                }
            }
        }
    }
}