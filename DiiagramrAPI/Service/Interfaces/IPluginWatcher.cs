using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IPluginWatcher : INotifyPropertyChanged
    {
        IList<Assembly> Assemblies { get; }

        void PluginAddedWatcher();
    }
}
