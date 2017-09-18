﻿using System.Linq;
using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;

namespace Diiagramr.PluginNodeApi
{
    /// <summary>
    /// Class that provides an API that is as english as possible to make creating nodes easy.
    /// </summary>
    public class NodeSetup
    {
        private readonly AbstractNodeViewModel _nodeViewModel;
        private readonly bool _loadMode;
        private int _terminalIndex;

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
        /// Sets the name of a node, this is what displays above the node on the diagram.
        /// </summary>
        /// <param name="name"></param>
        public void NodeName(string name)
        {
            _nodeViewModel.Name = name;
        }

        /// <summary>
        /// Sets up a input terminal on this node.
        /// </summary>
        /// <param name="name">The name of the terminal.</param>
        /// <param name="direction">The default side of the node this terminal belongs on.</param>
        /// <remarks>For now, dynamically creating input terminals at runtime is not supported</remarks>
        public Terminal<T> InputTerminal<T>(string name, Direction direction)
        {
            if (!_loadMode)
            {
                var terminal = new TerminalModel(name, typeof(T), direction, TerminalKind.Input, _terminalIndex);
                var terminalViewModel = new InputTerminalViewModel(terminal);
                _nodeViewModel.AddTerminalViewModel(terminalViewModel);
            }
            return CreateClientTerminal<T>(_terminalIndex);
        }

        /// <summary>
        /// Sets up a output terminal on this node.
        /// </summary>
        /// <param name="name">The name of the terminal.</param>
        /// <param name="direction">The default side of the node this terminal belongs on.</param>
        /// <remarks>For now, dynamically creating output terminals at runtime is not supported</remarks>
        public Terminal<T> OutputTerminal<T>(string name, Direction direction)
        {
            if (!_loadMode)
            {
                var terminal = new TerminalModel(name, typeof(T), direction, TerminalKind.Output, _terminalIndex);
                var terminalViewModel = new OutputTerminalViewModel(terminal);
                _nodeViewModel.AddTerminalViewModel(terminalViewModel);
            }
            return CreateClientTerminal<T>(_terminalIndex);
        }

        private Terminal<T> CreateClientTerminal<T>(int terminalIndex)
        {
            var terminalViewModel = _nodeViewModel.TerminalViewModels.First(viewModel => viewModel.Terminal.TerminalIndex == terminalIndex);
            var terminal = new Terminal<T>(terminalViewModel);
            _terminalIndex++;

            return terminal;
        }
    }
}