using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMappingHabanero.ReflectionWrappers;
using AutoMappingHabanero.Test.ExtensionMethods;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.Util;
using NUnit.Framework;
using Rhino.Mocks;

namespace AutoMappingHabanero.Test
{
    [TestFixture]
    public class TestIdentiyAutoMapper
    {
        [SetUp]
        public void Setup()
        {
            AllClassesAutoMapper.PropNamingConvention = new DefaultPropNamingConventions();
        }
        [Test]
        public void Test_Map_WhenHasPKProp_ShouldMapPK()
        {
            //---------------Set up test pack-------------------
            IClassDef cDef = GetClassDefWithPropsMapped(typeof(FakeBOWProps));
            //---------------Assert Precondition----------------
            AssertPrimaryKeyPropExists<FakeBOWProps>();
            //---------------Execute Test ----------------------
            var primaryKey = cDef.MapIndentity();
            //---------------Test Result -----------------------
            primaryKey.AssertHasOneGuidProp();
        }

        [Test]
        public void Test_Construct_WithNullclassDef_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                new IdentiyAutoMapper(null);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("classDef", ex.ParamName);
            }
        }

        [Test] public void Test_Map_WhenNullCDef_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            IClassDef cDef = null;
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var primaryKey = cDef.MapIndentity();
            //---------------Test Result -----------------------
            Assert.IsNull(primaryKey);
        }

        [Test]
        public void Test_Map_WhenNotHasPKProp_ShouldCreatePrimaryKeyProp()
        {
            //---------------Set up test pack-------------------
            IClassDef cDef = GetClassDefWithPropsMapped<FakeBONoPK>();
            //---------------Assert Precondition----------------
            AssertPrimaryKeyPropNotExists<FakeBONoPK>();    
            //---------------Execute Test ----------------------
            var primaryKey = cDef.MapIndentity();
            //---------------Test Result -----------------------
            primaryKey.AssertHasOneGuidProp();
            var propDef = primaryKey[0];
            Assert.AreEqual("FakeBONoPKID", propDef.PropertyName);
            IPropDef pkProp = cDef.GetPropDef(propDef.PropertyName, false);
            Assert.IsNotNull(pkProp);
            Assert.AreSame(typeof(Guid), pkProp.PropertyType);
            Assert.AreEqual(PropReadWriteRule.WriteNew, pkProp.ReadWriteRule);
            Assert.IsTrue(pkProp.Compulsory);
            Assert.AreEqual("System.Guid", pkProp.PropertyTypeName);
        }

        [Test]
        public void Test_Map_WhenHasPropDefined_ShouldHaveSamePropDefAsClassDef()
        {
            //---------------Set up test pack-------------------
            IClassDef cDef = GetClassDefWithPropsMapped<FakeBOWProps>();
            var expectedIDProp = GetPKPropDefWithStandardNamingConvention(cDef);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(cDef);
            Assert.IsNotNull(expectedIDProp);
            AssertPrimaryKeyPropExists<FakeBOWProps>();
            //---------------Execute Test ----------------------
            var primaryKey = cDef.MapIndentity();
            //---------------Test Result -----------------------
            primaryKey.AssertHasOneGuidProp();
            var propDef = primaryKey[0];
            Assert.AreSame(expectedIDProp, propDef);
        }


        [Test]
        public void Test_Map_WhenStdNamingPropNonExist_WhenUsePrimaryKeyAttribute_ShouldMapAttributeProp()
        {
            //---------------Set up test pack-------------------
            IClassDef cDef = GetClassDefWithPropsMapped<FakeBOAttributePK>();
            IPropDef stdNamingProp = GetPKPropDefWithStandardNamingConvention(cDef);
            //---------------Assert Precondition----------------
            Assert.Greater(cDef.PropDefcol.Count, 0);
            Assert.IsNull(stdNamingProp, "There should not be a property with a standard naming convention");
            Assert.IsTrue(cDef.HasPrimaryKeyAttribute("PublicGuidProp"));
            //---------------Execute Test ----------------------
            var primaryKeyDef = cDef.MapIndentity();
            //---------------Test Result -----------------------
            primaryKeyDef.AssertHasOneGuidProp();
            Assert.AreEqual("PublicGuidProp", primaryKeyDef[0].PropertyName);
        }

        [Test]
        public void Test_Map_WhenStdNamePropAndAttributeProp_ShouldUseAttributeDefinedProp()
        {
            //---------------Set up test pack-------------------
            IClassDef cDef = GetClassDefWithPropsMapped<FakeBOAttributePKAndPKNaming>();
            //---------------Assert Precondition----------------
            AssertPrimaryKeyPropExists<FakeBOAttributePKAndPKNaming>();
            Assert.IsTrue(cDef.HasPrimaryKeyAttribute("PublicGuidProp"));
            //---------------Execute Test ----------------------
            var primaryKeyDef = cDef.MapIndentity();
            //---------------Test Result -----------------------
            primaryKeyDef.AssertHasOneGuidProp();
            Assert.AreEqual("PublicGuidProp", primaryKeyDef[0].PropertyName);
        }

        [Test]
        public void Test_Map_WhenTwoPropsWithPrimaryKeyAttribute_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            IClassDef cDef = GetClassDefWithPropsMapped<FakeBOTwoPropsAttributePK>();
            IPropDef stdNamingProp = GetPKPropDefWithStandardNamingConvention(cDef);
            //---------------Assert Precondition----------------
            Assert.IsNull(stdNamingProp, "There should not be a property with a standard naming convention");

            Assert.IsTrue(cDef.HasPrimaryKeyAttribute("PublicGuidProp"));
            Assert.IsTrue(cDef.HasPrimaryKeyAttribute("AnotherPrimaryKeyProp"));
            //---------------Execute Test ----------------------

            try
            {
                cDef.MapIndentity();
                Assert.Fail("Expected to throw an HabaneroDeveloperException");
            }
                //---------------Test Result -----------------------
            catch (HabaneroApplicationException ex)
            {
                StringAssert.Contains("You cannot auto map Business Objects", ex.Message);
                StringAssert.Contains(" with Composite Primary Keys. Please map using ClassDefs", ex.Message);
            }
        }

        [Test]
        public void Test_IdentityNameConvention_ShouldBeDefaultIfNotSet()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var idConvention = IdentiyAutoMapper.PropNamingConvention;
            //---------------Test Result -----------------------
            idConvention.ShouldBeOfType<DefaultPropNamingConventions>();
        }

        [Test]
        public void Test_SetIdentityNameConvention_ShouldSetCustomConvention()
        {
            //---------------Set up test pack-------------------
            INamingConventions expectedConvention = MockRepository.GenerateMock<INamingConventions>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            AllClassesAutoMapper.PropNamingConvention = expectedConvention;
            //---------------Test Result -----------------------
            Assert.AreSame(expectedConvention, IdentiyAutoMapper.PropNamingConvention);
        }

        private static IPropDef GetPKPropDefWithStandardNamingConvention(IClassDef cDef)
        {
            string propertyName = GetStandardPKPropName(cDef.ClassType);
            return cDef.GetPropDef(propertyName, false);
        }

        private static string GetStandardPKPropName(Type classType)
        {
            var idConvention = IdentiyAutoMapper.PropNamingConvention;
            return idConvention.GetIDPropertyName(classType.ToTypeWrapper());
        }

        private static IClassDef GetClassDefWithPropsMapped<T>()
        {
            return GetClassDefWithPropsMapped(typeof (T));
        }
        private static IClassDef GetClassDefWithPropsMapped(Type type)
        {
            ClassAutoMapper classAutoMapper = new ClassAutoMapper(type.ToTypeWrapper());
            IClassDef classDef = CreateClassDef(classAutoMapper);
//            IClassDef classDef = classAutoMapper.CreateClassDef(type);

//            classDef.MapProperties();
            ReflectionUtilities.ExecutePrivateMethod(classAutoMapper, "MapProperties");
            return classDef;
        }

        private static IClassDef CreateClassDef(ClassAutoMapper classAutoMapper)
        {
            return ReflectionUtilities.ExecutePrivateMethod(classAutoMapper, "CreateClassDef") as IClassDef;
        }

        private static void AssertPrimaryKeyPropExists<T>()
        {
            AssertPropertyExists<T>(typeof(T).Name + "ID");
        }
        private static void AssertPrimaryKeyPropNotExists<T>()
        {
            AssertPropertyNotExists<T>(typeof(T).Name + "ID");
        }

        private static void AssertPropertyNotExists<T>(string propName)
        {
            Assert.IsNull(GetProperty<T>(propName), propName + " should not exist on the Type " + typeof(T));
        }
        private static void AssertPropertyExists<T>(string propName)
        {
            Assert.IsNotNull(GetProperty<T>(propName), propName + " should exist on the Type " + typeof(T));
        }

        private static PropertyInfo GetProperty<T>(string propName)
        {
            return typeof (T).GetProperty(propName);
        }
    }
}