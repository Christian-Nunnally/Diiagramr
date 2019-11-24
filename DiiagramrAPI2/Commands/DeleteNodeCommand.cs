using DiiagramrAPI.Application.Commands;
using DiiagramrAPI.Editor.Diagrams;
using System;

namespace DiiagramrAPI.Commands
{
    public class DeleteNodeCommand : ICommand
    {
        private readonly Diagram _diagram;

        public DeleteNodeCommand(Diagram diagram)
        {
            _diagram = diagram;
        }

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