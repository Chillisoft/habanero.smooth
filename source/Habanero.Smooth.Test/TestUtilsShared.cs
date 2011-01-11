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
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.Smooth.ReflectionWrappers;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Smooth.Test
{
    public static class TestUtilsShared
    {
        private static Random _randomGenerator;

        private static Random Random
        {
            get { return _randomGenerator ?? (_randomGenerator = new Random()); }
        }

        public static int GetRandomInt()
        {
            return GetRandomInt(100000);
        }

        public static int GetRandomInt(int max)
        {
            return Random.Next(0, max);
        }

        public static int GetRandomInt(int min, int max)
        {
            return Random.Next(min, max);
        }

        public static string GetRandomString()
        {
            return GetRandomInt().ToString();
        }

        public static string GetRandomString(int maxLength)
        {
            if (maxLength > 9) maxLength = 9;
            int max = Convert.ToInt32(Math.Pow(10, maxLength)) - 1;
            return GetRandomInt(max).ToString();
        }

        public static string GetRandomString(int minLength, int maxLength)
        {
            if (minLength > 9) minLength = 9;
            int min = Convert.ToInt32(Math.Pow(10, minLength)) - 1;
            if (maxLength > 9) maxLength = 9;
            int max = Convert.ToInt32(Math.Pow(10, maxLength)) - 1;
            return GetRandomInt(min, max).ToString();
        }

        public static bool GetRandomBoolean()
        {
            return (GetRandomInt(100000) > 50000);
        }

        public static DateTime GetRandomDate()
        {
            return DateTime.Now;
        }

        public static TimeSpan GetRandomTime()
        {
            return DateTime.Now.TimeOfDay;
        }

        public static DateTime GetRandomDate(string max)
        {
            string start = DateTime.MinValue.ToString("yyyy/MM/dd");
            return GetRandomDate(start, max);
        }

        public static DateTime GetRandomDate(string min, string max)
        {
            DateTime start = DateTime.Parse(min);

            int range = (DateTime.Parse(max) - start).Days;
            return start.AddDays(GetRandomInt(range));
        }

        /// <summary>
        /// Takes a lookup list generated by Habanero and randomly selects a value
        /// from the list
        /// </summary>
        public static object GetRandomLookupListValue(Dictionary<string, object> lookupList)
        {
            object[] values = new object[lookupList.Count];
            lookupList.Values.CopyTo(values, 0);
            return values[GetRandomInt(0, values.Length - 1)];
        }

        /// <summary>
        /// Waits for the garbage collector to clear dereferenced objects in order
        /// to ensure accurate testing
        /// </summary>
        public static void WaitForGC()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static TEnum GetRandomEnum<TEnum>()
           where TEnum : struct
        {
            return GetRandomEnum<TEnum>(null);
        }

        public static TEnum GetRandomEnum<TEnum>(TEnum? excluded)
            where TEnum : struct
        {
            Array values = Enum.GetValues(typeof(TEnum));
            int randomIndex = GetRandomInt(0, values.Length);
            TEnum value = (TEnum)values.GetValue(randomIndex);
            if (excluded.HasValue && excluded.Value.Equals(value))
            {
                return GetRandomEnum(excluded);
            }
            return value;
        }

        public static T AssertIsInstanceOf<T>(object obj)
        {
            Assert.IsInstanceOf(typeof(T), obj);
            return (T)obj;
        }

        public static void AssertIsMultipleRelationship(this PropertyInfo propertyInfo)
        {
            propertyInfo.AssertIsOfType<IBusinessObjectCollection>();
        }
        public static void AssertIsSingleRelationship(this PropertyInfo propertyInfo)
        {
            propertyInfo.AssertIsOfType<IBusinessObject>();
        }

        public static void AssertIsOfType(this PropertyInfo propertyinfo, Type parentClassType)
        {
            propertyinfo.PropertyType.AssertIsOfType(parentClassType);
        }
        public static void AssertIsOfType<T>(this PropertyInfo propertyInfo)
        {
            propertyInfo.PropertyType.AssertIsOfType<T>();
        }
        public static void AssertIsNotOfType<T>(this PropertyInfo propertyInfo)
        {
            propertyInfo.PropertyType.AssertIsNotOfType<T>();
        }


        public static void AssertIsMultipleRelationship(this PropertyWrapper propWrapper)
        {
            propWrapper.AssertIsOfType<IBusinessObjectCollection>();
        }
        public static void AssertIsSingleRelationship(this PropertyWrapper propWrapper)
        {
            propWrapper.AssertIsOfType<IBusinessObject>();
        }

        public static void AssertIsOfType(this PropertyWrapper propWrapper, Type parentClassType)
        {
            propWrapper.PropertyType.AssertIsOfType(parentClassType);
        }
        public static void AssertIsOfType<T>(this PropertyWrapper propWrapper)
        {
            propWrapper.PropertyType.AssertIsOfType<T>();
        }
        public static void AssertIsNotOfType<T>(this PropertyWrapper propWrapper)
        {
            propWrapper.PropertyType.AssertIsNotOfType<T>();
        }

        public static void AssertPropertyNotExists(this TypeWrapper classType, string propName)
        {
            Assert.IsNull(classType.GetProperty(propName), propName + " should exist on the Type " + classType);
        }
        public static void AssertPropertyExists(this Type classType, string propName)
        {
            Assert.IsNotNull(classType.GetProperty(propName), propName + " should exist on the Type " + classType);
        }
        public static void AssertPropertyExists(this TypeWrapper classType, string propName)
        {
            Assert.IsNotNull(classType.GetProperty(propName), propName + " should exist on the Type " + classType);
        }
 
        public static void AssertIsOfType<T>(this Type type)
        {
            Assert.IsTrue(typeof(T).IsAssignableFrom(type), type + " is not assignable from " + typeof(T));
        }
        public static void AssertIsOfType(this Type type, Type parentType)
        {
            Assert.IsTrue(parentType.IsAssignableFrom(type), type + " is not of type " + parentType.ToString());
        }
        public static void AssertIsOfType(this TypeWrapper type, Type parentType)
        {
            Assert.IsTrue(type.IsOfType(parentType), type + " is not of type " + parentType.ToString());
        }
        public static void AssertIsOfType<T>(this TypeWrapper type)
        {
            Assert.IsTrue(type.IsOfType<T>(), type + " is not assignable from " + typeof(T));
        }
        public static void AssertIsNotOfType<T>(this Type type)
        {
            Assert.IsFalse(typeof(T).IsAssignableFrom(type));
        } 
        public static void AssertIsNotOfType<T>(this TypeWrapper type)
        {
            Assert.IsFalse(type.IsOfType<T>());
        }


        public static PropertyInfo GetMockPropInfoWithNoAutoMapProp<TAutoMapAttribute>()
        {
            PropertyInfo propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.Stub(propInfo => propInfo.GetCustomAttributes(true)).Return(new object[0]);
            propertyInfo.Stub(propInfo => propInfo.GetCustomAttributes(typeof(TAutoMapAttribute), true)).Return(propertyInfo.GetCustomAttributes(true));
            return propertyInfo;
        }
        public static PropertyInfo GetMockPropInfoWithAutoMapAttribute<TAutoMapAttribute>(string reverseRelName)
        {
            PropertyInfo propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.Stub(propInfo => propInfo.GetCustomAttributes(true)).Return(new[]
                                     {
                                         Activator.CreateInstance(typeof(TAutoMapAttribute), reverseRelName)
                                     });
            propertyInfo.Stub(propInfo => propInfo.GetCustomAttributes(typeof(TAutoMapAttribute), true)).Return(propertyInfo.GetCustomAttributes(true));

            propertyInfo.Stub(info1 => info1.PropertyType).Return(typeof(FakeBONoPK));
            return propertyInfo;
        }
        public static PropertyInfo GetMockPropInfoWithAutoMapAttribute<TAutoMapAttribute>()
        {
            PropertyInfo propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.Stub(propInfo => propInfo.GetCustomAttributes(true)).Return(new[]
                                     {
                                         Activator.CreateInstance(typeof(TAutoMapAttribute))
                                     });
            propertyInfo.Stub(propInfo => propInfo.GetCustomAttributes(typeof(TAutoMapAttribute), true)).Return(propertyInfo.GetCustomAttributes(true));

            propertyInfo.Stub(info1 => info1.PropertyType).Return(typeof(FakeBONoPK));
            return propertyInfo;
        }

        public static void AssertHasAutoMapToReverseRelationship(this Type classType, string relName, string reverseRelName)
        {
            var relProp = classType.GetProperty(relName);
            AssertHasAutoMapReverseRelationship(relProp, reverseRelName);
        }

        public static void AssertHasAutoMapReverseRelationship(PropertyWrapper relProp, string reverseRelName)
        {
            var customRelationship = relProp.GetAttributes<AutoMapRelationshipAttribute>();
            Assert.IsTrue(customRelationship.Any(o => (o.ReverseRelationshipName == reverseRelName)),
                          relProp.Name + " on " + relProp.DeclaringClassName + " should have an AutoMapRelationshipAttribute attribute");
        }
        public static void AssertHasAttribute<TRelAttribute>(this PropertyWrapper relProp, Func<TRelAttribute, bool> expr) where TRelAttribute : AutoMapRelationshipAttribute
        {
            var customRelationship = relProp.GetAttributes<TRelAttribute>();
            Assert.IsTrue(customRelationship.Any(expr),
                          relProp.Name + " on " + relProp.DeclaringClassName + " should have an AutoMapRelationshipAttribute attribute matching ???");

        }

        public static void AssertHasAutoMapReverseRelationship(PropertyInfo relProp, string reverseRelName)
        {
            PropertyWrapper propertyWrapper = relProp.ToPropertyWrapper();
            var customRelationship = propertyWrapper.GetAttributes<AutoMapRelationshipAttribute>();
            Assert.IsTrue(customRelationship.Any(o => (o.ReverseRelationshipName == reverseRelName)),
                          relProp.Name + " on " + relProp.ToPropertyWrapper().DeclaringClassName + " should have an AutoMapRelationshipAttribute attribute");
        }

        public static void AssertHasAttribute<TRelAttribute>(this PropertyInfo relProp, Func<TRelAttribute, bool> expr) where TRelAttribute : AutoMapRelationshipAttribute
        {
            PropertyWrapper propertyWrapper = relProp.ToPropertyWrapper();
            var customRelationship = propertyWrapper.GetAttributes<TRelAttribute>();
            Assert.IsTrue(customRelationship.Any(expr),
                          relProp.Name + " on " + relProp.ToPropertyWrapper().DeclaringClassName + " should have an AutoMapRelationshipAttribute attribute matching ???");
 
        }

        public static void AssertHasOneToOneWithReverseRelationship(this PropertyWrapper propWrapper, string expectedRevRelName)
        {
            Assert.IsTrue(propWrapper.HasAutoMapOneToOneAttribute(expectedRevRelName), string.Format("Should have AutoMapOneToOne with ReverseRelationship '{0}'", expectedRevRelName));
        }


        public static void AssertHasAttribute<T>(this PropertyInfo propertyInfo) where T : class
        {
            PropertyWrapper propertyWrapper = propertyInfo.ToPropertyWrapper();
            Assert.IsTrue(propertyWrapper.HasAttribute<T>()
                    , propertyWrapper.Name + string.Format(" does not have a '{0}' attribute", typeof(T)));

        }
        public static void AssertHasAttribute<T>(this PropertyWrapper propWrapper) where T : class
        {
            Assert.IsTrue(propWrapper.HasAttribute<T>()
                    , propWrapper.Name + string.Format(" does not have a '{0}' attribute", typeof(T)));

        }
        public static void AssertHasIgnoreAttribute(this PropertyWrapper propWrapper)
        {
            propWrapper.AssertHasAttribute<AutoMapIgnoreAttribute>();
        }
        public static void AssertHasIgnoreAttribute(this PropertyInfo propertyInfo)
        {
            propertyInfo.AssertHasAttribute<AutoMapIgnoreAttribute>();
        }
    }
}