using DiiagramrAPI.Diagram.Interoperability;
using System;
using System.Collections.Generic;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IPluginLoader : IService
    {
        IEnumerable<Type> SerializeableTypes { get; set; }

        bool AddPluginFromDirectory(string dirPath, NodeLibrary libraryDependency);
    }
}
