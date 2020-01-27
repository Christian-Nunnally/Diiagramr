﻿using DiiagramrAPI.Editor;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Diiagramr.Application
{
    public static class BootstrapperUtilities
    {
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

        public static void BindServices(IStyletIoCBuilder builder)
        {
            IEnumerable<Type> loadedTypes = GetAllLoadedNonTestTypes();
            var loadedServiceInterfaces = loadedTypes.Where(t => t.IsInterface && t.GetInterface(nameof(ISingletonService)) != null);

            foreach (var loadedService in loadedServiceInterfaces)
            {
                BindEverythingThatImplementsInterfaceAsASingleton(loadedService, builder, loadedTypes, new Dictionary<Type, Type>());
            }
        }

        public static void BindTestServices(IStyletIoCBuilder builder)
        {
            var allLoadedTypes = GetAllLoadedTypesNotInTheGlobalAssemblyCache();
            var testImplementationTypes = GetAllLoadedNonTestTypes();
            var loadedServiceInterfaces = testImplementationTypes.Where(t => t.IsInterface && t.GetInterface(nameof(ISingletonService)) != null);

            var fakeTypes = allLoadedTypes.Where(t => t.IsClass && !t.IsAbstract && t.GetInterface("ITestImplementationOf`1") != null);
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
                BindEverythingThatImplementsInterfaceAsASingleton(loadedService, builder, testImplementationTypes, realToFakeTypeDictionary);
            }
        }

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