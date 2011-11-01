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
using System.Reflection;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Smooth.Test.ValidFakeBOs;
using Habanero.Base;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Smooth.Test
{
    [TestFixture]
    public class TestPropertyMapperExtensions
    {
        [TestCase("PublicGetGuidProp", typeof(Guid), "System.Guid")]
        [TestCase("PublicGetNullableGuidProp", typeof(Guid?), "System.Guid")]
        [TestCase("PublicStringProp", typeof(String), "System.String")]
        [TestCase("PublicIntProp", typeof(Int32), "System.Int32")]
        [TestCase("PublicEnumProp", typeof(FakeEnum), "Habanero.Smooth.Test.FakeEnum")]
        [TestCase("PublicNullableEnumProp", typeof(FakeEnum?), "Habanero.Smooth.Test.FakeEnum")]
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