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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Base;
using NUnit.Framework;

namespace Habanero.Smooth.Test
{
    [TestFixture]
    public class TestAutoMappingAttributes
    {
        // ReSharper disable InconsistentNaming
        [Test]
        public void Test_Default_Construct_WithValue_ShouldSetValue()
        {
            //---------------Set up test pack-------------------
            const string defaultValue = "Value";            
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            AutoMapDefaultAttribute attribute = new AutoMapDefaultAttribute(defaultValue);
            //---------------Test Result -----------------------
            Assert.AreSame(defaultValue, attribute.DefaultValue);
        }

        [Test]
        public void Test_ReadWriteRule_Construct_WithValue_ShouldSetValue()
        {
            //---------------Set up test pack-------------------
            PropReadWriteRule expecteRwRule = RandomValueGenerator.GetRandomEnum<PropReadWriteRule>();

            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            AutoMapReadWriteRuleAttribute attribute = new AutoMapReadWriteRuleAttribute(expecteRwRule);
            //---------------Test Result -----------------------
            Assert.AreEqual(expecteRwRule, attribute.ReadWriteRule);
        }

        [Test]
        public void Test_UniqueConstraint_ConstructWithValue_ShouldSetValue()
        {
            //---------------Set up test pack-------------------
            const string uniqueConstraintName = "myuniqueconstraint";
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            AutoMapUniqueConstraintAttribute attribute = new AutoMapUniqueConstraintAttribute(uniqueConstraintName);
            //---------------Test Result -----------------------
            Assert.AreEqual(uniqueConstraintName, attribute.UniqueConstraintName);
        }

        [Test]
        public void Test_TableName_ConstructWithValue_ShouldSetValue()
        {
            //---------------Set up test pack-------------------
            string tableName;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var attribute = new AutoMapTableNameAttribute(tableName = "myTable");
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<Attribute>(attribute);
            Assert.AreEqual(tableName, attribute.TableName);
        }

        [Test]
        public void Test_IntPropRule_ShouldSetDefaultValue()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var attribute = new AutoMapIntPropRuleAttribute();
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<Attribute>(attribute);
            Assert.AreEqual(int.MinValue, attribute.Min);
            Assert.AreEqual(int.MaxValue, attribute.Max);
        }

        [Test]
        public void Test_IntPropRule_ShouldSetValue()
        {
            //---------------Set up test pack-------------------
            const int min = 1;
            const int max = 10;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var attribute = new AutoMapIntPropRuleAttribute(min, max);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<Attribute>(attribute);
            Assert.AreEqual(min, attribute.Min);
            Assert.AreEqual(max, attribute.Max);
        }

        [Test]
        public void Test_DateTimePropRule_ShouldSetValue()
        {
            //---------------Set up test pack-------------------
            var startDateValue = DateTime.Now;
            var endDateValue = DateTime.Now.AddDays(10);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var attribute = new AutoMapDateTimePropRuleAttribute(startDateValue, endDateValue);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<Attribute>(attribute);
            Assert.AreEqual(startDateValue, attribute.StartDate);
            Assert.AreEqual(endDateValue, attribute.EndDate);
        }

        [Test]
        public void Test_DateTimeStringPropRule_ShouldSetValue()
        {
            //---------------Set up test pack-------------------
            const string startDateValue = "Today";
            const string endDateValue = "Tomorrow";
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var attribute = new AutoMapDateTimePropRuleAttribute(startDateValue, endDateValue);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<Attribute>(attribute);
            Assert.AreEqual(startDateValue, attribute.StartDateString);
            Assert.AreEqual(endDateValue, attribute.EndDateString);
        }

        [Test]
        public void Test_StringPatternMatchPropRule_ShouldSetValue()
        {
            //---------------Set up test pack-------------------
            const string pattern = @"^[-+.\w]{1,64}@[-.\w]{1,64}\.[-.\w]{2,6}$";
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var attribute = new AutoMapStringPatternMatchPropRuleAttribute(pattern);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<Attribute>(attribute);
            Assert.AreEqual(pattern, attribute.Pattern);
        }
        [Test]
        public void Test_StringPatternMatchPropRule_WithErrorMessage_ShouldSetMessage()
        {
            //---------------Set up test pack-------------------
            const string pattern = @"^[-+.\w]{1,64}@[-.\w]{1,64}\.[-.\w]{2,6}$";
            string message = GetRandomMessage();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var attribute = new AutoMapStringPatternMatchPropRuleAttribute(pattern, message);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<Attribute>(attribute);
            Assert.AreEqual(message, attribute.Message);
            Assert.AreEqual(pattern, attribute.Pattern);
        }

        [Test]
        public void Test_StringLengthPropRule_ShouldReturnDefault()
        {
            //---------------Set up test pack-------------------
           
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var attribute = new AutoMapStringLengthPropRuleAttribute();
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<Attribute>(attribute);
            Assert.AreEqual(0, attribute.MinLength);
            Assert.AreEqual(255, attribute.MaxLength);
        }

        [Test]
        public void Test_StringLengthPropRule_ShouldSetValue()
        {
            //---------------Set up test pack-------------------
            const int minLengthValue = 5;
            const int maxLengthValue = 400;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var attribute = new AutoMapStringLengthPropRuleAttribute(minLengthValue,maxLengthValue);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<Attribute>(attribute);
            Assert.AreEqual(minLengthValue, attribute.MinLength);
            Assert.AreEqual(maxLengthValue, attribute.MaxLength);
        }

        private string GetRandomMessage()
        {
            return RandomValueGenerator.GetRandomString();
        }
    }

   

}