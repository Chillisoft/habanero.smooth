// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2010 Chillisoft Solutions
//  
//  This file is part of the Habanero framework.
//  
//      Habanero is a free framework: you can redistribute it and/or modify
//      it under the terms of the GNU Lesser General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      The Habanero framework is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU Lesser General Public License for more details.
//  
//      You should have received a copy of the GNU Lesser General Public License
//      along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------
using System;
using System.Reflection;

namespace Habanero.Smooth.ReflectionWrappers
{



    /// <summary>
    /// 
    /// </summary>
    public abstract class MemberWrapper : IEquatable<MemberWrapper>
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// 
        /// </summary>
        public abstract TypeWrapper PropertyType { get; }
//        public abstract bool IsPublic { get; }
//        public abstract bool CanWrite { get; }
//        public abstract MemberInfo MemberInfo { get; }
        /// <summary>
        /// 
        /// </summary>
        public abstract TypeWrapper DeclaringType { get; }
        /// <summary>
        /// 
        /// </summary>
        public abstract TypeWrapper ReflectedType { get; }
//        public abstract bool HasIndexParameters { get; }
//        public abstract bool IsMethod { get; }
//        public abstract bool IsField { get; }
        /// <summary>
        /// 
        /// </summary>
        public abstract bool IsProperty { get; }
//        public abstract bool IsPrivate { get; }

        #region Equality
        /// <summary>
        /// 
        /// </summary>
        public bool Equals(MemberWrapper other)
        {
            return !ReferenceEquals(null, other);
        }
        /// <summary>
        /// 
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof(MemberWrapper) && Equals((MemberWrapper)obj);
        }
        /// <summary>
        /// 
        /// </summary>
        public override int GetHashCode()
        {
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(MemberWrapper left, MemberWrapper right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(MemberWrapper left, MemberWrapper right)
        {
            return !Equals(left, right);
        }

        #endregion

    }
}