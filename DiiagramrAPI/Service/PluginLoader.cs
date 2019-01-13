using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace DiiagramrAPI.Service
{
    public class PluginLoader : IPluginLoader
    {
        private readonly IProvideNodes _nodeProvider;
        private readonly IDirectoryService _directoryService;
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

        public void AddPluginFromDirectory(string dirPath, NodeLibrary libraryDependency)
        {
            foreach (var pluginAssembly in GetPluginAssemblies(dirPath))
            {
                LoadAssembly(pluginAssembly, libraryDependency);
            }
        }

        private IEnumerable<Assembly> GetPluginAssemblies(string directory)
        {
            return _directoryService.GetFiles(directory, "*.dll", SearchOption.AllDirectories).Select(Assembly.LoadFile);
        }

        private void GetInstalledPlugins()
        {
            var directories = _directoryService.GetDirectories(_pluginDirectory);
            directories.ForEach(d => AddPluginFromDirectory(d, CreateLibraryDescriptionFromNuspec(d)));
        }

        private NodeLibrary CreateLibraryDescriptionFromNuspec(string directory)
        {
            var nuspec = _directoryService.ReadAllText(_directoryService.GetFiles(directory, "*.nuspec").First());
            const string nameSearchString = "{http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd}id";
            const string versionSearchString = "{http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd}version";
            var xmlElement = XElement.Parse(nuspec);
            var libraryName = xmlElement.Descendants(nameSearchString).First().Value;
            var libraryMajorVersion = int.Parse(xmlElement.Descendants(versionSearchString).First().Value.Substring(0, 1));
            return new NodeLibrary(libraryName, "", libraryMajorVersion, 0, 0);
        }

        private void LoadAssembly(Assembly assembly, NodeLibrary nodeLibrary)
        {
            RegisterPluginNodesFromAssembly(assembly, nodeLibrary);
            LoadSerializeableTypesFromAssembly(assembly);
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

        private void RegisterPluginNodesFromAssembly(Assembly assembly, NodeLibrary libraryDependency)
        {
            foreach (var exportedType in assembly.ExportedTypes)
            {
                if (typeof(PluginNode).IsAssignableFrom(exportedType) && !exportedType.IsAbstract)
                {
                    TryRegisterNode(libraryDependency, exportedType);
                }
            }
        }

        private void TryRegisterNode(NodeLibrary libraryDependency, Type exportedType)
        {
            try
            {
                _nodeProvider.RegisterNode((PluginNode)Activator.CreateInstance(exportedType), libraryDependency);
            }
            catch (MissingMethodException)
            {
                Console.Error.WriteLine($"Unable to register node with type {exportedType} because it doesn't have a public parameterless constructor.");
            }
        }

        private void LoadNonPluginDll()
        {
            var dlls = _directoryService.GetFiles(_pluginDirectory, "*.dll").Select(Assembly.LoadFile);
            dlls.ForEach(dll => LoadAssembly(dll, new NodeLibrary()));
        }
    }
}
