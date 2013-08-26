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
    /// if 1:1Attribute then the owningBOHasForeignKey set to true then this is set else the reverse relationship owningBOHasForeignKeyIsSet
    /// Note this has now been set as compulsory.
    /// 
    /// --------------------------------------------------------------------------------------------------------------------
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

        ///<summary>
        /// Construct a One to One Relationship mapper.
        ///</summary>
        ///<param name="propWrap"></param>
        ///<exception cref="ArgumentNullException"></exception>
        public OneToOneAutoMapper(PropertyWrapper propWrap)
        {
            if (propWrap == null) throw new ArgumentNullException("propWrap");
            this.PropertyWrapper = propWrap;
        }

        /// <summary>
        /// Maps the <see cref="ReflectionWrappers.PropertyWrapper"/> to a <see cref="IRelationshipDef"/>.
        /// </summary>
        /// <returns></returns>
        public IRelationshipDef MapOneToOne()
        {
            if (!MustBeMapped()) return null;
            CheckReverseRelationshipValid();

            var relatedClassType = PropertyWrapper.RelatedClassType.UnderlyingType;
            var deleteAction = GetDeleteAction();

            var relDef = new SingleRelationshipDef(this.PropertyWrapper.Name, relatedClassType
                                                   , new RelKeyDef(), true, deleteAction)
                {
                    OwningBOHasForeignKey = this.OwningBOHasForeignKey,
                    ReverseRelationshipName = this.ReverseRelationshipName
                };
            SetRelationshipType(relDef);
            relDef.SetAsOneToOne();
            var relPropDef = this.CreateRelPropDef();
            relDef.RelKeyDef.Add(relPropDef);
            return relDef;
        }

        private DeleteParentAction GetDeleteAction()
        {
            if (IsDefinedAsCompositionOrAggregation()) return DeleteParentAction.Prevent;
            if (IsDefinedAsAssociation() || ReverseRelationshipIsDefinedAsAggregationOrComposition())
                return DeleteParentAction.DoNothing;

            //Else base on the OwningBOHasFK
            return this.OwningBOHasForeignKey
                       ? DeleteParentAction.DoNothing
                       : DeleteParentAction.Prevent;
        }

        private void SetRelationshipType(SingleRelationshipDef relDef)
        {
            var onToManyAtt = this.PropertyWrapper.GetAttribute<AutoMapOneToOneAttribute>();

            if (onToManyAtt != null) relDef.RelationshipType = onToManyAtt.RelationshipType;
        }

        /// <summary>
        /// Determines based on heuristics whether the Property wrapped by the
        /// <see cref="ReflectionWrappers.PropertyWrapper"/> must be mapped to a One to One
        /// Relationship or not
        /// </summary>
        /// <returns></returns>
        public bool MustBeMapped()
        {
            if (PropertyWrapper == null) return false;
            if (this.PropertyWrapper.IsInherited) return false;
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

        /// <summary>
        /// Using a set of heuristics the Owning Property Name
        /// is determined from the RelationshipName and any attributes on the
        /// Property.
        /// I.e. The Property of the class that owns this One To One Relationship.
        /// I.e. The Property on the Class that owns the Property that wrapped by
        /// the PropertyWrapper.
        /// </summary>
        /// <returns></returns>
        public virtual string GetOwningPropName()
        {
            var ownerClassType = this.PropertyWrapper.DeclaringType;
            var owningFKPropName = GetFkPropName(this.RelationshipName);

            return this.OwningBOHasForeignKey
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

        /// <summary>
        /// Using a set of heuristics the Related Property Name
        /// is determined from the RelationshipName and any attributes on the
        /// Property.
        /// I.e. The Property of the class that is related by One To One Relationship.
        /// I.e. The Property on the Class that is returned by the Property that is wrapped by
        /// the PropertyWrapper.
        /// </summary>
        /// <returns></returns>
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
            if (ownerClassType.Name == null) return false;
            return ownerClassType.Name.CompareTo(relatedClassType.Name) == 1;
        }

        /// <summary>
        /// Returns the object that contains the naming conventions used
        /// by this auto mapper.
        /// </summary>
        public static INamingConventions PropNamingConvention
        {
            get { return ClassAutoMapper.PropNamingConvention; }
        }

        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        /// <summary>
        /// Determines whether the Owning Class or the related class has the foriegn key.
        /// This is determined by looking at the Primary keys of each class.
        /// </summary>
        public bool OwningBOHasForeignKey
        {
            get
            {
                var relationshipAttribute = this.PropertyWrapper.GetAttribute<AutoMapOneToOneAttribute>();
                if (relationshipAttribute != null) return relationshipAttribute.OwningBOHasForeignKey;
                var relatedClassType = this.PropertyWrapper.RelatedClassType;
                var relatedClassHasFKProperty = FKFoundOnRelatedClass();

                var ownerClassType = this.PropertyWrapper.DeclaringType;
                var owningClassHasFKProperty = FKFoundOnOwnerClass();

                if (owningClassHasFKProperty && !relatedClassHasFKProperty) return true;
                if (relatedClassHasFKProperty && !owningClassHasFKProperty) return false;
                if (owningClassHasFKProperty && relatedClassHasFKProperty) return true;

                if (IsDefinedAsCompositionOrAggregation()) return false;

                if (ReverseRelationshipIsDefinedAsAggregationOrComposition()) return true;

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

        /// <summary>
        /// If the reverse relationship is defined as Composition or Aggregation via attrubutes then return true.
        /// </summary>
        private bool ReverseRelationshipIsDefinedAsAggregationOrComposition()
        {
            var reverseRelationshipName = this.ReverseRelationshipName;
            var revRelInfos = this.PropertyWrapper.GetOneToOneReverseRelationshipInfos();
            if (revRelInfos == null) return false;
            var revRelProp = revRelInfos.FirstOrDefault(wrapper => wrapper.Name == reverseRelationshipName);
            return IsDefinedAsCompositionOrAggregation(revRelProp);
        }

        private bool IsDefinedAsCompositionOrAggregation()
        {
            return IsDefinedAsCompositionOrAggregation(this.PropertyWrapper);
        }

        /// <summary>
        /// If the <paramref name="propertyWrapper"/> is defined as Composition or Aggregation via attrubutes then return true.
        /// </summary>
        /// <param name="propertyWrapper"></param>
        /// <returns></returns>
        private static bool IsDefinedAsCompositionOrAggregation(PropertyWrapper propertyWrapper)
        {
            if (propertyWrapper == null) return false;
            var autoMapAttribute = propertyWrapper.GetAttribute<AutoMapOneToOneAttribute>();
            if (autoMapAttribute == null) return false;
            return (autoMapAttribute.RelationshipType != RelationshipType.Association);
        }


        /// <summary>
        /// If this relationship is defined (I.e. defined via an attribute) as Association then return true.
        /// </summary>
        /// <returns></returns>
        private bool IsDefinedAsAssociation()
        {
            var autoMapAttribute = this.PropertyWrapper.GetAttribute<AutoMapOneToOneAttribute>();
            if (autoMapAttribute == null) return false;
            return (autoMapAttribute.RelationshipType == RelationshipType.Association);
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
            var reverseRelName = this.PropertyWrapper.GetSingleReverseRelationshipName<AutoMapOneToOneAttribute>();
            var relatedClassType = this.PropertyWrapper.RelatedClassType;
            var relatedFKPropName = PropNamingConvention.GetSingleRelOwningPropName(reverseRelName);
            return relatedClassType.HasProperty(relatedFKPropName);
        }

        private bool RelatedBoHasForeignKey
        {
            get { return FKFoundOnRelatedClass() ? true : !this.OwningBOHasForeignKey; }
        }

        /// <summary>
        /// The relationship name of the Relationship on the Related class that is the 
        /// reverse relationship of this relationship.
        /// </summary>
        public virtual string ReverseRelationshipName
        {
            get { return this.PropertyWrapper.GetSingleReverseRelationshipName<AutoMapOneToOneAttribute>(); }
        }

        /// <summary>
        /// Create a RelPropDef based on the OwningPrropName and the RelatedPropName.
        /// </summary>
        /// <returns></returns>
        public IRelPropDef CreateRelPropDef()
        {
            var relPropDef = new RelPropDef(this.GetOwningPropName(), this.GetRelatedPropName());
            return relPropDef;
        }

        private void CheckReverseRelationshipValid()
        {
            if (this.PropertyWrapper.HasMoreThanOneToOneReverseRelationship)
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

    /// <summary>
    /// Provides extension methods to provide a more fluent mechanism for 
    /// determining a RelationshipDef from the Property.
    /// </summary>
    public static class OneToOneAutoMapperExtensions
    {
        ///<summary>
        /// Creates the <see cref="IRelationshipDef"/> from the PropertyWrapper.
        ///</summary>
        ///<param name="propWrapper"></param>
        ///<returns></returns>
        public static IRelationshipDef MapOneToOne(this PropertyWrapper propWrapper)
        {
            if (propWrapper == null) return null;
            var autoMapper = new OneToOneAutoMapper(propWrapper);
            return autoMapper.MapOneToOne();
        }
    }

    // ReSharper restore ClassWithVirtualMembersNeverInherited.Global
}