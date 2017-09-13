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
    public abstract class PluginNode : AbstractNodeViewModel
    {
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
        }

        public override void OnNodeSaving()
        {
            base.OnNodeSaving();

            SaveValue("Width", Width);
            SaveValue("Height", Height);
        }

        /// <summary>
        /// All node customization such as turning on/off features and setting node geometry happens here.
        /// </summary>
        public abstract void SetupNode(NodeSetup setup);
    }

    public class PluginNodeException : Exception
    {
        public PluginNodeException(string s) : base(s)
        {
        }
    }
}