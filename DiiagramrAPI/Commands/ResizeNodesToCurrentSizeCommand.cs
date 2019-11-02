using DiiagramrAPI.Editor;
using DiiagramrAPI.Shell.Commands.Transacting;
using System.Collections.Generic;
using System.Windows;

namespace DiiagramrAPI.Commands
{
    public class ResizeNodesToCurrentSizeCommand : TransactingCommand
    {
        private readonly Dictionary<Node, Size> _nodeToSizeMap = new Dictionary<Node, Size>();
        private readonly IEnumerable<Node> _nodes;

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
