using DiiagramrCore;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DiiagramrAPI.Editor.Diagrams
{
    public class NodeTerminalManager
    {
        private readonly Node _node;

        public NodeTerminalManager(Node node)
        {
            _node = node;
            _node.PropertyChanged += NodePropertyChanged;
        }

        public static bool IsGenericList(Type type)
        {
            return type.IsGenericType
                && typeof(List<>) == type.GetGenericTypeDefinition();
        }

        public void CreateTerminals() => _node.GetType().GetProperties().Where(x => Attribute.IsDefined(x, typeof(TerminalAttribute))).ForEach(CreateTerminalForProperty);

        private static TerminalModel CreateTerminalModel(PropertyInfo property, TerminalAttribute terminalAttribute, Type terminalType)
        {
            return terminalAttribute is InputTerminalAttribute inputTerminalAttribute
                ? CreateInputTerminalModel(property, inputTerminalAttribute, terminalType)
                : (TerminalModel)new OutputTerminalModel(property.Name, terminalType, terminalAttribute.DefaultDirection);
        }

        private static InputTerminalModel CreateInputTerminalModel(PropertyInfo property, InputTerminalAttribute inputTerminalAttribute, Type terminalType)
        {
            if (inputTerminalAttribute.IsCoalescing)
            {
                if (!IsGenericList(terminalType))
                {
                    throw new InvalidOperationException("Coalescing terminals must be properties of type List<T>");
                }
                var genericType = terminalType.GetGenericArguments().First();
                return new CoalescingInputTerminalModel(property.Name, genericType, inputTerminalAttribute.DefaultDirection);
            }
            return new InputTerminalModel(property.Name, terminalType, inputTerminalAttribute.DefaultDirection);
        }

        private void CreateTerminalForProperty(PropertyInfo property)
        {
            var terminalAttribute = property.GetAttribute<TerminalAttribute>();
            var terminalType = property.PropertyType;
            var existingTerminalWithSameName = _node.Terminals.FirstOrDefault(t => t.Name == property.Name);
            var terminalModel = existingTerminalWithSameName?.Model ?? CreateTerminalModel(property, terminalAttribute, terminalType);
            terminalModel.OnDataSet = data => property.SetValue(_node, data);
            terminalModel.OnDataGet = () => property.GetValue(_node);
            if (existingTerminalWithSameName == null)
            {
                _node.AddTerminal(terminalModel);
            }
        }

        private void NodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var existingTerminal = _node.Terminals.FirstOrDefault(t => t.Name == e.PropertyName);
            if (existingTerminal?.Model is OutputTerminalModel outputTerminal)
            {
                var data = outputTerminal.OnDataGet();
                outputTerminal.Data = data;
            }
        }
    }
}