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
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.Smooth.Test;
using NUnit.Framework;
using TestProject.BO;

// ReSharper disable InconsistentNaming
namespace Habanero.Fluent.Tests
{
    [TestFixture]
    public class TestPropDefBuilder
    {
        [Test]
        public void Test_Build_WithPropertyName_ShouldUseDefaults()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propDef = GetPropDefBuilder<Car>()
                .WithPropertyName(propertyName)
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.AreEqual("System", propDef.PropertyTypeAssemblyName);
            Assert.AreEqual("String", propDef.PropertyTypeName);
            Assert.AreEqual(PropReadWriteRule.ReadWrite, propDef.ReadWriteRule);
            Assert.AreEqual(propertyName, propDef.DatabaseFieldName);
            Assert.IsNull(propDef.DefaultValueString);
            Assert.IsFalse(propDef.Compulsory);
            Assert.IsFalse(propDef.AutoIncrementing);
            Assert.IsFalse(propDef.KeepValuePrivate);
            Assert.AreEqual(Int32.MaxValue, propDef.Length);
        }


        [Test]
        public void Test_BuildPropDef_WithNoPropName_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            const string propertyName = "";
            //---------------Assert Precondition----------------
            Assert.IsEmpty(propertyName);
            //---------------Execute Test ----------------------
            try
            {
                GetPropDefBuilder<Car>()
                    .WithPropertyName(propertyName)
                    .Build();
                Assert.Fail("Expected to throw an HabaneroArgumentException");
            }
                //---------------Test Result -----------------------
            catch (HabaneroArgumentException ex)
            {
                StringAssert.Contains("The argument 'propertyName' is not valid. Argument cannot be a zero length string", ex.Message);
            }
        }

        [Test]
        public void Test_Build_WithAssemblyName_NoTypeName_ShouldBuildWithPropTypeNameEqString()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            string assemblyName = "A" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propDef = GetPropDefBuilder<Car>()
                .WithPropertyName(propertyName)
                .WithAssemblyName(assemblyName)
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.AreEqual(assemblyName, propDef.PropertyTypeAssemblyName);
            Assert.AreEqual("String", propDef.PropertyTypeName);
        }
        [Test]
        public void Test_Build_WithPropName_ShouldSetFieldNameEqPropName()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propDef = GetPropDefBuilder<Car>()
                .WithPropertyName(propertyName)
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.AreEqual(propertyName, propDef.DatabaseFieldName);
        }

        [Test]
        public void Test_Build_WithPropName_ShouldSetDisplayName_EqPastelCasePropName()
        {
            //---------------Set up test pack-------------------
            string propertyName = "PastelCase";
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propDef = GetPropDefBuilder<Car>()
                .WithPropertyName(propertyName)
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.AreEqual("Pastel Case", propDef.DisplayName);
        }

        [Test]
        public void Test_Build_WithTypeName_ShouldBuildWithTypeNameAssemblyEqSystem()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            string typeName = "T" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propDef = GetPropDefBuilder<Car>()
                .WithPropertyName(propertyName)
                .WithTypeName(typeName)
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.AreEqual(typeName, propDef.PropertyTypeName);
            Assert.AreEqual("System", propDef.PropertyTypeAssemblyName);
        }

        [Test]
        public void Test_Build_WithType_ShouldBuildWithTypeNameAndAssembly()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propDef = GetPropDefBuilder<Car>()
                .WithPropertyName(propertyName)
                .WithType(typeof(Car))
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.AreEqual("Car", propDef.PropertyTypeName);
            Assert.AreEqual("TestProject.BO", propDef.PropertyTypeAssemblyName);
        }
        [Test]
        public void Test_Build_WithTypeGeneric_ShouldBuildWithTypeNameAndAssembly()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propDef = GetPropDefBuilder<Car>()
                .WithPropertyName(propertyName)
                .WithType<Car>()
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual("Car", propDef.PropertyTypeName);
            Assert.AreEqual("TestProject.BO", propDef.PropertyTypeAssemblyName);
        }

        [Test]
        public void Test_Build_WithReadWriteRule_ShouldSetReadWriteRule()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            const PropReadWriteRule propReadWriteRule = PropReadWriteRule.WriteNew;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propDef = GetPropDefBuilder<Car>()
                .WithPropertyName(propertyName)
                .WithReadWriteRule(propReadWriteRule)
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.AreEqual(propReadWriteRule, propDef.ReadWriteRule);
        }


        [Test]
        public void Test_Build_WithDatabaseFieldName_ShouldSetDatabaseFieldName()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            string fieldName = "F" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var propDef = GetPropDefBuilder<Car>()
                .WithPropertyName(propertyName)
                .WithDatabaseFieldName(fieldName)
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.AreEqual(fieldName, propDef.DatabaseFieldName);
        }

        [Test]
        public void Test_Build_WithDefaultValue_ShouldSetDefaultValue()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            string defaultValue = "V" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var propDef = GetPropDefBuilder<Car>()
                .WithPropertyName(propertyName)
                .WithDefaultValue(defaultValue)
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.AreEqual(defaultValue, propDef.DefaultValueString);
        }

        [Test]
        public void Test_Build_WithCompulsoryProp_ShouldSetAsCompulsory()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propDef = GetPropDefBuilder<Car>()
                .WithPropertyName(propertyName)
                .IsCompulsory()
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.IsTrue(propDef.Compulsory);
        }

        [Test]
        public void Test_Build_WithAutoIncrementingProp_ShouldSetAsAutoIncrementing()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propDef = GetPropDefBuilder<Car>()
                .WithPropertyName(propertyName)
                .IsAutoIncrementing()
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.IsTrue(propDef.AutoIncrementing);
        }
        [Test]
        public void Test_Build_WithDescription_ShouldSetDescription()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            string description = "D" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propDef = GetPropDefBuilder<Car>()
                .WithPropertyName(propertyName)
                .WithDescription(description)
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.AreEqual(description, propDef.Description);
        }

        [Test]
        public void Test_Build_WithKeepValuePrivate_ShouldSetKeepVauePrivate()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propDef = GetPropDefBuilder<Car>()
                .WithPropertyName(propertyName)
                .KeepValuePrivate()
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.IsTrue(propDef.KeepValuePrivate);
        }

        [Test]
        public void Test_Build_WithDisplayName_ShouldSetDisplayName()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            string displayName = "P" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propDef = GetPropDefBuilder<Car>()
                .WithPropertyName(propertyName)
                .WithDisplayName(displayName)
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.AreEqual(displayName, propDef.DisplayName);
        }


        private static PropDefBuilder<T> GetPropDefBuilder<T>() where T : BusinessObject
        {
            return new PropDefBuilder<T>();
        }


    }
}