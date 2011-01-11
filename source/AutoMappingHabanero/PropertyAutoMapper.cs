using System;
using System.Reflection;
using AutoMappingHabanero.ReflectionWrappers;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace AutoMappingHabanero
{
    public class PropertyAutoMapper
    {
        public PropertyAutoMapper(PropertyInfo propInfo)
        {
            if (propInfo == null) throw new ArgumentNullException("propInfo");
            this.PropertyWrapper = propInfo.ToPropertyWrapper();
        }
        public PropertyAutoMapper(PropertyWrapper propertyWrapper)
        {
            if (propertyWrapper == null) throw new ArgumentNullException("propertyWrapper");
            this.PropertyWrapper = propertyWrapper;
        }

        private PropertyWrapper PropertyWrapper { get; set; }

        /// <summary>
        /// Will attempt to map a PropertyInfo to an IPropDef.
        /// If it cannot be mapped then will return null.
        /// </summary>
        /// <returns></returns>
        public IPropDef MapProperty()
        {
            
            if (MustMapProperty)
            {
                var propertyType = ReflectionUtilities.GetUndelyingPropertType(this.PropertyWrapper.PropertyInfo);
                var propDef = new PropDef(this.PropertyWrapper.Name, propertyType, PropReadWriteRule.ReadWrite, null)
                          {
                              Compulsory = this.PropertyWrapper.HasCompulsoryAttribute
                          };
                return propDef;
            }
            return null;
        }

        private bool MustMapProperty
        {
            get
            {
                if(this.PropertyWrapper.IsStatic) return false;
                if(!this.PropertyWrapper.IsPublic) return false;
                if (this.PropertyWrapper.IsInheritedProp) return false;
                var propertyType = ReflectionUtilities.GetUndelyingPropertType(this.PropertyWrapper.PropertyInfo);
                if (IsInheritedFromBO) return false;

                return !this.PropertyWrapper.HasIgnoreAttribute && propertyType.CanMapToProp();
            }
        }

        private bool IsInheritedFromBO
        {
            get
            {
                var propName = this.PropertyWrapper.Name;
                return propName == "DirtyXML" || propName == "ClassDefName";
            }
        }
    }

    //TODO brett 04 Feb 2010: change to internal set tests up to be friends
    public static class PropMapperExtensions
    {

        public static IPropDef MapProperty(this PropertyInfo propInfo)
        {
            PropertyAutoMapper autoMapper = new PropertyAutoMapper(propInfo);
            return autoMapper.MapProperty();
        }

        //This method is copied unchanged from SubSonic.
        //Thanks from the Habanero team 2 the Subsonic team for this.
        public static bool CanMapToProp(this Type type)
        {
            return type == typeof(string) ||
                   type == typeof(Guid) ||
                   type == typeof(Guid?) ||
                   type == typeof(decimal) ||
                   type == typeof(decimal?) ||
                   type == typeof(double) ||
                   type == typeof(double?) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTime?) ||
                   type == typeof(bool) ||
                   type == typeof(bool?) ||
                   type == typeof(Int16) ||
                   type == typeof(Int16?) ||
                   type == typeof(Int32) ||
                   type == typeof(Int32?) ||
                   type == typeof(Int64) ||
                   type == typeof(Int64?) ||
                   type == typeof(float?) ||
                   type == typeof(float) ||
                   type.IsEnum || type.IsNullableEnum();
        }
        private static bool IsEnumType(this Type type)
        {
            return type.IsEnum || type.IsNullableEnum();
        }
        //This method is copied unchanged from SubSonic.
        //Thanks from the Habanero team 2 the Subsonic team for this.
        private static bool IsNullableEnum(this Type type)
        {
            var enumType = Nullable.GetUnderlyingType(type);
            return enumType != null && enumType.IsEnum;
        }
    }
}