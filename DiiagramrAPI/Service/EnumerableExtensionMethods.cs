﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DiiagramrAPI.Service
{
    public static class EnumerableExtensionMethods
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null) return;
            var itemsArray = enumerable as T[] ?? enumerable.ToArray();
            for (var i = itemsArray.Length - 1; i >= 0; i--) action(itemsArray.ElementAt(i));
        }
    }
}