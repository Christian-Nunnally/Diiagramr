using System;
using System.Linq;
using Diiagramr.Executor;
using Diiagramr.Model;

namespace Diiagramr.ViewModel.Diagram
{
    /// <summary>
    ///     This class should be extended to make a node that can be place on the diagram.  It contains many features that can
    ///     be used by overriding relavent methods.
    /// </summary>
    /// <seealso cref="AbstractNodeViewModel" />
    [Serializable]
    public abstract class PluginNodeViewModel : AbstractNodeViewModel
    {
        /// <summary>
        /// Begins executing from output.
        /// </summary>
        /// <param name="value">The value to execute with.</param>
        /// <param name="outputTerminalIndex">Index of the output terminal to execute from.</param>
        protected void ExecuteFromOutput(object value, int outputTerminalIndex = 0)
        {
            if (OutputTerminalViewModels.Count() <= outputTerminalIndex) throw new PluginNodeException($"Plugin node \"{Name}\" has {OutputTerminalViewModels.Count()} terminals so executing from terminal #{outputTerminalIndex + 1} is not possible.");
            var outputTerminal = OutputTerminalViewModels.ElementAt(outputTerminalIndex).Terminal as OutputTerminal;

            IDiagramExecutor executor = new DiagramExecutor();
            executor.Execute(outputTerminal, value);
        }

        /// <summary>
        /// Saves a value to disk when the project closes.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected void SaveValue(string key, object value)
        {
            DiagramNode?.SetVariable(key, value);
        }

        /// <summary>
        /// Loads a previously saved value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        protected object LoadValue(string key)
        {
            return DiagramNode?.GetVariable(key);
        }

        public sealed override void InitializeWithNode(DiagramNode diagramNode)
        {
            base.InitializeWithNode(diagramNode);
            SetupDelegates(DelegateMapper);
            NodeLoaded();
        }

        #region Virtual Methods

        /// <summary>
        /// Called when the node is loaded.  Any view model properties that you want to presist on disk should be saved here via calls to <see cref="LoadValue"/>.
        /// </summary>
        public virtual void NodeLoaded()
        {
        }

        /// <summary>
        /// Called when the node is saved.  Any view model properties that you want to presist on disk should be saved here via calls to <see cref="SaveValue"/>.
        /// </summary>
        public virtual void NodeSaved()
        {
        }

        /// <summary>
        /// Called once when the node is first constructed.  The initial terminals that should exist on the node should be constructed here via calls to 
        /// <see cref="AbstractNodeViewModel.ConstructNewInputTerminal"/> and <see cref="AbstractNodeViewModel.ConstructNewOutputTerminal"/>.
        /// </summary>
        public override void ConstructTerminals()
        {
        }

        public virtual void SetupDelegates(DelegateMapper delegateMapper)
        {
        }

        #endregion
    }

    public class PluginNodeException : Exception
    {
        public PluginNodeException(string s) : base(s)
        {
        }
    }
}