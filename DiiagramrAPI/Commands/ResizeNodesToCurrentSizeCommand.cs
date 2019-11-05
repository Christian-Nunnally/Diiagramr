using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Editor.Diagrams;
using System.Collections.Generic;
using System.Windows;

namespace DiiagramrAPI.Commands
{
    public class ResizeNodesToCurrentSizeCommand : TransactingCommand
    {
        private readonly IEnumerable<Node> _nodes;
        private readonly Dictionary<Node, Size> _nodeToSizeMap = new Dictionary<Node, Size>();

        public ResizeNodesToCurrentSizeCommand(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                _nodeToSizeMap[node] = new Size(node.Width, node.Height);
            }

            _nodes = nodes;
        }

        protected override void Execute(ITransactor transactor, object parameter)
        {
            if (parameter is IEnumerable<Node> nodes)
            {
                foreach (var node in nodes)
                {
                    if (_nodeToSizeMap.ContainsKey(node))
                    {
                        transactor.Transact(new ResizeNodeCommand(_nodeToSizeMap[node]), node);
                    }
                }
            }
        }
    }
}