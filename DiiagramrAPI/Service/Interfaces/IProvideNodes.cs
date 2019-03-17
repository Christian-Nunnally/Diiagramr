﻿using DiiagramrAPI.Diagram;
using DiiagramrAPI.Diagram.Model;
using System.Collections.Generic;
using System.ComponentModel;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IProvideNodes : INotifyPropertyChanged, IService
    {
        Node CreateNodeViewModelFromName(string typeFullName);

        IEnumerable<Node> GetRegisteredNodes();

        Node LoadNodeViewModelFromNode(NodeModel node);

        void RegisterNode(Node node, NodeLibrary library);
    }
}
