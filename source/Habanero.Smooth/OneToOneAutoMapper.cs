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
using System.Reflection;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Util;
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
namespace Habanero.Smooth
{
    /// <summary>
    ///Conventions
    /// 1) If no single reverse relationship and no Attribute is found then it is assumed that relationship is a M:1 i.e. its rev is a 1:M
    /// 2) If no single rev rel and 1:1Attribute with no RevRelName then RevRelName = ClassName
    /// 3) If no single rev rel and 1:1Att with RevRelName then RevRelName = DeclaredRevRelName
    /// 4) If has single rev rel then RevRelName = foundRevRelationshipName 
    /// Determing RelatedProps 
    /// if ownerClass.HasProp(RelName+ID) and relatedClass.NotHasProp(RevRelName+ID) 
    ///      owningBOHasForeignKey = true;
    ///      ownerProp = foundOwnerPropName
    ///      relatedProp = RelatedClass.ID.
    /// if relatedClass.HasProp(reverseRel+ID) and ownerClass.NotHasProp(RelName+ID)
    ///      owningBOHasForeignKey = false;
    ///      ownerProp = OwnerClass.Id
    ///      relatedProp = foundRelatedPropName
    /// if ownerClass.HasProp(RelName+ID) and relatedClass.HasProp(RevRelName+ID)
    ///      if(RelName == relatedClassName) owningBOHasForeignKey = true;
    ///      ownerProp = foundOwnerClassId
    ///      relatedProp = foundRelatedPropName
    /// Else if ownerClassName LT relatedClassName
    ///     owningBOHasForeignKey = false;
    ///     ownerProp = OwnerClassId
    ///     relatedProp = reverseRelationshipName+ID
    /// Else
    ///    owningBOHasForeignKey = true;
    ///    ownerProp = RelationshipName+ID
    ///    relatedProp = RelatedClass.ID
    /// </summary>
    public class OneToOneAutoMapper
    {
        private PropertyWrapper PropertyWrapper { get; set; }

        public OneToOneAutoMapper(PropertyWrapper propWrap)
        {
            if (propWrap == null) throw new ArgumentNullException("propWrap");
            this.PropertyWrapper = propWrap;
        }

        public IRelationshipDef MapOneToOne()
        {
            if (!MustBeMapped()) return null;
            CheckReverseRelationshipValid();
            
            var relatedClassType = PropertyWrapper.RelatedClassType.GetUnderlyingType();
            DeleteParentAction deleteAction = this.OwningBoHasForeignKey 
                                                  ? DeleteParentAction.DoNothing 
                                                  : DeleteParentAction.Prevent;

            SingleRelationshipDef relDef
                    = new SingleRelationshipDef(this.PropertyWrapper.Name, relatedClassType
                                            , new RelKeyDef(), true, deleteAction)
                      {
                          OwningBOHasForeignKey = this.OwningBoHasForeignKey,
                          ReverseRelationshipName = this.ReverseRelationshipName
                      };
            relDef.SetAsOneToOne();
            IRelPropDef relPropDef = this.CreateRelPropDef();
            relDef.RelKeyDef.Add(relPropDef);
            return relDef;
        }

        public bool MustBeMapped()
        {
            if (PropertyWrapper == null) return false;
            if(this.PropertyWrapper.IsInherited) return false;
            if (this.PropertyWrapper.IsStatic) return false;
            if (!this.PropertyWrapper.IsPublic) return false;
            if (!PropertyWrapper.IsSingleRelationhip) return false;
            if (PropertyWrapper.HasIgnoreAttribute) return false;
            if (this.PropertyWrapper.PropertyInfo == null) return false;
            if (this.PropertyWrapper.DeclaringType.IsNull()) return false;
            return (this.PropertyWrapper.HasSingleReverseRelationship 
                    && !this.PropertyWrapper.HasMultipleReverseRelationship) 
                    || this.PropertyWrapper.HasOneToOneAttribute;
        }

        public virtual string GetOwningPropName()
        {
            var ownerClassType = this.PropertyWrapper.DeclaringType;
            var owningFKPropName = GetFkPropName(this.RelationshipName);

            return this.OwningBoHasForeignKey 
                       ? owningFKPropName 
                       : ownerClassType.GetPKPropName();
        }

        private static string GetFkPropName(string relationshipName)
        {
            return PropNamingConvention.GetSingleRelOwningPropName(relationshipName);
        }

        private string RelationshipName
        {
            get { return this.PropertyWrapper.Name; }
        }

        public virtual string GetRelatedPropName()
        {
            var relatedClassType = this.PropertyWrapper.RelatedClassType;
            var relatedRelPropName = GetFkPropName(this.ReverseRelationshipName);

            return this.RelatedBoHasForeignKey
                       ? relatedRelPropName
                       : relatedClassType.GetPKPropName();

        }

        private static bool OwnerClassNameGTRelatedClassName(TypeWrapper ownerClassType, TypeWrapper relatedClassType)
        {
            return ownerClassType.Name.CompareTo(relatedClassType.Name) == 1;
        }


        public static INamingConventions PropNamingConvention
        {
            get
            {
                return ClassAutoMapper.PropNamingConvention;
            }
        }

        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        public bool OwningBoHasForeignKey
        {
            get
            {
                var relatedClassType = this.PropertyWrapper.RelatedClassType;
                var relatedClassHasFKProperty = FKFoundOnRelatedClass();

                var ownerClassType = this.PropertyWrapper.DeclaringType;
                bool owningClassHasFKProperty = FKFoundOnOwnerClass();

                if (owningClassHasFKProperty && !relatedClassHasFKProperty) return true;
                if (relatedClassHasFKProperty && !owningClassHasFKProperty) return false;
                if (owningClassHasFKProperty && relatedClassHasFKProperty) return true;
                //This is somewhat arbitrary but if you cannot find any 
                // Props on either that act as an FKProp then use this
                // This might still cause problems where 
                // a one to one self referencing relationship where 
                // the reverse relationship is defined on the class
                // may result in strange behaviour if the relationship name
                // of neither side matches the related class name.
                return OwnerClassNameGTRelatedClassName(ownerClassType, relatedClassType);
            }
        }

        // ReSharper restore ConditionIsAlwaysTrueOrFalse

        private bool FKFoundOnOwnerClass()
        {
            var ownerClassType = this.PropertyWrapper.DeclaringType;
            var relationshipName = this.PropertyWrapper.Name;
            var owningFKPropName = PropNamingConvention.GetSingleRelOwningPropName(relationshipName);
            return ownerClassType.HasProperty(owningFKPropName);
        }

        private bool FKFoundOnRelatedClass()
        {
            var reverseRelName = this.PropertyWrapper.GetSingleReverseRelationshipName<AutoMapOneToOne>();
            var relatedClassType = this.PropertyWrapper.RelatedClassType;
            var relatedFKPropName = PropNamingConvention.GetSingleRelOwningPropName(reverseRelName);
            return relatedClassType.HasProperty(relatedFKPropName);
        }

        private bool RelatedBoHasForeignKey
        {
            get
            {
                return FKFoundOnRelatedClass() ? true : !this.OwningBoHasForeignKey;
            }
        }


        public virtual string ReverseRelationshipName
        {
            get { return this.PropertyWrapper.GetSingleReverseRelationshipName<AutoMapOneToOne>(); }
        }


        public IRelPropDef CreateRelPropDef()
        {
            var relPropDef = new RelPropDef(this.GetOwningPropName(), this.GetRelatedPropName());
            return relPropDef;
        }

        private void CheckReverseRelationshipValid()
        {
            if (this.PropertyWrapper.HasMoreThanOneToOneSingleReverseRelationship)
            {
                throw new InvalidDefinitionException("The Relationship '" + PropertyWrapper.Name
                                                     +
                                                     "' could not be automapped since there are multiple Single relationships on class '"
                                                     + PropertyWrapper.RelatedClassType +
                                                     "' that reference the BusinessObject Class '"
                                                     + PropertyWrapper.DeclaringClassName +
                                                     "'. Please map using ClassDef.XML or Attributes");
            }
        }
    }

    public static class OneToOneAutoMapperExtensions
    {
        public static IRelationshipDef MapOneToOne(this PropertyWrapper propWrapper)
        {
            if (propWrapper == null) return null;
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propWrapper);
            return autoMapper.MapOneToOne();
        }
    }
    // ReSharper restore ClassWithVirtualMembersNeverInherited.Global

}