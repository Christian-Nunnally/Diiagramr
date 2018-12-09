using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace DiiagramrAPI.Service
{
    public class ProjectLoadSave2 : IProjectLoadSave
    {
        private readonly IPluginLoader _pluginLoader;

        public ProjectLoadSave2(Func<IPluginLoader> pluginLoaderFactory)
        {
            _pluginLoader = pluginLoaderFactory.Invoke();
        }

        public ProjectModel Open(string fileName)
        {
            XDocument document = XDocument.Load(fileName);
            var projectElement = document.Root;
            var project = CreateModelFrom(projectElement, () => new ProjectModel());
            var diagramElements = projectElement.Descendants(nameof(DiagramModel));

            foreach (var diagramElement in diagramElements)
            {
                var terminalToIdMap = new Dictionary<int, TerminalModel>();
                var diagram = CreateModelFrom(diagramElement, () => new DiagramModel());
                var nodeElements = diagramElement.Descendants(nameof(NodeModel));
                foreach (var nodeElement in nodeElements)
                {
                    var node = CreateModelFrom(nodeElement, () => new NodeModel(string.Empty));
                    var terminalElements = nodeElement.Descendants(nameof(TerminalModel));
                    foreach (var terminalElement in terminalElements)
                    {
                        var terminal = CreateTerminalModelFrom(terminalElement);
                        terminalToIdMap.Add(terminal.Id, terminal);
                        node.AddTerminal(terminal);
                    }
                    diagram.AddNode(node);
                }
                project.AddDiagram(diagram);

                var wireElements = diagramElement.Descendants(nameof(WireModel));
                foreach (var wireElement in wireElements)
                {
                    var sinkTerminalId = int.Parse(wireElement.Descendants(nameof(WireModel.SinkTerminal)).First().Value);
                    var sourceTerminalId = int.Parse(wireElement.Descendants(nameof(WireModel.SourceTerminal)).First().Value);
                    var sinkTerminal = terminalToIdMap[sinkTerminalId];
                    var sourceTerminal = terminalToIdMap[sourceTerminalId];
                    var wire = new WireModel(sinkTerminal, sourceTerminal);
                }
            }
            var loaded = AppDomain.CurrentDomain.GetAssemblies();
            return project;
        }

        public void Save(ProjectModel project, string fileName)
        {
            XDocument document = new XDocument();

            var projectElement = CreateElementFrom(project);
            foreach (var diagram in project.Diagrams)
            {
                var diagramElement = CreateElementFrom(diagram, projectElement);
                foreach (var node in diagram.Nodes)
                {
                    var nodeElement = CreateElementFrom(node, diagramElement);
                    node.Terminals.ForEach(t => CreateTerminalElementFrom(t, nodeElement));
                }

                diagram.Nodes
                    .SelectMany(n => n.Terminals)
                    .SelectMany(t => t.ConnectedWires)
                    .Distinct()
                    .ForEach(w => CreateWireElementFrom(w, diagramElement));
            }
            document.Add(projectElement);
            document.Save(fileName);
        }

        private XElement CreateElementFrom(ModelBase model, XElement parent = null)
        {
            var element = new XElement(model.GetType().Name,
                new XElement(nameof(ModelBase.Name), model.Name),
                new XElement(nameof(ModelBase.Id), model.Id));
            parent?.Add(element);
            return element;
        }

        private XElement CreateTerminalElementFrom(TerminalModel terminal, XElement parent = null)
        {
            var terminalElement = CreateElementFrom(terminal, parent);
            terminalElement.Add(new XElement(nameof(TerminalModel.Kind), terminal.Kind));
            terminalElement.Add(new XElement(nameof(TerminalModel.Type), terminal.Type.AssemblyQualifiedName));
            terminalElement.Add(new XElement(nameof(TerminalModel.Direction), terminal.Direction));
            terminalElement.Add(new XElement(nameof(TerminalModel.TerminalIndex), terminal.TerminalIndex));

            // do something about custom types

            return terminalElement;
        }

        private XElement CreateWireElementFrom(WireModel wire, XElement parent = null)
        {
            var wireElement = CreateElementFrom(wire, parent);
            wireElement.Add(new XElement(nameof(WireModel.SinkTerminal), wire.SinkTerminal.Id));
            wireElement.Add(new XElement(nameof(WireModel.SourceTerminal), wire.SourceTerminal.Id));
            return wireElement;
        }

        private T CreateModelFrom<T>(XElement element, Func<T> factory) where T : ModelBase
        {
            var model = factory.Invoke();
            model.Name = element.Descendants(nameof(model.Name)).First().Value;
            model.Id = int.Parse(element.Descendants(nameof(model.Id)).First().Value);
            return model;
        }

        private TerminalModel CreateTerminalModelFrom(XElement element)
        {
            var name = element.Descendants(nameof(TerminalModel.Name)).First().Value;
            var id = int.Parse(element.Descendants(nameof(TerminalModel.Id)).First().Value);
            var typeString = element.Descendants(nameof(TerminalModel.Type)).First().Value;
            var direction = (PluginNodeApi.Direction)Enum.Parse(typeof(PluginNodeApi.Direction), element.Descendants(nameof(TerminalModel.Direction)).First().Value);
            var kind = (TerminalKind)Enum.Parse(typeof(TerminalKind), element.Descendants(nameof(TerminalModel.Kind)).First().Value);
            var index = int.Parse(element.Descendants(nameof(TerminalModel.TerminalIndex)).First().Value);
            var type = Type.GetType(typeString) ?? Type.GetType(typeString, PluginLoader.AssemblyResolver, TypeResolver);
            return new TerminalModel(name, type, direction, kind, index) { Id = id };
        }

        private Type TypeResolver(Assembly assembly, string name, bool ignore)
        {
            return assembly == null ? Type.GetType(name, true, ignore) : assembly.GetType(name, true, ignore);
        }
    }
}