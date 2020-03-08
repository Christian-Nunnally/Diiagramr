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
    public abstract class Node : ViewModel<NodeModel>, IMouseEnterLeaveReaction
    {
        public NodeServiceProvider _nodeServiceProvider;
        private readonly IDictionary<string, PropertyInfo> _pluginNodeSettingCache = new Dictionary<string, PropertyInfo>();

        public Node()
        {
            Terminals = new ViewModelCollection<Terminal, TerminalModel>(this, () => Model?.Terminals, Terminal.CreateTerminalViewModel);
            Terminals.CollectionChanged += TerminalsCollectionChanged;
            ExecuteWhenViewLoaded(ArrangeAllTerminals);
            Name ??= string.Empty;
        }

        public virtual IObservableCollection<Terminal> Terminals { get; set; }

        public virtual double MinimumHeight { get; set; } = Diagram.GridSnapInterval;

        public virtual double MinimumWidth { get; set; } = Diagram.GridSnapInterval;

        public virtual bool ResizeEnabled { get; set; }

        public virtual bool IsSelected { get; set; }

        public virtual string Name { get; set; }

        public virtual float Weight { get; set; }

        public virtual double X
        {
            get => Model?.X ?? 0;
            set => Model.RunIfNotNull(() => Model.X = value);
        }

        public virtual double Y
        {
            get => Model?.Y ?? 0;
            set => Model.RunIfNotNull(() => Model.Y = value);
        }

        public virtual double Width
        {
            get => Model?.Width ?? MinimumWidth;
            set
            {
                Model?.Width.RunIfNotNull(() => Model.Width = Math.Max(value, MinimumWidth));
                MinimumWidth = Model == null ? value : MinimumWidth;
            }
        }

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

        public void SetServiceProvider(NodeServiceProvider nodeServiceProvider)
        {
            _nodeServiceProvider = nodeServiceProvider;
            _nodeServiceProvider.ServiceRegistered += NodeServiceProviderServiceRegistered;
            UpdateServices(nodeServiceProvider);
        }

        public virtual void AddTerminal(TerminalModel terminalModel)
        {
            Model.AddTerminal(terminalModel);
        }

        public virtual void RemoveTerminal(TerminalModel terminalModel)
        {
            Model.RemoveTerminal(terminalModel);
        }

        public void HighlightWirableTerminals<T>(Type type)
        {
            Terminals.OfType<T>().OfType<Terminal>().ForEach(terminal => terminal.ShowHighlightIfCompatibleType(type));
        }

        public virtual void AttachToModel(NodeModel nodeModel)
        {
            if (Model == null)
            {
                base.Model = nodeModel;
                Model.Name = GetType().FullName;
                InitializePluginNodeSettings();
                var terminalCreator = new NodeTerminalManager(this);
                terminalCreator.CreateTerminals();
                Model.Width = MinimumWidth;
                Model.Height = MinimumHeight;
                Model.PropertyChanged += ModelPropertyChanged;
            }
        }

        public virtual void InitializePluginNodeSettings()
        {
            PluginNodeSettings.ForEach(info => _pluginNodeSettingCache.Add(info.Name, info));
            PluginNodeSettings.ForEach(PersistProperty);
            PluginNodeSettings.ForEach(info => info.SetValue(this, Model?.GetVariable(info.Name)));
        }

        public void MouseEntered()
        {
            SetAdorner(new NodeNameAdorner(View, this));
            MouseEnteredNode();
        }

        public void MouseLeft()
        {
            SetAdorner(null);
            MouseLeftNode();
        }

        public void UnhighlightTerminals()
        {
            Terminals.ForEach(t => t.HighlightVisible = false);
        }

        public void UnselectTerminals()
        {
            Terminals.ForEach(t => t.IsSelected = false);
            UnhighlightTerminals();
        }

        protected virtual void UpdateServices(NodeServiceProvider nodeServiceProvider)
        {
        }

        protected virtual void MouseEnteredNode()
        {
        }

        protected virtual void MouseLeftNode()
        {
        }

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

        private void NodeServiceProviderServiceRegistered()
        {
            UpdateServices(_nodeServiceProvider);
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