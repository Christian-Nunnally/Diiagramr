using DiiagramrCore;
using DiiagramrModel;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DiiagramrAPI.Editor.Diagrams
{
    public class NodeTerminalCreator
    {
        private readonly Node _node;

        public NodeTerminalCreator(Node node)
        {
            _node = node;
            _node.PropertyChanged += NodePropertyChanged;
        }

        public void CreateTerminals()
        {
            _node.GetType()
                .GetMethods()
                .Where(x => Attribute.IsDefined(x, typeof(InputTerminalAttribute)))
                .ForEach(CreateInputTerminalForMethod);
            _node.GetType()
                .GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(OutputTerminalAttribute)))
                .ForEach(CreateOutputTerminalForProperty);
        }

        private void CreateInputTerminalForMethod(MethodInfo methodInfo)
        {
            ValidateInputTerminalMethod(methodInfo);
            var inputTerminalAttribute = methodInfo.GetAttribute<InputTerminalAttribute>();
            var terminalType = methodInfo.GetParameters().First().ParameterType;
            var existingTerminalWithSameName = _node.Terminals.FirstOrDefault(t => t.Name == methodInfo.Name);
            var terminalModel = existingTerminalWithSameName?.TerminalModel
                ?? new InputTerminalModel(methodInfo.Name, terminalType, inputTerminalAttribute.DefaultDirection, 0);
            terminalModel.DataChanged += methodInfo.CreateMethodInvoker(_node);
            if (existingTerminalWithSameName == null)
            {
                _node.AddTerminal(terminalModel);
            }
        }

        private void CreateOutputTerminalForProperty(PropertyInfo property)
        {
            var outputTerminalAttribute = property.GetAttribute<OutputTerminalAttribute>();
            var terminalType = property.PropertyType;
            var existingTerminalWithSameName = _node.Terminals.FirstOrDefault(t => t.Name == property.Name);
            var outputTerminalModel = existingTerminalWithSameName?.TerminalModel as OutputTerminalModel
                ?? new OutputTerminalModel(property.Name, terminalType, outputTerminalAttribute.DefaultDirection, 0);
            outputTerminalModel.GetDataFromSource = () => property.GetValue(_node);
            outputTerminalModel.UpdateDataFromSource();
            if (existingTerminalWithSameName == null)
            {
                _node.AddTerminal(outputTerminalModel);
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

        private void NodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var existingTerminal = _node.Terminals.FirstOrDefault(t => t.Name == e.PropertyName);
            if (existingTerminal?.TerminalModel is OutputTerminalModel outputTerminal)
            {
                outputTerminal.UpdateDataFromSource();
            }
        }
    }
}