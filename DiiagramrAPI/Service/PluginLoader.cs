using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using Stylet;
using StyletIoC;
using StyletIoC.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DiiagramrAPI.Service
{
    public class PluginLoader : IPluginLoader
    {
        private readonly IProvideNodes _nodeProvider;
        private string _pluginDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Plugins";
        
        public PluginLoader(Func<IProvideNodes> nodeProvider)
        {
            _nodeProvider = nodeProvider.Invoke();
            RegisterNodeFromAssembly(Assembly.Load(nameof(DiiagramrAPI)), new DependencyModel("",""));
            if (!Directory.Exists(_pluginDirectory)) Directory.CreateDirectory(_pluginDirectory);
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
            return Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories).Select(Assembly.LoadFile);
        }

        private void GetInstalledPlugins()
        {
            var directories = Directory.GetDirectories(_pluginDirectory);
            directories.ForEach(d => AddPluginFromDirectory(d, CreateDependencyFromNuspec(d)));
        }

        private DependencyModel CreateDependencyFromNuspec(string directory)
        {
            var nuspec = File.ReadAllText(Directory.GetFiles(directory, "*.nuspec").First());
            const string nameSearchString = "{http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd}id";
            const string versionSearchString = "{http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd}version";
            var xmlElement = XElement.Parse(nuspec);
            var libraryName = xmlElement.Descendants(nameSearchString).First().Value;
            var libraryVersion = xmlElement.Descendants(versionSearchString).First().Value;
            return new DependencyModel(libraryName, libraryVersion);
        }

        private void RegisterNodeFromAssembly(Assembly assembly, DependencyModel dependency)
        {
            foreach (var exportedType in assembly.ExportedTypes)
                if (exportedType.Implements(typeof(PluginNode)) && !exportedType.IsAbstract)
                {
                    _nodeProvider.RegisterNode((PluginNode)Activator.CreateInstance(exportedType), dependency);
                }
        }

        private void LoadlNonPluginDll()
        {
            var dlls = Directory.GetFiles(_pluginDirectory, "*.dll").Select(Assembly.LoadFile);
            dlls.ForEach(dll => RegisterNodeFromAssembly(dll, new DependencyModel("","")));
        }
    }
}
