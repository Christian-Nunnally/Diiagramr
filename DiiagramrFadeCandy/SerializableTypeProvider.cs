using DiiagramrAPI.Application;
using DiiagramrFadeCandy.GraphicsProcessing;
using System;
using System.Collections.Generic;

namespace DiiagramrFadeCandy
{
    public class SerializableTypeProvider : ISerializableTypeProvider
    {
        public IEnumerable<Type> SerializableTypes => new[]
        {
            typeof(Corner),
        };
    }
}