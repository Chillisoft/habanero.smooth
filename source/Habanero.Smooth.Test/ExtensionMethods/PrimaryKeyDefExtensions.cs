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
using System.Linq;
using System.Reflection;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Base;
using NUnit.Framework;

namespace Habanero.Smooth.Test.ExtensionMethods
{
    public static class PrimaryKeyExtensions
    {
        public static void AssertHasOneGuidProp(this IPrimaryKeyDef primaryKey)
        {
            Assert.IsNotNull(primaryKey, "primary Key Should not be null");
            Assert.AreEqual(1, primaryKey.Count, "Should have one prop");
            Assert.IsFalse(primaryKey.IsCompositeKey, "should not be composite");
            Assert.IsTrue(primaryKey.IsGuidObjectID, "Should be object ID");
            var pkProp = primaryKey[0];
            Assert.AreEqual(PropReadWriteRule.WriteNew, pkProp.ReadWriteRule);
            Assert.IsTrue(pkProp.Compulsory);
            Assert.AreEqual(typeof(Guid), pkProp.PropertyType);
        }
        public static void AssertHasOneIntProp(this IPrimaryKeyDef primaryKey)
        {
            Assert.IsNotNull(primaryKey, "primary Key Should not be null");
            Assert.AreEqual(1, primaryKey.Count, "Should have one prop");
            Assert.IsFalse(primaryKey.IsCompositeKey, "should not be composite");
            Assert.IsFalse(primaryKey.IsGuidObjectID, "Should be object ID");
            var pkProp = primaryKey[0];
            Assert.AreEqual(PropReadWriteRule.WriteNew, pkProp.ReadWriteRule);
            Assert.IsTrue(pkProp.Compulsory);
            Assert.AreEqual(typeof(int), pkProp.PropertyType);
        }
        public static void AssertNotObjectID(this IPrimaryKeyDef primaryKey)
        {
            Assert.IsNotNull(primaryKey, "primary Key Should not be null");
            Assert.AreEqual(1, primaryKey.Count, "Should have one prop");
            Assert.IsFalse(primaryKey.IsCompositeKey, "should not be composite");
            Assert.IsFalse(primaryKey.IsGuidObjectID, "Should be object ID");
        }
        public static bool HasPrimaryKeyAttribute(this IClassDef cDef, string propName)
        {
            var propertyInfo = cDef.ClassType.GetPropertyWrapper(propName);
            return propertyInfo.HasAttribute<AutoMapPrimaryKeyAttribute>();
        }
        public static bool HasUniqueConstraintAttribute(this IClassDef cDef, string propName)
        {
            var propertyInfo = cDef.ClassType.GetPropertyWrapper(propName);
            return propertyInfo.HasAttribute<AutoMapUniqueConstraintAttribute>();
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


