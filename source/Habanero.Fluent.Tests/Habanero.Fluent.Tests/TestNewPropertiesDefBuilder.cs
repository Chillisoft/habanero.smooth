using System;
using System.Collections.Generic;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.Smooth.Test;
using NUnit.Framework;
using TestProject.BO;

namespace Habanero.Fluent.Tests
{
    [TestFixture]
    public class TestNewPropertiesDefBuilder
    {


        [Test]
        public void Test_Build_WithLambdaProp_ShouldSetPropNameAndType()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propDef = GetPropertiesDefBuilder<Car>()
                                .Property(c => c.NoOfDoors)
                                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual("NoOfDoors", propDef.PropertyName);
            Assert.AreEqual("System", propDef.PropertyTypeAssemblyName);
            Assert.AreEqual("Int32", propDef.PropertyTypeName);
            Assert.AreSame(typeof(Int32), propDef.PropertyType);
        }

        [Test]
        public void Test_Build_WithLambdaProp_GuidProp_ShouldSetPropTypeGuid()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propDef = GetPropertiesDefBuilder<Car>()
                                .Property(c => c.VehicleID)
                                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual("VehicleID", propDef.PropertyName);
            Assert.AreEqual("System", propDef.PropertyTypeAssemblyName);
            Assert.AreEqual("Guid", propDef.PropertyTypeName);
            Assert.AreSame(typeof(Guid), propDef.PropertyType);
        }

        [Test]
        public void Test_Build_WithLambdaProp_ShouldSetPropValues()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propDef = GetPropertiesDefBuilder<Car>()
                .Property(c => c.NoOfDoors)
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual("NoOfDoors", propDef.PropertyName);
            Assert.AreEqual("System", propDef.PropertyTypeAssemblyName);
            Assert.AreEqual("Int32", propDef.PropertyTypeName);
        }

        private static NewPropertiesDefBuilder<T> GetPropertiesDefBuilder<T>() where T : BusinessObject
        {
            return new NewPropertiesDefBuilder<T>(new NewClassDefBuilder2<T>(new NewClassDefBuilder<T>(), new List<string> { RandomValueGenerator.GetRandomString() }), new List<NewPropDefBuilder<T>>());
        }


    }
}