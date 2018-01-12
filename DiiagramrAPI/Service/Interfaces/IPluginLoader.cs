using System;
using System.Collections.Generic;
using System.Reflection;
using DiiagramrAPI.Model;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IPluginLoader
    {
        IEnumerable<Type> SerializeableTypes { get; set; }

        void AddPluginFromDirectory(string dirPath, NodeLibrary libraryDependency);
    }
}
