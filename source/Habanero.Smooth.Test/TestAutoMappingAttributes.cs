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
    }
}