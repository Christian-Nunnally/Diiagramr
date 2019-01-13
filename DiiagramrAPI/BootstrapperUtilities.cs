using DiiagramrAPI.Service.Interfaces;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiiagramrAPI
{
    public static class BootstrapperUtilities
    {
        public static void BindEverythingThatImplementsTheInterface(Type intreface, IStyletIoCBuilder builder, IEnumerable<Type> loadedTypes, Dictionary<Type, Type> typeReplacementMap)
        {
            var serviceImplementations = loadedTypes.Where(t => t.IsClass && t.GetInterface(intreface.Name) != null && !t.IsAbstract);
            foreach (var serviceImplementation in serviceImplementations)
            {
                var typeToBind = typeReplacementMap.ContainsKey(serviceImplementation)
                                    ? typeReplacementMap[serviceImplementation]
                                    : serviceImplementation;

                if (serviceImplementation.GetInterface(nameof(IKeyedDiiagramrService)) != null)
                {
                    var keyedService = (IKeyedDiiagramrService)Activator.CreateInstance(serviceImplementation);
                    builder.Bind(intreface).To(typeToBind).WithKey(keyedService.ServiceBindingKey);
                }
                else
                {
                    builder.Bind(intreface).To(typeToBind).InSingletonScope();
                }
            }
        }

        public static void BindServices(IStyletIoCBuilder builder)
        {
            var loadedTypes = AppDomain.CurrentDomain.GetAssemblies()
                            .Where(a => !a.GlobalAssemblyCache)
                            .SelectMany(x => x.GetExportedTypes())
                            .Where(t => t.GetInterface("ITestImplementationOf`1") == null);
            var loadedServiceInterfaces = loadedTypes.Where(t => t.IsInterface && t.GetInterface(nameof(IDiiagramrService)) != null);

            foreach (var loadedService in loadedServiceInterfaces)
            {
                BindEverythingThatImplementsTheInterface(loadedService, builder, loadedTypes, new Dictionary<Type, Type>());
            }
        }

        public static void BindTestServices(IStyletIoCBuilder builder)
        {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                            .Where(a => !a.GlobalAssemblyCache)
                            .SelectMany(x => x.GetTypes());
            var loadedTypes = allTypes.Where(t => t.GetInterface("ITestImplementationOf`1") == null);
            var loadedServiceInterfaces = loadedTypes.Where(t => t.IsInterface && t.GetInterface(nameof(IDiiagramrService)) != null);

            var fakeTypes = allTypes.Where(t => t.IsClass && !t.IsAbstract && t.GetInterface("ITestImplementationOf`1") != null);
            var realToFakeTypeDictionary = new Dictionary<Type, Type>();
            foreach (var fakeType in fakeTypes)
            {
                var realType = fakeType.GetInterface("ITestImplementationOf`1").GetGenericArguments()[0];
                try
                {
                    realToFakeTypeDictionary.Add(realType, fakeType);
                }
                catch (ArgumentException e)
                {
                    throw new ArgumentException($"There is already a test implementation of {realType.Name}, that has been provided by {realToFakeTypeDictionary[realType].Name}. {fakeType.Name} needs to be removed.", e);
                }
            }

            foreach (var loadedService in loadedServiceInterfaces)
            {
                BindEverythingThatImplementsTheInterface(loadedService, builder, loadedTypes, realToFakeTypeDictionary);
            }
        }
    }
}
