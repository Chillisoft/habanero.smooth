using System;
using System.Reflection;
using AutoMappingHabanero.ReflectionWrappers;
using AutoMappingHabanero.Test.ValidFakeBOs;
using Habanero.Base;
using NUnit.Framework;
using Rhino.Mocks;

namespace AutoMappingHabanero.Test
{
    [TestFixture]
    public class TestPropertyMapperExtensions
    {
        [TestCase("PublicGetGuidProp", typeof(Guid), "System.Guid")]
        [TestCase("PublicGetNullableGuidProp", typeof(Guid?), "System.Guid")]
        [TestCase("PublicStringProp", typeof(String), "System.String")]
        [TestCase("PublicIntProp", typeof(Int32), "System.Int32")]
        [TestCase("PublicEnumProp", typeof(FakeEnum), "AutoMappingHabanero.Test.FakeEnum")]
        [TestCase("PublicNullableEnumProp", typeof(FakeEnum?), "AutoMappingHabanero.Test.FakeEnum")]
        [TestCase("PublicPropWithAtt", typeof(float?), "System.Single")]
        public void Test_CanMapToProp_WhenPublicProp_ShouldSetPropNameAndType(string propName, Type propType, string typeName)
        {
            //---------------Set up test pack-------------------
            string expectedPropName = propName;
            var classType = typeof (FakeBOWProps);
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.AreEqual(expectedPropName, propertyInfo.Name);
            Assert.AreEqual(propType, propertyInfo.PropertyType);
            //---------------Execute Test ----------------------
            var canMapToProp = propertyInfo.PropertyType.CanMapToProp();
            //---------------Test Result -----------------------
            Assert.IsTrue(canMapToProp);
        }

        [Test]
        public void Test_CanMapToProp_WhenTypeNotSystem_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWProps);
            PropertyInfo propertyInfo = classType.GetProperty("ClassDef");
            var propTypeName = propertyInfo.PropertyType.FullName;
            //---------------Assert Precondition----------------
            propTypeName.ShouldNotStartWith("System.");
            Assert.IsFalse(propertyInfo.ToPropertyWrapper().HasIgnoreAttribute);
            Assert.IsFalse(propertyInfo.PropertyType.CanMapToProp());
            //---------------Execute Test ----------------------
            var canMapToProp = propertyInfo.PropertyType.CanMapToProp();
            //---------------Test Result -----------------------
            Assert.IsFalse(canMapToProp);
        }
/*

        private TypeWrapper GetFakeTypeWrapper()
        {
            return MockRepository.GenerateMock<FakeTypeWrapper>();
        }*/
    }
}