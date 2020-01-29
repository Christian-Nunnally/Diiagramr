using DiiagramrAPI.Application;
using DiiagramrAPI.Editor.Interactors;
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
    public abstract class Node : ViewModel, IMouseEnterLeaveReaction
    {
        private readonly IDictionary<string, PropertyInfo> _pluginNodeSettingCache = new Dictionary<string, PropertyInfo>();
        private readonly IDictionary<string, PropertyInfo> _terminalsPropertyInfos = new Dictionary<string, PropertyInfo>();
        private readonly IDictionary<string, TerminalModel> _nameToTerminalMap = new Dictionary<string, TerminalModel>();
        private readonly List<Action> _viewLoadedActions = new List<Action>();

        public Node()
        {
            Terminals = new ViewModelCollection<Terminal, TerminalModel>(this, () => NodeModel?.Terminals, Terminal.CreateTerminalViewModel);
            Terminals.CollectionChanged += TerminalsCollectionChanged;
            _viewLoadedActions.Add(ArrangeAllTerminals);
            PropertyChanged += NodePropertyChanged;
        }

        public virtual IObservableCollection<Terminal> Terminals { get; set; }

        public virtual double MinimumHeight { get; set; } = Diagram.GridSnapInterval;

        public virtual double MinimumWidth { get; set; } = Diagram.GridSnapInterval;

        public virtual bool ResizeEnabled { get; set; }

        public virtual bool IsSelected { get; set; }

        public virtual NodeModel NodeModel => Model as NodeModel;

        public virtual string Name { get; set; }

        public virtual float Weight { get; set; }

        public virtual double X
        {
            get => NodeModel?.X ?? 0;
            set => NodeModel.RunIfNotNull(() => NodeModel.X = value);
        }

        public virtual double Y
        {
            get => NodeModel?.Y ?? 0;
            set => NodeModel.RunIfNotNull(() => NodeModel.Y = value);
        }

        public virtual double Width
        {
            get => NodeModel?.Width ?? MinimumWidth;
            set
            {
                NodeModel?.Width.RunIfNotNull(() => NodeModel.Width = Math.Max(value, MinimumWidth));
                MinimumWidth = NodeModel == null ? value : MinimumWidth;
            }
        }

        public virtual double Height
        {
            get => NodeModel?.Height ?? MinimumHeight;
            set
            {
                NodeModel?.Height.RunIfNotNull(() => NodeModel.Height = Math.Max(value, MinimumHeight));
                MinimumHeight = NodeModel == null ? value : MinimumHeight;
            }
        }

        private IEnumerable<PropertyInfo> PluginNodeSettings => GetType().GetProperties().Where(i => Attribute.IsDefined(i, typeof(NodeSettingAttribute)));

        public virtual void AddTerminal(TerminalModel terminalModel)
        {
            _nameToTerminalMap[terminalModel.Name] = terminalModel;
            NodeModel.AddTerminal(terminalModel);
        }

        public virtual void RemoveTerminal(TerminalModel terminalModel)
        {
            _nameToTerminalMap.Remove(terminalModel.Name);
            NodeModel.RemoveTerminal(terminalModel);
        }

        public void HighlightWirableTerminals<T>(Type type)
        {
            Terminals.OfType<T>().OfType<Terminal>().ForEach(terminal => terminal.ShowHighlightIfCompatibleType(type));
        }

        public virtual void AttachToModel(NodeModel nodeModel)
        {
            if (NodeModel == null)
            {
                Model = nodeModel;
                NodeModel.Name = GetType().FullName;
                InitializePluginNodeSettings();
                LoadTerminalViewModels();
                CreateTerminals();
                NodeModel.Width = MinimumWidth;
                NodeModel.Height = MinimumHeight;
                NodeModel.PropertyChanged += ModelPropertyChanged;
            }
        }

        public virtual void InitializePluginNodeSettings()
        {
            PluginNodeSettings.ForEach(info => _pluginNodeSettingCache.Add(info.Name, info));
            PluginNodeSettings.ForEach(PersistProperty);
            PluginNodeSettings.ForEach(info => info.SetValue(this, NodeModel?.GetVariable(info.Name)));
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

        public void CreateTerminals()
        {
            GetType()
                .GetMethods()
                .Where(x => Attribute.IsDefined(x, typeof(InputTerminalAttribute)))
                .ForEach(CreateInputTerminalForMethod);
            GetType()
                .GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(OutputTerminalAttribute)))
                .ForEach(CreateOutputTerminalForProperty);
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
                NodeModel?.SetVariable(propertyName, value);
            }
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            _viewLoadedActions.ForEach(action => action());
            _viewLoadedActions.Clear();
        }

        private void TerminalsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ArrangeAllTerminals();
        }

        private void NodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_terminalsPropertyInfos.TryGetValue(e.PropertyName, out PropertyInfo propertyInfo)
                && _nameToTerminalMap.TryGetValue(e.PropertyName, out TerminalModel terminalModel))
            {
                terminalModel.Data = propertyInfo.GetValue(this);
            }
        }

        private void LoadTerminalViewModels()
        {
            foreach (var terminal in NodeModel.Terminals)
            {
                Terminals.Add(Terminal.CreateTerminalViewModel(terminal));
            }
        }

        private void PersistProperty(PropertyInfo info)
        {
            if (!NodeModel.PersistedVariables.ContainsKey(info.Name))
            {
                NodeModel.SetVariable(info.Name, info.GetValue(this));
            }
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DiiagramrModel.NodeModel.X):
                case nameof(DiiagramrModel.NodeModel.Y):
                    OnPropertyChanged(e.PropertyName);
                    break;
            }
        }

        private void ValidateInputTerminalMethod(MethodInfo methodInfo)
        {
            if (methodInfo.GetParameters().Length != 1)
            {
                var errorMessage = $"Input terminal method `{GetType().AssemblyQualifiedName}.{methodInfo.Name}` must have exactly one parameter.";
                throw new InvalidOperationException(errorMessage);
            }
        }

        private void CreateInputTerminalForMethod(MethodInfo methodInfo)
        {
            ValidateInputTerminalMethod(methodInfo);
            var inputTerminalAttribute = methodInfo.GetAttribute<InputTerminalAttribute>();
            var terminalType = methodInfo.GetParameters().First().ParameterType;
            var terminalModel = new InputTerminalModel(methodInfo.Name, terminalType, inputTerminalAttribute.DefaultDirection, 0);
            terminalModel.DataChanged += methodInfo.CreateMethodInvoker(this);
            AddTerminal(terminalModel);
        }

        private void CreateOutputTerminalForProperty(PropertyInfo property)
        {
            var outputTerminalAttribute = property.GetAttribute<OutputTerminalAttribute>();
            var terminalType = property.PropertyType;
            var terminalModel = new OutputTerminalModel(property.Name, terminalType, outputTerminalAttribute.DefaultDirection, 0);
            _terminalsPropertyInfos[property.Name] = property;
            AddTerminal(terminalModel);
            NotifyOfPropertyChange(property.Name);
        }

        private void ArrangeAllTerminals()
        {
            var placer = new TerminalPlacer(Height, Width);
            placer.ArrangeTerminals(Terminals);
        }
    }
}