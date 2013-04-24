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
    ///<summary>
    /// Simple class providing Extension methods for the
    /// TypeWrapper. These methods allow the more 
    /// fluent programming syntax for programming Habanero.Smooth.
    ///</summary>
    public static class TypeWrapperExtensions
    {
        ///<summary>
        /// Simple Extension that allows the more smooth 
        /// wrapping of a Type with a Type Wrapper making the 
        /// use in linq etc more readable.
        ///</summary>
        ///<param name="type"></param>
        ///<returns></returns>
        public static TypeWrapper ToTypeWrapper(this Type type)
        {
            return new TypeWrapper(type);
        }
        /// <summary>
        /// Simple extension that allows the more fluent programming of 
        /// Habanero Smooth. Making the use in linq etc more readable.
        /// </summary>
        /// <param name="typeWrapper"></param>
        /// <returns></returns>
        public static bool IsNull(this TypeWrapper typeWrapper)
        {
            return ReferenceEquals(typeWrapper, null);
        }
    }
    /// <summary>
    /// This is a class that wraps a <see cref="Type"/> and 
    /// provides specific methods and properties that provide
    /// specific additional capabalities required for AutoMapping.
    /// </summary>
    public class TypeWrapper
    {
        private readonly Type _underlyingType;

        /// <summary>
        /// Construct the Type Wrapper for the specified Type.
        /// </summary>
        /// <param name="type"></param>
        public TypeWrapper(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            _underlyingType = type;
        }
        /// <summary>
        /// Returns true if the <see cref="UnderlyingType"/> is a real class 
        /// i.e. it is not an abstract type, an interface or a generic type.
        /// </summary>
        public bool IsRealClass
        {
            get
            {
                return !_underlyingType.IsAbstract
                       && !_underlyingType.IsGenericType
                       && !_underlyingType.IsInterface;
            }
        }

        /// <summary>
        /// Returns the Types Name
        /// </summary>
        public virtual string Name
        {
            get { return _underlyingType == null ? "" : _underlyingType.Name; }
        }
        /// <summary>
        /// Returns the Types Name
        /// </summary>
        public virtual string Namespace
        {
            get { return _underlyingType == null ? "" : _underlyingType.Namespace; }
        }

        /// <summary>
        /// Returns true if this type is a Generic Type
        /// </summary>
        public virtual bool IsGenericType
        {
            get { return _underlyingType != null && _underlyingType.IsGenericType; }
        }
        /// <summary>
        /// Returns the Base Type if this Type Inherits from 
        /// another type.
        /// </summary>
        public virtual TypeWrapper BaseType
        {
            get { return _underlyingType.BaseType == null ? null : _underlyingType.BaseType.ToTypeWrapper(); }
        }
        /// <summary>
        /// Returns the True if this Type Inherits from 
        /// another type.
        /// </summary>
        public virtual bool HasBaseType
        {
            get {  return _underlyingType == null ? false : _underlyingType.BaseType != null; }
        }
        /// <summary>
        /// Returns the true if this type inherits from BusinessObject (i.e. BusinessObject is the layer super type).
        /// </summary>
        public virtual bool IsBaseTypeBusinessObject
        {
            get { return _underlyingType == null ? false : (_underlyingType.BaseType == typeof(BusinessObject) || IsBaseTypeGenericBusinessObject()); }
        }
        /// <summary>
        /// Returns the true if this type inherits from BusinessObject{T} (i.e. Generic BusinessObject is the layer super type).
        /// </summary>
        private bool IsBaseTypeGenericBusinessObject()
        {
            return !this.BaseType.IsNull() && this.BaseType.IsGenericType && this.BaseType.UnderlyingType.GetGenericTypeDefinition().Equals(typeof(BusinessObject<>));
        }
        ///<summary>
        /// Returns the Assembly Name for the Underlying Type.
        ///</summary>
        public virtual string AssemblyName
        {
            get
            {
                string assemblyName;
                string classNameFull;
                TypeLoader.ClassTypeInfo(this._underlyingType, out assemblyName, out classNameFull);
                return assemblyName;
            }
        }
//
//        public virtual bool IsGenericTypeDefinition
//        {
//            get
//            {
//                if (_underlyingType == null)
//                    return false;
//
//                return _underlyingType.IsGenericTypeDefinition;
//            }
//        }
//
//        public virtual Type GetGenericTypeDefinition()
//        {
//            return _underlyingType == null ? null : _underlyingType.GetGenericTypeDefinition();
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

        ///<summary>
        /// If the Type is generic e.g. IBusinessObjectCollection{T} then 
        /// this will return an enumerable with a Type wrapper for actual type T.
        ///</summary>
        ///<returns></returns>
        public virtual IEnumerable<TypeWrapper> GetGenericArguments()
        {
            return _underlyingType == null
                       ? new TypeWrapper[0]
                       : _underlyingType.GetGenericArguments().Select(type => type.ToTypeWrapper());
        }
        /// <summary>
        /// Returns try if the UnderlyingType has a property identified by
        /// <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual bool HasProperty(string propertyName)
        {
            return this.GetProperty(propertyName) != null;
        }

        ///<summary>
        /// Returns a Property Wrapper for the <see cref="PropertyInfo"/>
        /// identified by <paramref name="propertyName"/>.
        ///</summary>
        ///<param name="propertyName"></param>
        ///<returns></returns>
        public virtual PropertyWrapper GetProperty(string propertyName)
        {
            return UnderlyingType.GetProperty(propertyName).ToPropertyWrapper();
        }

//
//        public virtual IEnumerable<Type> GenericArguments
//        {
//            get { return GetGenericArguments(); }
//        }
        ///<summary>
        ///</summary>
        ///<param name="type"></param>
        ///<returns></returns>
        public bool IsOfType(Type type)
        {
            return type.IsAssignableFrom(this.UnderlyingType);
        }

        ///<summary>
        /// Returns true if the Type <typeparamref name="T"/> is assignable from
        /// the Underlying Type. I.e. the underlying type is a sub type of or 
        /// implements the interface of type <typeparamref name="T"/>.
        ///</summary>
        ///<typeparam name="T"></typeparam>
        ///<returns></returns>
        public bool IsOfType<T>()
        {
            return IsOfType(typeof (T));
        }
        ///<summary>
        /// Returns true if the Underlying Type has one or more <see cref="AutoMapIgnoreAttribute"/>.
        ///</summary>
        public bool HasIgnoreAttribute
        {
            get { return GetIgnoreAttributes().Count() > 0; }
        }

        private IEnumerable<object> GetIgnoreAttributes()
        {
            return _underlyingType.GetCustomAttributes(typeof(AutoMapIgnoreAttribute), true);
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public virtual IEnumerable<PropertyWrapper> GetProperties()
        {
            if (_underlyingType == null) return new PropertyWrapper[0];

            PropertyInfo[] propertyInfos = _underlyingType.GetProperties();
            return propertyInfos.Select(propertyInfo => propertyInfo.ToPropertyWrapper());
        }

        ///<summary>
        /// Returns the UnderlyingTypes AssemblyQualifiedName.
        ///</summary>
        public virtual string AssemblyQualifiedName
        {
            get { return _underlyingType.AssemblyQualifiedName; }
        }
        /// <summary>
        /// returns the underlying types to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _underlyingType == null ? "" : _underlyingType.ToString();
        }


        ///<summary>
        /// Returns the Underlying type
        ///</summary>
        ///<value></value>
        public virtual Type UnderlyingType
        {
            get { return _underlyingType; }
        }

        ///<summary>
        /// Creates a Generic Business Object Collection for the Underlying type.
        /// 
        ///</summary>
        ///<returns></returns>
        public TypeWrapper MakeGenericBusinessObjectCollection()
        {
            return typeof (BusinessObjectCollection<>).MakeGenericType(this.UnderlyingType).ToTypeWrapper();
        }


        ///<summary>
        /// Returns the PKPropName that will be used for the underlying type.
        /// The PKPropName will either be determined by attributes or by 
        /// convention e.g. ClassNameID.
        ///</summary>
        ///<returns></returns>
        public virtual string GetPKPropName()
        {
            if(!this.IsBaseTypeBusinessObject && this.HasBaseType && !this.BaseType.IsGenericType)
            {
                return this.BaseType.GetPKPropName();
            }

            var propName = GetPKPropNameFromClassDef();
            if (!string.IsNullOrEmpty(propName)) return propName;
            propName = this.GetPKPropNameFromAttribute();
            return propName ?? PropNamingConvention.GetIDPropertyName(this);
        }

        private string GetPKPropNameFromClassDef()
        {
            var classDef = AllClassesAutoMapper.ClassDefCol.FindByClassName(this.Name);
            if (classDef != null && classDef.PrimaryKeyDef.Count > 0)
            {
                return classDef.PrimaryKeyDef[0].PropertyName;
            }
            return null;
        }

        ///<summary>
        /// Returnes the Property naming convention being used for this
        /// automapping
        ///</summary>
        public static INamingConventions PropNamingConvention
        {
            get
            {
                return ClassAutoMapper.PropNamingConvention;
            }
        }

        ///<summary>
        /// Returns true if the <see cref="Type"/> being wrapped by this TypeWrapper is 
        /// a business object regardless of where in the inheritance Heirachy the Base Type is.
        ///</summary>
        public bool IsBusinessObject { get { return this.IsOfType<IBusinessObject>(); } }

        private string GetPKPropNameFromAttribute()
        {
            var propertyInfos = this.GetProperties().ToList();
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
            return IsNullableType ? Nullable.GetUnderlyingType(_underlyingType) : _underlyingType;
        }
        /// <summary>
        /// Returns true if the <see cref="UnderlyingType"/> is Generic and is Nullable
        /// e.g. Guid? is actually Nullable{Guid}.
        /// </summary>
        public bool IsNullableType
        {
            get { return this.IsGenericType && _underlyingType.GetGenericTypeDefinition().Equals(typeof (Nullable<>)); }
        }
        /// <summary>
        /// Does this type inherit from a Generic BO e.g. Person : BusinessObject{Person}
        /// </summary>
        public bool IsGenericBusinessObject
        {
            get
            {
                if (IsBaseTypeGenericBusinessObject()) return true;
                return !this.BaseType.IsNull() &&  this.BaseType.IsGenericBusinessObject;
            }
        }


        #region Equality

// ReSharper disable MemberCanBePrivate.Global
        /// <summary>
        /// Determines whether the <see cref="UnderlyingType"/>
        /// for other is the same as the <see cref="UnderlyingType"/>
        /// for this wrapper.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(TypeWrapper other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(other, null)) return false;
            if (other._underlyingType == null && _underlyingType == null) return true;
            if (ReferenceEquals(other._underlyingType, null)) return false;
            return other._underlyingType.Equals(_underlyingType);
        }
        /// <summary>
        /// Determines whether the <see cref="UnderlyingType"/>
        /// for this wrapper is the same as other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Type other)
        {
            return !ReferenceEquals(other, null) && other.Equals(_underlyingType);
        }
        /// <summary>
        /// Determines whether the <see cref="Name"/> of the <see cref="UnderlyingType"/>
        /// is the same as other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(string other)
        {
            return !ReferenceEquals(other, null) && other.Equals(Name);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        ///                 </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.
        ///                 </exception><filterpriority>2</filterpriority>
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

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_underlyingType != null
                             ? _underlyingType.GetHashCode()
                             : 0)*397) ^ (Name != null
                                              ? Name.GetHashCode()
                                              : 0);
            }
        }

        // ReSharper disable RedundantCast
        /// <summary>
        /// Determines whether the <see cref="UnderlyingType"/>
        /// for other is the same as the <see cref="UnderlyingType"/>
        /// for this wrapper.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool operator ==(TypeWrapper original, Type type)
        {
            if (type == null && ReferenceEquals(original, null)) return true;
            if (type == null && original._underlyingType == null) return true;
            if (ReferenceEquals(original, null) || original._underlyingType == null) return false;

            return original._underlyingType == type;
        }

        // ReSharper restore RedundantCast
        /// <summary>
        /// Determines whether the <see cref="UnderlyingType"/>
        /// for other type is Not the same as the <see cref="UnderlyingType"/>
        /// for this wrapper (origional).
        /// </summary>
        /// <param name="original"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool operator !=(TypeWrapper original, Type type)
        {
            return !(original == type);
        }
        /// <summary>
        /// Determines whether the origional
        /// is the same as 
        /// the <see cref="UnderlyingType"/> of type.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool operator ==(Type original, TypeWrapper type)
        {
            return type == original;
        }
        /// <summary>
        /// Determines whether the <see cref="UnderlyingType"/>
        /// for other (type) is Not the same as the <see cref="UnderlyingType"/>
        /// for this type (origional).
        /// </summary>
        public static bool operator !=(Type original, TypeWrapper type)
        {
            return !(original == type);
        }
        /// <summary>
        /// Determines whether the <see cref="UnderlyingType"/> of origional
        /// is the same as 
        /// the <see cref="UnderlyingType"/> of other.
        /// </summary>
        public static bool operator ==(TypeWrapper original, TypeWrapper other)
        {
            return ReferenceEquals(original, other) || original.Equals(other);
        }
        /// <summary>
        /// Determines whether the <see cref="UnderlyingType"/> of origional
        /// is Not the same as 
        /// the <see cref="UnderlyingType"/> of other.
        /// </summary>
        public static bool operator !=(TypeWrapper original, TypeWrapper other)
        {
            return !(original == other);
        }

        // ReSharper restore MemberCanBePrivate.Global

        #endregion

        // ReSharper restore ClassWithVirtualMembersNeverInherited.Global
        // ReSharper restore VirtualMemberNeverOverriden.Global

        /// <summary>
        /// Returns the First Attribute on the wrapped <see cref="PropertyInfo"/> of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetAttribute<T>() where T : class
        {
            var attributes = this.GetAttributes<T>();
            return attributes == null ? null : attributes.FirstOrDefault();
        }
        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        /// <summary>
        /// Returns all Attributes of type <typeparamref name="T"/> on the wrapped <see cref="PropertyInfo"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAttributes<T>() where T : class
        {
            var attributes = this._underlyingType.GetCustomAttributes(typeof(T), true);

            return attributes == null ? null : attributes.Select(y => y as T);
        }
        // ReSharper restore ConditionIsAlwaysTrueOrFalse


        ///<summary>
        /// Returns the mapped TableName for the underlying type
        ///</summary>
        ///<returns>null if the </returns>
        public string GetTableName()
        {
            var attribute = GetAttribute<AutoMapTableNameAttribute>();
            return attribute == null ? null : attribute.TableName;
        }
        /// <summary>
        /// Is this type an Enum or a Nullable Enum Type. <see cref="Enum"/>
        /// </summary>
        /// <returns></returns>
        public bool IsEnumType()
        {
            return this.UnderlyingType.IsEnum || this.IsNullableEnum();
        }
        //This method is copied unchanged from SubSonic.
        //Thanks from the Habanero team 2 the Subsonic team for this.
        private bool IsNullableEnum()
        {
            var enumType = Nullable.GetUnderlyingType(this.UnderlyingType);
            return enumType != null && enumType.IsEnum;
        }
    }
}