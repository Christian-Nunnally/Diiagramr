using System;
using System.Collections.Generic;
using Diiagramr.View;
using Diiagramr.View.Diagram;
using Diiagramr.View.Diagram.CoreNode;
using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.Diagram;
using DiiagramrAPI.ViewModel.Diagram.CoreNode;
using Stylet;

namespace Diiagramr
{
    public class CustomViewManager : ViewManager
    {
        private readonly Dictionary<Type, Type> viewModelToViewMapping;

        public CustomViewManager(ViewManagerConfig config) : base(config)
        {
        }

        protected override Type LocateViewForModel(Type modelType)
        {
            if (modelType == typeof(ShellViewModel)) return typeof(ShellView);
            if (modelType == typeof(ProjectExplorerViewModel)) return typeof(ProjectExplorerView);
            if (modelType == typeof(NodeSelectorViewModel)) return typeof(NodeSelectorView);
            if (modelType == typeof(DiagramWellViewModel)) return typeof(DiagramWellView);
            if (modelType == typeof(WireViewModel)) return typeof(WireView);
            if (modelType == typeof(TerminalViewModel)) return typeof(TerminalView);
            if (modelType == typeof(OutputTerminalViewModel)) return typeof(OutputTerminalView);
            if (modelType == typeof(InputTerminalViewModel)) return typeof(InputTerminalView);
            if (modelType == typeof(DiagramViewModel)) return typeof(DiagramView);
            if (modelType == typeof(DiagramControlViewModel)) return typeof(DiagramControlView);
            if (modelType == typeof(NumberNodeViewModel)) return typeof(NumberNodeView);
            if (modelType == typeof(DiagramOutputNodeViewModel)) return typeof(DiagramOutputNodeView);
            if (modelType == typeof(DiagramInputNodeViewModel)) return typeof(DiagramInputNodeView);
            if (modelType == typeof(DiagramCallNodeViewModel)) return typeof(DiagramCallNodeView);
            if (modelType == typeof(AddNodeViewModel)) return typeof(AddNodeView);
            return base.LocateViewForModel(modelType);
        }
    }
}