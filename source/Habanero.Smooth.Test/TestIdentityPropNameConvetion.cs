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
using System.Linq;
using System.Reflection;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Base;
using NUnit.Framework;

namespace Habanero.Smooth.Test
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