using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMappingHabanero.ReflectionWrappers;
using Habanero.Base;
using NUnit.Framework;

namespace AutoMappingHabanero.Test
{
    [TestFixture]
    public class TestIdentityPropNameConvetion
    {
        [Test]
        public void Test_DefaultNamingConvention_GetPropertyNameGeneric()
        {
            //---------------Set up test pack-------------------
            INamingConventions convention = new DefaultPropNamingConventions();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propertyName = convention.GetIDPropertyName<FakeBOWProps>();
            //---------------Test Result -----------------------
            Assert.AreEqual("FakeBOWPropsID", propertyName);
        }

        [Test]
        public void Test_DefaultNamingConvention_GetPropertyName()
        {
            //---------------Set up test pack-------------------
            INamingConventions convention = new DefaultPropNamingConventions();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propertyName = convention.GetIDPropertyName(typeof(FakeBOWProps).ToTypeWrapper());
            //---------------Test Result -----------------------
            Assert.AreEqual("FakeBOWPropsID", propertyName);
        }
    }
}