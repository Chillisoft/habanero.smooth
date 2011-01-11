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
    public static class OneToManyAutoMapperExtensions
    {

        public static IRelationshipDef MapOneToMany(this PropertyInfo propInfo)
        {
            return propInfo.ToPropertyWrapper().MapOneToMany();
        }
        public static IRelationshipDef MapOneToMany(this PropertyWrapper propertyWrapper)
        {
            if (propertyWrapper == null) return null;
            OneToManyAutoMapper autoMapper = new OneToManyAutoMapper(propertyWrapper);

            return autoMapper.MapOneToMany();
        }
    }
    public class OneToManyAutoMapper
    {
        private PropertyWrapper PropertyWrapper { get; set; }

        public OneToManyAutoMapper(PropertyInfo propInfo)
        {
            if (propInfo == null) throw new ArgumentNullException("propInfo");
            this.PropertyWrapper = propInfo.ToPropertyWrapper();
        }
        public OneToManyAutoMapper(PropertyWrapper propertyWrapper)
        {
            if (propertyWrapper == null) throw new ArgumentNullException("propertyWrapper");
            this.PropertyWrapper = propertyWrapper;
        }

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

            relDef.ReverseRelationshipName = GetReverseRelationshipName();

            IRelPropDef relPropDef = CreateRelPropDef();
            relDef.RelKeyDef.Add(relPropDef);
            return relDef;
        }

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
            return this.PropertyWrapper.GetSingleReverseRelationshipName<AutoMapOneToMany>();
        }

        public IRelPropDef CreateRelPropDef()
        {
            var ownerPropName = GetOwningPropName(this.PropertyWrapper.DeclaringType);
            var relatedPropName = GetRelatedPropName();
            IRelPropDef relPropDef = new RelPropDef(ownerPropName, relatedPropName);
            return relPropDef;
        }

        public static string GetOwningPropName(TypeWrapper ownerClassType)
        {
            return ownerClassType.GetPKPropName();
        }

        public string GetRelatedPropName()
        {
            if (this.PropertyWrapper.HasSingleReverseRelationship)
            {
                PropertyWrapper reverseRelPropInfo = this.PropertyWrapper.GetSingleReverseRelPropInfos()[0];
                return PropNamingConvention.GetSingleRelOwningPropName(reverseRelPropInfo.Name);
            }
            return GetOwningPropName(this.PropertyWrapper.DeclaringType);
        }
        public static INamingConventions PropNamingConvention
        {
            get
            {
                return ClassAutoMapper.PropNamingConvention;
            }

        }
            
    }
}