using System.Collections.Generic;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IProvideNodes
    {
        IProjectManager ProjectManager { get; set; }

        void RegisterNode(PluginNode node);

        PluginNode LoadNodeViewModelFromNode(NodeModel node);

        PluginNode CreateNodeViewModelFromName(string typeFullName);

        IEnumerable<PluginNode> GetRegisteredNodes();
    }
}
