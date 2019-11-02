using DiiagramrAPI.Editor;
using DiiagramrAPI.Service.Interfaces;
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
    public class PluginLoader : IPluginLoader
    {
        private readonly IDirectoryService _directoryService;
        private readonly IProvideNodes _nodeProvider;
        private readonly string _pluginDirectory;
        private List<Type> _serializeableTypes = new List<Type>();

        public PluginLoader(
            Func<IProvideNodes> nodeProviderFactory,
            Func<IDirectoryService> directoryServiceFactory)
        {
            _nodeProvider = nodeProviderFactory.Invoke();
            _directoryService = directoryServiceFactory.Invoke();
            _pluginDirectory = _directoryService.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Plugins";
            RegisterPluginNodesFromAssembly(Assembly.Load(nameof(DiiagramrAPI)), new NodeLibrary());
            if (!_directoryService.Exists(_pluginDirectory))
            {
                _directoryService.CreateDirectory(_pluginDirectory);
            }

            LoadNonPluginDll();
            GetInstalledPlugins();
        }

        public IEnumerable<Type> SerializeableTypes
        {
            get => _serializeableTypes;
            set => _serializeableTypes = value.ToList();
        }

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
            return new NodeLibrary(libraryName, "", libraryMajorVersion, 0, 0);
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
                    _serializeableTypes.Add(exportedType);
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
                _nodeProvider.RegisterNode((Node)Activator.CreateInstance(exportedType), libraryDependency);
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

            public PluginLoadException(string message) : base(message)
            {
            }
        }
    }
}