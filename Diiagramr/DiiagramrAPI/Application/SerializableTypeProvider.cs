using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;

namespace DiiagramrAPI.Application
{
    /// <summary>
    /// Provides the basic types to the serializization engine.
    /// </summary>
    public class SerializableTypeProvider : ISerializableTypeProvider
    {
        /// <inheritdoc/>
        public IEnumerable<Type> SerializableTypes => new[]
        {
            typeof(bool[]),
            typeof(byte[]),
            typeof(sbyte[]),
            typeof(char[]),
            typeof(decimal[]),
            typeof(double[]),
            typeof(float[]),
            typeof(int[]),
            typeof(uint[]),
            typeof(long[]),
            typeof(ulong[]),
            typeof(short[]),
            typeof(ushort[]),
            typeof(object[]),
            typeof(string[]),
            typeof(RawBox),
        };
    }
}