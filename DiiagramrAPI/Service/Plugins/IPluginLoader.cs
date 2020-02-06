using DiiagramrModel;

namespace DiiagramrAPI.Service.Plugins
{
    public interface IPluginLoader : ISingletonService
    {
        bool AddPluginFromDirectory(string dirPath, NodeLibrary libraryDependency);
    }
}