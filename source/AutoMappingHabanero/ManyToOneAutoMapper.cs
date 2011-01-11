using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMappingHabanero.ReflectionWrappers;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
namespace AutoMappingHabanero
{
    public static class ManyToOneAutoMapperExtensions
    {
        public static IRelationshipDef MapManyToOne(this PropertyInfo propInfo)
        {
            if (propInfo == null) return null;

            ManyToOneAutoMapper autoMapper = new ManyToOneAutoMapper(propInfo);
            return autoMapper.MapManyToOne();
        }
        public static IRelationshipDef MapManyToOne(this PropertyWrapper propWrapper)
        {
            if (propWrapper == null) return null;

            ManyToOneAutoMapper autoMapper = new ManyToOneAutoMapper(propWrapper);
            return autoMapper.MapManyToOne();
        }

    }
    public class ManyToOneAutoMapper
    {
        private PropertyWrapper PropertyWrapper { get; set; }

        public ManyToOneAutoMapper(PropertyInfo propInfo):this(propInfo.ToPropertyWrapper())
        {

        }
        public ManyToOneAutoMapper(PropertyWrapper propWrapper)
        {
            if (propWrapper == null) throw new ArgumentNullException("propWrapper");
            this.PropertyWrapper = propWrapper;
        }

        public IRelationshipDef MapManyToOne()
        {
            if (this.PropertyWrapper.DeclaringType == (Type)null) return null;
            if (!MustBeMapped()) return null;

            var propertyType = this.PropertyWrapper.PropertyType;
            SingleRelationshipDef relDef
                        = new SingleRelationshipDef(this.PropertyWrapper.Name, propertyType.GetUnderlyingType()
                        , new RelKeyDef(), true, DeleteParentAction.DoNothing);
            if(this.PropertyWrapper.HasCompulsoryAttribute) relDef.SetAsCompulsory();
            relDef.OwningBOHasForeignKey = true;
            SetReverseRelationshipName(relDef);
            var ownerPropName = GetOwningPropName();
            var relatedPropName = GetRelatedPropName(propertyType);
            IRelPropDef relPropDef = new RelPropDef(ownerPropName, relatedPropName);
            relDef.RelKeyDef.Add(relPropDef);
            return relDef;
        }

        private  void SetReverseRelationshipName(IRelationshipDef relDef)
        {
            IEnumerable<AutoMapManyToOneAttribute> attributes = this.PropertyWrapper.GetAttributes<AutoMapManyToOneAttribute>();
            AutoMapManyToOneAttribute mToOneAttribute = attributes.FirstOrDefault();
            if (mToOneAttribute != null && !string.IsNullOrEmpty(mToOneAttribute.ReverseRelationshipName))
            {
                relDef.ReverseRelationshipName = mToOneAttribute.ReverseRelationshipName;
                return;
            }
            var reverseRelPropInfo = this.PropertyWrapper.GetMultipleReverseRelPropInfo();
            if (reverseRelPropInfo != null)
            {
                relDef.ReverseRelationshipName = reverseRelPropInfo.Name;
                return;
            }
            relDef.ReverseRelationshipName = StringUtilities.Pluralize(this.PropertyWrapper.DeclaringClassName);
        }

        public static string GetRelatedPropName(TypeWrapper propertyType)
        {
            return propertyType.GetPKPropName();
        }

        private  string GetOwningPropName()
        {
            return PropNamingConvention.GetSingleRelOwningPropName(this.PropertyWrapper.Name);
        }

        public bool MustBeMapped()
        {

            if (this.PropertyWrapper.IsStatic) return false;
            if (!this.PropertyWrapper.IsPublic) return false;
            if (this.PropertyWrapper.IsInheritedProp) return false;
            if (this.PropertyWrapper.HasMultipleReverseRelationship
                    && this.PropertyWrapper.HasSingleReverseRelationship
                    && !this.PropertyWrapper.HasOneToOneAttribute
                    && !this.PropertyWrapper.HasManyToOneAttribute)
            {
                throw new InvalidDefinitionException("The Relationship '" + this.PropertyWrapper.Name
                        + "' could not be automapped since there are multiple relationships on class '"
                        + this.PropertyWrapper.PropertyType.Name + "' that reference the BusinessObject Class '"
                        + this.PropertyWrapper.DeclaringClassName + "'. Please map using ClassDef.XML or Attributes");
            }
            if (this.PropertyWrapper.HasManyToOneAttribute) return true;

            if (!this.PropertyWrapper.IsSingleRelationhip
                    || this.PropertyWrapper.HasIgnoreAttribute
                    || this.PropertyWrapper.HasOneToOneAttribute)
            {
                return false;
            }
            return !this.PropertyWrapper.HasSingleReverseRelationship;
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