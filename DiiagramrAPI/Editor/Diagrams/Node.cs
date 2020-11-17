using DiiagramrAPI.Application;
using DiiagramrAPI.Editor.Interactors;
using DiiagramrAPI.Service.Editor;
using DiiagramrCore;
using DiiagramrModel;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DiiagramrAPI.Editor.Diagrams
{
    /// <summary>
    /// A node on the diagram.
    /// </summary>
    public abstract class Node : ViewModel<NodeModel>, IMouseEnterLeaveReaction
    {
        private readonly IDictionary<string, PropertyInfo> _pluginNodeSettingCache = new Dictionary<string, PropertyInfo>();
        private NodeServiceProvider _nodeServiceProvider;

        /// <summary>
        /// Creates a new instance of <see cref="Node"/>.
        /// </summary>
        public Node()
        {
            Terminals = new ViewModelCollection<Terminal, TerminalModel>(this, () => Model?.Terminals, Terminal.CreateTerminalViewModel);
            Terminals.CollectionChanged += TerminalsCollectionChanged;
            ExecuteWhenViewLoaded(ArrangeAllTerminals);
            Name ??= string.Empty;
        }

        /// <summary>
        /// The list visible list visible terminal on this node.
        /// </summary>
        public virtual IObservableCollection<Terminal> Terminals { get; set; }

        /// <summary>
        /// Get or sets the nodes minimum height.
        /// </summary>
        public virtual double MinimumHeight { get; set; } = Diagram.GridSnapInterval;

        /// <summary>
        /// Get or sets the nodes minimum width.
        /// </summary>
        public virtual double MinimumWidth { get; set; } = Diagram.GridSnapInterval;

        /// <summary>
        /// Get or sets whether this node can be resized by dragging near the border.
        /// </summary>
        public virtual bool ResizeEnabled { get; set; }

        /// <summary>
        /// Get or sets whether the node is selected and should show a selection border.
        /// </summary>
        public virtual bool IsSelected { get; set; }

        /// <summary>
        /// Get or sets whether the mouse is over the node visual.
        /// </summary>
        public bool IsMouseWithin { get; set; }

        /// <summary>
        /// Get or sets the name of the node.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Get or sets the weight of the node as it appears in the node palette.
        /// </summary>
        public virtual float Weight { get; set; }

        /// <summary>
        /// Get or sets the nodes X position on the diagram.
        /// </summary>
        public virtual double X
        {
            get => Model?.X ?? 0;
            set => Model.RunIfNotNull(() => Model.X = value);
        }

        /// <summary>
        /// Get or sets the nodes Y position on the diagram.
        /// </summary>
        public virtual double Y
        {
            get => Model?.Y ?? 0;
            set => Model.RunIfNotNull(() => Model.Y = value);
        }

        /// <summary>
        /// Get or sets the nodes wdith.
        /// </summary>
        [NodeSetting]
        public virtual double Width
        {
            get => Model?.Width ?? MinimumWidth;
            set
            {
                Model?.Width.RunIfNotNull(() => Model.Width = Math.Max(value, MinimumWidth));
                MinimumWidth = Model == null ? value : MinimumWidth;
            }
        }

        /// <summary>
        /// Get or sets the nodes height.
        /// </summary>
        [NodeSetting]
        public virtual double Height
        {
            get => Model?.Height ?? MinimumHeight;
            set
            {
                Model?.Height.RunIfNotNull(() => Model.Height = Math.Max(value, MinimumHeight));
                MinimumHeight = Model == null ? value : MinimumHeight;
            }
        }

        private IEnumerable<PropertyInfo> PluginNodeSettings => GetType().GetProperties().Where(i => Attribute.IsDefined(i, typeof(NodeSettingAttribute)));

        /// <summary>
        /// Sets the service provider implementations of this node can use to share services.
        /// </summary>
        /// <param name="nodeServiceProvider">The service provider.</param>
        public void SetServiceProvider(NodeServiceProvider nodeServiceProvider)
        {
            _nodeServiceProvider = nodeServiceProvider;
            _nodeServiceProvider.ServiceRegistered += NodeServiceProviderServiceRegistered;
            ServicesUpdated(nodeServiceProvider);
        }

        /// <summary>
        /// Adds a terminal to this node.
        /// </summary>
        /// <param name="terminalModel">The terminal to add.</param>
        public virtual void AddTerminal(TerminalModel terminalModel)
        {
            Model.AddTerminal(terminalModel);
        }

        /// <summary>
        /// Removes a terminal from this node.
        /// </summary>
        /// <param name="terminalModel">The terminal to remove.</param>
        public virtual void RemoveTerminal(TerminalModel terminalModel)
        {
            Model.RemoveTerminal(terminalModel);
        }

        /// <summary>
        /// Highlights all terminals that are compatable with the given type.
        /// </summary>
        /// <typeparam name="T">Input or output terminls</typeparam>
        /// <param name="type">The type to highlight terminals that are compatable with.</param>
        public void HighlightWirableTerminals<T>(Type type)
        {
            Terminals.OfType<T>().OfType<Terminal>().ForEach(terminal => terminal.ShowHighlightIfCompatibleType(type));
        }

        /// <summary>
        /// Attach this node view model to a model object, loading in  saved properties from the model if it was deserialized.
        /// </summary>
        /// <param name="nodeModel">The model to attach to.</param>
        public virtual void AttachToModel(NodeModel nodeModel)
        {
            if (Model == null)
            {
                Model = nodeModel;
                Model.Name = GetType().FullName;
                InitializePluginNodeSettings();
                new NodeTerminalManager(this);
                Model.Width = MinimumWidth;
                Model.Height = MinimumHeight;
                Model.PropertyChanged += ModelPropertyChanged;
            }
        }

        /// <inheritdoc/>
        public void MouseEntered()
        {
            SetAdorner(new NodeNameAdorner(View, this));
            IsMouseWithin = true;
            MouseEnteredNode();
        }

        /// <inheritdoc/>
        public void MouseLeft()
        {
            SetAdorner(null);
            IsMouseWithin = false;
            MouseLeftNode();
        }

        /// <summary>
        /// Unhighlights all terminals in this node.
        /// </summary>
        public void UnhighlightTerminals()
        {
            Terminals.ForEach(t => t.HighlightVisible = false);
        }

        /// <summary>
        /// Unselects all terminals in this node.
        /// </summary>
        public void UnselectTerminals()
        {
            Terminals.ForEach(t => t.IsSelected = false);
            UnhighlightTerminals();
        }

        /// <summary>
        /// Called when a service is changed, added, or removed to allow implementations of <see cref="Node"/> to react or use new services.
        /// </summary>
        /// <param name="nodeServiceProvider">The node service provider that contains updated services.</param>
        protected virtual void ServicesUpdated(NodeServiceProvider nodeServiceProvider)
        {
        }

        /// <summary>
        /// Called when the mouse enters the node.
        /// </summary>
        protected virtual void MouseEnteredNode()
        {
        }

        /// <summary>
        /// Called when the mouse leaves the node.
        /// </summary>
        protected virtual void MouseLeftNode()
        {
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(Width) || propertyName == nameof(Height))
            {
                ArrangeAllTerminals();
            }
            else if (_pluginNodeSettingCache.TryGetValue(propertyName, out PropertyInfo propertyInfo))
            {
                var value = propertyInfo.GetValue(this);
                Model?.SetVariable(propertyName, value);
            }
        }

        private void InitializePluginNodeSettings()
        {
            PluginNodeSettings.ForEach(info => _pluginNodeSettingCache.Add(info.Name, info));
            PluginNodeSettings.ForEach(PersistProperty);
            PluginNodeSettings.ForEach(info => info.SetValue(this, Model?.GetVariable(info.Name)));
        }

        private void NodeServiceProviderServiceRegistered()
        {
            ServicesUpdated(_nodeServiceProvider);
        }

        private void TerminalsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ArrangeAllTerminals();
        }

        private void PersistProperty(PropertyInfo info)
        {
            if (!Model.PersistedVariables.ContainsKey(info.Name))
            {
                Model.SetVariable(info.Name, info.GetValue(this));
            }
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(NodeModel.X):
                case nameof(NodeModel.Y):
                    OnPropertyChanged(e.PropertyName);
                    break;
            }
        }

        private void ArrangeAllTerminals()
        {
            var placer = new TerminalPlacer(Height, Width);
            placer.ArrangeTerminals(Terminals);
        }
    }
}