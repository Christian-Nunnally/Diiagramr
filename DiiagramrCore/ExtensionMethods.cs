namespace DiiagramrCore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

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
    }
}
