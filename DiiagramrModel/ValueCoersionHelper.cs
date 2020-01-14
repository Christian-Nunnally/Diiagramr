using System;
using System.Collections.Generic;

namespace DiiagramrModel2
{
    public static class ValueCoersionHelper
    {
        public static Dictionary<Type, Dictionary<Type, Func<object, object>>> CoersionFunctionDictionary = new Dictionary<Type, Dictionary<Type, Func<object, object>>>();

        public static bool CanCoerseValue(Type fromType, Type toType)
        {
            return CoersionFunctionDictionary.TryGetValue(fromType, out Dictionary<Type, Func<object, object>> innerDictionary)
                   && innerDictionary.ContainsKey(toType);
        }

        public static object CoerseValue(Type fromType, Type toType, object value)
        {
            if (CoersionFunctionDictionary.TryGetValue(fromType, out Dictionary<Type, Func<object, object>> innerDictionary)
                   && innerDictionary.TryGetValue(toType, out Func<object, object> coersionFunction))
            {
                return value == null ? value : coersionFunction(value);
            }
            throw new InvalidOperationException($"Can not coerse {fromType} to {toType}");
        }

        public static void InitializeDefaultCoersionFunctions()
        {
            AddCoersionFunction(typeof(byte[]), typeof(object[]), ByteArrayToObjectArrayCoersion);
            AddCoersionFunction(typeof(byte[]), typeof(int[]), ByteArrayToIntArrayCoersion);
            AddCoersionFunction(typeof(int[]), typeof(object[]), IntArrayToObjectArrayCoersion);
            AddCoersionFunction(typeof(int[]), typeof(byte[]), IntArrayToByteArrayCoersion);
            AddTwoWayCoersionFunction(typeof(int), typeof(float), ImplicitCoersion);
            AddTwoWayCoersionFunction(typeof(int), typeof(double), ImplicitCoersion);
            AddTwoWayCoersionFunction(typeof(int), typeof(short), ImplicitCoersion);
            AddTwoWayCoersionFunction(typeof(int), typeof(long), ImplicitCoersion);
            AddTwoWayCoersionFunction(typeof(int), typeof(byte), ImplicitCoersion);
        }

        public static void AddTwoWayCoersionFunction(Type type1, Type type2, Func<object, object> function)
        {
            AddCoersionFunction(type1, type2, function);
            AddCoersionFunction(type2, type1, function);
        }

        public static void AddCoersionFunction(Type fromType, Type toType, Func<object, object> function)
        {
            if (!CoersionFunctionDictionary.ContainsKey(fromType))
            {
                CoersionFunctionDictionary.Add(fromType, new Dictionary<Type, Func<object, object>>());
            }
            CoersionFunctionDictionary[fromType].Add(toType, function);
        }

        private static object ByteArrayToObjectArrayCoersion(object value) => Array.ConvertAll(value as byte[], obj => (object)obj);

        private static object ByteArrayToIntArrayCoersion(object value) => Array.ConvertAll(value as byte[], obj => (int)obj);

        private static object IntArrayToObjectArrayCoersion(object value) => Array.ConvertAll(value as int[], obj => (object)obj);

        private static object IntArrayToByteArrayCoersion(object value) => Array.ConvertAll(value as int[], obj => (byte)obj);

        private static object ImplicitCoersion(object value) => value;
    }
}