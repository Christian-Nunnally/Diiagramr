using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiiagramrAPI.Service
{
    public class PluginWatcher : IPluginWatcher
    {
        public IList<Assembly> Assemblies { get; private set; }
        
        public PluginWatcher()
        {
            Assemblies = new List<Assembly>();
            Assemblies.Add(Assembly.Load(nameof(DiiagramrAPI)));
            GetPluginAssemblies().ForEach(Assemblies.Add);
            PluginAddedWatcher();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public void PluginAddedWatcher()
        {
            FileSystemWatcher watch = new FileSystemWatcher(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Plugins");
            FileSystemEventHandler createdHandler = (s, e) =>
            {
                GetPluginAssemblies().ForEach(Assemblies.Add);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Assembly"));
            };

            watch.Created += createdHandler;

            watch.EnableRaisingEvents = true;
        }


        private IEnumerable<Assembly> GetPluginAssemblies()
        {
            var pluginDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Plugins";
            if (!Directory.Exists(pluginDir)) Directory.CreateDirectory(pluginDir);
            return Directory.GetFiles(pluginDir, "*.dll").Select(Assembly.LoadFile);
        }
    }
}
