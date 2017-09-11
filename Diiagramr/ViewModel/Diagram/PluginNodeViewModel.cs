using Diiagramr.Executor;
using Diiagramr.Model;
using System;
using System.Linq;

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

        protected float LoadFloatValue(string key)
        {
            var value = DiagramNode?.GetVariable(key);
            return (float)(value ?? 0.0f);
        }

        public sealed override void InitializeWithNode(DiagramNode diagramNode)
        {
            base.InitializeWithNode(diagramNode);
            var nodeSetterUpper = new NodeSetup(this, diagramNode.Initialized);
            diagramNode.Initialized = true;
            SetupNode(nodeSetterUpper);

            NodeLoaded();
            LoadSizeIfNotZero();
        }

        private void LoadSizeIfNotZero()
        {
            Width = Math.Abs(LoadFloatValue("Width")) < 1 ? Width : LoadFloatValue("Width");
            Height = Math.Abs(LoadFloatValue("Height")) < 1 ? Height : LoadFloatValue("Height");
        }

        public override void OnNodeSaving()
        {
            base.OnNodeSaving();

            NodeSaved();

            SaveValue("Width", Width);
            SaveValue("Height", Height);
        }

        #region Virtual Methods

        /// <summary>
        /// All node customization such as turning on/off features and setting node geometry happens here.
        /// </summary>
        public abstract void SetupNode(NodeSetup setup);

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

        #endregion
    }

    public class PluginNodeException : Exception
    {
        public PluginNodeException(string s) : base(s)
        {
        }
    }
}