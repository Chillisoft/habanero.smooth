#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
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
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.Util;

// ReSharper disable VirtualMemberNeverOverriden.Global
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace Habanero.Smooth.ReflectionWrappers
{
    /// <summary>
    /// Extension methods to make the programming of PropertyWrapper more fluent.
    /// </summary>
    public static class PropertyExtensions
    {
        ///<summary>
        /// Creates a PropertyWrapper that wraps <paramref name="propertyInfo"/>.<br/>
        /// If <paramref name="propertyInfo"/> is null returns null.
        ///</summary>
        ///<param name="propertyInfo">the property info being wrapped</param>
        ///<returns></returns>
        public static PropertyWrapper ToPropertyWrapper(this PropertyInfo propertyInfo)
        {
            return propertyInfo == null ? null : new PropertyWrapper(propertyInfo);
        }

        ///<summary>
        /// Creates a PropertyWrapper that wraps a <see cref="PropertyInfo"/>.
        /// The property Info is for type <paramref name="t"/> and is identified 
        /// by <paramref name="propertyName"/>
        ///</summary>
        ///<param name="t"></param>
        ///<param name="propertyName"></param>
        ///<returns></returns>
        public static PropertyWrapper GetPropertyWrapper(this Type t, string propertyName)
        {
            return t.GetProperty(propertyName).ToPropertyWrapper();
        }
    }
    /// <summary>
    /// This property wrapper wraps a PropertyInfo and provides 
    /// more easily usable methods for Programmming the Automapping functionality.
    /// Also makes for easier testing.
    /// </summary>
    public class PropertyWrapper : MemberWrapper
    {
        #region ImplementationOfMember  

        private readonly PropertyInfo _propertyInfo;
        private TypeWrapper _declaringTypeWrapper;
        private TypeWrapper _propertyTypeWrapper;
        private TypeWrapper _reflectedTypeWrapper;
        /// <summary>
        /// Constructs a PropertyWrapper to wrap the <paramref name="propertyInfo"/>
        /// </summary>
        /// <param name="propertyInfo"></param>
        public PropertyWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");
            _propertyInfo = propertyInfo;
        }
        /// <summary>
        /// The Name of the the wrapped <see cref="PropertyInfo"/>.
        /// </summary>
        public override string Name
        {
            get { return _propertyInfo.Name; }
        }
        /// <summary>
        /// A <see cref="TypeWrapper"/> for PropertyType of the wrapped <see cref="PropertyInfo"/>.
        /// </summary>
        public override TypeWrapper PropertyType
        {
            get
            {
                if (_propertyTypeWrapper.IsNull())
                {
                    _propertyTypeWrapper = _propertyInfo.PropertyType.ToTypeWrapper();
                }
                return _propertyTypeWrapper;
            }
        }
        /// <summary>
        /// The AssemblyQualifiedName for the declaring type of the wrapped <see cref="PropertyInfo"/>.
        /// </summary>
        public virtual string AssemblyQualifiedName
        {
            get { return DeclaringType.AssemblyQualifiedName; }
        }

//        public override bool IsPublic
//        {
//            get {  throw new NotImplementedException(); }
//        }
//        public override bool IsPrivate
//                                                                
//            get { throw new NotImplementedException(); }
//        }
//
//        public override bool CanWrite
//        {
//            get { return _propertyInfo.CanWrite; }
//        }
//        public override MemberInfo MemberInfo
//        {
//            get { return _propertyInfo; }
//        }
        /// <summary>
        /// Returns the wrapped <see cref="PropertyInfo"/> 
        /// </summary>
        public virtual PropertyInfo PropertyInfo
        {
            get { return _propertyInfo; }
        }
        /// <summary>
        /// Returns a type wrapper for the DeclaringType for the wrapped <see cref="PropertyInfo"/> 
        /// </summary>
        public override TypeWrapper DeclaringType
        {
            get
            {
                if (_declaringTypeWrapper.IsNull()) _declaringTypeWrapper = new TypeWrapper(_propertyInfo.DeclaringType);
                return _declaringTypeWrapper;
            }
        }
        /// <summary>
        /// Returns a type wrapper for the ReflectedType for the wrapped <see cref="PropertyInfo"/> 
        /// </summary>
        public override TypeWrapper ReflectedType
        {
            get
            {
                if (_reflectedTypeWrapper.IsNull()) _reflectedTypeWrapper = new TypeWrapper(_propertyInfo.ReflectedType);
                return _reflectedTypeWrapper;
            }
        }
        /// <summary>
        /// Returns the className of the declaring type.
        /// </summary>
        public virtual string DeclaringClassName
        {
            get { return this.DeclaringType.Name; }
        }
        /// <summary>
        /// Returns true.
        /// </summary>
        public override bool IsProperty
        {
            get { return true; }
        }

        #endregion

        /// <summary>
        /// Returns true if the <see cref="PropertyInfo"/> has an <see cref="AutoMapIgnoreAttribute"/>
        /// false otherwise. Returns false if propInfo is null
        /// </summary>
        /// <value></value>
        public virtual bool HasIgnoreAttribute
        {
            get { return this.HasAttribute<AutoMapIgnoreAttribute>(); }
        }

        public virtual bool HasDisplayNameAttribute
        {
            get { return this.HasAttribute<AutoMapDisplayNameAttribute>(); }
        }

        public virtual bool HasDescriptionAttribute
        {
            get { return this.HasAttribute<AutoMapDescriptionAttribute>(); }
        }

        /// <summary>
        /// Returns true if the Property has a <see cref="AutoMapDefaultAttribute"/> set on it.
        /// </summary>
        public bool HasDefaultAttribute
        {
            get { return this.HasAttribute<AutoMapDefaultAttribute>(); }
        }
        /// <summary>
        /// Returns true if this property has the <see cref="AutoMapCompulsoryAttribute"/>
        /// </summary>
        public bool HasCompulsoryAttribute
        {
            get { return this.HasAttribute<AutoMapCompulsoryAttribute>(); }
        }       
        /// <summary>
        /// Returns true if this property has the <see cref="AutoMapKeepValuePrivate"/>
        /// </summary>
        public bool HasKeepValuePrivateAttribute
        {
            get { return this.HasAttribute<AutoMapKeepValuePrivate>(); }
        }
        /// <summary>
        /// has an ManyToOne Attribute on the Property.
        /// </summary>
        public virtual bool HasManyToOneAttribute
        {
            get { return this.HasAttribute<AutoMapManyToOneAttribute>(); }
        }
        /// <summary>
        /// has an OneToOne Attribute on the Property.
        /// </summary>
        public virtual bool HasOneToOneAttribute
        {
            get { return this.HasAttribute<AutoMapOneToOneAttribute>(); }
        }        
        /// <summary>
        /// has an AutoNumber Attribute on the Property.
        /// </summary>
        public virtual bool HasAutoIncrementingAttribute
        {
            get { return this.HasAttribute<AutoMapAutoIncrementingAttribute>(); }
        }       
        /// <summary>
        /// has a ReadWriteRule Attribute on the Property.
        /// </summary>
        public virtual bool HasReadWriteRuleAttribute
        {
            get { return this.HasAttribute<AutoMapReadWriteRuleAttribute>(); }
        }
        /// <summary>
        /// has a IntPropRule Attribute on the Property.
        /// </summary>
        public virtual bool HasIntPropRuleAttribute
        {
            get { return this.HasAttribute<AutoMapIntPropRuleAttribute>(); }
        }   
        /// <summary>
        /// has a UniqueConstraint Attribute on the Property.
        /// </summary>
        public virtual bool HasUniqueConstraintAttribute
        {
            get { return this.HasAttribute<AutoMapUniqueConstraintAttribute>(); }
        }
        /// <summary>
        /// Does the relationship prop identified by <param name="relationshipName"/> have an OneToOne Attribute <see cref="AutoMapOneToOneAttribute"/> on the Property.
        /// </summary>
        public virtual bool HasAutoMapOneToOneAttribute(string relationshipName)
        {
            return this.HasAutoMapAttribute<AutoMapOneToOneAttribute>(relationshipName);
        }
        /// <summary>
        /// Does the relationship Prop identified by <param name="relationshipName"/> have an Auto mapping Attribute on the Property.
        /// </summary>
        private bool HasAutoMapAttribute<T>(string relationshipName) where T : AutoMapRelationshipAttribute
        {
            var attributes = this.GetAttributes<T>();
            return attributes.Any(a => a.ReverseRelationshipName == relationshipName);
        }
        /// <summary>
        /// Does the relationship prop identified by <param name="relationshipName"/> have an ManyToOne Attribute <see cref="AutoMapManyToOneAttribute"/> on the Property.
        /// </summary>
        public virtual bool HasAutoMapManyToOneAttribute(string relationshipName)
        {
            return this.HasAutoMapAttribute<AutoMapManyToOneAttribute>(relationshipName);
        }
        /// <summary>
        /// Is the return type of the wrapped <see cref="PropertyInfo"/>  of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual bool IsOfType<T>()
        {
            return this.PropertyType.IsOfType<T>();
        }
        /// <summary>
        /// Does the wrapped <see cref="PropertyInfo"/> have an attribute of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual bool HasAttribute<T>() where T : class
        {
            T attribute = this.GetAttribute<T>();
            return attribute != null;
        }
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
        /// <summary>
        /// Returns all Attributes of type <typeparamref name="T"/> on the wrapped <see cref="PropertyInfo"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAttributes<T>() where T : class
        {
            var attributes = this._propertyInfo.GetCustomAttributes(typeof (T), true);
            return attributes == null ? null : attributes.Select(y => y as T);
        }
        /// <summary>
        /// Get the PropertyWrappers for Properties on Types Related to the wrapped <see cref="PropertyInfo"/> where these could represent a OneToOneRelationship.
        /// This will return a list only where the wrapped <see cref="PropertyInfo"/> represents a relationship.
        /// </summary>
        /// <returns></returns>
        public virtual IList<PropertyWrapper> GetOneToOneReverseRelationshipInfos()
        {
            if (this.IsMultipleRelationship || this.HasIgnoreAttribute || this.HasAttribute<AutoMapManyToOneAttribute>())
                return new List<PropertyWrapper>();

            return this.GetSingleReverseRelPropInfos<AutoMapManyToOneAttribute, AutoMapOneToOneAttribute, AutoMapOneToOneAttribute>();
        }
        /// <summary>
        /// Get the PropertyWrappers for Properties on Types Related to the wrapped <see cref="PropertyInfo"/> where these could represent a ManyToOneRelationship.
        /// This will return a list only where the wrapped <see cref="PropertyInfo"/> represents a relationship.
        /// </summary>
        /// <returns></returns>
        public virtual IList<PropertyWrapper> GetManyToOneReverseRelationshipInfos()
        {
            if (this.IsSingleRelationhip || this.HasIgnoreAttribute) return new List<PropertyWrapper>();

            return this.GetSingleReverseRelPropInfos<AutoMapOneToOneAttribute, AutoMapManyToOneAttribute, AutoMapOneToManyAttribute>();
        }

        private IList<PropertyWrapper> GetSingleReverseRelPropInfos
            <TIignoreAttribute, TReverseAutoMapAttribute, TAutoMapAttribute>()
            where TIignoreAttribute : AutoMapRelationshipAttribute
            where TAutoMapAttribute : AutoMapRelationshipAttribute
            where TReverseAutoMapAttribute : AutoMapRelationshipAttribute
        {
            var ownerClassType = this.DeclaringType;
            var relatedClassType = this.RelatedClassType;
            var properties = relatedClassType.GetProperties();
            var propertyInfos = properties.Select(info1 => info1)
                .Where(info => info.PropertyType == ownerClassType
                               && !info.HasIgnoreAttribute
                               && !info.HasAttribute<TIignoreAttribute>());

            string mappedReverseRelationshipName = this.GetMappedReverseRelationshipName<TAutoMapAttribute>();
            if (!string.IsNullOrEmpty(mappedReverseRelationshipName))
            {
                var declaredRevRelProp = propertyInfos.FirstOrDefault
                    (propertyInfo => propertyInfo.Name == mappedReverseRelationshipName);
                return declaredRevRelProp == null
                           ? new List<PropertyWrapper>()
                           : new List<PropertyWrapper> {declaredRevRelProp};
            }
            var declaredRelName = propertyInfos.FirstOrDefault
                (propertyInfo => propertyInfo.HasAutoMapAttribute<TReverseAutoMapAttribute>(this.Name));
            return declaredRelName != null ? new List<PropertyWrapper> {declaredRelName} : propertyInfos.ToList();
        }
        /// <summary>
        /// Returns the reverse relationship name in cases where the reverse relationship
        /// is declared using <see cref="AutoMapRelationshipAttribute"/>
        /// </summary>
        /// <typeparam name="TAutoMapAttribute"></typeparam>
        /// <returns></returns>
        public virtual string GetMappedReverseRelationshipName<TAutoMapAttribute>()
            where TAutoMapAttribute : AutoMapRelationshipAttribute
        {
            if (!this.HasAttribute<TAutoMapAttribute>()) return null;
            TAutoMapAttribute autoMapAttribute = this.GetAttribute<TAutoMapAttribute>();
            return autoMapAttribute.ReverseRelationshipName;
        }
        /// <summary>
        /// Returns the Related Type in the cases where this property
        /// is the declaration of either a single or a multiple relationship.
        /// </summary>
        public virtual TypeWrapper RelatedClassType
        {
            get
            {
                var propertyType = this.PropertyType;
                return propertyType.IsGenericType ? propertyType.GetGenericArguments().First() : propertyType;
            }
        }
        /// <summary>
        /// Returns the class name of the Related Type in the cases where this property
        /// is the declaration of either a single or a multiple relationship.
        /// </summary>
        public virtual string RelatedClassName
        {
            get
            {
                return this.RelatedClassType.Name;
            }
        }
        /// <summary>
        /// Is the wrapped <see cref="PropertyInfo"/> a declaration of a relationship that has a 
        /// Single Reverse Relationship.
        /// </summary>
        public virtual bool HasSingleReverseRelationship
        {
            get { return this.GetSingleReverseRelPropInfos() != null && this.GetSingleReverseRelPropInfos().Count > 0; }
        }

        /// <summary>
        /// Gets all single ReverseRelationship 
        /// </summary>
        /// <returns></returns>
        public virtual IList<PropertyWrapper> GetSingleReverseRelPropInfos()
        {
            if (!this.IsRelationship) return null;

            return this.IsSingleRelationhip
                       ? GetOneToOneReverseRelationshipInfos()
                       : GetManyToOneReverseRelationshipInfos();
        }
        /// <summary>
        /// Is the wrapped <see cref="PropertyInfo"/> inherited i.e. it is declared on a base type.
        /// and may be overriden on this type.
        /// </summary>
        public virtual bool IsInherited
        {
            get { return ReflectedType != DeclaringType || this.IsOverridden; }
        }
        /// <summary>
        /// Is the wrapped <see cref="PropertyInfo"/> a declaration for a single relationship.
        /// I.e. Returns a BusinessObject
        /// </summary>
        public virtual bool IsSingleRelationhip
        {
            get { return this.PropertyType.IsOfType<BusinessObject>(); }
        }
        /// <summary>
        /// Is the wrapped <see cref="PropertyInfo"/> a declaration for a Multiple Relationship.
        /// i.e. returns a BusinessObjectCollection.
        /// </summary>
        public virtual bool IsMultipleRelationship
        {
            get { return this.PropertyType.IsOfType<IBusinessObjectCollection>(); }
        }
        /// <summary>
        /// Is the wrapped <see cref="PropertyInfo"/> a declaration for a relationship.
        /// I.e. it returns a BusinessObject or a BusinessObjectCollection.
        /// </summary>
        public virtual bool IsRelationship
        {
            get { return this.IsSingleRelationhip || this.IsMultipleRelationship; }
        }

        /// <summary>
        /// If no single rev rel and Attribute with no RevRelName then RevRelName = ClassName
        /// If no single rev rel and Att with RevRelName then RevRelName = DeclaredRevRelName
        /// If has single rev rel then RevRelName = foundRevRelationshipName 
        /// </summary>
        /// <typeparam name="TAttributeType"></typeparam>
        /// <returns></returns>
        public virtual string GetSingleReverseRelationshipName<TAttributeType>()
            where TAttributeType : AutoMapRelationshipAttribute
        {
            string mappedReverseRelationshipName = this.GetMappedReverseRelationshipName<TAttributeType>();
            if (!string.IsNullOrEmpty(mappedReverseRelationshipName))
            {
                return mappedReverseRelationshipName;
            }
            PropertyWrapper singleReverseRelProp = this.SingleReverseRelProp;
            return singleReverseRelProp == null ? this.DeclaringClassName : singleReverseRelProp.Name;
        }

        protected virtual PropertyWrapper SingleReverseRelProp
        {
            get { return this.GetSingleReverseRelPropInfos().FirstOrDefault(); }
        }

        /// <summary>
        /// Are there potentially multiple reverse relationships that could be found.
        /// </summary>
        public virtual bool HasMultipleReverseRelationship
        {
            get { return (this.GetMultipleReverseRelPropInfo() != null); }
        }
        /// <summary>
        /// Are there more than one OneToOne Relationships that could be found.
        /// </summary>
        public virtual bool HasMoreThanOneToOneReverseRelationship
        {
            get { return this.GetOneToOneReverseRelationshipInfos().Count > 1; }
        }
        /// <summary>
        /// Is the wrapped <see cref="PropertyInfo"/> a static property.
        /// </summary>
        public virtual bool IsStatic
        {
            get
            {
                PropertyInfo propInfo = this.PropertyInfo;
                var getMethod = this.PropertyInfo.GetGetMethod();
                var setMethod = propInfo.GetSetMethod();
                return (propInfo.CanRead && getMethod != null && getMethod.IsStatic) ||
                       (propInfo.CanWrite && setMethod != null && setMethod.IsStatic);
            }
        }
        /// <summary>
        /// Is the wrapped <see cref="PropertyInfo"/> a public property.
        /// </summary>
        public virtual bool IsPublic
        {
            get
            {
                var getMethod = this.PropertyInfo.GetGetMethod();
                return getMethod != null && getMethod.IsPublic;
            }
        }
        /// <summary>
        /// Is the wrapped <see cref="PropertyInfo"/> a overriden property.
        /// </summary>
        public virtual bool IsOverridden
        {
            get
            {
                if (ReflectedType != DeclaringType) return false;
                return this.PropertyInfo.GetBaseDefinition() != this.PropertyInfo;
            }
        }
        /// <summary>
        /// Returns teh Underlying PropertyType in cases where this property returns a generic or nullable type.
        /// </summary>
        public virtual Type UndelyingPropertyType
        {
            get { return ReflectionUtilities.GetUndelyingPropertType(this.PropertyInfo); }
        }

        /// <summary>
        /// has a StringLengthRule Attribute on the Property.
        /// </summary>
        public virtual bool HasStringLengthRuleAttribute
        {
            get { return this.HasAttribute<AutoMapStringLengthPropRuleAttribute>(); }
        }

        ///<summary>
        /// has a StringPatternMatchRule Attribute on the Property.
        ///</summary>
        public virtual bool HasStringPatternMatchRuleAttribute
        {
            get { return this.HasAttribute<AutoMapStringPatternMatchPropRuleAttribute>(); }
        }

        public virtual bool HasDateTimeRuleAttribute
        {
            get { return this.HasAttribute<AutoMapDateTimePropRuleAttribute>(); }
        }

        public virtual bool HasDateTimeStringRuleAttribute
        {
            get { return this.HasAttribute<AutoMapDateTimePropRuleAttribute>(); }
        }


        //TODO brett 10 Jul 2010: This looks wrong to me surely it should use the UnderlyingType as above
        // and not the PropertyType.
        /// <summary>
        /// Returns the First PropertyWrapper for a Property declared on the RelatedType.
        /// </summary>
        /// <returns></returns>
        public virtual PropertyWrapper GetMultipleReverseRelPropInfo()
        {
            TypeWrapper multipleRelationshipType = this.DeclaringType.MakeGenericBusinessObjectCollection();
            IEnumerable<PropertyWrapper> propertyWrappers = this.PropertyType.GetProperties();
            return propertyWrappers.FirstOrDefault(info => info.PropertyType == multipleRelationshipType);
        }

#region Equality
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(PropertyInfo other)
        {
            return !ReferenceEquals(other, null) && other.Equals(this.PropertyInfo);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(string other)
        {
            return !ReferenceEquals(other, null) && other.Equals(Name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="propInfo"></param>
        /// <returns></returns>
        public static bool operator ==(PropertyWrapper original, PropertyInfo propInfo)
        {
            if (propInfo == null && ReferenceEquals(original, null)) return true;
            if (propInfo == null && original._propertyInfo == null) return true;
            if (ReferenceEquals(original, null) || original._propertyInfo == null) return false;

            return original._propertyInfo == propInfo;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="propWrapper"></param>
        /// <returns></returns>
        public static bool operator ==(PropertyInfo original, PropertyWrapper propWrapper)
        {
            return propWrapper == original;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="propInfo"></param>
        /// <returns></returns>
        public static bool operator !=(PropertyWrapper original, PropertyInfo propInfo)
        {
            return !(original == propInfo);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="propWrapper"></param>
        /// <returns></returns>
        public static bool operator !=(PropertyInfo original, PropertyWrapper propWrapper)
        {
            return !(original == propWrapper);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(PropertyWrapper other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(other, null)) return false;
            if (other.PropertyInfo == null && PropertyInfo == null) return true;
            if (ReferenceEquals(other.PropertyInfo, null)) return false;
            return ReferenceEquals(other.PropertyInfo, PropertyInfo);
            //return other.PropertyInfo.Equals(PropertyInfo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (PropertyWrapper)) return false;
            return Equals((PropertyWrapper) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (PropertyInfo.GetHashCode() * 397) ^ (Name != null ? Name.GetHashCode(): 0);
            }
        }

        #endregion
    }

    ///<summary>
    /// Extension methods for propertyInfo that allow you to obtain the PropertyInfo for the highest superclass.
    ///</summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// When overridden in a derived class, returns the <see cref="propertyInfo"/> object for the
        /// method on the direct or indirect base class in which the property represented
        /// by this instance was first declared.
        /// </summary>
        /// <returns>A <see cref="propertyInfo"/> object for the first implementation of this property.</returns>
        public static PropertyInfo GetBaseDefinition(this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");

            var method = propertyInfo.GetGetMethod();
            if (method == null) return null;

            var baseMethod = method.GetBaseDefinition();

            if (baseMethod == method) return propertyInfo;

            const BindingFlags allProperties = BindingFlags.Instance | BindingFlags.Public
                                               | BindingFlags.NonPublic | BindingFlags.Static;

            var arguments = propertyInfo.GetIndexParameters().Select(p => p.ParameterType).ToArray();

            return baseMethod.DeclaringType.GetProperty(propertyInfo.Name, allProperties,
                null, propertyInfo.PropertyType, arguments, null);
        }
    }
    // ReSharper restore ClassWithVirtualMembersNeverInherited.Global
}