using System.Collections.Generic;
using System.ComponentModel;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IProvideNodes : INotifyPropertyChanged
    {
        void RegisterNode(PluginNode node, DependencyModel dependency);

        PluginNode LoadNodeViewModelFromNode(NodeModel node);

        PluginNode CreateNodeViewModelFromName(string typeFullName);

        IEnumerable<PluginNode> GetRegisteredNodes();
    }
}