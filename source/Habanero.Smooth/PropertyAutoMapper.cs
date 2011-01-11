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
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Util;

namespace Habanero.Smooth
{
    ///<summary>
    /// This is a class that will automap all properties in the BusinessObject.
    ///</summary>
    public class PropertyAutoMapper
    {
        ///<summary>
        /// Constructs the Property Automapper for a particular <see cref="PropertyInfo"/>.
        ///</summary>
        ///<param name="propInfo"></param>
        ///<exception cref="ArgumentNullException"></exception>
        public PropertyAutoMapper(PropertyInfo propInfo)
        {
            if (propInfo == null) throw new ArgumentNullException("propInfo");
            this.PropertyWrapper = propInfo.ToPropertyWrapper();
        }
        /// <summary>
        /// Constructs the Property Automapper for a particular <see cref="ReflectionWrappers.PropertyWrapper"/>
        /// where a PropertyWrapper typically wraps a <see cref="PropertyInfo"/> and provides additional methods.
        /// </summary>
        /// <param name="propertyWrapper"></param>
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
                var propertyType = this.PropertyWrapper.UndelyingPropertyType;
                var propDef = new PropDef(this.PropertyWrapper.Name, propertyType, PropReadWriteRule.ReadWrite, null)
                          {
                              Compulsory = this.PropertyWrapper.HasCompulsoryAttribute
                          };
                if(this.PropertyWrapper.HasDefaultAttribute)
                {
                    propDef.DefaultValueString = this.PropertyWrapper.GetAttribute<AutoMapDefaultAttribute>().DefaultValue;
                }
                if (this.PropertyWrapper.HasReadWriteRuleAttribute)
                {
                    propDef.ReadWriteRule = this.PropertyWrapper.GetAttribute<AutoMapReadWriteRuleAttribute>().ReadWriteRule;
                }
                propDef.AutoIncrementing = this.PropertyWrapper.HasAutoIncrementingAttribute;
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
                if (this.PropertyWrapper.IsInherited) return false;
                var propertyType = this.PropertyWrapper.UndelyingPropertyType;
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

    /// <summary>
    /// Provides extension methods that allow a more fluent programming style 
    /// </summary>
    public static class PropMapperExtensions
    {
        /// <summary>
        /// Based on the information available for the PropertyInfo (Name, Attributes, DataTypes etc)
        /// And a set of Heuristics a Property Definition is created for this <see cref="PropertyInfo"/>
        /// </summary>
        /// <param name="propInfo"></param>
        /// <returns></returns>
        public static IPropDef MapProperty(this PropertyInfo propInfo)
        {
            PropertyAutoMapper autoMapper = new PropertyAutoMapper(propInfo);
            return autoMapper.MapProperty();
        }

        //This method is copied unchanged from SubSonic.
        //Thanks from the Habanero team 2 the Subsonic team for this.
        /// <summary>
        /// Using the heuristic that the property should be a mappable type
        /// or an <see cref="Enum"/> to be mapped as a property else it should be a 
        /// Relationship or a component.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanMapToProp(this Type type)
        {
            if(type == null) return false;
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
                   type.IsEnumType();
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