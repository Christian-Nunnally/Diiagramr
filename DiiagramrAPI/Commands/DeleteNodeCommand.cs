using DiiagramrAPI.Editor;
using DiiagramrAPI.Shell.Commands;
using System;

namespace DiiagramrAPI.Commands
{
    public class DeleteNodeCommand : ICommand
    {
        private Diagram _diagram;

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