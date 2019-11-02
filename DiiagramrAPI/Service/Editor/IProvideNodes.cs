using DiiagramrAPI.Editor;
using DiiagramrModel;
using System.Collections.Generic;
using System.ComponentModel;

namespace DiiagramrAPI.Service.Editor
{
    public interface IProvideNodes : INotifyPropertyChanged, IService
    {
        Node CreateNodeFromName(string typeFullName);

        IEnumerable<Node> GetRegisteredNodes();

        Node LoadNodeViewModelFromNode(NodeModel node);

        void RegisterNode(Node node, NodeLibrary library);
    }
}