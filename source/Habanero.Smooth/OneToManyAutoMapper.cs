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
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Util;

namespace Habanero.Smooth
{
    // ReSharper disable ClassWithVirtualMembersNeverInherited.Global
    /// <summary>
    /// Extension methods to make the automapping testing and using easier through a fluent style interface
    /// </summary>
    public static class OneToManyAutoMapperExtensions
    {
        /// <summary>
        /// Automap the propInfo as a 
        /// </summary>
        /// <param name="propInfo"></param>
        /// <returns></returns>
        public static IRelationshipDef MapOneToMany(this PropertyInfo propInfo)
        {
            return propInfo.ToPropertyWrapper().MapOneToMany();
        }
        /// <summary>
        /// Map this property to a Relationship definition based on its Defintiion
        /// </summary>
        /// <param name="propertyWrapper"></param>
        /// <returns></returns>
        public static IRelationshipDef MapOneToMany(this PropertyWrapper propertyWrapper)
        {
            if (propertyWrapper == null) return null;
            var autoMapper = new OneToManyAutoMapper(propertyWrapper);

            return autoMapper.MapOneToMany();
        }
    }
    /// <summary>
    /// AutoMapper that used used to create the relationship in the case 
    /// for a One To Many Relationship.
    /// </summary>
    public class OneToManyAutoMapper
    {
        private PropertyWrapper PropertyWrapper { get; set; }
        /// <summary>
        /// Construct the AutoMapper for a specified PropertyInfo.
        /// </summary>
        /// <param name="propInfo"></param>
        public OneToManyAutoMapper(PropertyInfo propInfo)
        {
            if (propInfo == null) throw new ArgumentNullException("propInfo");
            this.PropertyWrapper = propInfo.ToPropertyWrapper();
        }
        /// <summary>
        /// Construct the AutoMapper for a specified propertyWrapper.
        /// </summary>
        /// <param name="propertyWrapper"></param>
        public OneToManyAutoMapper(PropertyWrapper propertyWrapper)
        {
            if (propertyWrapper == null) throw new ArgumentNullException("propertyWrapper");
            this.PropertyWrapper = propertyWrapper;
        }
        /// <summary>
        /// Map the relationship including the Relationship props.
        /// </summary>
        /// <returns></returns>
        public IRelationshipDef MapOneToMany()
        {
            var propertyType = this.PropertyWrapper.PropertyType;
            if(!MustBeMapped()) return null;

            var singleReverseRelPropInfos = this.PropertyWrapper.GetSingleReverseRelPropInfos();
            if (singleReverseRelPropInfos.Count > 1)
            {
                throw new InvalidDefinitionException("The Relationship '" + this.PropertyWrapper.Name
                    + "' could not be automapped since there are multiple Single relationships on class '"
                    + this.PropertyWrapper.RelatedClassType + "' that reference the BusinessObject Class '"
                    + this.PropertyWrapper.DeclaringClassName + "'. Please map using ClassDef.XML or Attributes");
            }
            var relationshipAttribute = this.PropertyWrapper.GetAttribute<AutoMapOneToManyAttribute>();
            MultipleRelationshipDef relDef;
            if (propertyType.IsGenericType)
            {
                var relatedClassType = this.PropertyWrapper.RelatedClassType.UnderlyingType;
                relDef = new MultipleRelationshipDef(this.PropertyWrapper.Name, relatedClassType, new RelKeyDef(), true, "", DeleteParentAction.Prevent);

            }else
            {
                string className = StringUtilities.Singularize(this.PropertyWrapper.Name);
                relDef = new MultipleRelationshipDef(this.PropertyWrapper.Name, this.PropertyWrapper.AssemblyQualifiedName, className
                        , new RelKeyDef(), true, "", DeleteParentAction.Prevent);
            }
            if(relationshipAttribute != null)
            {
                relDef.RelationshipType = relationshipAttribute.RelationshipType;
                relDef.DeleteParentAction = relationshipAttribute.DeleteParentAction;
            }
            relDef.ReverseRelationshipName = GetReverseRelationshipName();

            var relPropDef = CreateRelPropDef();
            relDef.RelKeyDef.Add(relPropDef);
            return relDef;
        }
        /// <summary>
        /// Must the Relationship be Mapped.
        /// </summary>
        /// <returns></returns>
        public bool MustBeMapped()
        {
            if (this.PropertyWrapper.IsStatic) return false;
            if (!this.PropertyWrapper.IsPublic) return false;
            if (this.PropertyWrapper.IsInherited) return false;
            if (this.PropertyWrapper.DeclaringType == (Type)null) return false;
            if (!this.PropertyWrapper.IsMultipleRelationship) return false;
            return !this.PropertyWrapper.HasIgnoreAttribute;
        }

        private string GetReverseRelationshipName()
        {
            return this.PropertyWrapper.GetSingleReverseRelationshipName<AutoMapOneToManyAttribute>();
        }
        /// <summary>
        /// Create a new Relationship Prop Def.
        /// </summary>
        /// <returns></returns>
        public IRelPropDef CreateRelPropDef()
        {
            var ownerPropName = GetOwningPropName(this.PropertyWrapper.DeclaringType);
            var relatedPropName = GetRelatedPropName();
            IRelPropDef relPropDef = new RelPropDef(ownerPropName, relatedPropName);
            return relPropDef;
        }
        /// <summary>
        /// Returns the owning property name 
        /// </summary>
        /// <param name="ownerClassType"></param>
        /// <returns></returns>
        public static string GetOwningPropName(TypeWrapper ownerClassType)
        {
            return ownerClassType.GetPKPropName();
        }
        /// <summary>
        /// Returns the Related Property name.
        /// </summary>
        /// <returns></returns>
        public string GetRelatedPropName()
        {
            if (this.PropertyWrapper.HasSingleReverseRelationship)
            {
                PropertyWrapper reverseRelPropInfo = this.PropertyWrapper.GetSingleReverseRelPropInfos()[0];
                return PropNamingConvention.GetSingleRelOwningPropName(reverseRelPropInfo.Name);
            }
            return GetOwningPropName(this.PropertyWrapper.DeclaringType);
        }
        /// <summary>
        /// Returns the property naming convention.
        /// </summary>
        public static INamingConventions PropNamingConvention
        {
            get
            {
                return ClassAutoMapper.PropNamingConvention;
            }

        }
            
    }
}