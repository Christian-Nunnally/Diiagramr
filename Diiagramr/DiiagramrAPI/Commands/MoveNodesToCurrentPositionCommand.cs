using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Editor.Diagrams;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DiiagramrAPI.Commands
{
    /// <summary>
    /// An undoable command that moves a list of nodes to their current position.
    /// </summary>
    public class MoveNodesToCurrentPositionCommand : TransactingCommand
    {
        private readonly IEnumerable<Node> _nodes;
        private readonly Dictionary<Node, Point> _nodeToPositionMap = new Dictionary<Node, Point>();

        /// <summary>
        /// Creates a new instance of <see cref="MoveNodesToCurrentPositionCommand"/>.
        /// </summary>
        /// <param name="nodes">The list of nodes to move.</param>
        public MoveNodesToCurrentPositionCommand(IEnumerable<Node> nodes)
        {
            _nodes = nodes ?? Enumerable.Empty<Node>();
            foreach (var node in _nodes)
            {
                _nodeToPositionMap[node] = new Point(node.X, node.Y);
            }
        }

        /// <inheritdoc/>
        protected override void Execute(ITransactor transactor, object parameter)
        {
            foreach (var node in _nodes)
            {
                transactor.Transact(new MoveNodeCommand(_nodeToPositionMap[node]), node);
            }
        }
    }
}