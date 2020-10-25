﻿using DiiagramrCore;
using DiiagramrModel;
using System;
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
            _node.GetType()
                .GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(InputTerminalAttribute)))
                .ForEach(CreateInputTerminalForProperty);
        }

        private void CreateInputTerminalForMethod(MethodInfo methodInfo)
        {
            ValidateInputTerminalMethod(methodInfo);
            var inputTerminalAttribute = methodInfo.GetAttribute<InputTerminalAttribute>();
            var terminalType = methodInfo.GetParameters().First().ParameterType;
            var existingTerminalWithSameName = _node.Terminals.FirstOrDefault(t => t.Name == methodInfo.Name);
            var terminalModel = existingTerminalWithSameName?.Model
                ?? new InputTerminalModel(methodInfo.Name, terminalType, inputTerminalAttribute.DefaultDirection);
            terminalModel.DataChanged += methodInfo.CreateMethodInvoker(_node);
            if (existingTerminalWithSameName == null)
            {
                _node.AddTerminal(terminalModel);
            }
        }

        private void CreateInputTerminalForProperty(PropertyInfo property)
        {
            var inputTerminalAttribute = property.GetAttribute<InputTerminalAttribute>();
            var terminalType = property.PropertyType;
            var existingTerminalWithSameName = _node.Terminals.FirstOrDefault(t => t.Name == property.Name);
            var terminalModel = existingTerminalWithSameName?.Model
                ?? new InputTerminalModel(property.Name, terminalType, inputTerminalAttribute.DefaultDirection);
            terminalModel.DataChanged += data => property.SetValue(_node, data);
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
            var helpAttribute = property.GetCustomAttributes(typeof(HelpAttribute), true).FirstOrDefault() as HelpAttribute;
            var helpString = helpAttribute?.HelpText ?? "";
            var outputTerminalModel = existingTerminalWithSameName?.Model as OutputTerminalModel
                ?? new OutputTerminalModel(property.Name, terminalType, outputTerminalAttribute.DefaultDirection);
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
            if (existingTerminal?.Model is OutputTerminalModel outputTerminal)
            {
                outputTerminal.UpdateDataFromSource();
            }
        }
    }
}