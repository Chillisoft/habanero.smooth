using System;
using System.Reflection;

namespace AutoMappingHabanero.ReflectionWrappers
{



    public abstract class MemberWrapper : IEquatable<MemberWrapper>
    {
        public abstract string Name { get; }
        public abstract TypeWrapper PropertyType { get; }
//        public abstract bool IsPublic { get; }
//        public abstract bool CanWrite { get; }
//        public abstract MemberInfo MemberInfo { get; }
        public abstract TypeWrapper DeclaringType { get; }
        public abstract TypeWrapper ReflectedType { get; }
//        public abstract bool HasIndexParameters { get; }
//        public abstract bool IsMethod { get; }
//        public abstract bool IsField { get; }
        public abstract bool IsProperty { get; }
//        public abstract bool IsPrivate { get; }

        #region Equality

        public bool Equals(MemberWrapper other)
        {
            return !ReferenceEquals(null, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof(MemberWrapper) && Equals((MemberWrapper)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public static bool operator ==(MemberWrapper left, MemberWrapper right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MemberWrapper left, MemberWrapper right)
        {
            return !Equals(left, right);
        }

        #endregion

    }
}