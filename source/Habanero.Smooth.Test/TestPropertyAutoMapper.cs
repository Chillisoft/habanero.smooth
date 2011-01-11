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
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Smooth.Test.ValidFakeBOs;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Smooth.Test
{
    [TestFixture]
    public class TestPropertyAutoMapper
    {

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

        [TestCase("PublicGetGuidProp", typeof(Guid), "System.Guid")]
        [TestCase("PublicGetNullableGuidProp", typeof(Guid?), "System.Guid")]
        [TestCase("PublicStringProp", typeof(String), "System.String")]
        [TestCase("PublicIntProp", typeof(Int32), "System.Int32")]
        [TestCase("PublicEnumProp", typeof(FakeEnum), "Habanero.Smooth.Test.FakeEnum")]
        [TestCase("PublicNullableEnumProp", typeof(FakeEnum?), "Habanero.Smooth.Test.FakeEnum")]
        [TestCase("PublicPropWithAtt", typeof(float?), "System.Single")]
        public void Test_MapProperty_WhenPublicProp_ShouldSetPropNameAndType(string propName, Type propType, string typeName)
        {
            //---------------Set up test pack-------------------
            string expectedPropName = propName;
            var classType = typeof (FakeBOWProps);
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
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
            PropertyInfo propertyInfo = classType.GetProperty("ClassDef");
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
            PropertyInfo propertyInfo = classType.GetProperty("PublicStringProp");
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
            PropertyInfo propertyInfo = classType.GetProperty("PublicWithIgnoreAtt");
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
        public void Test_PropWithNonIgnoreAttribute_ShouldReturnPropDef()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWProps);
            PropertyInfo propertyInfo = classType.GetProperty("PublicPropWithAtt");
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
            PropertyInfo propertyInfo = classType.GetProperty("PrivateStringProp", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
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
            PropertyInfo propertyInfo = classType.GetProperty("FakeBOSuperClassWithDescType");
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
            PropertyInfo propertyInfo = classType.GetProperty("OverriddenProp");
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
            PropertyInfo propertyInfo = classType.GetProperty("ImplementedProp");
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
            PropertyInfo propertyInfo = classType.GetProperty(propName);
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
            PropertyWrapper wrapper = GetMockPropWrapper();
            SetHasDefaultAttribute(wrapper, true, GetRandomString());
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            //---------------Assert Precondition----------------
            Assert.IsTrue(wrapper.HasDefaultAttribute);
//            Assert.IsTrue(wrapper.);
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            string defaultValueString = propDef.DefaultValueString;
            Assert.IsNotNull(defaultValueString);
        }
        [Test]
        public void Test_MapWithFake_WhenNotHasDefaultAttribute_ShouldReturnPropDefWithDefaultNull()
        {
            //---------------Set up test pack-------------------
            PropertyWrapper wrapper = GetMockPropWrapper();
            SetHasDefaultAttribute(wrapper, false, GetRandomString());
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(wrapper.HasDefaultAttribute);
            Assert.IsNull(wrapper.GetAttribute<AutoMapDefaultAttribute>());
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            string defaultValueString = propDef.DefaultValueString;
            Assert.IsNull(defaultValueString);
        }

        [Test]
        public void Test_Map_WhenDefaultAttribute_ShouldReturnPropDefWithDefaultSet()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithDefaultProp);
            const string propName = "DefaultProp";
            PropertyInfo propertyInfo = classType.GetProperty(propName);
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

        [Test]
        public void Test_Map_WhenNoCompulsoryAttribute_ShouldReturnMakePropDefCompulsoryFalse()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithCompulsoryProp);
            const string propName = "NonCompulsoryProp";
            PropertyInfo propertyInfo = classType.GetProperty(propName);
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
            PropertyInfo propertyInfo = classType.GetProperty(propName);
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
            PropertyInfo propertyInfo = classType.GetProperty(propName);
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
            PropertyWrapper wrapper = GetMockPropWrapper();
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
            PropertyWrapper wrapper = GetMockPropWrapper();
            var propertyAutoMapper = new PropertyAutoMapper(wrapper);
            //---------------Assert Precondition----------------
            Assert.IsNull(wrapper.GetAttribute<AutoMapReadWriteRuleAttribute>());
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            string defaultValueString = propDef.DefaultValueString;
            Assert.IsNull(defaultValueString);
        }

        [Test]
        public void Test_ReflectionExtensions_GetBaseDefinition_WhenNotInherited_ShouldReturnSelf()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "PublicGetGuidProp";
            var classType = typeof(FakeBOWProps);
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
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
            PropertyInfo propertyInfo = classType.GetProperty("FakeBOSuperClassWithDescType");
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
            PropertyInfo propertyInfo = classType.GetProperty("OverriddenProp");
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            //---------------Execute Test ----------------------
            var basePropInfo = propertyInfo.GetBaseDefinition();
            //---------------Test Result -----------------------
            Assert.AreNotSame(propertyInfo, basePropInfo);
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
        private static void SetReadWriteAttribute(PropertyWrapper wrapper, PropReadWriteRule rule)
        {
            wrapper.Stub(propertyWrapper => propertyWrapper.GetAttribute<AutoMapReadWriteRuleAttribute>()).Return(
                    new AutoMapReadWriteRuleAttribute(rule));
            wrapper.Stub(propertyWrapper => propertyWrapper.HasReadWriteRuleAttribute).Return(true);
        }

        private TypeWrapper GetFakeTypeWrapper()
        {
            return MockRepository.GenerateMock<FakeTypeWrapper>();
        }
    }

}