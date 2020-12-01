using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DiiagramrApplication.Application
{
    /// <summary>
    /// Custom <see cref="ViewManager"/> implementation to customize what assembly the application looks for views in.
    /// </summary>
    public class DiiagramrViewManager : ViewManager
    {
        private readonly Dictionary<Type, Type> _viewModelToViewMapping = new Dictionary<Type, Type>();

        /// <summary>
        /// Creates a new instance of <see cref="DiiagramrViewManager"/>.
        /// </summary>
        /// <param name="config">The configuration of the view manager.</param>
        public DiiagramrViewManager(ViewManagerConfig config) : base(config)
        {
            ViewFactory = Activator.CreateInstance;
        }

        /// <inheritdoc/>
        protected override Type LocateViewForModel(Type modelType)
        {
            if (_viewModelToViewMapping.TryGetValue(modelType, out var viewType))
            {
                return viewType;
            }
            viewType = FindViewTypeFromViewAssemblies(modelType);
            _viewModelToViewMapping.Add(modelType, viewType);
            return viewType;
        }

        private Type FindViewTypeFromViewAssemblies(Type modelType)
        {
            AddTypesAssemblyToListOfPossibleViewAssemblies(modelType);
            var viewModelName = modelType.Name;
            var viewName = GuessViewName(viewModelName);
            var viewType = ViewAssemblies
                .SelectMany(a => a.ExportedTypes)
                .FirstOrDefault(t => t.Name == viewName)
                ?? typeof(MissingView);
            return viewType;
        }

        private void AddTypesAssemblyToListOfPossibleViewAssemblies(Type type)
        {
            var assembly = Assembly.GetAssembly(type);
            if (!ViewAssemblies.Contains(assembly))
            {
                ViewAssemblies.Add(assembly);
            }
        }

        private string GuessViewName(string viewModelName)
        {
            return viewModelName.EndsWith("ViewModel")
                ? viewModelName.Substring(viewModelName.Length - 5)
                : viewModelName + "View";
        }
    }
}