using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;

namespace Diiagramr.PluginNodeApi
{
    /// <summary>
    ///     This class should be extended to make a node that can be place on the diagram.  It contains many features that can
    ///     be used by overriding relavent methods.
    /// </summary>
    /// <seealso cref="AbstractNodeViewModel" />
    [Serializable]
    public abstract class PluginNode : AbstractNodeViewModel
    {
        private readonly IDictionary<string, PropertyInfo> _pluginNodeSettingProperties = new Dictionary<string, PropertyInfo>();

        public sealed override void InitializeWithNode(NodeModel nodeModel)
        {
            base.InitializeWithNode(nodeModel);
            var nodeSetterUpper = new NodeSetup(this, nodeModel.Initialized);
            nodeModel.Initialized = true;
            SetupNode(nodeSetterUpper);

            SetupPluginNodeSettings();
        }

        private void SetupPluginNodeSettings()
        {
            foreach (var propertyInfo in GetImplementingClassSettings())
            {
                _pluginNodeSettingProperties.Add(propertyInfo.Name, propertyInfo);
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (_pluginNodeSettingProperties.ContainsKey(propertyName))
            {
                var changedPropertyInfo = _pluginNodeSettingProperties[propertyName];
                var value = changedPropertyInfo.GetValue(this);
                NodeModel?.SetVariable(propertyName, value);
            }
        }

        public override void LoadNodeVariables()
        {
            foreach (var propertyInfo in GetImplementingClassSettings())
            {
                var key = propertyInfo.Name;
                var value = NodeModel?.GetVariable(key);
                propertyInfo.SetValue(this, value);
            }
        }

        private IEnumerable<PropertyInfo> GetImplementingClassSettings()
        {
            var actualType = GetType();
            var pluginNodeType = typeof(PluginNode);
            var properties = actualType.GetProperties();
            var pluginNodeProperties = pluginNodeType.GetProperties();
            var pluginNodePropertyNames = pluginNodeProperties.Select(i => i.Name).ToList();
            var implementingClassProperties = properties.Where(i => !pluginNodePropertyNames.Contains(i.Name));
            var propertiesWithPublicGetSet = implementingClassProperties.Where(i => i.GetGetMethod() != null && i.GetSetMethod() != null);
            return propertiesWithPublicGetSet.Where(i => Attribute.IsDefined(i, typeof(PluginNodeSetting)));
        }

        /// <summary>
        /// All node customization such as turning on/off features and setting node geometry happens here.
        /// </summary>
        public abstract void SetupNode(NodeSetup setup);

        #region Plugin Node Opt-in Features

        /// <summary>
        /// Called when a node is added on a diagram or loaded from disk.
        /// </summary>
        protected override void Initialize() { }

        /// <summary>
        /// Called when a node is removed from a diagram.
        /// </summary>
        public override void Uninitialize() { }

        #endregion
    }
}