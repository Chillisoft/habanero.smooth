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
using Habanero.BO;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Smooth.Test.ValidFakeBOs;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Smooth.Test
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestPropertyAutoMapper
    {
        // ReSharper disable ObjectCreationAsStatement
        // ReSharper disable ExpressionIsAlwaysNull
        [Test]
        public void Test_ConstructWithNullPropInfo_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            PropertyInfo info = null;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                new PropertyAutoMapper(info);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("propInfo", ex.ParamName);
            }
        }
        [Test]
        public void Test_ConstructWithNullPropWrapper_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            PropertyWrapper info = null;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                new PropertyAutoMapper(info);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("propertyWrapper", ex.ParamName);
            }
        }
        // ReSharper restore ExpressionIsAlwaysNull
        // ReSharper restore ObjectCreationAsStatement
        [TestCase("PublicGetGuidProp", typeof(Guid), "System.Guid")]
        [TestCase("PublicGetNullableGuidProp", typeof(Guid?), "System.Guid")]
        [TestCase("PublicStringProp", typeof(String), "System.String")]
        [TestCase("PublicIntProp", typeof(Int32), "System.Int32")]
        [TestCase("PublicEnumProp", typeof(FakeEnum), "Habanero.Smooth.Test.FakeEnum")]
        [TestCase("PublicNullableEnumProp", typeof(FakeEnum?), "Habanero.Smooth.Test.FakeEnum")]
        [TestCase("PublicPropWithAtt", typeof(float?), "System.Single")]
        [TestCase("PublicImageProp", typeof(System.Drawing.Image), "System.Drawing.Image")]
        [TestCase("PublicByteArrayProp", typeof(byte[]), "System.Byte[]")]
        public void Test_MapProperty_WhenPublicProp_ShouldSetPropNameAndType(string propName, Type propType, string typeName)
        {
            //---------------Set up test pack-------------------
            var expectedPropName = propName;
            var classType = typeof (FakeBOWProps);
            var propertyInfo = classType.GetProperty(expectedPropName);
            var propertyAutoMapper = new PropertyAutoMapper(propertyInfo);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.AreEqual(expectedPropName, propertyInfo.Name);
            Assert.AreEqual(propType, propertyInfo.PropertyType);
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            Assert.AreEqual(expectedPropName, propDef.PropertyName);
            Assert.AreEqual(typeName, propDef.PropertyTypeName);
            Assert.AreEqual(PropReadWriteRule.ReadWrite, propDef.ReadWriteRule);
        }

        

        [Test]
        public void Test_Map_WhenTypeNotSystem_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWProps);
            var propertyInfo = classType.GetProperty("ClassDef");
            var propTypeName = propertyInfo.PropertyType.FullName;
            var propertyAutoMapper = new PropertyAutoMapper(propertyInfo);
            //---------------Assert Precondition----------------
            propTypeName.ShouldNotStartWith("System.");
            Assert.IsFalse(propertyInfo.ToPropertyWrapper().HasIgnoreAttribute);
            Assert.IsFalse(propertyInfo.PropertyType.CanMapToProp());
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNull(propDef);
        }

        [Test]
        public void Test_Map_WhenIsStatic_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBoWithStaticProperty);
            var propertyInfo = classType.GetProperty("PublicStringProp");
            var propertyAutoMapper = new PropertyAutoMapper(propertyInfo);
            //---------------Assert Precondition----------------
            Assert.IsFalse(propertyInfo.ToPropertyWrapper().HasIgnoreAttribute);
            Assert.IsTrue(propertyInfo.PropertyType.CanMapToProp());
            Assert.IsTrue(propertyInfo.ToPropertyWrapper().IsStatic);
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNull(propDef);
        }

        [Test]
        public void Test_Map_WhenHasIgnoreAttribute_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWProps);
            var propertyInfo = classType.GetProperty("PublicWithIgnoreAtt");
            var propertyAutoMapper = new PropertyAutoMapper(propertyInfo);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.AreEqual(1, propertyInfo.GetCustomAttributes(true).Length);
            Assert.IsTrue(propertyInfo.ToPropertyWrapper().HasIgnoreAttribute);
            Assert.IsTrue(propertyInfo.PropertyType.CanMapToProp());
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNull(propDef);
        }

        [Test]
        public void MapProperty_WhenHasNoAttribute_ShouldSetKeepValuePrivateFalse()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWProps);
            var propertyInfo = classType.GetProperty("PublicGetGuidProp");
            var propertyAutoMapper = new PropertyAutoMapper(propertyInfo);
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsFalse(propDef.KeepValuePrivate);
        }

        [Test]
        public void MapProperty_WhenHasKeepValuePrivateAttribute_ShouldSetKeepValuePrivateTrue()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWProps);
            var propertyInfo = classType.GetProperty("PublicWithKeepValuePrivateAtt");
            var propertyAutoMapper = new PropertyAutoMapper(propertyInfo);
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsTrue(propDef.KeepValuePrivate);
        }

        [Test]
        public void Test_PropWithNonIgnoreAttribute_ShouldReturnPropDef()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWProps);
            var propertyInfo = classType.GetProperty("PublicPropWithAtt");
            var propertyAutoMapper = new PropertyAutoMapper(propertyInfo);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.AreEqual(1, propertyInfo.GetCustomAttributes(true).Length);
            Assert.IsFalse(propertyInfo.ToPropertyWrapper().HasIgnoreAttribute);
            Assert.IsTrue(propertyInfo.PropertyType.CanMapToProp());
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            Assert.AreEqual("PublicPropWithAtt", propDef.PropertyName);
        }
        [Test]
        public void Test_PrivateProp_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBoWithPrivateProps);
            var propertyInfo = classType.GetProperty("PrivateStringProp", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            var propertyAutoMapper = new PropertyAutoMapper(propertyInfo);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.IsFalse(propertyWrapper.HasIgnoreAttribute);

            Assert.IsTrue(propertyInfo.PropertyType.CanMapToProp());
            Assert.IsFalse(propertyWrapper.IsPublic);
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNull(propDef);
        }

        [Test]
        public void Test_Map_WhenInheritedProp_ShouldReturnNull()
        {
            //When Prop inherits from another class (i.e. overrides previous
            //declaration the prop will be mapped as part of the base class
            // and should not be mapped as part of this class.
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOSubClassSuperHasDesc);
            var propertyInfo = classType.GetProperty("FakeBOSuperClassWithDescType");
            var propertyAutoMapper = new PropertyAutoMapper(propertyInfo);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.IsFalse(propertyWrapper.HasIgnoreAttribute);

            Assert.IsTrue(propertyInfo.PropertyType.CanMapToProp());
            Assert.IsTrue(propertyWrapper.IsPublic);
            Assert.IsTrue(propertyWrapper.IsInherited);
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNull(propDef);
        }

        [Test]
        public void Test_Map_WhenOverriddenProp_ShouldReturnNull()
        {
            //When Prop inherits from another class (i.e. overrides previous
            //declaration the prop will be mapped as part of the base class
            // and should not be mapped as part of this class.
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOSubClassWithOverridenProps);
            var propertyInfo = classType.GetProperty("OverriddenProp");
            var propertyAutoMapper = new PropertyAutoMapper(propertyInfo);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.IsFalse(propertyWrapper.HasIgnoreAttribute);

            Assert.IsTrue(propertyInfo.PropertyType.CanMapToProp());
            Assert.IsTrue(propertyWrapper.IsPublic);
            Assert.IsTrue(propertyWrapper.IsOverridden);
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNull(propDef);
        }

        [Test]
        public void Test_Map_WhenImplementedProp_ShouldReturnPropDef()
        {
            //When Prop is implemented as part of an interface it should still be mapped
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOImplementingInterface);
            var propertyInfo = classType.GetProperty("ImplementedProp");
            var propertyAutoMapper = new PropertyAutoMapper(propertyInfo);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.IsFalse(propertyWrapper.HasIgnoreAttribute);

            Assert.IsTrue(propertyInfo.PropertyType.CanMapToProp());
            Assert.IsTrue(propertyWrapper.IsPublic);
            Assert.IsFalse(propertyWrapper.IsInherited);
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            Assert.AreEqual("ImplementedProp", propDef.PropertyName);
        }

        [Test]
        public void Test_Map_WhenCompulsoryAttribute_ShouldReturnMakePropDefCompulsoryTrue()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithCompulsoryProp);
            const string propName = "CompulsoryProp";
            var propertyInfo = classType.GetProperty(propName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            var propertyAutoMapper = new PropertyAutoMapper(propertyInfo);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.IsTrue(propertyWrapper.HasCompulsoryAttribute);

            Assert.IsTrue(propertyInfo.PropertyType.CanMapToProp());
            Assert.IsTrue(propertyWrapper.IsPublic);
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            Assert.AreEqual(propName, propDef.PropertyName);
            Assert.IsTrue(propDef.Compulsory);
        }

        [Test]
        public void Test_MapWithFake_WhenHasDefaultAttribute_ShouldReturnPropDefWithDefault()
        {
            //---------------Set up test pack-------------------
            var wrapper = GetMockPropWrapper();
            SetHasDefaultAttribute(wrapper, true, GetRandomString());
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            //---------------Assert Precondition----------------
            Assert.IsTrue(wrapper.HasDefaultAttribute);
//            Assert.IsTrue(wrapper.);
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            var defaultValueString = propDef.DefaultValueString;
            Assert.IsNotNull(defaultValueString);
        }

        [Test]
        public void Test_MapWithFake_WhenNotHasDefaultAttribute_ShouldReturnPropDefWithDefaultNull()
        {
            //---------------Set up test pack-------------------
            var wrapper = GetMockPropWrapper();
            SetHasDefaultAttribute(wrapper, false, GetRandomString());
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(wrapper.HasDefaultAttribute);
            Assert.IsNull(wrapper.GetAttribute<AutoMapDefaultAttribute>());
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            var defaultValueString = propDef.DefaultValueString;
            Assert.IsNull(defaultValueString);
        }

        [Test]
        public void Test_MapWithFake_GivenHasPropFieldNameAttribute_ShouldMapWithExpectedFieldName()
        {
            //---------------Set up test pack-------------------
            var wrapper = GetMockPropWrapper();
            var expectedDatabaseFieldName = GetRandomString();
            wrapper.Stub(wrapper1 => wrapper1.HasAttribute<AutoMapFieldNameAttribute>()).Return(true);
//            if(false)
//            {
            wrapper.Stub(propertyWrapper => propertyWrapper.GetAttribute<AutoMapFieldNameAttribute>()).Return(
                    new AutoMapFieldNameAttribute(expectedDatabaseFieldName));
            //}
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(wrapper.GetAttribute<AutoMapFieldNameAttribute>());
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            var databaseFieldName = propDef.DatabaseFieldName;
            Assert.AreEqual(expectedDatabaseFieldName,  databaseFieldName);
        }

        [Test]
        public void Test_MapWithFake_GivenNotHasPropFieldNameAttribute_ShouldMapWithPropertyName()
        {
            //---------------Set up test pack-------------------
            var wrapper = GetMockPropWrapper();
            wrapper.Stub(wrapper1 => wrapper1.HasAttribute<AutoMapFieldNameAttribute>()).Return(false);
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            //---------------Assert Precondition----------------
            Assert.IsNull(wrapper.GetAttribute<AutoMapFieldNameAttribute>());
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            var databaseFieldName = propDef.DatabaseFieldName;
            Assert.AreEqual(propDef.PropertyName, databaseFieldName);
        }

        [Test]
        public void Test_Map_WhenDefaultAttribute_ShouldReturnPropDefWithDefaultSet()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithDefaultProp);
            const string propName = "DefaultProp";
            var propertyInfo = classType.GetProperty(propName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            var propertyAutoMapper = new PropertyAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.IsTrue(propertyWrapper.HasDefaultAttribute);

            Assert.IsTrue(propertyInfo.PropertyType.CanMapToProp());
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            Assert.AreEqual(propName, propDef.PropertyName);
            Assert.NotNull(propDef.DefaultValueString);
        }

        #region DisplayName
        
        [Test]
        public void Test_MapWithFake_WhenNotHasDisplayNameAttribute_ShouldReturnPropDefWithDefaultDisplayName()
        {
            //---------------Set up test pack-------------------
            var wrapper = GetMockPropWrapper();
            var expectedDisplayName = GetRandomString();
            SetHasDisplayNameAttribute(wrapper, false, expectedDisplayName);
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(wrapper.HasDisplayNameAttribute);
            Assert.IsNull(wrapper.GetAttribute<AutoMapDisplayNameAttribute>());
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            var actualDisplayName = propDef.DisplayName;
            Assert.AreNotEqual(expectedDisplayName, actualDisplayName);
        }

        [Test]
        public void Test_MapWithFake_WhenHasDisplayNameAttribute_ShouldReturnPropDefWithDisplayName()
        {
            //---------------Set up test pack-------------------
            var wrapper = GetMockPropWrapper();
            var expectedDisplayName = GetRandomString();
            SetHasDisplayNameAttribute(wrapper, true, expectedDisplayName);
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            //---------------Assert Precondition----------------
            Assert.IsTrue(wrapper.HasDisplayNameAttribute);
            //            Assert.IsTrue(wrapper.);
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            var actualDisplayName = propDef.DisplayName;
            Assert.IsNotNull(actualDisplayName);
            Assert.AreEqual(expectedDisplayName, actualDisplayName);
        }

        [Test]
        public void Test_Map_WhenHasDisplayNameAttribute_ShouldReturnPropDefWithDisplayNameSet()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWitDisplayNameProp);
            const string propName = "DisplayNameProp";
            var propertyInfo = classType.GetProperty(propName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            var propertyAutoMapper = new PropertyAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.IsTrue(propertyWrapper.HasDisplayNameAttribute);

            Assert.IsTrue(propertyInfo.PropertyType.CanMapToProp());
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            Assert.AreEqual(propName, propDef.PropertyName);
            Assert.NotNull(propDef.DisplayName);
            Assert.AreEqual("MyDisplayName", propDef.DisplayName);
        }

        #endregion




        [Test]
        public void Test_Map_WhenNoCompulsoryAttribute_ShouldReturnMakePropDefCompulsoryFalse()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithCompulsoryProp);
            const string propName = "NonCompulsoryProp";
            var propertyInfo = classType.GetProperty(propName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            var propertyAutoMapper = new PropertyAutoMapper(propertyInfo);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.IsFalse(propertyWrapper.HasCompulsoryAttribute);

            Assert.IsTrue(propertyInfo.PropertyType.CanMapToProp());
            Assert.IsTrue(propertyWrapper.IsPublic);
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            Assert.IsFalse(propDef.Compulsory);
        }

        [Test]
        public void Test_Map_WhenHasAutoIncrementingAttribute_ShouldMakePropertyAutoIncrementing()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithAutoIncrementingProp);
            const string propName = "AutoIncrementingProp";
            var propertyInfo = classType.GetProperty(propName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            var propertyAutoMapper = new PropertyAutoMapper(propertyInfo);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.IsTrue(propertyWrapper.HasAutoIncrementingAttribute);
            Assert.IsTrue(propertyInfo.PropertyType.CanMapToProp());
            Assert.IsTrue(propertyWrapper.IsPublic);
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            Assert.IsTrue(propDef.AutoIncrementing);
        }

        [Test]
        public void Test_Map_WhenHasNoAutoIncrementingAttribute_ShouldNotMakePropertyAutoIncrementing()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithAutoIncrementingProp);
            const string propName = "NonAutoIncrementingProp";
            var propertyInfo = classType.GetProperty(propName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            var propertyAutoMapper = new PropertyAutoMapper(propertyInfo);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.IsFalse(propertyWrapper.HasAutoIncrementingAttribute);
            Assert.IsTrue(propertyInfo.PropertyType.CanMapToProp());
            Assert.IsTrue(propertyWrapper.IsPublic);
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            Assert.IsFalse(propDef.AutoIncrementing);
        }

        [Test]
        public void Test_MapWithFake_WhenHasReadWriteAttribute_ShouldReturnPropDefWithAttribute()
        {
            //---------------Set up test pack-------------------
            var wrapper = GetMockPropWrapper();
            var expectedReadWriteRule = RandomValueGenerator.GetRandomEnum<PropReadWriteRule>();
            SetReadWriteAttribute(wrapper, expectedReadWriteRule);
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(wrapper.GetAttribute<AutoMapReadWriteRuleAttribute>());
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            var actualReadWriteRule = propDef.ReadWriteRule;
            Assert.IsNotNull(actualReadWriteRule);
            Assert.AreEqual(expectedReadWriteRule, actualReadWriteRule);
        }
        [Test]
        public void Test_MapWithFake_WhenNotHasReadWriteAttribute_ShouldReturnPropDefWithReadWrite()
        {
            //---------------Set up test pack-------------------
            var wrapper = GetMockPropWrapper();
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            //---------------Assert Precondition----------------
            Assert.IsNull(wrapper.GetAttribute<AutoMapReadWriteRuleAttribute>());
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            var defaultValueString = propDef.DefaultValueString;
            Assert.IsNull(defaultValueString);
        }

        [Test]
        public void Test_ReflectionExtensions_GetBaseDefinition_WhenNotInherited_ShouldReturnSelf()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "PublicGetGuidProp";
            var classType = typeof(FakeBOWProps);
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            //---------------Execute Test ----------------------
            var basePropInfo = propertyInfo.GetBaseDefinition();
            //---------------Test Result -----------------------
            Assert.AreSame(propertyInfo, basePropInfo);
        }

        [Test]
        public void Test_ReflectionExtensions_GetBaseDefinition_WhenInheritedNotOverriden_ShouldNotReturnSelf()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOSubClassSuperHasDesc);
            var propertyInfo = classType.GetProperty("FakeBOSuperClassWithDescType");
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.IsTrue(propertyWrapper.IsInherited);
            //---------------Execute Test ----------------------
            var basePropInfo = propertyInfo.GetBaseDefinition();
            //---------------Test Result -----------------------
            Assert.AreNotSame(propertyInfo, basePropInfo);
        }

        [Test]
        public void Test_ReflectionExtensions_GetBaseDefinition_WhenOverriden_ShouldNotReturnSelf()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOSubClassWithOverridenProps);
            var propertyInfo = classType.GetProperty("OverriddenProp");
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            //---------------Execute Test ----------------------
            var basePropInfo = propertyInfo.GetBaseDefinition();
            //---------------Test Result -----------------------
            Assert.AreNotSame(propertyInfo, basePropInfo);
        }

        [Test]
        public void Test_MapWithFake_WhenIntPropRuleAttribute_ShouldReturnPropDefWithIntMinMaxRuleDefault()
        {
            //---------------Set up test pack-------------------
            var wrapper = GetMockPropWrapper();
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            //---------------Assert Precondition----------------
            Assert.IsNull(wrapper.GetAttribute<AutoMapIntPropRuleAttribute>());
            //---------------Execute Test ----------------------
            SetIntPropRuleAttributeWithDefaultConstructor(wrapper);
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            var defaultMinValue = (int) propDef.PropRules[0].Parameters["min"];
            var defaultMaxValue = (int) propDef.PropRules[0].Parameters["max"];
            Assert.AreEqual(int.MinValue,defaultMinValue);
            Assert.AreEqual(int.MaxValue,defaultMaxValue);
        }        
        
        [Test]
        public void Test_MapWithFake_WhenIntPropRuleAttribute_ShouldReturnPropDefWithIntMinMaxRuleWithValues()
        {
            //---------------Set up test pack-------------------
            var wrapper = GetMockPropWrapper();
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            var minValue = 9;
            var maxValue = 89;
            //---------------Assert Precondition----------------
            Assert.IsNull(wrapper.GetAttribute<AutoMapIntPropRuleAttribute>());
            //---------------Execute Test ----------------------
            SetIntPropRuleAttributeWithDefaultConstructor(wrapper,minValue,maxValue);
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            var propMinValue = (int) propDef.PropRules[0].Parameters["min"];
            var propMaxValue = (int) propDef.PropRules[0].Parameters["max"];
            Assert.AreEqual(minValue, propMinValue);
            Assert.AreEqual(maxValue, propMaxValue);
        }

        [Test]
        public void Test_MapWithFake_WhenStringLengthPropRuleAttribute_ShouldReturnPropDefWithStringLengthPropRuleDefault()
        {
            //---------------Set up test pack-------------------
            var wrapper = GetMockPropWrapper();
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
          
            //---------------Assert Precondition----------------
            Assert.IsNull(wrapper.GetAttribute<AutoMapStringLengthPropRuleAttribute>());
            //---------------Execute Test ----------------------
            SetStringLengthPropRuleAttributeWithDefaultConstructor(wrapper);
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            var propMinValue = (int)propDef.PropRules[0].Parameters["minLength"];
            var propMaxValue = (int)propDef.PropRules[0].Parameters["maxLength"];
            Assert.AreEqual(0, propMinValue);
            Assert.AreEqual(255, propMaxValue);
        }

        [Test]
        public void Test_MapWithFake_WhenStringLengthPropRuleAttribute_ShouldReturnPropDefWithStringLengthPropRuleWithValues()
        {
            //---------------Set up test pack-------------------
            var wrapper = GetMockPropWrapper();
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            var minLength = 10;
            var maxLength = 100;
            //---------------Assert Precondition----------------
            Assert.IsNull(wrapper.GetAttribute<AutoMapStringLengthPropRuleAttribute>());
            //---------------Execute Test ----------------------
        
            SetStringLengthPropRuleAttributeWithDefaultConstructor(wrapper,minLength,maxLength);
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            var propMinValue = (int)propDef.PropRules[0].Parameters["minLength"];
            var propMaxValue = (int)propDef.PropRules[0].Parameters["maxLength"];
            Assert.AreEqual(minLength, propMinValue);
            Assert.AreEqual(maxLength, propMaxValue);
        }      
        
        [Test]
        public void Test_MapWithFake_WhenDateTimePropRuleAttribute_ShouldReturnPropDefWithDateTimePropRuleWithValues()
        {
            //---------------Set up test pack-------------------
            var wrapper = GetMockPropWrapper();
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            var StartDate = DateTime.Now;
            var EndDate = StartDate.AddDays(10);
            //---------------Assert Precondition----------------
            Assert.IsNull(wrapper.GetAttribute<AutoMapDateTimePropRuleAttribute>());
            //---------------Execute Test ----------------------
        
            SetDateTimePropRuleAttributeWithDefaultConstructor(wrapper,StartDate,EndDate);
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            Assert.AreEqual(1,propDef.PropRules.Count);
        } 
        
        [Test]
        public void Test_MapWithFake_WhenDateTimeStringPropRuleAttribute_ShouldReturnPropDefWithDateTimePropRuleWithValues()
        {
            //---------------Set up test pack-------------------
            var wrapper = GetMockPropWrapper();
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            var StartDate = "Today";
            var EndDate = "Tomorrow";
            //---------------Assert Precondition----------------
            Assert.IsNull(wrapper.GetAttribute<AutoMapDateTimePropRuleAttribute>());
            //---------------Execute Test ----------------------

            SetDateTimeStringPropRuleAttributeWithDefaultConstructor(wrapper, StartDate, EndDate);
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            Assert.AreEqual(1,propDef.PropRules.Count);
        }



        [Test]
        public void Test_MapWithFake_WhenStringPatternMatchPropRuleAttribute_ShouldReturnPropDefWithStringPatternMatchPropRuleWithValue()
        {
            //---------------Set up test pack-------------------
            var wrapper = GetMockPropWrapper();
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            const string emailPatternMatch = @"\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b";
            //---------------Assert Precondition----------------
            Assert.IsNull(wrapper.GetAttribute<AutoMapStringPatternMatchPropRuleAttribute>());
            //---------------Execute Test ----------------------

           
            SetStringPatternMatchPropRuleAttributeWithDefaultConstructor(wrapper, emailPatternMatch);
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            Assert.AreEqual(1,propDef.PropRules.Count);
            var actualPattern = (string)propDef.PropRules[0].Parameters["patternMatch"];
            StringAssert.AreEqualIgnoringCase(emailPatternMatch, actualPattern);
        }

        [Test]
        public void Test_MapWithFake_WhenStringPatternMatchPropRuleAttribute_WhenMessageSet_ShouldReturnPropDefWithStringPatternMatchPropRuleWithMessage()
        {
            //---------------Set up test pack-------------------
            var wrapper = GetMockPropWrapper();
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            const string fourDigitStringRegex = @"^\d{4}$";
            var expectedMessage = GetRandomString();
            //---------------Assert Precondition----------------
            Assert.IsNull(wrapper.GetAttribute<AutoMapStringPatternMatchPropRuleAttribute>());
            //---------------Execute Test ----------------------
            SetStringPatternMatchPropRuleAttributeWithDefaultConstructor(wrapper, fourDigitStringRegex, expectedMessage);
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            Assert.AreEqual(1,propDef.PropRules.Count);
            var actualMessage = (string)propDef.PropRules[0].Parameters["patternMatchMessage"];
            StringAssert.AreEqualIgnoringCase(expectedMessage, actualMessage);
        }

        private static void SetStringPatternMatchPropRuleAttributeWithDefaultConstructor(PropertyWrapper wrapper, string regexPattern, string message)
        {
            wrapper.Stub(propertyWrapper => propertyWrapper.GetAttribute<AutoMapStringPatternMatchPropRuleAttribute>()).Return(
                  new AutoMapStringPatternMatchPropRuleAttribute(regexPattern, message));
            wrapper.Stub(propertyWrapper => propertyWrapper.HasStringPatternMatchRuleAttribute).Return(true);
        }

        private static void SetStringPatternMatchPropRuleAttributeWithDefaultConstructor(PropertyWrapper wrapper, string emailPatternMatch)
        {
            SetStringPatternMatchPropRuleAttributeWithDefaultConstructor(wrapper, emailPatternMatch, "");
        }


        private static string GetRandomString()
        {
            return RandomValueGenerator.GetRandomString();
        }

        private static PropertyWrapper GetMockPropWrapper()
        {
            PropertyWrapper wrapper = MockRepository.GenerateMock<FakePropertyWrapper>();
            wrapper.Stub(wrapper1 => wrapper1.IsPublic).Return(true);
            wrapper.Stub(wrapper1 => wrapper1.UndelyingPropertyType).Return(typeof(bool));
            wrapper.Stub(wrapper1 => wrapper1.Name).Return(GetRandomString());
            return wrapper;
        }


        private static void SetHasDefaultAttribute(PropertyWrapper wrapper, bool hasDefault, string defaultValue)
        {
            wrapper.Stub(wrapper1 => wrapper1.HasDefaultAttribute).Return(hasDefault);
            if(hasDefault)
            {
                wrapper.Stub(propertyWrapper => propertyWrapper.GetAttribute<AutoMapDefaultAttribute>()).Return(
                    new AutoMapDefaultAttribute(defaultValue));
            }
        }
        private static void SetHasDisplayNameAttribute(PropertyWrapper wrapper, bool hasDisplayName, string displayName)
        {
            wrapper.Stub(wrapper1 => wrapper1.HasDisplayNameAttribute).Return(hasDisplayName);
            if (hasDisplayName)
            {
                wrapper.Stub(propertyWrapper => propertyWrapper.GetAttribute<AutoMapDisplayNameAttribute>()).Return(
                    new AutoMapDisplayNameAttribute(displayName));
            }
        }


        private static void SetReadWriteAttribute(PropertyWrapper wrapper, PropReadWriteRule rule)
        {
            wrapper.Stub(propertyWrapper => propertyWrapper.GetAttribute<AutoMapReadWriteRuleAttribute>()).Return(
                    new AutoMapReadWriteRuleAttribute(rule));
            wrapper.Stub(propertyWrapper => propertyWrapper.HasReadWriteRuleAttribute).Return(true);
        }

        private static void SetIntPropRuleAttributeWithDefaultConstructor(PropertyWrapper wrapper)
        {
            wrapper.Stub(propertyWrapper => propertyWrapper.GetAttribute<AutoMapIntPropRuleAttribute>()).Return(
                    new AutoMapIntPropRuleAttribute());
            wrapper.Stub(propertyWrapper => propertyWrapper.HasIntPropRuleAttribute).Return(true);
        }
        
        private static void SetIntPropRuleAttributeWithDefaultConstructor(PropertyWrapper wrapper,int minValue,int maxValue)
        {
            wrapper.Stub(propertyWrapper => propertyWrapper.GetAttribute<AutoMapIntPropRuleAttribute>()).Return(
                    new AutoMapIntPropRuleAttribute(minValue,maxValue));
            wrapper.Stub(propertyWrapper => propertyWrapper.HasIntPropRuleAttribute).Return(true);
        }

        private void SetStringLengthPropRuleAttributeWithDefaultConstructor(PropertyWrapper wrapper)
        {
            wrapper.Stub(propertyWrapper => propertyWrapper.GetAttribute<AutoMapStringLengthPropRuleAttribute>()).Return(
                   new AutoMapStringLengthPropRuleAttribute());
            wrapper.Stub(propertyWrapper => propertyWrapper.HasStringLengthRuleAttribute).Return(true);
        }

        private void SetStringLengthPropRuleAttributeWithDefaultConstructor(PropertyWrapper wrapper,int minLength,int maxLength)
        {
            wrapper.Stub(propertyWrapper => propertyWrapper.GetAttribute<AutoMapStringLengthPropRuleAttribute>()).Return(
                   new AutoMapStringLengthPropRuleAttribute(minLength,maxLength));
            wrapper.Stub(propertyWrapper => propertyWrapper.HasStringLengthRuleAttribute).Return(true);
        }

        private void SetDateTimePropRuleAttributeWithDefaultConstructor(PropertyWrapper wrapper, DateTime startDate, DateTime endDate)
        {
            wrapper.Stub(propertyWrapper => propertyWrapper.GetAttribute<AutoMapDateTimePropRuleAttribute>()).Return(
                   new AutoMapDateTimePropRuleAttribute(startDate, endDate));
            wrapper.Stub(propertyWrapper => propertyWrapper.HasDateTimeRuleAttribute).Return(true);
        }

        private void SetDateTimeStringPropRuleAttributeWithDefaultConstructor(PropertyWrapper wrapper, string startDate, string endDate)
        {
            wrapper.Stub(propertyWrapper => propertyWrapper.GetAttribute<AutoMapDateTimePropRuleAttribute>()).Return(
                   new AutoMapDateTimePropRuleAttribute(startDate, endDate));
            wrapper.Stub(propertyWrapper => propertyWrapper.HasDateTimeStringRuleAttribute).Return(true);
        }
//
//        private TypeWrapper GetFakeTypeWrapper()
//        {
//            return MockRepository.GenerateMock<FakeTypeWrapper>();
//        }
    }

}