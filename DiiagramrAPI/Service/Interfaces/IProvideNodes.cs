using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using System.Collections.Generic;
using System.ComponentModel;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IProvideNodes : INotifyPropertyChanged, IService
    {
        PluginNode CreateNodeViewModelFromName(string typeFullName);

        IEnumerable<PluginNode> GetRegisteredNodes();

        PluginNode LoadNodeViewModelFromNode(NodeModel node);

        void RegisterNode(PluginNode node, NodeLibrary library);
    }
}
