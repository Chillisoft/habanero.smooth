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
using System.Reflection;
using Habanero.Base;
using NUnit.Framework;

namespace Habanero.Smooth.Test
{
    [TestFixture]
    public class TestReflectionUtils
    {
        //TODO brett 30 Jan 2010: Must not Props that have private Get's
/*        [Test]
        public void Test_GetProperty_WithExpression_ShouldReturnProperty()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var property = ReflectionUtils.GetProperty<FakeBOWProps>(x => x.PublicGetGuidProp);
            //---------------Test Result -----------------------
            var propertyInfo = typeof(FakeBOWProps).GetProperty("PublicGetGuidProp");
            property.ShouldEqual(propertyInfo);
            property.PropertyType.ShouldEqual(propertyInfo.PropertyType);
        }*/
    }
}