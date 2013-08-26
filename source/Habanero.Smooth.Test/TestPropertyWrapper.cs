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
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Smooth.Test.ValidFakeBOs;
using NUnit.Framework;
using Rhino.Mocks;
// ReSharper disable InconsistentNaming
namespace Habanero.Smooth.Test
{
    [TestFixture]
    public class TestPropertyWrapper
    {
        [SetUp]
        public void Setup()
        {
            AllClassesAutoMapper.PropNamingConvention = new DefaultPropNamingConventions();
        }

        #region WrapperProps

        [Test]
        public void Test_Name_ShouldReturnInfoName()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "ExpectedName";
            var propertyInfo = new FakePropertyInfo(expectedPropName, typeof (FakeEnum));
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedPropName, propertyInfo.Name);
            //---------------Execute Test ----------------------
            var actualName = propertyWrapper.Name;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedPropName, actualName);
        }

        [Test]
        public void Test_PropType_ShouldReturnPropInfoType()
        {
            //---------------Set up test pack-------------------
            var expectedType = typeof (FakeEnum).ToTypeWrapper();
            var propertyInfo = new FakePropertyInfo("fdafads", expectedType.UnderlyingType);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.AreSame(expectedType.UnderlyingType, propertyInfo.PropertyType);
            //---------------Execute Test ----------------------
            var actualType = propertyWrapper.PropertyType;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedType, actualType);
        }

        [Test]
        public void Test_PropertyInfo_ShouldReturnPropInfo()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = new FakePropertyInfo(typeof (FakeEnum));
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var propInfo = propertyWrapper.PropertyInfo;
            //---------------Test Result -----------------------
            Assert.AreEqual(propertyInfo, propInfo);
        }

        [Test]
        public void Test_DeclaringType_ShouldReturnDeclaringTypeWrapped()
        {
            //---------------Set up test pack-------------------
            var expectedDeclaringType = typeof (FakeBoNoProps);
            var propertyInfo = new FakePropertyInfo(expectedDeclaringType);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.AreSame(expectedDeclaringType, propertyInfo.DeclaringType);
            //---------------Execute Test ----------------------
            var declaringType = propertyWrapper.DeclaringType;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedDeclaringType, declaringType.UnderlyingType);
        }

        [Test]
        public void Test_DeclaringTypeName_ShouldReturnUndelyingTypeName()
        {
            //---------------Set up test pack-------------------
            TypeWrapper declaringType = MockRepository.GenerateMock<FakeTypeWrapper>();
            var expectedTypeName = RandomValueGenerator.GetRandomString();
            declaringType.SetName(expectedTypeName);
            PropertyWrapper propertyWrapper = new FakePropertyWrapper(declaringType);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyWrapper.DeclaringType);
            //---------------Execute Test ----------------------
            var declaringTypeName = propertyWrapper.DeclaringClassName;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedTypeName, declaringTypeName);
        }
        [Test]
        public void Test_AssemblyQualifiedName_ShouldReturnUndelyingAssemblyName()
        {
            //---------------Set up test pack-------------------
            TypeWrapper declaringType = MockRepository.GenerateMock<FakeTypeWrapper>();
            var expectedTypeName = RandomValueGenerator.GetRandomString();
            declaringType.SetAssemblyName(expectedTypeName);
            PropertyWrapper propertyWrapper = new FakePropertyWrapper(declaringType);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyWrapper.DeclaringType);
            //---------------Execute Test ----------------------
            var declaringTypeName = propertyWrapper.AssemblyQualifiedName;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedTypeName, declaringTypeName);
        }

        [Test]
        public void Test_RelatedClassName_ShouldReturnUndelyingTypeName()
        {
            //---------------Set up test pack-------------------
            TypeWrapper returnType = MockRepository.GenerateMock<FakeTypeWrapper>();
            var expectedReturnTypeName = RandomValueGenerator.GetRandomString();
            returnType.SetName(expectedReturnTypeName);
            var propertyWrapper = new FakePropertyWrapper {MyRelatedType = returnType};
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyWrapper.RelatedClassType);
            //---------------Execute Test ----------------------
            var relatedClassName = propertyWrapper.RelatedClassName;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedReturnTypeName, relatedClassName);
        }

        [Test]
        public void Test_RelatedClassType_WhenNotGenericShouldReturnPropType()
        {
            //---------------Set up test pack-------------------
            var returnType = MockRepository.GenerateMock<Type>();
            var info = new FakePropertyInfo("fdafasd", returnType);
            var propertyWrapper = new PropertyWrapper(info);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyWrapper.RelatedClassType);
            Assert.IsFalse(returnType.IsGenericType);
            //---------------Execute Test ----------------------
            var relatedClassType = propertyWrapper.RelatedClassType;
            //---------------Test Result -----------------------
            Assert.AreSame(returnType, relatedClassType.UnderlyingType);
        }
        [Test]
        public void Test_RelatedClassType_WhenIsGenericShouldReturnUnderlyingPropType()
        {
            //---------------Set up test pack-------------------
            var underlying = new TypeWrapper(typeof(FakeBOWProps));
            var genericReturnType = underlying.MakeGenericBusinessObjectCollection();
            var info = new FakePropertyInfo("fdafasd", genericReturnType.UnderlyingType);
            var propertyWrapper = new PropertyWrapper(info);
            
            //---------------Assert Precondition----------------
            Assert.IsTrue(genericReturnType.IsGenericType);
            //---------------Execute Test ----------------------
            var relatedClassType = propertyWrapper.RelatedClassType;
            //---------------Test Result -----------------------
            Assert.AreSame(underlying.UnderlyingType, relatedClassType.UnderlyingType);
        }
        [Test]
        public void Test_IsSingleRel_WhenIsGenericShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var underlying = new TypeWrapper(typeof(FakeBOWProps));
            var genericReturnType = underlying.MakeGenericBusinessObjectCollection();
            var info = new FakePropertyInfo("fdafasd", genericReturnType.UnderlyingType);
            var propertyWrapper = new PropertyWrapper(info);
            
            //---------------Assert Precondition----------------
            Assert.IsTrue(genericReturnType.IsGenericType);
            //---------------Execute Test ----------------------
            var isSingleRelationhip = propertyWrapper.IsSingleRelationhip;
            //---------------Test Result -----------------------
            Assert.IsFalse(isSingleRelationhip);
        }

        [Test]
        public void Test_IsSingleRel_WhenIsStringShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var info = new FakePropertyInfo("fdafasd", typeof(string));
            var propertyWrapper = new PropertyWrapper(info);
            
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var isSingleRelationhip = propertyWrapper.IsSingleRelationhip;
            //---------------Test Result -----------------------
            Assert.IsFalse(isSingleRelationhip);
        }
        [Test]
        public void Test_IsSingleRel_WhenIsBOShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var info = new FakePropertyInfo("fdafasd", typeof(FakeBOWProps));
            var propertyWrapper = new PropertyWrapper(info);
            
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var isSingleRelationhip = propertyWrapper.IsSingleRelationhip;
            //---------------Test Result -----------------------
            Assert.IsTrue(isSingleRelationhip);
        }

        [Test]
        public void Test_IsMultipleRel_WhenIsGenericShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var underlying = new TypeWrapper(typeof(FakeBOWProps));
            var genericReturnType = underlying.MakeGenericBusinessObjectCollection();
            var info = new FakePropertyInfo("fdafasd", genericReturnType.UnderlyingType);
            var propertyWrapper = new PropertyWrapper(info);

            //---------------Assert Precondition----------------
            Assert.IsTrue(genericReturnType.IsGenericType);
            //---------------Execute Test ----------------------
            var isMultipleRel = propertyWrapper.IsMultipleRelationship;
            //---------------Test Result -----------------------
            Assert.IsTrue(isMultipleRel);
        }

        [Test]
        public void Test_IsMultipleRel_WhenIsStringShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var info = new FakePropertyInfo("fdafasd", typeof(string));
            var propertyWrapper = new PropertyWrapper(info);

            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var isMultipleRel = propertyWrapper.IsMultipleRelationship;
            //---------------Test Result -----------------------
            Assert.IsFalse(isMultipleRel);
        }
        [Test]
        public void Test_IsMultipleRel_WhenIsBOShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var info = new FakePropertyInfo("fdafasd", typeof(FakeBOWProps));
            var propertyWrapper = new PropertyWrapper(info);

            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var isMultipleRel = propertyWrapper.IsMultipleRelationship;
            //---------------Test Result -----------------------
            Assert.IsFalse(isMultipleRel);
        }

        [Test]
        public void Test_IsRel_WhenIsGenericShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var underlying = new TypeWrapper(typeof(FakeBOWProps));
            var genericReturnType = underlying.MakeGenericBusinessObjectCollection();
            var info = new FakePropertyInfo("fdafasd", genericReturnType.UnderlyingType);
            var propertyWrapper = new PropertyWrapper(info);

            //---------------Assert Precondition----------------
            Assert.IsTrue(genericReturnType.IsGenericType);
            //---------------Execute Test ----------------------
            var isMultipleRel = propertyWrapper.IsRelationship;
            //---------------Test Result -----------------------
            Assert.IsTrue(isMultipleRel);
        }

        [Test]
        public void Test_IsRel_WhenIsStringShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var info = new FakePropertyInfo("fdafasd", typeof(string));
            var propertyWrapper = new PropertyWrapper(info);

            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var isMultipleRel = propertyWrapper.IsRelationship;
            //---------------Test Result -----------------------
            Assert.IsFalse(isMultipleRel);
        }
        [Test]
        public void Test_IsRel_WhenIsBOShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var info = new FakePropertyInfo("fdafasd", typeof(FakeBOWProps));
            var propertyWrapper = new PropertyWrapper(info);

            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var isMultipleRel = propertyWrapper.IsRelationship;
            //---------------Test Result -----------------------
            Assert.IsTrue(isMultipleRel);
        }

        [Test]
        public void Test_HasIgnoreAttribute_WhenHas_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.SetCustomAttribute<AutoMapIgnoreAttribute>();
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof (AutoMapIgnoreAttribute), true);
            Assert.AreEqual(1, customAttributes.Count());
            //---------------Execute Test ----------------------
            var hasIgnoreAttribute = propertyWrapper.HasIgnoreAttribute;
            //---------------Test Result -----------------------
            Assert.IsTrue(hasIgnoreAttribute);
        }
        [Test]
        public void Test_HasIgnoreAttribute_WhenNotHas_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.ClearCustomAttributes<AutoMapIgnoreAttribute>();
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof (AutoMapIgnoreAttribute), true);
            Assert.AreEqual(0, customAttributes.Count());
            //---------------Execute Test ----------------------
            var hasIgnoreAttribute = propertyWrapper.HasIgnoreAttribute;
            //---------------Test Result -----------------------
            Assert.IsFalse(hasIgnoreAttribute);
        }

        [Test]
        public void Test_HasKeepValuePrivateAttribute_WhenHas_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.SetCustomAttribute<AutoMapKeepValuePrivate>();
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Execute Test ----------------------
            var hasAttribute = propertyWrapper.HasKeepValuePrivateAttribute;
            //---------------Test Result -----------------------
            Assert.IsTrue(hasAttribute);
        }

        [Test]
        public void Test_HasKeepValuePrivateAttribute_WhenNotHas_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.ClearCustomAttributes<AutoMapKeepValuePrivate>();
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Execute Test ----------------------
            var hasAttribute = propertyWrapper.HasKeepValuePrivateAttribute;
            //---------------Test Result -----------------------
            Assert.IsFalse(hasAttribute);
        }

        [Test]
        public void Test_HasCompulsoryAttribute_WhenHas_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.SetCustomAttribute<AutoMapCompulsoryAttribute>();
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapCompulsoryAttribute), true);
            Assert.AreEqual(1, customAttributes.Count());
            //---------------Execute Test ----------------------
            var hasCustomAttribute = propertyWrapper.HasCompulsoryAttribute;
            //---------------Test Result -----------------------
            Assert.IsTrue(hasCustomAttribute);
        }
 
        [Test]
        public void Test_HasCompulsoryAttribute_WhenNotHas_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.ClearCustomAttributes<AutoMapCompulsoryAttribute>();
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapCompulsoryAttribute), true);
            Assert.AreEqual(0, customAttributes.Count());
            //---------------Execute Test ----------------------
            var hasCustomAttribute = propertyWrapper.HasCompulsoryAttribute;
            //---------------Test Result -----------------------
            Assert.IsFalse(hasCustomAttribute);
        }
        [Test]
        public void TestIntegrate_HasCompulsoryAttribute_WhenHas_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithCompulsoryProp);
            var propertyInfo = classType.GetProperty("CompulsoryProp");
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapCompulsoryAttribute), true);
            Assert.AreEqual(1, customAttributes.Count());
            //---------------Execute Test ----------------------
            var hasCustomAttribute = propertyWrapper.HasCompulsoryAttribute;
            //---------------Test Result -----------------------
            Assert.IsTrue(hasCustomAttribute);
        }
        [Test]
        public void TestIntegrate_HasCompulsoryAttribute_WhenNotHas_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------

            var classType = typeof(FakeBOWithCompulsoryProp);
            var propertyInfo = classType.GetProperty("NonCompulsoryProp");
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapCompulsoryAttribute), true);
            Assert.AreEqual(0, customAttributes.Count());
            //---------------Execute Test ----------------------
            var hasCustomAttribute = propertyWrapper.HasCompulsoryAttribute;
            //---------------Test Result -----------------------
            Assert.IsFalse(hasCustomAttribute);
        }
        [Test]
        public void Test_HasManyToOneAttribute_WhenHas_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.SetCustomAttribute<AutoMapManyToOneAttribute>();
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapManyToOneAttribute), true);
            Assert.AreEqual(1, customAttributes.Count());
            //---------------Execute Test ----------------------
            var hasManyToOneAttribute = propertyWrapper.HasManyToOneAttribute;
            //---------------Test Result -----------------------
            Assert.IsTrue(hasManyToOneAttribute);
        }
        [Test]
        public void Test_HasManyToOneAttribute_WhenNotHas_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.ClearCustomAttributes<AutoMapManyToOneAttribute>();
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapManyToOneAttribute), true);
            Assert.AreEqual(0, customAttributes.Count());
            //---------------Execute Test ----------------------
            var hasManyToOneAttribute = propertyWrapper.HasManyToOneAttribute;
            //---------------Test Result -----------------------
            Assert.IsFalse(hasManyToOneAttribute);
        }
        [Test]
        public void Test_HasOneToOneAttribute_WhenHas_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.SetCustomAttribute<AutoMapOneToOneAttribute>();
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapOneToOneAttribute), true);
            Assert.AreEqual(1, customAttributes.Count());
            //---------------Execute Test ----------------------
            var hasOneToOneAttribute = propertyWrapper.HasOneToOneAttribute;
            //---------------Test Result -----------------------
            Assert.IsTrue(hasOneToOneAttribute);
        }
        [Test]
        public void Test_HasOneToOneAttribute_WhenNotHas_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.ClearCustomAttributes<AutoMapOneToOneAttribute>();
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapOneToOneAttribute), true);
            Assert.AreEqual(0, customAttributes.Count());
            //---------------Execute Test ----------------------
            var hasOneToOneAttribute = propertyWrapper.HasOneToOneAttribute;
            //---------------Test Result -----------------------
            Assert.IsFalse(hasOneToOneAttribute);
        }
        [Test]
        public void Test_HasAutoIncrementingAttribute_WhenHas_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.SetCustomAttribute<AutoMapAutoIncrementingAttribute>();
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapAutoIncrementingAttribute), true);
            Assert.AreEqual(1, customAttributes.Count());
            //---------------Execute Test ----------------------
            var hasAutoIncrementingAttribute = propertyWrapper.HasAutoIncrementingAttribute;
            //---------------Test Result -----------------------
            Assert.IsTrue(hasAutoIncrementingAttribute);
        }       

        [Test]
        public void Test_HasAutoIncrementingAttribute_WhenNotHas_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.ClearCustomAttributes<AutoMapAutoIncrementingAttribute>();
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapAutoIncrementingAttribute), true);
            Assert.AreEqual(0, customAttributes.Count());
            //---------------Execute Test ----------------------
            var hasAutoNumberAttribute = propertyWrapper.HasAutoIncrementingAttribute;
            //---------------Test Result -----------------------
            Assert.IsFalse(hasAutoNumberAttribute);
        }       
        
        [Test]
        public void Test_HasReadWriteRuleAttribute_WhenHas_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.SetCustomAttribute(new AutoMapReadWriteRuleAttribute(PropReadWriteRule.ReadOnly));
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapReadWriteRuleAttribute), true);
            Assert.AreEqual(1, customAttributes.Count());
            //---------------Execute Test ----------------------
            var hasAttribute = propertyWrapper.HasReadWriteRuleAttribute;
            //---------------Test Result -----------------------
            Assert.IsTrue(hasAttribute);
        }       

        [Test]
        public void Test_HasReadWriteRuleAttribute_WhenNotHas_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.ClearCustomAttributes<AutoMapReadWriteRuleAttribute>();
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapReadWriteRuleAttribute), true);
            Assert.AreEqual(0, customAttributes.Count());
            //---------------Execute Test ----------------------
            var hasAttribute = propertyWrapper.HasReadWriteRuleAttribute;
            //---------------Test Result -----------------------
            Assert.IsFalse(hasAttribute);
        }

        [Test]
        public void Test_HasUniqueConstraintAttribute_WhenHas_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.SetCustomAttribute(new AutoMapUniqueConstraintAttribute("UC1"));
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapUniqueConstraintAttribute), true);
            Assert.AreEqual(1, customAttributes.Count());
            //---------------Execute Test ----------------------
            var hasAttribute = propertyWrapper.HasUniqueConstraintAttribute;
            //---------------Test Result -----------------------
            Assert.IsTrue(hasAttribute);
        }       

        [Test]
        public void Test_HasUniqueConstraintAttribute_WhenNotHas_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.ClearCustomAttributes<AutoMapUniqueConstraintAttribute>();
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapUniqueConstraintAttribute), true);
            Assert.AreEqual(0, customAttributes.Count());
            //---------------Execute Test ----------------------
            var hasAttribute = propertyWrapper.HasUniqueConstraintAttribute;
            //---------------Test Result -----------------------
            Assert.IsFalse(hasAttribute);
        }       

     


        [Test]
        public void Test_GetAttributes_WhenHas_ShouldReturnOne()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.SetCustomAttribute<AutoMapIgnoreAttribute>();
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof (AutoMapIgnoreAttribute), true);
            Assert.AreEqual(1, customAttributes.Count());
            //---------------Execute Test ----------------------
            var attributes = propertyWrapper.GetAttributes<AutoMapIgnoreAttribute>();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, attributes.Count());
        }
        [Test]
        public void Test_GetAttributes_WhenNotHas_ShouldReturnNone()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
            propertyInfo.ClearCustomAttributes<AutoMapIgnoreAttribute>();
            var propertyWrapper = new PropertyWrapper(propertyInfo);
            //---------------Assert Precondition----------------
            var customAttributes = propertyInfo.GetCustomAttributes(typeof (AutoMapIgnoreAttribute), true);
            Assert.AreEqual(0, customAttributes.Count());
            //---------------Execute Test ----------------------
            var attributes = propertyWrapper.GetAttributes<AutoMapIgnoreAttribute>();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, attributes.Count());
        }

        [Test]
        public void Test_IsStatic_WhenIs_ShouldReturnTrue()
        {
            const string expectedPropName = "MySingleRelationship";
            var type = typeof(FakeBoWithStaticProperty);
            //---------------Assert Precondition----------------
            var propertyInfo = type.GetProperty(expectedPropName);
            var property = propertyInfo.ToPropertyWrapper();
            Assert.IsTrue(propertyInfo.GetGetMethod().IsStatic);
            //---------------Execute Test ----------------------
            var isStatic = property.IsStatic;
            //---------------Test Result -----------------------
            Assert.IsTrue(isStatic);
        }
        [Test]
        public void Test_IsStatic_WhenNot_ShouldReturnFalse()
        {
            const string expectedPropName = "MySingleRelationship1";
            var type = typeof(FakeBOWithAllTypesOfRel);
            //---------------Assert Precondition----------------
            var propertyInfo = type.GetProperty(expectedPropName);
            var property = propertyInfo.ToPropertyWrapper();
            Assert.IsFalse(propertyInfo.GetGetMethod().IsStatic);
            //---------------Execute Test ----------------------
            var isStatic = property.IsStatic;
            //---------------Test Result -----------------------
            Assert.IsFalse(isStatic);
        }
        [Test]
        public void Test_IsPublic_WhenPublicGet_ShouldReturnTrue()
        {
            const string expectedPropName = "PublicStringProp";
            var type = typeof(FakeBoWithPrivateProps);
            var propertyInfo = type.GetProperty(expectedPropName);
            var property = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo.GetGetMethod());
            Assert.IsTrue(propertyInfo.GetGetMethod().IsPublic);
            //---------------Execute Test ----------------------
            var isPublic = property.IsPublic;
            //---------------Test Result -----------------------
            Assert.IsTrue(isPublic);
        }
        [Test]
        public void Test_IsPublic_WhenPrivateGet_ShouldReturnFalse()
        {
            const string expectedPropName = "PrivateStringProp";
            var type = typeof(FakeBoWithPrivateProps);
            var propertyInfo = type.GetProperty(expectedPropName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            var property = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            //---------------Execute Test ----------------------
            var isPublic = property.IsPublic;
            //---------------Test Result -----------------------
            Assert.IsFalse(isPublic);
        }

        [Test]
        public void Test_IsPublic_WhenInternalGet_ShouldReturnFalse()
        {
            const string expectedPropName = "InternalStringProp";
            var type = typeof(FakeBoWithPrivateProps);
            var propertyInfo = type.GetProperty(expectedPropName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            var property = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            //---------------Execute Test ----------------------
            var isPublic = property.IsPublic;
            //---------------Test Result -----------------------
            Assert.IsFalse(isPublic);
        }

        [Test]
        public void Test_IsPublic_WhenProtectedGet_ShouldReturnFalse()
        {
            const string expectedPropName = "ProtectedStringProp";
            var type = typeof(FakeBoWithPrivateProps);
            var propertyInfo = type.GetProperty(expectedPropName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            var property = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            //---------------Execute Test ----------------------
            var isPublic = property.IsPublic;
            //---------------Test Result -----------------------
            Assert.IsFalse(isPublic);
        }
        #endregion

        #region OneToOneReverseRelationship

        [Test]
        public void Test_HasSingleReverseRelationship_WhenRevHasM_1Attr_ShouldBeFalse()
        {
            //FakeBOWithSingleAttributeDeclaredRevRel has a relationship 'MySingleRelationship'
            // that is mapped via an AutoMapOneToMany Attribute to 'FakeBOWithUndefinableSingleRel'
            // AttributeRevRelName relationship. Because it is a single Relationship
            // and is mapped via AutoMap Attribute as a ManyToOne this will always return
            // that there are no single reverse relationships event though 'SingleRel' would 
            // be found if it were not for the autoMapping Prop.
            //---------------Set up test pack-------------------
            const string expectedPropName = "MySingleRelationship";
            var type = typeof (FakeBOWithSingleAttributeDeclaredRevRel);
            //---------------Assert Precondition----------------
            var propertyInfo = type.GetProperty(expectedPropName);
            var property = propertyInfo.ToPropertyWrapper();
            Assert.IsTrue(property.IsSingleRelationhip);
            //---------------Execute Test ----------------------
            var hasSingleReverseRelationship = property.HasSingleReverseRelationship;
            //---------------Test Result -----------------------
            Assert.IsFalse(hasSingleReverseRelationship);
        }
        [Test]
        public void Test_HasSingleReverseRelationship_WhenGetSingleReverseRelPropInfoIsNull_ShouldBeFalse()
        {
            //FakeBOWithSingleAttributeDeclaredRevRel has a relationship 'MySingleRelationship'
            // that is mapped via an AutoMapOneToMany Attribute to 'FakeBOWithUndefinableSingleRel'
            // AttributeRevRelName relationship. Because it is a single Relationship
            // and is mapped via AutoMap Attribute as a ManyToOne this will always return
            // that there are no single reverse relationships event though 'SingleRel' would 
            // be found if it were not for the autoMapping Prop.
            //---------------Set up test pack-------------------
            PropertyInfo propInfo = new FakePropertyInfo();
            //---------------Assert Precondition----------------
            var property = propInfo.ToPropertyWrapper();
            Assert.IsNull(property.GetSingleReverseRelPropInfos());
            //---------------Execute Test ----------------------
            var hasSingleReverseRelationship = property.HasSingleReverseRelationship;
            //---------------Test Result -----------------------
            Assert.IsFalse(hasSingleReverseRelationship);
        }

        [Test]
        public void Test_HasSingleReverseRelationship_WhenHasAndOwnerSingle_ShouldBeTrue()
        {
            //---------------Set up test pack-------------------
            var classType = typeof (FakeBOWithReverseSingleRel).ToTypeWrapper();
            const string expectedPropName = "MySingleRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<FakeBOWithTwoSingleRel>();
            var reversePropInfo = reverseClassType.GetProperty("MyReverseSingleRel");
            Assert.IsNotNull(reversePropInfo);
            reversePropInfo.AssertIsOfType<FakeBOWithReverseSingleRel>();
            //---------------Execute Test ----------------------
            var hasSingleReverseRelationship = propertyInfo.HasSingleReverseRelationship;
            //---------------Test Result -----------------------
            Assert.IsTrue(hasSingleReverseRelationship);
        }

        [Test]
        public void Test_HasSingleReverseRelationship_WhenHasAndOwnerMultiple_ShouldBeTrue()
        {
            //'FakeBoWithMultipleRel' has relationship 'MyMultipleWithTwoSingleReverse'
            // that is related to 'FakeWithTwoSingleReverseRel' which has two 
            // single relationships that could be mapped back.
            var classType = typeof (FakeBoWithMultipleRel).ToTypeWrapper();
            const string expectedPropName = "MyMultipleWithTwoSingleReverse";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship1");
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship2");
            //---------------Execute Test ----------------------
            var hasSingleReverseRelationship = propertyInfo.HasSingleReverseRelationship;
            //---------------Test Result -----------------------
            Assert.IsTrue(hasSingleReverseRelationship);
        }

        //The GetOneToOneReverseRelationshipInfos for a SingleRelationship should return all Props for SingleRelationships 
        //      that that are for the ownerClass 
        //   ----Unless----
        //1- This Relationship is mapped as a ManyToOneAttribute Or ReverseRelationship MappedVia M:1
        //2 - ReverseRel directly mapped via a OneToOne attribute to this relationship.
        //Or 3 - This Relationship is mapped by a OnetoOne Attribute on reverse relationship.
        //Or 4 - This Relationship has an Ignore Attribute.
        //or 5 - The Reverse Relationship has an Ignore Attribute
        [Test]
        public void Test_Get1_1RevRels_WhenNotHas_ShouldNotHaveItems()
        {
            //You have a single Relationship that is referencing a class
            // that does not have any reverse single relationships back to
            // 'FakeBOWithSingleAttributeDeclaredRevRel'
            //---------------Set up test pack-------------------
            const string expectedPropName = "MySingleRelationship";
            var type = typeof (FakeBoWithNoSingleReverse);
            var propertyInfo = type.GetProperty(expectedPropName);
            var property = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            propertyInfo.AssertIsSingleRelationship();
            //---------------Execute Test ----------------------
            var singleRevRels = property.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

        [Test]
        public void Test_Get1_1RevRels_WhenRelHasManyToOneAttribute_ShouldNotHaveItems()
        {
            //You have a single Relationship that has a ManyToOne Attribute
            // Its reverse relationship can never be single.
            //---------------Set up test pack-------------------
            const string expectedPropName = "MySingleRelationshipRev";
            var type = typeof (FakeBOWithM1Attribute);
            var propertyInfo = type.GetProperty(expectedPropName);
            var property = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            propertyInfo.AssertIsSingleRelationship();
            property.HasAttribute<AutoMapManyToOneAttribute>();
            //---------------Execute Test ----------------------
            var singleRevRels = property.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

        [Test]
        public void Test_Get1_1RevRels_WhenReverseRelHasManyToOne_ShouldNotHaveItems()
        {
            //You have a single Relationship that has a ManyToOne Attribute
            // Its reverse relationship can never be single.
            //---------------Set up test pack-------------------
            const string expectedPropName = "MySingleRelationship";
            var ownerClassType = typeof (FakeBOWithReverseSingle).ToTypeWrapper();
            var propertyInfo = ownerClassType.GetProperty(expectedPropName);
            var property = propertyInfo;
            var reverseClassType = property.RelatedClassType;
            //---------------Assert Precondition----------------
            propertyInfo.AssertIsSingleRelationship();
            const string reveresRelationshipName = "MySingleRelationshipRev";
            AssertRelationshipIsForOwnerClass(ownerClassType, reverseClassType, reveresRelationshipName);
            var reversePropInfo = reverseClassType.GetProperty(reveresRelationshipName);
            reversePropInfo.HasAttribute<AutoMapManyToOneAttribute>();
            //---------------Execute Test ----------------------
            var singleRevRels = property.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

        [Test]
        public void Test_Get1_1RevRels_WhenHasOneSingleRev_ShouldReturnOneItem()
        {
            //'FakeBOWithReverseSingleRel' has a Relationship 'MySingleRelationship'
            // to 'FakeBOWithTwoSingleRelNoRevs' that has two single relationships.
            // but only 'MyReverseSingleRel' returns the owner Class.
            //---------------Set up test pack-------------------
            var classType = typeof (FakeBOWithReverseSingleRel).ToTypeWrapper();
            const string expectedPropName = "MySingleRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;

            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<FakeBOWithTwoSingleRel>();
            propertyInfo.AssertIsSingleRelationship();
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MyReverseSingleRel");
            AssertReverseRelationshipNotForOwnerClass(classType, reverseClassType, "MySingleRelationship");
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, singleRevRels.Count);
            Assert.AreEqual("MyReverseSingleRel", singleRevRels[0].Name);
        }

        [Test]
        public void Test_Get1_1RevRels_WhenAutoMapOneToOne_ShouldReturnOneItem()
        {
            //---------------Set up test pack-------------------
            var classType = typeof (FakeBOWithReverseSingle).ToTypeWrapper();
            const string expectedPropName = "MySingleRelationship2";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;

            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<FakeBOWith11Attribute>();
            propertyInfo.AssertIsSingleRelationship();
            propertyInfo.AssertHasAttribute<AutoMapOneToOneAttribute>();
            const string expectedRevRelName = "MySingleRevRelationship";
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, expectedRevRelName);
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship2");
            propertyInfo.AssertHasOneToOneWithReverseRelationship(expectedRevRelName);
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, singleRevRels.Count);
            Assert.AreEqual(expectedRevRelName, singleRevRels[0].Name);
        }

        [Test]
        public void Test_Get1_1RevRels_WhenReverseAutoMapOneToOne_ShouldReturnOneItem()
        {
            //'FakeBOWith11Attribute' has a Relationship 'MySingleRevRelationship'
            // to 'FakeBOWithReverseSingle' that has two single relationships back.
            // but 'MySingleRelationship2' has a OneToOneAttribute back to 'MySingleRevRelationship'
            //---------------Set up test pack-------------------
            var classType = typeof (FakeBOWith11Attribute).ToTypeWrapper();
            const string expectedPropName = "MySingleRevRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<FakeBOWithReverseSingle>();
            propertyInfo.AssertIsSingleRelationship();

            const string expectedRevRelName = "MySingleRelationship2";
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, expectedRevRelName);
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship2");
            var reversePropertyInfo = reverseClassType.GetProperty(expectedRevRelName);
            AssertHasOneToOneWithReverseRelationship(reversePropertyInfo, expectedPropName);
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, singleRevRels.Count);
            Assert.AreEqual(expectedRevRelName, singleRevRels[0].Name);
        }

        [Test]
        public void Test_Get1_1RevRels_WhenHasIgnore_ShouldReturnNoItems()
        {
            //---------------Set up test pack-------------------
            var classType = typeof (FakeBOWithReverseSingle).ToTypeWrapper();
            const string expectedPropName = "MySingleIgnorRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<FakeBOWith11Attribute>();
            propertyInfo.AssertIsSingleRelationship();
            propertyInfo.AssertHasIgnoreAttribute();
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRevRelationship");
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship2");

            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

        [Test]
        public void Test_Get1_1RevRels_WhenReverseHasIgnore_ShouldReturnNoItems()
        {
            //---------------Set up test pack-------------------
            var classType = typeof (FakeBOWithReverseSingle).ToTypeWrapper();
            const string expectedPropName = "MySingleWithReverseIgnore";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<FakeBOWithIgnoreAttribute>();
            propertyInfo.AssertIsSingleRelationship();

            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship1");
            AssertReverseRelationshipHasIgnoreAttribute(reverseClassType, "MySingleRelationship1");
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

        [Test]
        public void Test_HasMoreThanOneSingleReverseRel_WhenHas2_ShouldRetTrue()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBoWithMultipleRel);
            const string expectedPropName = "MySingleWithTwoSingleReverse";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, propertyInfo.ToPropertyWrapper().GetOneToOneReverseRelationshipInfos().Count);
            //---------------Execute Test ----------------------
            var moreThanOneSingleReverseRelationship = propertyWrapper.HasMoreThanOneToOneReverseRelationship;
            //---------------Test Result -----------------------
            Assert.IsTrue(moreThanOneSingleReverseRelationship);
        }
        [Test]
        public void Test_Get1_1RevRels_WhenHasMultipleReverse_NoAttributes_ShouldReturnBothPotentialReverseRels()
        {
            //---------------Set up test pack-------------------
            var classType = typeof (FakeBOWithReverseSingle).ToTypeWrapper();
            const string expectedPropName = "MySingleRelationship3";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<FakeBOWith11Attribute>();
            propertyInfo.AssertIsSingleRelationship();

            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRevRelationship");
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship2");
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, singleRevRels.Count);
        }

        [Test]
        public void Test_Get1_1RevRels_WhenIsMultiple_ShouldReturnNone()
        {
            var classType = typeof (FakeBoWithMultipleRel).ToTypeWrapper();
            const string expectedPropName = "MyMultipleWithTwoSingleReverse";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var relatedClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();

            AssertRelationshipIsForOwnerClass(classType, relatedClassType, "MySingleRelationship1");
            AssertRelationshipIsForOwnerClass(classType, relatedClassType, "MySingleRelationship2");
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }
        [Test]
        public void Test_HasMoreThanOneSingleRev_WhenHasNone_ShouldReturnFalse()
        {
            var classType = typeof(FakeBoWithMultipleRel).ToTypeWrapper();
            const string expectedPropName = "MyMultipleWithTwoSingleReverse";
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            propertyInfo.GetOneToOneReverseRelationshipInfos().ShouldBeEmpty();
            //---------------Execute Test ----------------------
            var moreThanOneSingleReverseRelationship = propertyInfo.HasMoreThanOneToOneReverseRelationship;
            //---------------Test Result -----------------------
            Assert.IsFalse(moreThanOneSingleReverseRelationship);
        }

        [Test]
        public void Test_HasMoreThanOneSingleRev__WhenHasOneItem_ShouldReturnFalse()
        {
            //'FakeBOWith11Attribute' has a Relationship 'MySingleRevRelationship'
            // to 'FakeBOWithReverseSingle' that has two single relationships back.
            // but 'MySingleRelationship2' has a OneToOneAttribute back to 'MySingleRevRelationship'
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWith11Attribute).ToTypeWrapper();
            const string expectedPropName = "MySingleRevRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            propertyInfo.GetOneToOneReverseRelationshipInfos().ShouldHaveCount(1);
            //---------------Execute Test ----------------------
            var moreThanOneSingleReverseRelationship = propertyInfo.HasMoreThanOneToOneReverseRelationship;
            //---------------Test Result -----------------------
            Assert.IsFalse(moreThanOneSingleReverseRelationship);
        }

        #endregion

        #region ManyToOneReverseRelationship

        //The GetManyToOneReverseRelationshipInfos for a Multiple should return ALL Props for SingleRelationships 
        //      that that are for this ownerClass 
        //   ----Unless----
        //1 - ReverseRel directly mapped via a ManyToOne Attribute on this Relationship.
        //Or 2 - This Relationship is mapped by a OneToMany Attribute on reverse relationship.
        //Or 3 - This Relationship has an Ignore Attribute.
        //or 4 - The Reverse Relationship has an Ignore Attribute
        [Test]
        public void Test_GetM1RevRels_WhenHasTwo_ShouldReturnBoth()
        {
            var classType = typeof (FakeBoWithMultipleRel).ToTypeWrapper();
            const string expectedPropName = "MyMultipleWithTwoSingleReverse";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var relatedClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();

            AssertRelationshipIsForOwnerClass(classType, relatedClassType, "MySingleRelationship1");
            AssertRelationshipIsForOwnerClass(classType, relatedClassType, "MySingleRelationship2");
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, singleRevRels.Count);
            singleRevRels.ShouldContain(info => info.Name == "MySingleRelationship1");
            singleRevRels.ShouldContain(info => info.Name == "MySingleRelationship2");
        }

        [Test]
        public void Test_GetM1RevRels_WhenNotHas_ShouldNotHaveItems()
        {
            var classType = typeof (FakeBoWithMultipleRel).ToTypeWrapper();
            const string expectedPropName = "MyMultipleRevRel";
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

        [Test]
        public void Test_GetM1RevRels_WhenOneRevIgnored_ShouldReturnOne()
        {
            //Have a relationship that references a class that has two reverse relationships
            // to this class but one of them has an ignore attribute.
            //Should return one relationship.
            //---------------Set up test pack-------------------
            const string expectedPropName = "MyMultipleWithTwoSingleReverseOneIgnore";
            var classType = typeof (FakeBoWithMultipleRel).ToTypeWrapper();
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;

            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship1");
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship2");
            AssertReverseRelationshipHasIgnoreAttribute(reverseClassType, "MySingleRelationship2");
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, singleRevRels.Count);
            singleRevRels.ShouldContain(info => info.Name == "MySingleRelationship1");
        }

        [Test]
        public void Test_GetSingleRevRels_WhenRevRelHasAutomapRel_ShouldReturnItemWithMappingRel()
        {
            //The Single Reverse Relationship has an Attribute mapping it to this relationship.
            //Even though there are other single reverse relationships to this relationship onle the one should be returned.
            var classType = typeof (FakeBoWithMultipleRel).ToTypeWrapper();

            const string expectedPropName = "MyMultipleReverseAutoMapped";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship1");
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship2");
            const string expectedReverseRel = "MySingleRelationship3";
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, expectedReverseRel);
            AssertReverseRelationshipHasAutoMapToThisRel(reverseClassType, expectedReverseRel, expectedPropName);
            var reversePropInfo = reverseClassType.GetProperty(expectedReverseRel);
            Assert.IsTrue(reversePropInfo.HasAutoMapManyToOneAttribute(expectedPropName),
                          "Should have automap attribute");
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, singleRevRels.Count);
            singleRevRels.ShouldContain(info => info.Name == expectedReverseRel);
        }

        [Test]
        public void Test_GetSingleRels_WhenRevRelHasOneToOneAtt_ShouldReturnNoItems()
        {
            //The Single Reverse Relationship has an Attribute mapping it to this relationship.
            //Even though there are other single reverse relationships to this relationship onle the one should be returned.
            var classType = typeof (FakeBoWithMultipleRel).ToTypeWrapper();

            const string expectedPropName = "ReverseHasAutoMapOneToOne";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship1");

            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

        [Test]
        public void Test_HasSingleReverseRelationship_WhenNotHas_ShouldBeFalse()
        {
            //'FakeBoWithMultipleRel' has relationship 'MyMultipleRevRel'
            // that is related to 'FakeBOWithUndefinableSingleRel' which does not
            // have any single relationships mapped back to this class.
            var classType = typeof (FakeBoWithMultipleRel).ToTypeWrapper();
            const string expectedPropName = "MyMultipleRevRel";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();
            AssertReverseRelationshipNotForOwnerClass(classType, reverseClassType, "MySingleRelationship");
            AssertReverseRelationshipNotForOwnerClass(classType, reverseClassType, "MySingleRelWithOneToOneAttribute");
            AssertReverseRelationshipNotForOwnerClass(classType, reverseClassType, "MySingleRelWithOneToManyAttribute");
            //---------------Execute Test ----------------------
            var hasSingleReverseRelationship = propertyInfo.HasSingleReverseRelationship;
            //---------------Test Result -----------------------
            Assert.IsFalse(hasSingleReverseRelationship);
        }

        [Test]
        public void Test_GetM1RevRels_WhenHasTwo_ThisRelIgnored_ShouldReturnNone()
        {
            var classType = typeof (FakeBoWithMultipleRel).ToTypeWrapper();
            const string expectedPropName = "MyMultipleWithTwoSingleReverseThisIgnore";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var relatedClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();

            AssertRelationshipIsForOwnerClass(classType, relatedClassType, "MySingleRelationship1");
            AssertRelationshipIsForOwnerClass(classType, relatedClassType, "MySingleRelationship2");
            propertyInfo.AssertHasIgnoreAttribute();
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

        [Test]
        public void Test_GetM1RevRels_WhenRelIsSingle_ShouldReturnNone()
        {
            //---------------Set up test pack-------------------
            var classType = typeof (FakeBOWithReverseSingle).ToTypeWrapper();
            const string expectedPropName = "MySingleRelationship3";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<FakeBOWith11Attribute>();
            propertyInfo.AssertIsSingleRelationship();

            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRevRelationship");
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship2");
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

        [Test]
        public void Test_GetM1RevRels_WhenHasTwoButRelHasOneToManyMapping_ShouldReturnMappedRel()
        {
            var classType = typeof (FakeBoWithMultipleRel).ToTypeWrapper();
            const string expectedPropName = "MyMultipleAutoMapWithTwoSingleReverse";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var relatedClassType = propertyInfo.RelatedClassType;
            const string expectedMappedReverseRel = "MySingleRelationship2";
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();

            AssertRelationshipIsForOwnerClass(classType, relatedClassType, "MySingleRelationship1");
            AssertRelationshipIsForOwnerClass(classType, relatedClassType, expectedMappedReverseRel);
            AssertReverseRelationshipHasAutoMapToThisRel(classType, expectedPropName, expectedMappedReverseRel);
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, singleRevRels.Count);
            singleRevRels.ShouldContain(info => info.Name == expectedMappedReverseRel);
        }

        [Test]
        public void Test_GetM1RevRels_WhenMappedToNonExistentReverse_ShouldReturnNoItems()
        {
            var classType = typeof (FakeBoWithMultipleRel).ToTypeWrapper();
            const string expectedPropName = "MultipleMappedToNonExistentReverse";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var relatedClassType = propertyInfo.RelatedClassType;
            const string expectedMappedReverseRel = "NonExistentReverseRel";
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();
            relatedClassType.AssertPropertyNotExists(expectedMappedReverseRel);
            AssertRelationshipHasAutoMapToThisRel(classType, expectedPropName, expectedMappedReverseRel);
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

        #endregion

        #region GetMappedReverseRelationshipName

        [Test]
        public void Test_GetMappedReverseRelationshipName_WhenNoAttribute_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var info = RandomValueGenerator.GetMockPropInfoWithNoAutoMapProp<AutoMapOneToManyAttribute>();
            var propertyWrapper = info.ToPropertyWrapper();
            var customAttributes = info.GetCustomAttributes(typeof (AutoMapOneToManyAttribute), true);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(customAttributes);
            Assert.AreEqual(0, customAttributes.Count());
            //---------------Execute Test ----------------------
            var mappedReverseRelationshipName = propertyWrapper.GetMappedReverseRelationshipName<AutoMapOneToManyAttribute>();
            //---------------Test Result -----------------------
            Assert.IsNull(mappedReverseRelationshipName);
        }

        [Test]
        public void Test_GetMappedReverseRelationshipName_WhenAutoMapPropNoReverseRel_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var info = RandomValueGenerator.GetMockPropInfoWithAutoMapAttribute<AutoMapOneToManyAttribute>();
            var propertyWrapper = info.ToPropertyWrapper();
            var customAttributes = info.GetCustomAttributes(typeof (AutoMapOneToManyAttribute), true);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(customAttributes);
            Assert.AreEqual(1, customAttributes.Count());
            var autoMapRelationshipAttribute = customAttributes[0] as AutoMapRelationshipAttribute;
            Assert.IsNotNull(autoMapRelationshipAttribute);
            Assert.IsNull(autoMapRelationshipAttribute.ReverseRelationshipName);
            //---------------Execute Test ----------------------
            var mappedReverseRelationshipName = propertyWrapper.GetMappedReverseRelationshipName<AutoMapOneToManyAttribute>();
            //---------------Test Result -----------------------
            Assert.IsNull(mappedReverseRelationshipName);
        }

        [Test]
        public void Test_GetMappedReverseRelationshipName_WhenAutoMapPropWithReverseRel_ShouldReturnReverseRel()
        {
            //---------------Set up test pack-------------------
            const string expectedRevRelName = "MappedRevRel";
            var info = RandomValueGenerator.GetMockPropInfoWithAutoMapAttribute<AutoMapOneToManyAttribute>(expectedRevRelName);
            var propertyWrapper = info.ToPropertyWrapper();
            var customAttributes = info.GetCustomAttributes(typeof (AutoMapOneToManyAttribute), true);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(customAttributes);
            Assert.AreEqual(1, customAttributes.Count());
            var autoMapRelationshipAttribute = customAttributes[0] as AutoMapRelationshipAttribute;
            Assert.IsNotNull(autoMapRelationshipAttribute);
            Assert.IsNotNullOrEmpty(autoMapRelationshipAttribute.ReverseRelationshipName);
            //---------------Execute Test ----------------------
            var mappedReverseRelationshipName = propertyWrapper.GetMappedReverseRelationshipName<AutoMapOneToManyAttribute>();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedRevRelName, mappedReverseRelationshipName);
        }

        #endregion

        #region GetSingleReverseRelationshipName

        [Test]
        public void
            Test_GetSingleRevRelationshipName_WhenNoRevRel_WhenHasAttribute_WhenHasRevRelName__ShouldBeRevRelName()
        {
            //---------------Set up test pack-------------------
            var classType = typeof (FakeBOWithReverseSingle).ToTypeWrapper();
            const string relationshipName = "MySingleWithAutoMapNoReverse";
            var relPropInfo = classType.GetProperty(relationshipName);
            const string mappedRevRelName = "NoRevRel";
            //---------------Assert Precondition----------------
            relPropInfo.AssertHasAttribute<AutoMapOneToOneAttribute>(
                attribute => attribute.ReverseRelationshipName == mappedRevRelName);
            //---------------Execute Test ----------------------
            var reverseRelationshipName = relPropInfo.GetSingleReverseRelationshipName<AutoMapOneToOneAttribute>();
            //---------------Test Result -----------------------
            Assert.AreEqual(mappedRevRelName, reverseRelationshipName);
        }

        [Test]
        public void Test_GetSingleRevRelationshipName_WhenHasRevRel_ShouldBeFoundRevRelName()
        {
            //---------------Set up test pack-------------------
            var classType = typeof (FakeBOWithReverseSingle).ToTypeWrapper();
            const string relationshipName = "SingleWithRevesre";
            var relPropInfo = classType.GetProperty(relationshipName);
            const string foundRevRelationship = "ReverseSingleRel";
            var relatedClassType = relPropInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(relPropInfo);
            relatedClassType.AssertPropertyExists(foundRevRelationship);
            //---------------Execute Test ----------------------
            var reverseRelationshipName = relPropInfo.GetSingleReverseRelationshipName<AutoMapOneToOneAttribute>();
            //---------------Test Result -----------------------
            Assert.AreEqual(foundRevRelationship, reverseRelationshipName);
        }

        [Test]
        public void
            Test_GetSingleRevRelationshipName_WhenNoRevRel_WhenHasAttribute_WhenNoRevRelName__ShouldBeOwnerClassName()
        {
            //---------------Set up test pack-------------------
            var classType = typeof (FakeBOWithOneToOneAttribute).ToTypeWrapper();
            const string relationshipName = "MySingleRelationship";
            var relPropInfo = classType.GetProperty(relationshipName);

            //---------------Assert Precondition----------------
            relPropInfo.AssertHasAttribute<AutoMapOneToOneAttribute>(attribute => attribute.ReverseRelationshipName == null);
            //---------------Execute Test ----------------------
            var reverseRelationshipName = relPropInfo.GetSingleReverseRelationshipName<AutoMapOneToOneAttribute>();
            //---------------Test Result -----------------------
            Assert.AreEqual(classType.Name, reverseRelationshipName);
        }

        [Test]
        public void Test_GetSingleRevRelationshipName_ShouldReturnRelDefWithRevRelNameSet()
        {
            //---------------Set up test pack-------------------
            var classType = typeof (FakeBOAttributePKAndPKNaming).ToTypeWrapper();
            const string expectedPropName = "MyMultipleRevRel2";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseRelPropInfo = propertyInfo.GetSingleReverseRelPropInfos()[0];
            //---------------Assert Precondition----------------
            propertyInfo.AssertIsMultipleRelationship();
            Assert.IsTrue(propertyInfo.HasSingleReverseRelationship, "There is no reverse single rel");

            Assert.AreNotEqual(classType.Name, reverseRelPropInfo.Name);
            //---------------Execute Test ----------------------
            var reverseRelationshipName = propertyInfo.GetSingleReverseRelationshipName<AutoMapOneToManyAttribute>();
            //---------------Test Result -----------------------
            Assert.AreEqual(reverseRelPropInfo.Name, reverseRelationshipName);
        }

        #endregion


#region EqualityTests

        [TestCase(1, false)]
        [TestCase("fdafdfasdfa", false)]
        [TestCase(typeof(string), false)]
        [TestCase(null, false)]
        [TestCase((Type)null, false)]
        [TestCase((string)null, false)]
        public void Test_Equals(object other, bool expectedResult)
        {
            //---------------Set up test pack-------------------
            var propInfo = new FakePropertyInfo(typeof(FakeBOWProps));
            var propertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper.Equals(other);
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedResult, isEqual
                    , string.Format("'{0}' Equals '{1}' should be '{2}'"
                    , other, propertyWrapper, expectedResult));
        }

        [Test]
        public void Test_Equals_WhenPropInfo_ShouldRetTrue()
        {
            //---------------Set up test pack-------------------
            var propInfo = new FakePropertyInfo(typeof(FakeBOWProps));
            var propertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper.Equals(propInfo);
            //---------------Test Result -----------------------
            Assert.IsTrue(isEqual);
        }
        [Test]
        public void Test_Equals_WhenNotPropInfo_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            var propInfo = new FakePropertyInfo(typeof(FakeBOWProps));
            var otherPropInfo = new FakePropertyInfo(typeof(FakeBOWProps));
            var propertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper.Equals(otherPropInfo);
            //---------------Test Result -----------------------
            Assert.IsFalse(isEqual);
        }
        [Test]
        public void Test_Equals_WhenNull_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            var propInfo = new FakePropertyInfo(typeof(FakeBOWProps));
            PropertyInfo otherPropInfo = null;
            var propertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper.Equals(otherPropInfo);
            //---------------Test Result -----------------------
            Assert.IsFalse(isEqual);
        }
        [Test]
        public void Test_Equals_WhenName_ShouldRetTrue()
        {
            //---------------Set up test pack-------------------
            var propInfo = MockRepository.GenerateMock<PropertyInfo>();
            propInfo.SetName(GetRandomString());
            var propertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------
            Assert.IsNotNullOrEmpty(propInfo.Name);
            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper.Equals(propInfo.Name);
            //---------------Test Result -----------------------
            Assert.IsTrue(isEqual);
        }
        [Test]
        public void Test_Equals__WhenRandomString_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            var propInfo = new FakePropertyInfo(typeof(FakeBOWProps));
            var propertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper.Equals(GetRandomString());
            //---------------Test Result -----------------------
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Test_Equals_WhenNullString_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            var propInfo = new FakePropertyInfo(typeof(FakeBOWProps));
            string otherPropInfo = null;
            var propertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper.Equals(otherPropInfo);
            //---------------Test Result -----------------------
            Assert.IsFalse(isEqual);
        }
        [Test]
        public void Test_Equals_WhenSelf_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var propInfo = MockRepository.GenerateMock<PropertyInfo>();
            var propertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper.Equals(propertyWrapper);
            //---------------Test Result -----------------------
            Assert.IsTrue(isEqual);
        }
        [Test]
        public void Test_Equals_WhenPropertyWrapperNull_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var propInfo = MockRepository.GenerateMock<PropertyInfo>();
            var propertyWrapper = new PropertyWrapper(propInfo);
            PropertyWrapper nulPropWrap = null;
            //---------------Assert Precondition----------------
            Assert.IsNull(nulPropWrap);
            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper.Equals(nulPropWrap);
            //---------------Test Result -----------------------
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Test_Equals_WhenWrapSameType_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var propInfo = MockRepository.GenerateMock<PropertyInfo>();
            var propertyWrapper = new PropertyWrapper(propInfo);
            var otherPropertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------
            Assert.AreSame(propertyWrapper.PropertyInfo, otherPropertyWrapper.PropertyInfo);
            //For some reason the dot net 4.0 framework has chnaged this 
            // Even though the two PropertyInfos AreSame they are no longer seen as Equal.
            //Assert.IsTrue(propertyWrapper.PropertyInfo.Equals(otherPropertyWrapper.PropertyInfo), "The PropertyInfo.Equals Should be true");
            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper.Equals(otherPropertyWrapper);
            //---------------Test Result -----------------------
            Assert.IsTrue(isEqual);
        }
        [Test]
        public void Test_Equals_WhenWrapDiffType_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var propInfo = MockRepository.GenerateMock<PropertyInfo>();
            var propertyWrapper = new PropertyWrapper(propInfo);
            var otherPropertyWrapper = new PropertyWrapper(MockRepository.GenerateMock<PropertyInfo>());
            //---------------Assert Precondition----------------
            Assert.AreNotSame(propertyWrapper.PropertyInfo, otherPropertyWrapper.PropertyInfo);
            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper.Equals(otherPropertyWrapper);
            //---------------Test Result -----------------------
            Assert.IsFalse(isEqual);
        }



        [Test]
        public void Test_EqualEquals_WhenPropInfo_ShouldRetTrue()
        {
            //---------------Set up test pack-------------------
            var propInfo = new FakePropertyInfo(typeof(FakeBOWProps));
            var propertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper == propInfo;
            //---------------Test Result -----------------------
            Assert.IsTrue(isEqual);
        }
        [Test]
        public void Test_EqualEquals_WhenNotPropInfo_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            var propInfo = new FakePropertyInfo(typeof(FakeBOWProps));
            var otherPropInfo = new FakePropertyInfo(typeof(FakeBOWProps));
            var propertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper == otherPropInfo;
            //---------------Test Result -----------------------
            Assert.IsFalse(isEqual);
        }
       
        [Test]
        public void Test_EqualEquals_WhenNull_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            var propInfo = new FakePropertyInfo(typeof(FakeBOWProps));
            PropertyInfo otherPropInfo = null;
            var propertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper == otherPropInfo;
            //---------------Test Result -----------------------
            Assert.IsFalse(isEqual);
        }
      
        [Test]
        public void Test_EqualEqualsOtherProp_WhenNull_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            var propInfo = new FakePropertyInfo(typeof(FakeBOWProps));
            PropertyInfo otherPropInfo = null;
            var propertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = otherPropInfo == propertyWrapper;
            //---------------Test Result -----------------------
            Assert.IsFalse(isEqual);
        }

        // ReSharper disable EqualExpressionComparison
        [Test]
        public void Test_EqualEquals_WhenSelf_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var propInfo = MockRepository.GenerateMock<PropertyInfo>();
            var propertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = (propertyWrapper == propertyWrapper);
            //---------------Test Result -----------------------
            Assert.IsTrue(isEqual);
        }
        // ReSharper restore EqualExpressionComparison

        [Test]
        public void Test_EqualEquals_WhenPropertyWrapperNull_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var propInfo = MockRepository.GenerateMock<PropertyInfo>();
            var propertyWrapper = new PropertyWrapper(propInfo);
            PropertyWrapper nulPropWrap = null;
            //---------------Assert Precondition----------------
            Assert.IsNull(nulPropWrap);
            //---------------Execute Test ----------------------
            var isEqual = (propertyWrapper == nulPropWrap);
            //---------------Test Result -----------------------
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Test_EqualEquals_WhenWrapSameProp_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var propInfo = MockRepository.GenerateMock<PropertyInfo>();
            var propertyWrapper = new PropertyWrapper(propInfo);
            var otherPropertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------
            Assert.AreSame(propertyWrapper.PropertyInfo, otherPropertyWrapper.PropertyInfo);
            Assert.IsTrue(propertyWrapper.Equals(otherPropertyWrapper), "Equals should work");
            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper == otherPropertyWrapper;
            //---------------Test Result -----------------------
            Assert.IsTrue(isEqual, "The == should have been implemented correctly");
        }
        [Test]
        public void Test_EqualEquals_WhenWrapDiffProp_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var propInfo = MockRepository.GenerateMock<PropertyInfo>();
            var propertyWrapper = new PropertyWrapper(propInfo);
            var otherPropertyWrapper = new PropertyWrapper(MockRepository.GenerateMock<PropertyInfo>());
            //---------------Assert Precondition----------------
            Assert.AreNotSame(propertyWrapper.PropertyInfo, otherPropertyWrapper.PropertyInfo);
            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper == otherPropertyWrapper;
            //---------------Test Result -----------------------
            Assert.IsFalse(isEqual);
        }
        [Test]
        public void Test_NotEquals_WhenWrapDiffProp_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var propInfo = MockRepository.GenerateMock<PropertyInfo>();
            var propertyWrapper = new PropertyWrapper(propInfo);
            var otherPropertyWrapper = new PropertyWrapper(MockRepository.GenerateMock<PropertyInfo>());
            //---------------Assert Precondition----------------
            Assert.AreNotSame(propertyWrapper.PropertyInfo, otherPropertyWrapper.PropertyInfo);
            //---------------Execute Test ----------------------
            var isNotEqual = propertyWrapper != otherPropertyWrapper;
            //---------------Test Result -----------------------
            Assert.IsTrue(isNotEqual);
        }
        [Test]
        public void Test_NotEqualsOnProp_WhenWrapDiffProp_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var propInfo = MockRepository.GenerateMock<PropertyInfo>();
            var propertyWrapper = new PropertyWrapper(propInfo);
            var otherPropInfor = MockRepository.GenerateMock<PropertyInfo>();
            //---------------Assert Precondition----------------
            Assert.AreNotSame(propertyWrapper.PropertyInfo, otherPropInfor);
            //---------------Execute Test ----------------------
            var isNotEqual = propertyWrapper != otherPropInfor;
            //---------------Test Result -----------------------
            Assert.IsTrue(isNotEqual);
        }
        [Test]
        public void Test_NotEqualsOnOtherProp_WhenWrapDiffProp_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var propInfo = MockRepository.GenerateMock<PropertyInfo>();
            var propertyWrapper = new PropertyWrapper(propInfo);
            var otherPropInfor = MockRepository.GenerateMock<PropertyInfo>();
            //---------------Assert Precondition----------------
            Assert.AreNotSame(propertyWrapper.PropertyInfo, otherPropInfor);
            //---------------Execute Test ----------------------
            var isNotEqual = otherPropInfor != propertyWrapper;
            //---------------Test Result -----------------------
            Assert.IsTrue(isNotEqual);
        }

        [Test]
        public void Test_EqualEquals_WhenBothNullPropertyWrapper_ShouldRetTrue()
        {
            //---------------Set up test pack-------------------
            PropertyWrapper propertyWrapper = null;
            PropertyWrapper otherPropWrap = null;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper == otherPropWrap;
            //---------------Test Result -----------------------
            Assert.IsTrue(isEqual);
        }


        [Test]
        public void Test_EqualEquals_WhenNullPropertyWrapper_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            var propInfo = MockRepository.GenerateMock<PropertyInfo>();
            var propertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = propertyWrapper == (PropertyWrapper)null;
            //---------------Test Result -----------------------
            Assert.IsFalse(isEqual);
        }


        [Test]
        public void Test_NotEquals_WhenNullPropertyWrapper_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            var propInfo = MockRepository.GenerateMock<PropertyInfo>();
            var propertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isNotEqual = propertyWrapper != (PropertyWrapper)null;
            //---------------Test Result -----------------------
            Assert.IsTrue(isNotEqual);
        }

        [Test]
        public void Test_GetHashCode_ShouldReturnCalculatedCode()
        {
            //---------------Set up test pack-------------------
            var propInfo = new FakePropertyInfo(GetRandomString(), typeof(FakeBOWProps));
            var expectedHashCode = propInfo.GetHashCode() * 397 ^ propInfo.Name.GetHashCode();
            var propertyWrapper = new PropertyWrapper(propInfo);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var hashCode = propertyWrapper.GetHashCode();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedHashCode, hashCode);
        }

        [Test]
        public void Test_IsInheritedProp_WhenInheritedProp_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOSubClassSuperHasDesc);
            var propertyInfo = classType.GetProperty("FakeBOSuperClassWithDescType");
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.AreNotSame(propertyInfo.ReflectedType, propertyInfo.DeclaringType);
            //---------------Execute Test ----------------------
            var isInherited = propertyWrapper.IsInherited;
            //---------------Test Result -----------------------
            Assert.IsTrue(isInherited);
        }    

        [Test]
        public void Test_IsInheritedProp_WhenNotInheritedProp_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOAttributePK);
            var propertyInfo = classType.GetProperty("AnotherProp");
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.AreSame(propertyInfo.ReflectedType, propertyInfo.DeclaringType);
            //---------------Execute Test ----------------------
            var isInherited = propertyWrapper.IsInherited;
            //---------------Test Result -----------------------
            Assert.IsFalse(isInherited);
        }
        [Test]
        public void Test_IsInheritedProp_WhenPropFromInterface_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOSuperClassWithDesc);
            var propertyInfo = classType.GetProperty("FakeBOSuperClassWithDescType");
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.AreSame(propertyInfo.ReflectedType, propertyInfo.DeclaringType);
            //---------------Execute Test ----------------------
            var isInherited = propertyWrapper.IsInherited;
            //---------------Test Result -----------------------
            Assert.IsFalse(isInherited);
        }

        [Test]
        public void Test_ReflectedType_ShouldReturnUnderlyingTypesReflectedType()
        {
            //---------------Set up test pack-------------------
            PropertyInfo propertyInfo = MockRepository.GenerateMock<FakePropertyInfo>();
            var expectedReflectedType = MockRepository.GenerateMock<Type>();
            propertyInfo.Stub(info => info.ReflectedType).Return(expectedReflectedType);

            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo.ReflectedType);
            //---------------Execute Test ----------------------
            var reflectedType = propertyWrapper.ReflectedType;
            //---------------Test Result -----------------------
            Assert.AreSame(expectedReflectedType, reflectedType.UnderlyingType);
        }

        private static string GetRandomString()
        {
            return RandomValueGenerator.GetRandomString();
        }

        #endregion
        [Test]
        public void Test_IsOverridden_WhenInheritedPropNotOverriden_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOSubClassSuperHasDesc);
            var propertyInfo = classType.GetProperty("FakeBOSuperClassWithDescType");
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.IsTrue(propertyWrapper.IsInherited);
            //---------------Execute Test ----------------------
            var isOverridden = propertyWrapper.IsOverridden;
            //---------------Test Result -----------------------
            Assert.IsFalse(isOverridden);
        }
        
        [Test]
        public void Test_IsOverridden_WhenOverriden_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOSubClassWithOverridenProps);
            var propertyInfo = classType.GetProperty("OverriddenProp");
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            //---------------Execute Test ----------------------
            var isOverridden = propertyWrapper.IsOverridden;
            //---------------Test Result -----------------------
            Assert.IsTrue(isOverridden);
        }        
        [Test]
        public void Test_IsInherited_WhenOverriden_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOSubClassWithOverridenProps);
            var propertyInfo = classType.GetProperty("OverriddenProp");
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            //---------------Execute Test ----------------------
            var isInherited = propertyWrapper.IsInherited;
            //---------------Test Result -----------------------
            Assert.IsTrue(isInherited);
        }  
        [Test]
        public void Test_IsOverridden_WhenImplementedProp_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOImplementingInterface);
            var propertyInfo = classType.GetProperty("ImplementedProp");
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.IsFalse(propertyWrapper.IsInherited);
            //---------------Execute Test ----------------------
            var isOverridden = propertyWrapper.IsOverridden;
            //---------------Test Result -----------------------
            Assert.IsFalse(isOverridden);
        } 
        [Test]
        public void Test_IsOverridden_WhenDeclaredOnClass_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "PublicGetGuidProp";
            var classType = typeof(FakeBOWProps);
            var propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.IsFalse(propertyWrapper.IsInherited);
            //---------------Execute Test ----------------------
            var isOverridden = propertyWrapper.IsOverridden;
            //---------------Test Result -----------------------
            Assert.IsFalse(isOverridden);
        }

        [Test]
        public void Test_HasDefaultAttribute_WhenHasOne_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "DefaultProp";
            var classType = typeof(FakeBOWithDefaultProp);
            var propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var hasDefaultAttribute = propertyWrapper.HasDefaultAttribute;
            //---------------Test ResultpropertyWrapper -----------------------
            Assert.IsTrue(hasDefaultAttribute);
        }

        [Test]
        public void Test_HasDefaultAttribute_WhenNotHasOne_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "NonDefaultProp";
            var classType = typeof(FakeBOWithDefaultProp);
            var propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var hasDefaultAttribute = propertyWrapper.HasDefaultAttribute;
            //---------------Test ResultpropertyWrapper -----------------------
            Assert.IsFalse(hasDefaultAttribute);
        }

        [Test]
        public void Test_HasFieldNameAttribute_WhenHasOne_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "PropWithFieldName";
            var classType = typeof(FakeBoWithTableName);
            var propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var hasDefaultAttribute = propertyWrapper.HasAttribute<AutoMapFieldNameAttribute>();
            //---------------Test ResultpropertyWrapper -----------------------
            Assert.IsTrue(hasDefaultAttribute);
        }

        [Test]
        public void Test_HasFieldNameAttribute_WhenNotHasOne_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "PropNoFieldName";
            var classType = typeof(FakeBoWithTableName);
            var propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var hasDefaultAttribute = propertyWrapper.HasAttribute<AutoMapFieldNameAttribute>();
            //---------------Test ResultpropertyWrapper -----------------------
            Assert.IsFalse(hasDefaultAttribute);
        }
        [Test]
        public void Test_GetFieldNameAttribute_WhenShouldReturnAttributeWithFieldNameSet()
        {
            //---------------Set up test pack-------------------
            const string fieldName = "MyFieldName";
            var classType = typeof(FakeBoWithTableName);
            const string expectedPropName = "PropWithFieldName";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var fieldNameAttribute = propertyWrapper.GetAttribute<AutoMapFieldNameAttribute>();
            //---------------Test Result -----------------------
            Assert.IsNotNull(fieldNameAttribute);
            Assert.AreEqual(fieldName, fieldNameAttribute.FieldName);
        }

//
//        [Test]
//        public void Test_HasDefaultAttribute_WhenHasOne_ShouldReturnTrue()
//        {
//            //---------------Set up test pack-------------------
//            const string expectedPropName = "DefaultProp";
//            var classType = typeof(FakeBOWithDefaultProp);
//            var propertyInfo = classType.GetProperty(expectedPropName);
//            var propertyWrapper = propertyInfo.ToPropertyWrapper();
//            //---------------Assert Precondition----------------
//
//            //---------------Execute Test ----------------------
//            var hasDefaultAttribute = propertyWrapper.HasDefaultAttribute;
//            //---------------Test ResultpropertyWrapper -----------------------
//            Assert.IsTrue(hasDefaultAttribute);
//        }


        [Test]
        public void Test_HasDisplayNameAttribute_WhenHasOne_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "DisplayNameProp";
            var classType = typeof(FakeBOWitDisplayNameProp);
            var propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var hasDisplayNameAttribute = propertyWrapper.HasDisplayNameAttribute;
            //---------------Test ResultpropertyWrapper -----------------------
            Assert.IsTrue(hasDisplayNameAttribute);
        }
        [Test]
        public void Test_HasDisplayNameAttribute_WhenNotHasOne_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "NonDisplayNameProp";
            var classType = typeof(FakeBOWitDisplayNameProp);
            var propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var hasDisplayNameAttribute = propertyWrapper.HasDisplayNameAttribute;
            //---------------Test ResultpropertyWrapper -----------------------
            Assert.IsFalse(hasDisplayNameAttribute);
        }



        [Test]
        public void Test_GetAttribute_WhenHasAttribute_ShouldReturnAttribute()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "DefaultProp";
            var classType = typeof(FakeBOWithDefaultProp);
            var propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var attribute = propertyWrapper.GetAttribute<AutoMapDefaultAttribute>();
            //---------------Test ResultpropertyWrapper -----------------------
            Assert.NotNull(attribute);
        }
        [Test]
        public void Test_GetAttribute_WhenNotHasAttribute_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "NonDefaultProp";
            var classType = typeof(FakeBOWithDefaultProp);
            var propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var attribute = propertyWrapper.GetAttribute<AutoMapDefaultAttribute>();
            //---------------Test ResultpropertyWrapper -----------------------
            Assert.Null(attribute);
        }

        [Test]
        public void Test_UnderlyingPropertyType_WhenNullableGuid_ShouldReturnGuid()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "PublicGetNullableGuidProp";
            var classType = typeof(FakeBOWProps);
            var propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(Guid?), propertyInfo.PropertyType);
            //---------------Execute Test ----------------------
            var propType = propertyWrapper.UndelyingPropertyType;
            //---------------Test Result -----------------------
            Assert.AreEqual(typeof(Guid), propType);
        }

        [Test]
        public void Test_UnderlyingPropertyType_WhenGuid_ShouldReturnGuid()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "PublicGetGuidProp";
            var classType = typeof(FakeBOWProps);
            var propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(Guid), propertyInfo.PropertyType);
            //---------------Execute Test ----------------------
            var propType = propertyWrapper.UndelyingPropertyType;
            //---------------Test Result -----------------------
            Assert.AreEqual(typeof(Guid), propType);
        }
        [Test]
        public void Test_UnderlyingPropertyType_WhenInt_ShouldReturnInt()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "PublicIntProp";
            var classType = typeof(FakeBOWProps);
            var propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(int), propertyInfo.PropertyType);
            //---------------Execute Test ----------------------
            var propType = propertyWrapper.UndelyingPropertyType;
            //---------------Test Result -----------------------
            Assert.AreEqual(typeof(int), propType);
        }

        private static void AssertRelationshipHasAutoMapToThisRel(TypeWrapper classType, string relName,
                                                                  string expectedReversRelName)
        {
            var relProp = classType.GetProperty(relName);
            var customRelationship = relProp.GetAttributes<AutoMapRelationshipAttribute>();
            Assert.IsTrue(customRelationship.Any(o => (o.ReverseRelationshipName == expectedReversRelName)),
                          relName + " on " + classType.Name + " should have an AutoMapManyToOne attribute");
        }

        private static void AssertReverseRelationshipHasAutoMapToThisRel(TypeWrapper reverseClassType, string relName,
                                                                         string expectedReversRelName)
        {
            var reverseRelProp = reverseClassType.GetProperty(relName);
            var customRelationship = reverseRelProp.GetAttributes<AutoMapRelationshipAttribute>();
            Assert.IsTrue(customRelationship.Any(o => (o.ReverseRelationshipName == expectedReversRelName)),
                          relName + " on " + reverseClassType.Name + " should have an AutoMapManyToOne attribute");
        }

        private static void AssertHasOneToOneWithReverseRelationship(PropertyWrapper propertyInfo,
                                                                     string expectedRevRelName)
        {
            Assert.IsTrue(propertyInfo.HasAutoMapOneToOneAttribute(expectedRevRelName),
                          string.Format("Should have AutoMapOneToOne with ReverseRelationship '{0}'", expectedRevRelName));
        }

        private static void AssertReverseRelationshipHasIgnoreAttribute(TypeWrapper reverseClassType, string relName)
        {
            var singleRelProp = reverseClassType.GetProperty(relName);
            Assert.IsTrue(singleRelProp.HasIgnoreAttribute,
                          relName + " on " + reverseClassType.Name + " should have an ignore attribute");
        }

        private static void AssertRelationshipIsForOwnerClass(TypeWrapper ownerClassType, TypeWrapper reverseClassType,
                                                              string reveresRelationshipName)
        {
            var reversePropInfo = reverseClassType.GetProperty(reveresRelationshipName);
            Assert.IsNotNull(reversePropInfo,
                             reveresRelationshipName + " on " + reverseClassType.Name + " should not be null");
            Assert.AreSame(ownerClassType.UnderlyingType,
                           reversePropInfo.RelatedClassType.UnderlyingType);
        }

        private static void AssertReverseRelationshipNotForOwnerClass(TypeWrapper ownerClassType,
                                                                      TypeWrapper reverseClassType,
                                                                      string reveresRelationshipName)
        {
            var reversePropInfo = reverseClassType.GetProperty(reveresRelationshipName);
            Assert.IsNotNull(reversePropInfo, "No Reverse Relationship found with the name");
            Assert.AreNotSame(ownerClassType, reversePropInfo.RelatedClassType);
        }
    }

    public class FakePropertyWrapper : PropertyWrapper
    {
        private TypeWrapper MyDeclaringType { get; set; }
        private TypeWrapper _relatedType;
        private TypeWrapper _propertyType;

        private FakePropertyWrapper(PropertyInfo propertyInfo, TypeWrapper declaringType) : base(propertyInfo)
        {
            MyDeclaringType = declaringType;
        }

        public FakePropertyWrapper(PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
        }

        public FakePropertyWrapper()
            : base(MockRepository.GenerateStub<PropertyInfo>())
        {
        }

        public FakePropertyWrapper(TypeWrapper declaringType)
            : this(MockRepository.GenerateStub<PropertyInfo>(), declaringType)
        {
        }


        public override TypeWrapper DeclaringType
        {
            get
            {
                if(MyDeclaringType.IsNull())
                {
                    MyDeclaringType = MockRepository.GenerateStub<FakeTypeWrapper>();
                    MyDeclaringType.SetName(RandomValueGenerator.GetRandomString());
                }
                return MyDeclaringType;
            }
        }
        public override TypeWrapper RelatedClassType
        {
            get
            {
                if (MyRelatedType.IsNull())
                {
                    MyRelatedType = MockRepository.GenerateStub<FakeTypeWrapper>();
                    MyRelatedType.SetName(RandomValueGenerator.GetRandomString());
                }
                return MyRelatedType;
            }
        }        
        public override TypeWrapper PropertyType
        {
            get
            {
                if (MyPropertyType.IsNull())
                {
                    MyPropertyType = MockRepository.GenerateStub<FakeTypeWrapper>();
                    MyPropertyType.SetName(RandomValueGenerator.GetRandomString());
                }
                return MyPropertyType;
            }
        }

        public TypeWrapper MyRelatedType
        {
            get { return _relatedType; }
            set { _relatedType = value; }
        }

        public TypeWrapper MyPropertyType
        {
            get { return _propertyType; }
            set { _propertyType = value; }
        }
    }

    public class FakeTypeWrapper : TypeWrapper
    {
        public FakeTypeWrapper(Type type) : base(type)
        {
        }

        public FakeTypeWrapper() : base(MockRepository.GenerateMock<Type>())
        {
        }
        public override IEnumerable<PropertyWrapper> GetProperties()
        {
            if (this.UnderlyingType == null || this.UnderlyingType.GetProperties() == null)
            {
                return new List<PropertyWrapper>();
            }
            return base.GetProperties();
        }
    }

    internal static class WrapperMockExtenstionsForTesting
    {

        internal static void SetPKPropName(this TypeWrapper propWrapper, string pkName)
        {
            propWrapper.Stub(wrapper => wrapper.GetPKPropName()).Return(pkName);
        }
        internal static void SetName(this TypeWrapper typeWrapper, string expectedTypeName)
        {
            typeWrapper.Stub(wrapper => wrapper.Name).Return(expectedTypeName);
        }
        internal static void SetAssemblyName(this TypeWrapper typeWrapper, string expectedTypeName)
        {
            typeWrapper.Stub(wrapper => wrapper.AssemblyQualifiedName).Return(expectedTypeName);
        }
        internal static void SetName(this PropertyInfo propInfo, string expectedTypeName)
        {

            propInfo.Stub(wrapper => wrapper.Name).Return(expectedTypeName);
        }
        internal static void SetName(this PropertyWrapper propWrapper, string expectedTypeName)
        {
            propWrapper.Stub(wrapper => wrapper.Name).Return(expectedTypeName);
        }

        public static void SetHasProperty(this TypeWrapper ownerType, string relatedPropName, bool hasProperty)
        {
            ownerType.Stub(wrapper => wrapper.HasProperty(relatedPropName)).Return(hasProperty);
        }

        public static PropertyInfo SetProperty(this Type type, string propName)
        {
            var randomPropInfo = GetRandomPropInfo();
            type.Stub(type1 => type1.GetProperty(propName)).Return(randomPropInfo);
            return randomPropInfo;
        }

        public static void SetOneToOneAttributeOnStub(this PropertyWrapper propertyWrapper, RelationshipType relationshipType)
        {
            propertyWrapper.Stub(pw => pw.GetAttribute<AutoMapOneToOneAttribute>()).Return(
                new AutoMapOneToOneAttribute(relationshipType));
        }
        private static PropertyInfo GetRandomPropInfo()
        {
            return MockRepository.GenerateMock<PropertyInfo>();
        }

//        private static Type[] GetRandomTypeArray()
//        {
//            var randomInt = RandomValueGenerator.GetRandomInt(1, 3);
//            Type[] types = new Type[randomInt];
//            for (int i = 0; i < randomInt; i++)
//            {
//                types[i] = MockRepository.GenerateMock<Type>();
//            }
//            return types;
//        }
//
//        private static PropertyInfo[] GetRandomPropInfoArray()
//        {
//            var randomInt = RandomValueGenerator.GetRandomInt(1, 3);
//            PropertyInfo[] types = new PropertyInfo[randomInt];
//            for (int i = 0; i < randomInt; i++)
//            {
//                types[i] = MockRepository.GenerateMock<PropertyInfo>();
//            }
//            return types;
//        }

        internal static void SetOneToOneReverseRelName(this PropertyWrapper propWrapper, string reverseRelName)
        {
            propWrapper.Stub(wrapper
                             => wrapper.GetSingleReverseRelationshipName<AutoMapOneToOneAttribute>())
                .Return(reverseRelName);
        }

        public static void SetIsSingleRelationship(this PropertyWrapper propertyWrapper, bool isSingle)
        {
            propertyWrapper.Stub(wrapper => wrapper.IsSingleRelationhip).Return(isSingle);
        }

        public static void SetIgnoreAttribute(this PropertyWrapper propertyInfo, bool mustSetAttribute)
        {
            propertyInfo.Stub(wrapper => wrapper.HasIgnoreAttribute).Return(mustSetAttribute);
        }

        internal static void SetCustomAttribute<T>(this PropertyInfo propertyInfo) where T: Attribute
        {
            propertyInfo.Stub(propInfo => propInfo.GetCustomAttributes(true))
                .Return(new[]
                            {
                                MockRepository.GenerateMock<T>()
                            });
            propertyInfo.Stub(propInfo
                              => propInfo.GetCustomAttributes(typeof(T), true))
                .Return(propertyInfo.GetCustomAttributes(true));
        }
        internal static void SetCustomAttribute(this PropertyInfo propertyInfo, Attribute attribute)
        {
            propertyInfo.Stub(propInfo => propInfo.GetCustomAttributes(true))
                .Return(new[]
                            {
                                attribute
                            });
            propertyInfo.Stub(propInfo
                              => propInfo.GetCustomAttributes(attribute.GetType(), true))
                .Return(propertyInfo.GetCustomAttributes(true));
        }
        internal static void ClearCustomAttributes<T>(this PropertyInfo propertyInfo) where T : Attribute
        {
            propertyInfo.Stub(propInfo => propInfo.GetCustomAttributes(true))
                .Return(new Attribute[0]);
            propertyInfo.Stub(propInfo
                              => propInfo.GetCustomAttributes(typeof(T), true))
                .Return(propertyInfo.GetCustomAttributes(true));
        }
/*
        internal static void SetCustomAutoMapRelationshipAttribute<T>(this PropertyInfo propertyInfo, string reverseRelName) where T : AutoMapRelationshipAttribute
        {
            propertyInfo.Stub(propInfo => propInfo.GetCustomAttributes(true))
                .Return(new[]
                            {
                                MockRepository.GenerateMock<T>(reverseRelName)
                            });
            propertyInfo.Stub(propInfo
                              => propInfo.GetCustomAttributes(typeof(T), true))
                .Return(propertyInfo.GetCustomAttributes(true));
        }*/
        internal static void SetDeclaringType(this PropertyWrapper propertyWrapper, TypeWrapper wrapper)
        {
            propertyWrapper.Stub(wrapper1 => wrapper1.DeclaringType).Return(wrapper);
        }

        public static void SetRelatedType(this PropertyWrapper propertyWrapper, TypeWrapper relatedClassType)
        {
            propertyWrapper.Stub(wrapper1 => wrapper1.RelatedClassType).Return(relatedClassType);
        }
/*
        public static void SetPropertyType(this PropertyWrapper propertyWrapper, TypeWrapper propertyType)
        {
            propertyWrapper.Stub(wrapper1 => wrapper1.PropertyType).Return(propertyType);
        }*/
    }
}