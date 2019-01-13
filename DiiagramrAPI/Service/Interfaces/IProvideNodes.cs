using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using System.Collections.Generic;
using System.ComponentModel;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IProvideNodes : INotifyPropertyChanged, IDiiagramrService
    {
        void RegisterNode(PluginNode node, NodeLibrary library);

        PluginNode LoadNodeViewModelFromNode(NodeModel node);

        PluginNode CreateNodeViewModelFromName(string typeFullName);

        IEnumerable<PluginNode> GetRegisteredNodes();
    }
}
