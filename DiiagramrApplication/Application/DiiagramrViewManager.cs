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
        }

        protected override Type LocateViewForModel(Type modelType)
        {
            if (_viewModelToViewMapping.ContainsKey(modelType))
            {
                return _viewModelToViewMapping[modelType];
            }

            var viewModelName = modelType.Name;
            var viewName = GuessViewName(viewModelName);

            var assembly = Assembly.GetAssembly(modelType);
            if (!ViewAssemblies.Contains(assembly))
            {
                ViewAssemblies.Add(assembly);
            }
            var viewType = ViewAssemblies.SelectMany(a => a.ExportedTypes).FirstOrDefault(t => t.Name == viewName);
            _viewModelToViewMapping.Add(modelType, viewType);

            if (viewType == null)
            {
                throw new ViewNotFoundException(modelType.FullName);
            }

            return viewType;
        }

        private string GuessViewName(string viewModelName)
        {
            return viewModelName.EndsWith("ViewModel")
                ? viewModelName.Substring(0, viewModelName.Length - 5)
                : viewModelName + "View";
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