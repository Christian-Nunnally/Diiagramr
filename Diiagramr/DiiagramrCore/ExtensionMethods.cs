namespace DiiagramrCore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public static class ExtensionMethods
    {
        /// <summary>
        /// Iterates over a collection and applies a function to each element.
        /// </summary>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <param name="enumerable">The collection to iterate through.</param>
        /// <param name="action">The action to apply to each element in <paramref name="enumerable"/>.</param>
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

        /// <summary>
        /// Updates a property after ubsubscribing a property changed handler from its old value. Then resubscribes the handler to the new value.
        /// </summary>
        /// <param name="oldValue">The old property value.</param>
        /// <param name="newValue">The new property value.</param>
        /// <param name="setAction">The action to update the property itself.</param>
        /// <param name="handler">The handler to unsubscribe and then subscribe.</param>
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

        /// <summary>
        /// Gets an attribute from a type, if it exists.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Attribute"/> to get.</typeparam>
        /// <param name="memberInfo">The member to look for the attribute on.</param>
        /// <returns></returns>
        public static T GetAttribute<T>(this MemberInfo memberInfo)
        {
            return (T)memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
        }

        /// <summary>
        /// Executes the given action if <paramref name="value"/> is not null.
        /// </summary>
        /// <param name="value">The value to check for null.</param>
        /// <param name="action">The action to run.</param>
        public static void RunIfNotNull(this object value, Action action)
        {
            if (value != null)
            {
                action();
            }
        }
    }
}