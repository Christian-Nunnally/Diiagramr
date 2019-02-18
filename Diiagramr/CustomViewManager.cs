using Diiagramr.View;
using Diiagramr.View.Diagram;
using Diiagramr.View.Diagram.CoreNode;
using Diiagramr.View.ShellScreen;
using Diiagramr.View.ShellScreen.ProjectScreen;
using Diiagramr.View.VisualDrop;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.Diagram;
using DiiagramrAPI.ViewModel.Diagram.CoreNode;
using DiiagramrAPI.ViewModel.ProjectScreen;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;
using DiiagramrAPI.ViewModel.VisualDrop;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Diiagramr
{
    public class DiiagramrViewManager : ViewManager
    {
        private readonly Dictionary<Type, Type> _viewModelToViewMapping = new Dictionary<Type, Type>();

        public DiiagramrViewManager(ViewManagerConfig config) : base(config)
        {
            ViewFactory = Activator.CreateInstance;

            _viewModelToViewMapping.Add(typeof(ShellViewModel), typeof(ShellView));
            _viewModelToViewMapping.Add(typeof(ProjectScreenViewModel), typeof(ProjectScreenView));
            _viewModelToViewMapping.Add(typeof(LibraryManagerWindowViewModel), typeof(LibraryManagerWindowView));
            _viewModelToViewMapping.Add(typeof(LibrarySourceManagerWindowViewModel), typeof(LibrarySourceManagerWindowView));
            _viewModelToViewMapping.Add(typeof(StartScreenViewModel), typeof(StartScreenView));
            _viewModelToViewMapping.Add(typeof(VisualDropStartScreenViewModel), typeof(VisualDropStartScreenView));
            _viewModelToViewMapping.Add(typeof(ProjectExplorerViewModel), typeof(ProjectExplorerView));
            _viewModelToViewMapping.Add(typeof(NodeSelectorViewModel), typeof(NodeSelectorView));
            _viewModelToViewMapping.Add(typeof(DiagramWellViewModel), typeof(DiagramWellView));
            _viewModelToViewMapping.Add(typeof(WireViewModel), typeof(WireView));
            _viewModelToViewMapping.Add(typeof(TerminalViewModel), typeof(TerminalView));
            _viewModelToViewMapping.Add(typeof(OutputTerminalViewModel), typeof(OutputTerminalView));
            _viewModelToViewMapping.Add(typeof(InputTerminalViewModel), typeof(InputTerminalView));
            _viewModelToViewMapping.Add(typeof(DiagramViewModel), typeof(DiagramView));
            _viewModelToViewMapping.Add(typeof(DiagramControlViewModel), typeof(DiagramControlView));
            _viewModelToViewMapping.Add(typeof(NumberNodeViewModel), typeof(NumberNodeView));
            _viewModelToViewMapping.Add(typeof(DiagramOutputNodeViewModel), typeof(DiagramOutputNodeView));
            _viewModelToViewMapping.Add(typeof(DiagramInputNodeViewModel), typeof(DiagramInputNodeView));
            _viewModelToViewMapping.Add(typeof(DiagramCallNodeViewModel), typeof(DiagramCallNodeView));
            _viewModelToViewMapping.Add(typeof(AddNodeViewModel), typeof(AddNodeView));
            _viewModelToViewMapping.Add(typeof(ContextMenuViewModel), typeof(ContextMenuView));
        }

        protected override Type LocateViewForModel(Type modelType)
        {
            if (_viewModelToViewMapping.ContainsKey(modelType))
            {
                return _viewModelToViewMapping[modelType];
            }

            if (!modelType.IsSubclassOf(typeof(PluginNode)))
            {
                throw new ViewNotFoundException(modelType.FullName);
            }

            var viewModelName = modelType.Name;
            var viewName = viewModelName.Substring(0, viewModelName.Length - 5);
            var assembly = Assembly.GetAssembly(modelType);
            if (!ViewAssemblies.Contains(assembly))
            {
                ViewAssemblies.Add(assembly);
            }
            var viewType = assembly.ExportedTypes.First(t => t.Name == viewName);
            _viewModelToViewMapping.Add(modelType, viewType);
            return viewType;
        }
    }

    public class ViewNotFoundException : Exception
    {
        public ViewNotFoundException(string message) : base(message)
        {
        }
    }
}
