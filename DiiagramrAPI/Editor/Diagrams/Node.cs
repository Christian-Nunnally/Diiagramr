using DiiagramrAPI.Application;
using DiiagramrAPI.Editor.Interactors;
using DiiagramrCore;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DiiagramrAPI.Editor.Diagrams
{
    public abstract class Node : ViewModel, IMouseEnterLeaveReaction
    {
        private readonly IDictionary<string, PropertyInfo> _pluginNodeSettingCache = new Dictionary<string, PropertyInfo>();
        private readonly IDictionary<string, PropertyInfo> _terminalsPropertyInfos = new Dictionary<string, PropertyInfo>();
        private readonly IDictionary<string, Terminal> _nameToTerminalMap = new Dictionary<string, Terminal>();
        private readonly List<Action> _viewLoadedActions = new List<Action>();

        public virtual ObservableCollection<Terminal> Terminals { get; } = new ObservableCollection<Terminal>();

        public virtual double MinimumHeight { get; set; } = Diagram.GridSnapInterval;

        public virtual double MinimumWidth { get; set; } = Diagram.GridSnapInterval;

        public virtual bool ResizeEnabled { get; set; }

        public virtual bool IsSelected { get; set; }

        public virtual NodeModel Model { get; set; }

        public virtual string Name { get; set; }

        public virtual float Weight { get; set; }

        public virtual double X
        {
            get => Model?.X ?? 0;
            set => X.SetIfNotNull(() => X = value);
        }

        public virtual double Y
        {
            get => Model?.Y ?? 0;
            set => Y.SetIfNotNull(() => Y = value);
        }

        public virtual double Width
        {
            get => Model?.Width ?? MinimumWidth;
            set
            {
                Model?.Width.SetIfNotNull(() => Model.Width = Math.Max(value, MinimumWidth));
                MinimumWidth = Model == null ? value : MinimumWidth;
            }
        }

        public virtual double Height
        {
            get => Model?.Height ?? MinimumHeight;
            set
            {
                Model?.Height.SetIfNotNull(() => Model.Height = Math.Max(value, MinimumHeight));
                MinimumHeight = Model == null ? value : MinimumHeight;
            }
        }

        private IEnumerable<PropertyInfo> PluginNodeSettings => GetType().GetProperties().Where(i => Attribute.IsDefined(i, typeof(NodeSettingAttribute)));

        public virtual void AddTerminal(Terminal terminal)
        {
            _nameToTerminalMap[terminal.Name] = terminal;
            Terminals.Add(terminal);
            Model.AddTerminal(terminal.Model);
            ArrangeAllTerminals();
        }

        public virtual void RemoveTerminal(Terminal terminal)
        {
            _nameToTerminalMap.Remove(terminal.Name);
            Terminals.Remove(terminal);
            Model.RemoveTerminal(terminal.Model);
            ArrangeAllTerminals();
        }

        public void HighlightTerminalsOfType<T>(Type type)
        {
            Terminals.OfType<T>().OfType<Terminal>().ForEach(terminal => terminal.ShowHighlightIfCompatibleType(type));
        }

        public virtual void AttachToModel(NodeModel nodeModel)
        {
            if (Model == null)
            {
                Model = nodeModel;
                Model.Name = GetType().FullName;
                InitializePluginNodeSettings();
                LoadTerminalViewModels();
                CreateTerminals();
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
                Model?.SetVariable(propertyName, value);
            }
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            _viewLoadedActions.ForEach(action => action.Invoke());
            _viewLoadedActions.Clear();
        }

        protected void Output(object data, string terminalName)
        {
            if (_terminalsPropertyInfos.TryGetValue(terminalName, out PropertyInfo propertyInfo)
                && _nameToTerminalMap.TryGetValue(terminalName, out Terminal terminal))
            {
                propertyInfo.SetValue(this, data);
                terminal.Data = data;
            }
        }

        private void LoadTerminalViewModels()
        {
            foreach (var terminal in Model.Terminals)
            {
                Terminals.Add(Terminal.CreateTerminalViewModel(terminal));
            }
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
            var terminalModel = new InputTerminalModel(inputTerminalAttribute.TerminalName, terminalType, inputTerminalAttribute.DefaultDirection, 0);
            var terminal = new InputTerminal(terminalModel);
            terminal.DataChanged += methodInfo.CreateMethodInvoker(this);
            AddTerminal(terminal);
        }

        private void CreateOutputTerminalForProperty(PropertyInfo property)
        {
            var inputTerminalAttribute = property.GetAttribute<OutputTerminalAttribute>();
            var terminalType = property.PropertyType;
            var terminalModel = new OutputTerminalModel(inputTerminalAttribute.TerminalName, terminalType, inputTerminalAttribute.DefaultDirection, 0);
            var terminal = new OutputTerminal(terminalModel);
            _terminalsPropertyInfos[inputTerminalAttribute.TerminalName] = property;
            AddTerminal(terminal);
        }

        private void ArrangeAllTerminals()
        {
            var placer = new TerminalPlacer(Width, Height);
            placer.ArrangeTerminals(Terminals);
        }
    }
}