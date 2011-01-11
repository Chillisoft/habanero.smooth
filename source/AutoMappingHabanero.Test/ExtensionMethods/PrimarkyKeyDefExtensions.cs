using System;
using System.Linq;
using System.Reflection;
using AutoMappingHabanero.ReflectionWrappers;
using Habanero.Base;
using NUnit.Framework;

namespace AutoMappingHabanero.Test.ExtensionMethods
{
    public static class PrimaryKeyExtensions
    {
        public static void AssertHasOneGuidProp(this IPrimaryKeyDef primaryKey)
        {
            Assert.IsNotNull(primaryKey, "primary Key Should not be null");
            Assert.AreEqual(1, primaryKey.Count, "Should have one prop");
            Assert.IsFalse(primaryKey.IsCompositeKey, "should not be composite");
            Assert.IsTrue(primaryKey.IsGuidObjectID, "Should be object ID");
            IPropDef pkProp = primaryKey[0];
            Assert.AreEqual(PropReadWriteRule.WriteNew, pkProp.ReadWriteRule);
            Assert.IsTrue(pkProp.Compulsory);
            Assert.AreEqual(typeof(Guid), pkProp.PropertyType);
        }
        public static bool HasPrimaryKeyAttribute(this IClassDef cDef, string propName)
        {
            var propertyInfo = cDef.ClassType.GetPropertyWrapper(propName);
            return propertyInfo.HasAttribute<AutoMapPrimaryKeyAttribute>();
        }
        public static bool HasPrimaryKeyAttribute(this Type classType, string propName)
        {
            var propertyInfo = classType.GetPropertyWrapper(propName);
            return propertyInfo.HasAttribute<AutoMapPrimaryKeyAttribute>();
        }


        public static bool HasReverseRelationshipOfType<T>(this Type relatedClassType)
        {
            return relatedClassType.GetProperties().Any(propInfo => relatedClassType.IsAssignableFrom(typeof (T)));
        }


        public static string GetOwningPropName(this IRelationshipDef relationshipDef)
        {
            return new DefaultPropNamingConventions().GetSingleRelOwningPropName(relationshipDef.RelationshipName);
        }



    }
}


