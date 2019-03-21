using DiiagramrAPI.Diagram;
using DiiagramrAPI.Diagram.Model;
using System;
using System.Linq;

namespace DiiagramrAPI.Diagram
{
    /// <summary>
    ///     Class that provides an API that is as english as possible to make creating nodes easy.
    /// </summary>
    public class NodeSetup
    {
        private readonly Node _nodeViewModel;
        private int _terminalIndex;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NodeSetup" /> class.
        /// </summary>
        /// <param name="nodeViewModel">The node view model.</param>
        public NodeSetup(Node nodeViewModel)
        {
            _nodeViewModel = nodeViewModel ?? throw new ArgumentNullException(nameof(nodeViewModel));
        }

        public TypedTerminal<T> CreateClientTerminal<T>(Terminal terminalViewModel)
        {
            if (!_nodeViewModel.TerminalViewModels.Contains(terminalViewModel))
            {
                throw new InvalidOperationException("Can not create a client terminal for a terminal view model that is not on the node.");
            }

            return new TypedTerminal<T>(terminalViewModel);
        }

        /// <summary>
        ///     Allows the node to be resized via an adorner that appears when the node is selected.
        /// </summary>
        public void EnableResize()
        {
            _nodeViewModel.ResizeEnabled = true;
        }

        /// <summary>
        ///     Sets up a input terminal on this node.
        /// </summary>
        /// <param name="name">The name of the terminal.</param>
        /// <param name="direction">The default side of the node this terminal belongs on.</param>
        /// <remarks>For now, dynamically creating input terminals at runtime is not supported</remarks>
        public TypedTerminal<T> InputTerminal<T>(string name, Direction direction)
        {
            return CreateClientTerminal<T>(name, direction, TerminalKind.Input);
        }

        /// <summary>
        ///     Sets the name of a node, this is what displays above the node on the diagram.
        /// </summary>
        /// <param name="name"></param>
        public void NodeName(string name)
        {
            _nodeViewModel.Name = name;
        }

        /// <summary>
        ///     Sets the weight of the node in the node selector (where it gets inserted in the list).
        /// </summary>
        /// <param name="weight">Usually between 0 and 1, 1 being at the bottom of the list and 0 being at the top of the list.</param>
        public void NodeWeight(float weight)
        {
            _nodeViewModel.Weight = weight;
        }

        /// <summary>
        ///     Sets the initial node geometry.
        /// </summary>
        /// <param name="width">The width of the node.</param>
        /// <param name="height">The height of the node.</param>
        public void NodeSize(int width, int height)
        {
            if (Math.Abs(_nodeViewModel.Model.Width) < 0.01)
            {
                _nodeViewModel.Width = width;
            }

            if (Math.Abs(_nodeViewModel.Model.Height) < 0.01)
            {
                _nodeViewModel.Height = height;
            }

            _nodeViewModel.MinimumHeight = height;
            _nodeViewModel.MinimumWidth = width;
        }

        /// <summary>
        ///     Sets up a output terminal on this node.
        /// </summary>
        /// <param name="name">The name of the terminal.</param>
        /// <param name="direction">The default side of the node this terminal belongs on.</param>
        /// <remarks>For now, dynamically creating output terminals at runtime is not supported</remarks>
        public TypedTerminal<T> OutputTerminal<T>(string name, Direction direction)
        {
            return CreateClientTerminal<T>(name, direction, TerminalKind.Output);
        }

        /// <summary>
        ///     Sets up a terminal on this node of the given kind.
        /// </summary>
        /// <param name="name">The name of the terminal.</param>
        /// <param name="direction">The default side of the node this terminal belongs on.</param>
        /// <param name="kind">The kind of terminal to create.</param>
        /// <remarks>For now, dynamically creating terminals at runtime is not supported</remarks>
        private TypedTerminal<T> CreateClientTerminal<T>(string name, Direction direction, TerminalKind kind)
        {
            var terminalViewModel = FindOrCreateTerminalViewModel<T>(name, direction, kind);
            _terminalIndex++;
            return new TypedTerminal<T>(terminalViewModel);
        }

        private Terminal FindOrCreateTerminalViewModel<T>(string name, Direction direction, TerminalKind kind)
        {
            var terminalViewModel = _nodeViewModel.TerminalViewModels.FirstOrDefault(viewModel => viewModel.Model.TerminalIndex == _terminalIndex);
            if (terminalViewModel != null)
            {
                return terminalViewModel;
            }

            var terminalModel = new TerminalModel(name, typeof(T), direction, kind, _terminalIndex);
            terminalViewModel = Terminal.CreateTerminalViewModel(terminalModel);
            _nodeViewModel.AddTerminalViewModel(terminalViewModel);
            return terminalViewModel;
        }
    }
}