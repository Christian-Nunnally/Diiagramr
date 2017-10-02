using System.Collections.Generic;
using Diiagramr.Model;
using Diiagramr.PluginNodeApi;

namespace Diiagramr.Service.Interfaces
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
