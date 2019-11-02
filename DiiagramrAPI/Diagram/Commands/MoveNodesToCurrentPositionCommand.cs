﻿using DiiagramrAPI.Shell.Commands.Transacting;
using System.Collections.Generic;
using System.Windows;

namespace DiiagramrAPI.Diagram.Commands
{
    public class MoveNodesToCurrentPositionCommand : TransactingCommand
    {
        private readonly Dictionary<Node, Point> _nodeToPositionMap = new Dictionary<Node, Point>();
        private readonly IEnumerable<Node> _nodes;

        public MoveNodesToCurrentPositionCommand(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                _nodeToPositionMap[node] = new Point(node.X, node.Y);
            }
            _nodes = nodes;
        }

        protected override void Execute(ITransactor transactor, object parameter)
        {
            if (parameter is IEnumerable<Node> nodes)
            {
                foreach (var node in nodes)
                {
                    if (_nodeToPositionMap.ContainsKey(node))
                    {
                        transactor.Transact(new MoveNodeCommand(_nodeToPositionMap[node]), node);
                    }
                }
            }
        }
    }
}