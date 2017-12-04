using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service.Interfaces;
using StyletIoC.Internal;

namespace DiiagramrAPI.Service
{
    public class PluginLoader : IPluginLoader
    {
        private readonly IProvideNodes _nodeProvider;
        private readonly IDirectoryService _directoryService;
        private readonly string _pluginDirectory;

        public PluginLoader(
            Func<IProvideNodes> nodeProviderFactory,
            Func<IDirectoryService> directoryServiceFactory)
        {
            _nodeProvider = nodeProviderFactory.Invoke();
            _directoryService = directoryServiceFactory.Invoke();
            _pluginDirectory = _directoryService.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Plugins";
            RegisterNodeFromAssembly(Assembly.Load(nameof(DiiagramrAPI)), new DependencyModel("", 0));
            if (!_directoryService.Exists(_pluginDirectory)) _directoryService.CreateDirectory(_pluginDirectory);
            LoadlNonPluginDll();
            GetInstalledPlugins();
        }

        public void AddPluginFromDirectory(string dirPath, DependencyModel dependency)
        {
            foreach (var pluginAssembly in GetPluginAssemblies(dirPath))
                RegisterNodeFromAssembly(pluginAssembly, dependency);
        }

        private IEnumerable<Assembly> GetPluginAssemblies(string directory)
        {
            return _directoryService.GetFiles(directory, "*.dll", SearchOption.AllDirectories).Select(Assembly.LoadFile);
        }

        private void GetInstalledPlugins()
        {
            var directories = _directoryService.GetDirectories(_pluginDirectory);
            directories.ForEach(d => AddPluginFromDirectory(d, CreateDependencyFromNuspec(d)));
        }

        private DependencyModel CreateDependencyFromNuspec(string directory)
        {
            var nuspec = _directoryService.ReadAllText(_directoryService.GetFiles(directory, "*.nuspec").First());
            const string nameSearchString = "{http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd}id";
            const string versionSearchString = "{http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd}version";
            var xmlElement = XElement.Parse(nuspec);
            var libraryName = xmlElement.Descendants(nameSearchString).First().Value;
            var libraryVersion = int.Parse(xmlElement.Descendants(versionSearchString).First().Value.Substring(0, 1));
            return new DependencyModel(libraryName, libraryVersion);
        }

        private void RegisterNodeFromAssembly(Assembly assembly, DependencyModel dependency)
        {
            foreach (var exportedType in assembly.ExportedTypes)
                if (exportedType.Implements(typeof(PluginNode)) && !exportedType.IsAbstract)
                    _nodeProvider.RegisterNode((PluginNode) Activator.CreateInstance(exportedType), dependency);
        }

        private void LoadlNonPluginDll()
        {
            var dlls = _directoryService.GetFiles(_pluginDirectory, "*.dll").Select(Assembly.LoadFile);
            dlls.ForEach(dll => RegisterNodeFromAssembly(dll, new DependencyModel("", 0)));
        }
    }
}