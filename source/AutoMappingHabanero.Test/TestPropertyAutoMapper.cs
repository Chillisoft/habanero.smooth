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
        [TestCase("PublicEnumProp", typeof(FakeEnum), "AutoMappingHabanero.Test.FakeEnum")]
        [TestCase("PublicNullableEnumProp", typeof(FakeEnum?), "AutoMappingHabanero.Test.FakeEnum")]
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
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOSubClassSuperhasDesc);
            PropertyInfo propertyInfo = classType.GetProperty("FakeBOSuperClassWithDescType");
            var propertyAutoMapper = new PropertyAutoMapper(propertyInfo);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.IsFalse(propertyWrapper.HasIgnoreAttribute);

            Assert.IsTrue(propertyInfo.PropertyType.CanMapToProp());
            Assert.IsTrue(propertyWrapper.IsPublic);
            Assert.IsTrue(propertyWrapper.IsInheritedProp);
            //---------------Execute Test ----------------------
            var propDef = propertyAutoMapper.MapProperty();
            //---------------Test Result -----------------------
            Assert.IsNull(propDef);
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

        private TypeWrapper GetFakeTypeWrapper()
        {
            return MockRepository.GenerateMock<FakeTypeWrapper>();
        }
    }

}