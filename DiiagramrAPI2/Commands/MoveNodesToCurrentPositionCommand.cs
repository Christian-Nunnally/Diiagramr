using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Editor.Diagrams;
using System.Collections.Generic;
using System.Windows;

namespace DiiagramrAPI.Commands
{
    public class MoveNodesToCurrentPositionCommand : TransactingCommand
    {
        private readonly IEnumerable<Node> _nodes;
        private readonly Dictionary<Node, Point> _nodeToPositionMap = new Dictionary<Node, Point>();

        public MoveNodesToCurrentPositionCommand(IEnumerable<Node> nodes)
        {
            _nodes = nodes;
            foreach (var node in nodes)
            {
                _nodeToPositionMap[node] = new Point(node.X, node.Y);
            }
        }

        protected override void Execute(ITransactor transactor, object parameter)
        {
            foreach (var node in _nodes)
            {
                transactor.Transact(new MoveNodeCommand(_nodeToPositionMap[node]), node);
            }
        }
    }
}