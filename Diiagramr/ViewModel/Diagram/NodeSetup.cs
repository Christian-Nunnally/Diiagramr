using System;

namespace Diiagramr.ViewModel.Diagram
{
    /// <summary>
    /// Class that provides an API that is as english as possible to make creating nodes easy.
    /// </summary>
    public class NodeSetup
    {
        private readonly AbstractNodeViewModel _nodeViewModel;
        private readonly bool _loadMode;
        private int _inputTerminalIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeSetup"/> class.
        /// </summary>
        /// <param name="nodeViewModel">The node view model.</param>
        public NodeSetup(AbstractNodeViewModel nodeViewModel)
        {
            _nodeViewModel = nodeViewModel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeSetup"/> class.
        /// </summary>
        /// <param name="nodeViewModel">The node view model.</param>
        /// <param name="loadMode">if set to <c>true</c> lots of setup methods will noop, only methods that are needed to setup the node after load will be used..</param>
        public NodeSetup(AbstractNodeViewModel nodeViewModel, bool loadMode)
        {
            _nodeViewModel = nodeViewModel;
            _loadMode = loadMode;
        }

        /// <summary>
        /// Sets the initial node geometry.
        /// </summary>
        /// <param name="width">The width of the node.</param>
        /// <param name="height">The height of the node.</param>
        public void NodeSize(int width, int height)
        {
            if (_loadMode) return;
            _nodeViewModel.Width = width;
            _nodeViewModel.Height = height;
        }

        /// <summary>
        /// Sets up a input terminal on this node.
        /// </summary>
        /// <param name="name">The name of the terminal.</param>
        /// <param name="type">The data type the new terminal should accept.</param>
        /// <param name="direction">The default side of the node this terminal belongs on.</param>
        /// <param name="function">The function that data comming in to this terminal gets passed in to.</param>
        /// <remarks>For now, dynamically creating input terminals at runtime is not supported</remarks>
        public void InputTerminal(string name, Type type, Direction direction, InputTerminalDelegate function)
        {
            _nodeViewModel.DelegateMapper.AddMapping(_inputTerminalIndex++, function);
            if (_loadMode)
            {
                return;
            }
            _nodeViewModel.ConstructNewInputTerminal(name, type, direction, _inputTerminalIndex - 1);
        }

        /// <summary>
        /// Sets up a output terminal on this node.
        /// </summary>
        /// <param name="name">The name of the terminal.</param>
        /// <param name="type">The data type of the terminal.  This is not currently enforced so returning a value out of this output that does not match this type might yeild unexpected results.</param>
        /// <param name="direction">The default side of the node this terminal belongs on.</param>
        public void OutputTerminal(string name, Type type, Direction direction)
        {
            if (_loadMode) return;
            _nodeViewModel.OutputTerminals.Add(_nodeViewModel.ConstructNewOutputTerminal(name, type, direction));
        }
    }
}