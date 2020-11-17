using DiiagramrAPI.Application;
using DiiagramrAPI.Editor;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Editor;
using DiiagramrAPI.Service.IO;
using DiiagramrCore;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace DiiagramrAPI.Service.Plugins
{
    /// <summary>
    /// Loads plugin dlls and registers the contents with the appropriate systems.
    /// </summary>
    public class PluginLoader : IPluginLoader
    {
        private readonly IDirectoryService _directoryService;
        private readonly INodeProvider _nodeProvider;
        private readonly string _pluginDirectory;

        /// <summary>
        /// Creates a new instance of <see cref="PluginLoader"/>.
        /// </summary>
        /// <param name="nodeProviderFactory">A factory that returns an instance of <see cref="INodeProvider"/>.</param>
        /// <param name="directoryServiceFactory">A factory that returns an instance of <see cref="IDirectoryService"/>.</param>
        public PluginLoader(
            Func<INodeProvider> nodeProviderFactory,
            Func<IDirectoryService> directoryServiceFactory)
        {
            _nodeProvider = nodeProviderFactory.Invoke();
            _directoryService = directoryServiceFactory.Invoke();
            _pluginDirectory = _directoryService.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Plugins";
            LoadAssembly(Assembly.Load(nameof(DiiagramrAPI)), new NodeLibrary());
            if (!_directoryService.Exists(_pluginDirectory))
            {
                _directoryService.CreateDirectory(_pluginDirectory);
            }

            LoadNonPluginDll();
            GetInstalledPlugins();
        }

        /// <inheritdoc/>
        public bool AddPluginFromDirectory(string dirPath, NodeLibrary libraryDependency)
        {
            try
            {
                foreach (var pluginAssembly in GetPluginAssemblies(dirPath))
                {
                    LoadAssembly(pluginAssembly, libraryDependency);
                }

                return true;
            }
            catch (TypeLoadException)
            {
                return false;
            }
        }

        private NodeLibrary CreateLibraryDescriptionFromNuspec(string directory)
        {
            var nuspec = ReadNuspecInDirectory(directory);
            const string nameSearchString = "{http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd}id";
            const string versionSearchString = "{http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd}version";
            var xmlElement = XElement.Parse(nuspec);
            var libraryName = xmlElement.Descendants(nameSearchString).First().Value;
            var libraryMajorVersion = int.Parse(xmlElement.Descendants(versionSearchString).First().Value.Substring(0, 1));
            return new NodeLibrary(libraryName, string.Empty, libraryMajorVersion, 0, 0);
        }

        private void GetInstalledPlugins()
        {
            var directories = _directoryService.GetDirectories(_pluginDirectory);
            try
            {
                directories.ForEach(d => AddPluginFromDirectory(d, CreateLibraryDescriptionFromNuspec(d)));
            }
            catch (PluginLoadException)
            {
                Debug.WriteLine("Error loading plugins: a plugin is missing a .nuspec file.");
            }
        }

        private IEnumerable<Assembly> GetPluginAssemblies(string directory)
        {
            return _directoryService
                .GetFiles(directory, "*.dll", SearchOption.AllDirectories)
                .Select(path => Assembly.Load(File.ReadAllBytes(path)));
        }

        private void LoadAssembly(Assembly assembly, NodeLibrary nodeLibrary)
        {
            RegisterPluginNodesFromAssembly(assembly, nodeLibrary);
            LoadSerializeableTypesFromAssembly(assembly);
            LoadColorsFromAssembly(assembly);
        }

        private void LoadColorsFromAssembly(Assembly assembly)
        {
            foreach (var wireableType in assembly.GetExportedTypes()
                .Where(t => t.GetInterface("IWireableType") != null)
                .Where(t => t.IsClass && !t.IsAbstract))
            {
                var wireableInstance = (IWireableType)Activator.CreateInstance(wireableType);
                var color = wireableInstance.GetTypeColor();
                TypeColorProvider.Instance.RegisterColorForType(wireableType, color);
            }
        }

        private void LoadNonPluginDll()
        {
            var dlls = _directoryService
                .GetFiles(_pluginDirectory, "*.dll")
                .Select(path => Assembly.Load(File.ReadAllBytes(path)));
            dlls.ForEach(dll => LoadAssembly(dll, new NodeLibrary()));
        }

        private void LoadSerializeableTypesFromAssembly(Assembly assembly)
        {
            foreach (var exportedType in assembly.ExportedTypes)
            {
                if ((exportedType.Attributes & TypeAttributes.Serializable) != 0)
                {
                    ModelBase.SerializeableTypes.Add(exportedType);
                }
                if (exportedType.IsClass && typeof(ISerializableTypeProvider).IsAssignableFrom(exportedType))
                {
                    if (Activator.CreateInstance(exportedType) is ISerializableTypeProvider provider)
                    {
                        provider.SerializableTypes.ForEach(t => ModelBase.SerializeableTypes.Add(t));
                    }
                }
            }
        }

        private string ReadNuspecInDirectory(string directory)
        {
            var nuspecPath = _directoryService
                .GetFiles(directory, "*.nuspec")
                .FirstOrDefault();
            if (nuspecPath == null)
            {
                throw new PluginLoadException($".nuspec file not found in directory {directory}");
            }

            return _directoryService.ReadAllText(nuspecPath);
        }

        private void RegisterPluginNodesFromAssembly(Assembly assembly, NodeLibrary libraryDependency)
        {
            foreach (var exportedType in assembly.ExportedTypes)
            {
                if (typeof(Node).IsAssignableFrom(exportedType) && !exportedType.IsAbstract)
                {
                    TryRegisterNode(libraryDependency, exportedType);
                }
            }
        }

        private void TryRegisterNode(NodeLibrary libraryDependency, Type exportedType)
        {
            try
            {
                var node = (Node)Activator.CreateInstance(exportedType);
                foreach (var terminal in node.Terminals)
                {
                    ModelBase.SerializeableTypes.Add(terminal.Model.Type);
                }
                _nodeProvider.RegisterNode(node, libraryDependency);
            }
            catch (TypeLoadException)
            {
                Console.Error.WriteLine($"Unable to register node with type {exportedType}.");
            }
            catch (MissingMethodException)
            {
                Console.Error.WriteLine($"Unable to register node with type {exportedType}. This might be because it doesn't have a public parameterless constructor.");
            }
        }

        [Serializable]
        private class PluginLoadException : Exception
        {
            public PluginLoadException()
            {
            }

            public PluginLoadException(string message)
                : base(message)
            {
            }
        }
    }
}