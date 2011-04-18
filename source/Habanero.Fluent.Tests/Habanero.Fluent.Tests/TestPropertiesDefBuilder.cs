using System;
using System.Collections.Generic;
using Habanero.BO;
using Habanero.Smooth.Test;
using NUnit.Framework;
using TestProject.BO;

// ReSharper disable InconsistentNaming
namespace Habanero.Fluent.Tests
{
    [TestFixture]
    public class TestPropertiesDefBuilder
    {
        [Test]
        public void Test_Build_WithLambdaProp_ShouldSetPropNameAndType()
        {

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
            //---------------Execute Test ----------------------
            var propDef = GetPropertiesDefBuilder<Car>()
                .Property(c => c.NoOfDoors)
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual("NoOfDoors", propDef.PropertyName);
            Assert.AreEqual("System", propDef.PropertyTypeAssemblyName);
            Assert.AreEqual("Int32", propDef.PropertyTypeName);
        }

        private static PropertiesDefBuilder<T> GetPropertiesDefBuilder<T>() where T : BusinessObject
        {
            return new PropertiesDefBuilder<T>(new ClassDefBuilder2<T>(new ClassDefBuilder<T>(), new List<string> { RandomValueGenerator.GetRandomString() }), new List<PropDefBuilder<T>>());
        }


    }
}