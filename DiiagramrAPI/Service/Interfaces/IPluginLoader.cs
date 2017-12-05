using DiiagramrAPI.Model;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IPluginLoader
    {
        void AddPluginFromDirectory(string dirPath, NodeLibrary libraryDependency);
    }
}
