using DiiagramrAPI.Application;
using Xunit;

namespace DiiagramrAPIUnitTests
{
    public class SerializableTypeProviderUnitTests : UnitTest
    {
        [Fact]
        public void SerializableTypeProvider_PrimitiveArraysProvided()
        {
            var serializableTypeProvider = new SerializableTypeProvider();

            Assert.Contains(typeof(bool[]), serializableTypeProvider.SerializableTypes);
            Assert.Contains(typeof(byte[]), serializableTypeProvider.SerializableTypes);
            Assert.Contains(typeof(sbyte[]), serializableTypeProvider.SerializableTypes);
            Assert.Contains(typeof(char[]), serializableTypeProvider.SerializableTypes);
            Assert.Contains(typeof(decimal[]), serializableTypeProvider.SerializableTypes);
            Assert.Contains(typeof(double[]), serializableTypeProvider.SerializableTypes);
            Assert.Contains(typeof(float[]), serializableTypeProvider.SerializableTypes);
            Assert.Contains(typeof(int[]), serializableTypeProvider.SerializableTypes);
            Assert.Contains(typeof(uint[]), serializableTypeProvider.SerializableTypes);
            Assert.Contains(typeof(long[]), serializableTypeProvider.SerializableTypes);
            Assert.Contains(typeof(ulong[]), serializableTypeProvider.SerializableTypes);
            Assert.Contains(typeof(short[]), serializableTypeProvider.SerializableTypes);
            Assert.Contains(typeof(ushort[]), serializableTypeProvider.SerializableTypes);
            Assert.Contains(typeof(object[]), serializableTypeProvider.SerializableTypes);
            Assert.Contains(typeof(string[]), serializableTypeProvider.SerializableTypes);
        }
    }
}