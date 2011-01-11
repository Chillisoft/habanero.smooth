using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Habanero.Base;
using Habanero.BO;
// ReSharper disable VirtualMemberNeverOverriden.Global
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace AutoMappingHabanero.ReflectionWrappers
{
    public static class PropertyExtensions
    {
        public static PropertyWrapper ToPropertyWrapper(this PropertyInfo propertyInfo)
        {
            return propertyInfo == null ? null : new PropertyWrapper(propertyInfo);
        }

        public static PropertyWrapper GetPropertyWrapper(this Type t, string propertyName)
        {
            return t.GetProperty(propertyName).ToPropertyWrapper();
        }
    }

    public class PropertyWrapper : MemberWrapper
    {
        #region ImplementationOfMember  

        private readonly PropertyInfo _propertyInfo;
        private TypeWrapper _declaringTypeWrapper;
        private TypeWrapper _propertyTypeWrapper;
        private TypeWrapper _reflectedTypeWrapper;

        public PropertyWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");
            _propertyInfo = propertyInfo;
        }

        public override string Name
        {
            get { return _propertyInfo.Name; }
        }

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
        public virtual PropertyInfo PropertyInfo
        {
            get { return _propertyInfo; }
        }

        public override TypeWrapper DeclaringType
        {
            get
            {
                if (_declaringTypeWrapper.IsNull()) _declaringTypeWrapper = new TypeWrapper(_propertyInfo.DeclaringType);
                return _declaringTypeWrapper;
            }
        }

        public override TypeWrapper ReflectedType
        {
            get
            {
                if (_reflectedTypeWrapper.IsNull()) _reflectedTypeWrapper = new TypeWrapper(_propertyInfo.ReflectedType);
                return _reflectedTypeWrapper;
            }
        }

        public virtual string DeclaringClassName
        {
            get { return this.DeclaringType.Name; }
        }

//        public override bool HasIndexParameters
//        {
//            get { return _propertyInfo.GetIndexParameters().Length > 0; }
//        }
//        public override bool IsMethod
//        {
//            get { return false; }
//        }
//        public override bool IsField
//        {
//            get { return false; }
//        }
        public override bool IsProperty
        {
            get { return true; }
        }

        #endregion

//        public override string ToString()
//        {
//            return this._propertyInfo.ToString();
//        }

        /// <summary>
        /// Returns true if the <see cref="PropertyInfo"/> has an <see cref="AutoMapIgnoreAttribute"/>
        /// false otherwise. Returns false if propInfo is null
        /// </summary>
        /// <value></value>
        public virtual bool HasIgnoreAttribute
        {
            get { return this.HasAttribute<AutoMapIgnoreAttribute>(); }
        }

        /// <summary>
        /// Returns true if this property has the <see cref="AutoMapCompulsoryAttribute"/>
        /// </summary>
        public bool HasCompulsoryAttribute
        {
            get { return this.HasAttribute<AutoMapCompulsoryAttribute>(); }
        }
        public virtual bool HasManyToOneAttribute
        {
            get { return this.HasAttribute<AutoMapManyToOneAttribute>(); }
        }

        public virtual bool HasOneToOneAttribute
        {
            get { return this.HasAttribute<AutoMapOneToOne>(); }
        }
        public virtual bool HasAutoMapOneToOneAttribute(string relationshipName)
        {
            return this.HasAutoMapAttribute<AutoMapOneToOne>(relationshipName);
        }

        private bool HasAutoMapAttribute<T>(string relationshipName) where T : AutoMapRelationshipAttribute
        {
            var attributes = this.GetAttributes<T>();
            return attributes.Any(a => a.ReverseRelationshipName == relationshipName);
        }

        public virtual bool HasAutoMapManyToOneAttribute(string relationshipName)
        {
            return this.HasAutoMapAttribute<AutoMapManyToOneAttribute>(relationshipName);
        }

        public virtual bool IsOfType<T>()
        {
            return this.PropertyType.IsOfType<T>();
        }

        public virtual bool HasAttribute<T>() where T : class
        {
            T attribute = this.GetAttribute<T>();
            return attribute != null;
        }

        protected virtual T GetAttribute<T>() where T : class
        {
            var attributes = this.GetAttributes<T>();
            return attributes == null ? null : attributes.FirstOrDefault();
        }

        public virtual IEnumerable<T> GetAttributes<T>() where T : class
        {
            var attributes = this._propertyInfo.GetCustomAttributes(typeof (T), true);
            return attributes == null ? null : attributes.Select(y => y as T);
        }

        public virtual IList<PropertyWrapper> GetOneToOneReverseRelationshipInfos()
        {
            if (this.IsMultipleRelationship || this.HasIgnoreAttribute || this.HasAttribute<AutoMapManyToOneAttribute>())
                return new List<PropertyWrapper>();

            return this.GetSingleReverseRelPropInfos<AutoMapManyToOneAttribute, AutoMapOneToOne, AutoMapOneToOne>();
        }

        public virtual IList<PropertyWrapper> GetManyToOneReverseRelationshipInfos()
        {
            if (this.IsSingleRelationhip || this.HasIgnoreAttribute) return new List<PropertyWrapper>();

            return this.GetSingleReverseRelPropInfos<AutoMapOneToOne, AutoMapManyToOneAttribute, AutoMapOneToMany>();
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

        public virtual string GetMappedReverseRelationshipName<TAutoMapAttribute>()
            where TAutoMapAttribute : AutoMapRelationshipAttribute
        {
            if (!this.HasAttribute<TAutoMapAttribute>()) return null;
            TAutoMapAttribute autoMapAttribute = this.GetAttribute<TAutoMapAttribute>();
            return autoMapAttribute.ReverseRelationshipName;
        }

        public virtual TypeWrapper RelatedClassType
        {
            get
            {
                var propertyType = this.PropertyType;
                return propertyType.IsGenericType ? propertyType.GetGenericArguments().First() : propertyType;
            }
        }

        public virtual string RelatedClassName
        {
            get
            {
                return this.RelatedClassType.Name;
            }
        }

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

        public virtual bool IsInheritedProp
        {
            get { return ReflectedType != DeclaringType; }
        }
        public virtual bool IsSingleRelationhip
        {
            get { return this.PropertyType.IsOfType<BusinessObject>(); }
        }

        public virtual bool IsMultipleRelationship
        {
            get { return this.PropertyType.IsOfType<IBusinessObjectCollection>(); }
        }

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


        public virtual bool HasMultipleReverseRelationship
        {
            get { return (this.GetMultipleReverseRelPropInfo() != null); }
        }

        public virtual bool HasMoreThanOneToOneSingleReverseRelationship
        {
            get { return this.GetOneToOneReverseRelationshipInfos().Count > 1; }
        }

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

        public virtual bool IsPublic
        {
            get
            {
                var getMethod = this.PropertyInfo.GetGetMethod();
                return getMethod != null && getMethod.IsPublic;
            }
        }



        public virtual PropertyWrapper GetMultipleReverseRelPropInfo()
        {
            TypeWrapper multipleRelationshipType = this.DeclaringType.MakeGenericBusinessObjectCollection();
            IEnumerable<PropertyWrapper> propertyWrappers = this.PropertyType.GetProperties();
            return propertyWrappers.FirstOrDefault(info => info.PropertyType == multipleRelationshipType);
        }

#region Equality
        public bool Equals(PropertyInfo other)
        {
            return !ReferenceEquals(other, null) && other.Equals(this.PropertyInfo);
        }

        public bool Equals(string other)
        {
            return !ReferenceEquals(other, null) && other.Equals(Name);
        }
        public static bool operator ==(PropertyWrapper original, PropertyInfo propInfo)
        {
            if (propInfo == null && ReferenceEquals(original, null)) return true;
            if (propInfo == null && original._propertyInfo == null) return true;
            if (ReferenceEquals(original, null) || original._propertyInfo == null) return false;

            return original._propertyInfo == propInfo;
        }

        public static bool operator ==(PropertyInfo original, PropertyWrapper propWrapper)
        {
            return propWrapper == original;
        }
        public static bool operator !=(PropertyWrapper original, PropertyInfo propInfo)
        {
            return !(original == propInfo);
        }

        public static bool operator !=(PropertyInfo original, PropertyWrapper propWrapper)
        {
            return !(original == propWrapper);
        }
        public bool Equals(PropertyWrapper other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(other, null)) return false;
            if (other.PropertyInfo == null && PropertyInfo == null) return true;
            if (ReferenceEquals(other.PropertyInfo, null)) return false;
            return other.PropertyInfo.Equals(PropertyInfo);
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

    // ReSharper restore ClassWithVirtualMembersNeverInherited.Global
}