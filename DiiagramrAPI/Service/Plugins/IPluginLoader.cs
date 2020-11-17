using DiiagramrModel;

namespace DiiagramrAPI.Service.Plugins
{
    /// <summary>
    /// Interface for loading plugins from a directory.
    /// </summary>
    public interface IPluginLoader : ISingletonService
    {
        /// <summary>
        /// Add plugin to the system.
        /// </summary>
        /// <param name="pluginDirectoryPath">The plugin to load.</param>
        /// <param name="libraryDependency">The dependancy this plugin has to add to the project it is used in.</param>
        /// <returns>True if the plugin was added.</returns>
        bool AddPluginFromDirectory(string pluginDirectoryPath, NodeLibrary libraryDependency);
    }
}