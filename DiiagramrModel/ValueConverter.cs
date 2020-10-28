using System;
using System.Collections.Generic;

namespace DiiagramrModel
{
    public static class ValueConverter
    {
        public static Dictionary<Type, HashSet<Type>> _knownFailedTypeConversions = new Dictionary<Type, HashSet<Type>>();

        /// <summary>
        /// Calling <see cref="TryCoerseValue(object, Type, out object)"/> is slow if the conversion fails,
        /// this will try to predict whether conversion will succeed or not.
        /// </summary>
        /// <param name="fromType">The type to convert from.</param>
        /// <param name="toType">The type to convert to.</param>
        /// <returns>Whether we think <see cref="TryCoerseValue"/> will return true.</returns>
        public static bool NonExaustiveCanConvertToType(Type fromType, Type toType)
        {
            return fromType == toType
                || toType.IsAssignableFrom(fromType)
                || !IsKnownFailedConversion(fromType, toType);
        }

        public static bool TryCoerseValue(object value, Type toType, out object coersedValue)
        {
            var fromType = value?.GetType();
            if (toType == null)
            {
                coersedValue = null;
                return false;
            }
            if (value == null || toType.IsAssignableFrom(fromType))
            {
                coersedValue = value;
                return true;
            }

            if (!IsKnownFailedConversion(fromType, toType))
            {
                try
                {
                    if (value is Array array && toType.IsArray)
                    {
                        coersedValue = Array.CreateInstance(toType.GetElementType(), array.Length);
                        Array.Copy(array, (Array)coersedValue, array.Length);
                    }
                    else
                    {
                        coersedValue = Convert.ChangeType(value, toType);
                    }

                    return true;
                }
                catch (OverflowException)
                {
                }
                catch (FormatException)
                {
                }
                catch (InvalidCastException)
                {
                    if (value != null)
                    {
                        if (fromType != typeof(object) && !(fromType.IsArray && fromType.GetElementType() == typeof(object) && toType.IsArray))
                        {
                            AddToKnownFailedConversionsMap(toType, fromType);
                        }
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    // Todo: handle this by adding it to know failed conversions maps.
                }
            }

            coersedValue = null;
            return false;
        }

        private static bool IsKnownFailedConversion(Type fromType, Type toType)
        {
            return _knownFailedTypeConversions.TryGetValue(fromType, out var knownConversions)
                    && knownConversions.Contains(toType);
        }

        private static void AddToKnownFailedConversionsMap(Type toType, Type fromType)
        {
            if (fromType == null)
            {
                return;
            }
            if (!_knownFailedTypeConversions.TryGetValue(fromType, out var knownFailedConversions))
            {
                _knownFailedTypeConversions.Add(fromType, new HashSet<Type> { toType });
            }
            else if (!knownFailedConversions.Contains(toType))
            {
                knownFailedConversions.Add(toType);
            }
        }
    }
}