namespace DiiagramrCore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public static class ExtensionMethods
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null)
            {
                return;
            }

            var itemsArray = enumerable as T[] ?? enumerable.ToArray();
            for (var i = itemsArray.Length - 1; i >= 0; i--)
            {
                action(itemsArray.ElementAt(i));
            }
        }

        public static void UpdateListeningProperty(this INotifyPropertyChanged oldValue, INotifyPropertyChanged newValue, Action setAction, PropertyChangedEventHandler handler)
        {
            if (oldValue != null)
            {
                oldValue.PropertyChanged -= handler;
            }

            setAction();
            if (newValue != null)
            {
                newValue.PropertyChanged += handler;
            }
        }

        public static T GetAttribute<T>(this MemberInfo memberInfo)
        {
            return (T)memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
        }

        public static Action<object> CreateMethodInvoker(this MethodInfo method, object target) => data =>
        {
            method.Invoke(target, new[] { data });
        };

        public static void SetIfNotNull(this object value, Action setter)
        {
            if (value != null)
            {
                setter();
            }
        }
    }
}