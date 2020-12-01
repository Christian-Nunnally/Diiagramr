using DiiagramrAPI.Editor;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DiiagramrApplication.Application
{
    /// <summary>
    /// Core bootstrapper operations.
    /// </summary>
    public static class BootstrapperUtilities
    {
        /// <summary>
        /// Finds everything that implements the given type and binds it to the interface.
        /// </summary>
        /// <param name="interfaceType">The type to find implementations of.</param>
        /// <param name="builder">The IoC container builder.</param>
        /// <param name="loadedTypes">All loaded types.</param>
        /// <param name="typeReplacementMap">A special case map to use if particular types should be replaced.</param>
        public static void BindEverythingThatImplementsInterfaceAsASingleton(Type interfaceType, IStyletIoCBuilder builder, IEnumerable<Type> loadedTypes, Dictionary<Type, Type> typeReplacementMap)
        {
            var serviceImplementations = loadedTypes.Where(t => t.IsClass && t.GetInterface(interfaceType.Name) != null && !t.IsAbstract);
            foreach (var serviceImplementation in serviceImplementations)
            {
                var typeToBind = typeReplacementMap.ContainsKey(serviceImplementation)
                                    ? typeReplacementMap[serviceImplementation]
                                    : serviceImplementation;

                if (serviceImplementation.GetInterface(nameof(IKeyedService)) != null)
                {
                    var keyedService = (IKeyedService)Activator.CreateInstance(serviceImplementation);
                    builder.Bind(interfaceType).To(typeToBind).WithKey(keyedService.ServiceBindingKey);
                }
                else
                {
                    builder.Bind(interfaceType).To(typeToBind).InSingletonScope();
                }
            }
        }

        /// <summary>
        /// Binds all interfaces that implement <see cref="ISingletonService"/> to thier implementations.
        /// </summary>
        /// <param name="builder">The IoC container builder.</param>
        public static void BindServices(IStyletIoCBuilder builder)
        {
            IEnumerable<Type> loadedTypes = GetAllLoadedNonTestTypes();
            var loadedServiceInterfaces = loadedTypes.Where(t => t.IsInterface && t.GetInterface(nameof(ISingletonService)) != null);

            foreach (var loadedService in loadedServiceInterfaces)
            {
                BindEverythingThatImplementsInterfaceAsASingleton(loadedService, builder, loadedTypes, new Dictionary<Type, Type>());
            }
        }

        /// <summary>
        /// Loads all wire type colors from plugins.
        /// </summary>
        public static void LoadColorInformation()
        {
            var wireableTypes = GetAllLoadedTypesNotInTheGlobalAssemblyCache()
                .Where(t => t.GetInterface("IWireableType") != null);
            foreach (var wireableType in wireableTypes)
            {
                var wireableInstance = (IWireableType)Activator.CreateInstance(wireableType);
                var color = wireableInstance.GetTypeColor();
                TypeColorProvider.Instance.RegisterColorForType(wireableType, color);
            }
        }

        private static IEnumerable<Type> GetAllLoadedNonTestTypes()
        {
            return GetAllLoadedTypesNotInTheGlobalAssemblyCache().Where(t => t.GetInterface("ITestImplementationOf`1") == null);
        }

        private static IEnumerable<Type> GetAllLoadedTypesNotInTheGlobalAssemblyCache()
        {
            return GetLoadedAssemblies(a => !a.GlobalAssemblyCache).SelectMany(a => a.GetTypes());
        }

        private static IEnumerable<Assembly> GetLoadedAssemblies(Func<Assembly, bool> filter)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(filter);
        }
    }
}