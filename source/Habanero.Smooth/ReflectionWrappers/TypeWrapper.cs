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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.Util;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable VirtualMemberNeverOverriden.Global

namespace Habanero.Smooth.ReflectionWrappers
{
    public static class TypeWrapperExtensions
    {
        public static TypeWrapper ToTypeWrapper(this Type type)
        {
            return new TypeWrapper(type);
        }

        public static bool IsNull(this TypeWrapper typeWrapper)
        {
            return ReferenceEquals(typeWrapper, null);
        }
    }

    public class TypeWrapper
    {
        private readonly Type _innerType;

        public bool IsRealClass
        {
            get
            {
                return !_innerType.IsAbstract
                       && !_innerType.IsGenericType
                       && !_innerType.IsInterface;
            }
        }

        public TypeWrapper(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            _innerType = type;
        }
        /// <summary>
        /// Returns the Types Name
        /// </summary>
        public virtual string Name
        {
            get { return _innerType == null ? "" : _innerType.Name; }
        }
        /// <summary>
        /// Returns the Types Name
        /// </summary>
        public virtual string Namespace
        {
            get { return _innerType == null ? "" : _innerType.Namespace; }
        }

        /// <summary>
        /// Returns true if this type si a Generic Type
        /// </summary>
        public virtual bool IsGenericType
        {
            get { return _innerType != null && _innerType.IsGenericType; }
        }
        /// <summary>
        /// Returns the Base Type if this Type Inherits from 
        /// another type.
        /// </summary>
        public virtual TypeWrapper BaseType
        {
            get { return _innerType.BaseType == null ? null : _innerType.BaseType.ToTypeWrapper(); }
        }
        /// <summary>
        /// Returns the True if this Type Inherits from 
        /// another type.
        /// </summary>
        public virtual bool HasBaseType
        {
            get {  return _innerType == null ? false : _innerType.BaseType != null; }
        }
        /// <summary>
        /// Returns the Base Type if this Type Inherits from 
        /// another type.
        /// </summary>
        public virtual bool IsBaseTypeLayerSuperType
        {
            get {  return _innerType == null ? false : _innerType.BaseType == typeof(BusinessObject); }
        }

        public virtual string AssemblyName
        {
            get
            {
                string assemblyName;
                string classNameFull;
                TypeLoader.ClassTypeInfo(this._innerType, out assemblyName, out classNameFull);
                return assemblyName;
            }
        }
//
//        public virtual bool IsGenericTypeDefinition
//        {
//            get
//            {
//                if (_innerType == null)
//                    return false;
//
//                return _innerType.IsGenericTypeDefinition;
//            }
//        }
//
//        public virtual Type GetGenericTypeDefinition()
//        {
//            return _innerType == null ? null : _innerType.GetGenericTypeDefinition();
//        }

//        public virtual Type GenericTypeDefinition
//        {
//            get { return GetGenericTypeDefinition(); }
//        }
//
//        public virtual bool IsNullable
//        {
//            get { return GenericTypeDefinition == typeof(Nullable<>); }
//        }

        public virtual IEnumerable<TypeWrapper> GetGenericArguments()
        {
            return _innerType == null
                       ? new TypeWrapper[0]
                       : _innerType.GetGenericArguments().Select(type => type.ToTypeWrapper());
        }

        public virtual bool HasProperty(string propName)
        {
            return this.GetProperty(propName) != null;
        }

        public virtual PropertyWrapper GetProperty(string propertyName)
        {
            return GetUnderlyingType().GetProperty(propertyName).ToPropertyWrapper();
        }

//
//        public virtual IEnumerable<Type> GenericArguments
//        {
//            get { return GetGenericArguments(); }
//        }
        public bool IsOfType(Type type)
        {
            return type.IsAssignableFrom(this.GetUnderlyingType());
        }

        public bool IsOfType<T>()
        {
            return IsOfType(typeof (T));
        }
        public bool HasIgnoreAttribute
        {
            get { return GetCustomAttributes().Count() > 0; }
        }

        private IEnumerable<object> GetCustomAttributes()
        {
            return _innerType.GetCustomAttributes(typeof(AutoMapIgnoreAttribute), true);
        }

        public virtual IEnumerable<PropertyWrapper> GetProperties()
        {
            if (_innerType == null) return new PropertyWrapper[0];

            PropertyInfo[] propertyInfos = _innerType.GetProperties();
            return propertyInfos.Select(propertyInfo => propertyInfo.ToPropertyWrapper());
        }

        public virtual string AssemblyQualifiedName
        {
            get { return _innerType.AssemblyQualifiedName; }
        }

        public override string ToString()
        {
            return _innerType == null ? "" : _innerType.ToString();
        }


        public virtual Type GetUnderlyingType()
        {
            return _innerType;
        }

        public TypeWrapper MakeGenericBusinessObjectCollection()
        {
            return typeof (BusinessObjectCollection<>).MakeGenericType(this.GetUnderlyingType()).ToTypeWrapper();
        }


        public virtual string GetPKPropName()
        {
            if(!this.IsBaseTypeLayerSuperType && this.HasBaseType)
            {
                return this.BaseType.GetPKPropName();
            }
            var propName = this.GetPKPropNameFromAttribute();
            return propName ?? PropNamingConvention.GetIDPropertyName(this);
        }

        public static INamingConventions PropNamingConvention
        {
            get
            {
                return ClassAutoMapper.PropNamingConvention;
            }
        }

        public bool IsBusinessObject { get { return this.IsOfType<IBusinessObject>(); } }

        private string GetPKPropNameFromAttribute()
        {
            var propertyInfos = this.GetProperties();
            var noPropsWithPKAttribute =
                propertyInfos.Count(propInfo => propInfo.HasAttribute<AutoMapPrimaryKeyAttribute>());
            if (noPropsWithPKAttribute > 1)
            {
                throw new HabaneroApplicationException(
                    string.Format("You cannot auto map Business Objects '{0}' with Composite Primary Keys. Please map using ClassDefs", this.Name));
            }
            if (noPropsWithPKAttribute == 1)
            {
                var propertyInfo = propertyInfos.First(propInfo => propInfo.HasAttribute<AutoMapPrimaryKeyAttribute>());
                return propertyInfo.Name;
            }

            return null;
        }

        /// <summary>
        /// Returnes the Prop Type for the PropertyInfo.
        /// If the PropertyInfo is Nullable then it returns the underlying type. 
        /// (i.e bool? will return bool)
        /// </summary>
        /// <returns></returns>
        public Type GetNullableUndelyingType()
        {
            return IsNullableType ? Nullable.GetUnderlyingType(_innerType) : _innerType;
        }

        public bool IsNullableType
        {
            get { return this.IsGenericType && _innerType.GetGenericTypeDefinition().Equals(typeof (Nullable<>)); }
        }

        #region Equality

// ReSharper disable MemberCanBePrivate.Global
        public bool Equals(TypeWrapper other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(other, null)) return false;
            if (other._innerType == null && _innerType == null) return true;
            if (ReferenceEquals(other._innerType, null)) return false;
            return other._innerType.Equals(_innerType);
        }

        public bool Equals(Type other)
        {
            return !ReferenceEquals(other, null) && other.Equals(_innerType);
        }

        public bool Equals(string other)
        {
            return !ReferenceEquals(other, null) && other.Equals(Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(obj, null)) return false;
            if (obj.GetType() == typeof (TypeWrapper))
                return Equals((TypeWrapper) obj);
            if (obj is Type)
                return Equals((Type) obj);
            return obj.GetType() == typeof (string) && Equals((string) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_innerType != null
                             ? _innerType.GetHashCode()
                             : 0)*397) ^ (Name != null
                                              ? Name.GetHashCode()
                                              : 0);
            }
        }

        // ReSharper disable RedundantCast
        public static bool operator ==(TypeWrapper original, Type type)
        {
            if (type == null && ReferenceEquals(original, null)) return true;
            if (type == null && original._innerType == null) return true;
            if (ReferenceEquals(original, null) || original._innerType == null) return false;

            return original._innerType == type;
        }

        // ReSharper restore RedundantCast

        public static bool operator !=(TypeWrapper original, Type type)
        {
            return !(original == type);
        }

        public static bool operator ==(Type original, TypeWrapper type)
        {
            return type == original;
        }

        public static bool operator !=(Type original, TypeWrapper type)
        {
            return !(original == type);
        }

        public static bool operator ==(TypeWrapper original, TypeWrapper other)
        {
            return ReferenceEquals(original, other) || original.Equals(other);
        }

        public static bool operator !=(TypeWrapper original, TypeWrapper other)
        {
            return !(original == other);
        }

        // ReSharper restore MemberCanBePrivate.Global

        #endregion

        // ReSharper restore ClassWithVirtualMembersNeverInherited.Global
        // ReSharper restore VirtualMemberNeverOverriden.Global

    }
}