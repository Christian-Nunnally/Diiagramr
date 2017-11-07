using System.Collections.Generic;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using System.ComponentModel;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IProvideNodes : INotifyPropertyChanged
    {
        IProjectManager ProjectManager { get; set; }

        void RegisterNode(PluginNode node, DependencyModel dep);

        PluginNode LoadNodeViewModelFromNode(NodeModel node);

        PluginNode CreateNodeViewModelFromName(string typeFullName);

        IEnumerable<PluginNode> GetRegisteredNodes();
    }
}
