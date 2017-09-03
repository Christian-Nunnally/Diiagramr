using System;
using System.Runtime.Serialization;

namespace Diiagramr.Service
{
    // a version of System.Type that can be serialized
    [DataContract]
    [Serializable]
    public class SerializableType
    {
        public Type type;

        // constructors
        public SerializableType()
        {
            type = null;
        }

        public SerializableType(Type t)
        {
            type = t;
        }

        // when serializing, store as a string
        [DataMember]
        private string TypeString
        {
            get
            {
                return type == null ? null : type.FullName;
            }
            set
            {
                type = value == null ? null : Type.GetType(value);
            }
        }

        // allow SerializableType to implicitly be converted to and from System.Type
        public static implicit operator Type(SerializableType stype)
        {
            return stype.type;
        }

        public static implicit operator SerializableType(Type t)
        {
            return new SerializableType(t);
        }

        // overload the == and != operators
        public static bool operator ==(SerializableType a, SerializableType b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
                return true;

            // If one is null, but not both, return false.
            if (((object) a == null) || ((object) b == null))
                return false;

            // Return true if the fields match:
            return a.type == b.type;
        }

        public static bool operator !=(SerializableType a, SerializableType b)
        {
            return !(a == b);
        }

        // we don't need to overload operators between SerializableType and System.Type because we already enabled them to implicitly convert

        public override int GetHashCode()
        {
            return type.GetHashCode();
        }

        // overload the .Equals method
        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
                return false;

            // If parameter cannot be cast to SerializableType return false.
            var p = obj as SerializableType;
            if (p == null)
                return false;

            // Return true if the fields match:
            return type == p.type;
        }

        public bool Equals(SerializableType p)
        {
            // If parameter is null return false:
            if (p == null)
                return false;

            // Return true if the fields match:
            return type == p.type;
        }
    }
}