using System;
using System.Collections.Generic;

namespace DiiagramrAPI.Service.Editor
{
    /// <summary>
    /// Provides services to implementations of <see cref="Node"/> that need to have shared services.
    /// </summary>
    public class NodeServiceProvider
    {
        private readonly Dictionary<Type, object> _registeredServices = new Dictionary<Type, object>();

        /// <summary>
        /// Triggered when a new service is registered to this provider.
        /// </summary>
        public event Action ServiceRegistered;

        /// <summary>
        /// Register a service to this provider.
        /// </summary>
        /// <typeparam name="T">The type of the service.</typeparam>
        /// <param name="service">The service instance.</param>
        public void RegisterService<T>(T service)
        {
            if (_registeredServices.ContainsKey(typeof(T)))
            {
                return;
            }
            _registeredServices.Add(typeof(T), service);
            ServiceRegistered?.Invoke();
        }

        /// <summary>
        /// Gets a registered service of a particular type.
        /// </summary>
        /// <typeparam name="T">The type of service to get.</typeparam>
        /// <returns>The service, or null if no service is registered to that type.</returns>
        public T GetService<T>()
        {
            _registeredServices.TryGetValue(typeof(T), out object service);
            return (T)service;
        }
    }
}