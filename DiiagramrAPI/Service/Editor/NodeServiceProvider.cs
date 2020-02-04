using System;
using System.Collections.Generic;

namespace DiiagramrAPI.Service.Editor
{
    public class NodeServiceProvider
    {
        private readonly Dictionary<Type, object> _registeredServices = new Dictionary<Type, object>();

        public event Action ServiceRegistered;

        public void RegisterService<T>(T service)
        {
            if (_registeredServices.ContainsKey(typeof(T)))
            {
                return;
            }
            _registeredServices.Add(typeof(T), service);
            ServiceRegistered?.Invoke();
        }

        public T GetService<T>()
        {
            _registeredServices.TryGetValue(typeof(T), out object service);
            return (T)service;
        }
    }
}