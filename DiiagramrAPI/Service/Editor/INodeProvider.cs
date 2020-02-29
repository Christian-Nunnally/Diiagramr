﻿using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System.Collections.Generic;
using System.ComponentModel;

namespace DiiagramrAPI.Service.Editor
{
    public interface INodeProvider : INotifyPropertyChanged, ISingletonService
    {
        Node CreateNodeFromName(string typeFullName);

        IEnumerable<Node> GetRegisteredNodes();

        Node CreateNodeFromModel(NodeModel node);

        void RegisterNode(Node node, NodeLibrary library);
    }
}