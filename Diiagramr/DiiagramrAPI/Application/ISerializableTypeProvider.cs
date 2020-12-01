using System;
using System.Collections.Generic;

namespace DiiagramrAPI.Application
{
    /// <summary>
    /// Provides additional serializeable types to the core application so that it knows that it can serialize them.
    /// </summary>
    public interface ISerializableTypeProvider
    {
        /// <summary>
        /// A collection of custom types that can be serialized.
        /// </summary>
        IEnumerable<Type> SerializableTypes { get; }
    }
}