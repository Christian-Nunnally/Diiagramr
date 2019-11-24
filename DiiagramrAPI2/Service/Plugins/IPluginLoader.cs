using DiiagramrModel;
using System;
using System.Collections.Generic;

namespace DiiagramrAPI.Service.Plugins
{
    public interface IPluginLoader : IService
    {
        IEnumerable<Type> SerializeableTypes { get; set; }

        bool AddPluginFromDirectory(string dirPath, NodeLibrary libraryDependency);
    }
}