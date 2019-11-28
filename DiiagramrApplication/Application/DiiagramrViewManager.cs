using Diiagramr.Application.Tools;
using Diiagramr.Editor;
using Diiagramr.Editor.Interactors;
using Diiagramr.Editor.Nodes;
using Diiagramr.Project;
using Diiagramr.View.ShellScreen;
using DiiagramrAPI.Application;
using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Editor.Interactors;
using DiiagramrAPI.Project;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Diiagramr.Application
{
    public class DiiagramrViewManager : ViewManager
    {
        private readonly Dictionary<Type, Type> _viewModelToViewMapping = new Dictionary<Type, Type>();

        public DiiagramrViewManager(ViewManagerConfig config) : base(config)
        {
            ViewFactory = Activator.CreateInstance;

            // todo: automate this
            _viewModelToViewMapping.Add(typeof(ShellViewModel), typeof(ShellView));
            _viewModelToViewMapping.Add(typeof(ProjectScreen), typeof(ProjectScreenView));
            _viewModelToViewMapping.Add(typeof(LibraryManagerWindow), typeof(LibraryManagerWindowView));
            _viewModelToViewMapping.Add(typeof(LibrarySourceManagerWindow), typeof(LibrarySourceManagerWindowView));
            _viewModelToViewMapping.Add(typeof(StartScreenViewModel), typeof(StartScreenView));
            _viewModelToViewMapping.Add(typeof(VisualDropStartScreenViewModel), typeof(VisualDropStartScreenView));
            _viewModelToViewMapping.Add(typeof(ProjectExplorer), typeof(ProjectExplorerView));
            _viewModelToViewMapping.Add(typeof(NodePalette), typeof(NodePaletteView));
            _viewModelToViewMapping.Add(typeof(DiagramWell), typeof(DiagramWellView));
            _viewModelToViewMapping.Add(typeof(Wire), typeof(WireView));
            _viewModelToViewMapping.Add(typeof(Terminal), typeof(TerminalView));
            _viewModelToViewMapping.Add(typeof(OutputTerminal), typeof(OutputTerminalView));
            _viewModelToViewMapping.Add(typeof(InputTerminal), typeof(InputTerminalView));
            _viewModelToViewMapping.Add(typeof(Diagram), typeof(DiagramView));
            _viewModelToViewMapping.Add(typeof(DiagramOutputNode), typeof(DiagramOutputNodeView));
            _viewModelToViewMapping.Add(typeof(DiagramInputNode), typeof(DiagramInputNodeView));
            _viewModelToViewMapping.Add(typeof(ContextMenu), typeof(ContextMenuView));
            _viewModelToViewMapping.Add(typeof(ToolbarViewModel), typeof(ToolbarView));
            _viewModelToViewMapping.Add(typeof(LassoNodeSelector), typeof(LassoNodeSelectorView));
            _viewModelToViewMapping.Add(typeof(PointSelector), typeof(PointSelectorView));
            _viewModelToViewMapping.Add(typeof(NodePlacer), typeof(NodePlacerView));
            _viewModelToViewMapping.Add(typeof(NodeDeleter), typeof(NodeDeleterView));
            _viewModelToViewMapping.Add(typeof(NodeDragger), typeof(NodeDraggerView));
            _viewModelToViewMapping.Add(typeof(NodeResizer), typeof(NodeResizerView));
            _viewModelToViewMapping.Add(typeof(DiagramPanner), typeof(DiagramPannerView));
            _viewModelToViewMapping.Add(typeof(DiagramZoomer), typeof(DiagramZoomerView));
            _viewModelToViewMapping.Add(typeof(DiagramInteractionManager), typeof(DiagramInteractionManagerView));
            _viewModelToViewMapping.Add(typeof(TerminalWirer), typeof(TerminalWirerView));
            _viewModelToViewMapping.Add(typeof(HotkeyHelp), typeof(HotkeyHelpView));
            _viewModelToViewMapping.Add(typeof(DiagramRifter), typeof(DiagramRifterView));
        }

        protected override Type LocateViewForModel(Type modelType)
        {
            if (_viewModelToViewMapping.ContainsKey(modelType))
            {
                return _viewModelToViewMapping[modelType];
            }

            if (!typeof(Node).IsAssignableFrom(modelType))
            {
                throw new ViewNotFoundException(modelType.FullName);
            }

            var viewModelName = modelType.Name;
            var viewName = viewModelName + "View";
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

    [Serializable]
    public class ViewNotFoundException : Exception
    {
        public ViewNotFoundException(string message) : base(message)
        {
        }
    }
}