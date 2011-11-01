#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
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
#endregion
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
            return new PropertiesDefBuilder<T>(new ClassDefBuilder2<T>(new ClassDefBuilder<T>(), new List<PropDefBuilder<T>>(), new List<string> { RandomValueGenerator.GetRandomString() }), new List<PropDefBuilder<T>>());
        }


    }
}