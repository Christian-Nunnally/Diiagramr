using DiiagramrAPI.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IPluginLoader
    {
        void AddPluginFromDirectory(string dirPath, DependencyModel dependency);
    }
}
