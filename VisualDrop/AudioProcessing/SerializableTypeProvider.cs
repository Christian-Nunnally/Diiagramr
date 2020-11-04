using CSCore.DSP;
using DiiagramrAPI.Application;
using System;
using System.Collections.Generic;

namespace VisualDrop.AudioProcessing
{
    public class SerializableTypeProvider : ISerializableTypeProvider
    {
        public IEnumerable<Type> SerializableTypes => new List<Type> { typeof(FftSize), typeof(WindowFunctionType) };
    }
}