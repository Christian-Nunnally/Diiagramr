using System;
using System.Collections.Generic;

namespace DiiagramrAPI.Application
{
    public interface ISerializableTypeProvider
    {
        IEnumerable<Type> SerializableTypes { get; }
    }
}