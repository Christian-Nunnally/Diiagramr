using DiiagramrAPI.Diagram.Model;
using System;
using System.Collections.Generic;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IPluginLoader : IService
    {
        IEnumerable<Type> SerializeableTypes { get; set; }

        void AddPluginFromDirectory(string dirPath, NodeLibrary libraryDependency);
    }
}
