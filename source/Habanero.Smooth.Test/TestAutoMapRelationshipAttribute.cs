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
using System.Text;
using Habanero.Base;
using NUnit.Framework;

namespace Habanero.Smooth.Test
{
    [TestFixture]
    public class TestAutoMapRelationshipAttribute
    {
// ReSharper disable InconsistentNaming
        [Test]
        public void Test_ConstructRelationship_WithNoParams_ShouldSetRelationshipTypeAssociation()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var attribute = new AutoMapRelationshipAttributeStub();
            //---------------Test Result -----------------------
            Assert.AreEqual(RelationshipType.Association, attribute.RelationshipType);
        } 
        [Test]
        public void Test_ConstructRelationship_WithRevRelName_ShouldSetRelationshipTypeAssociation()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var attribute = new AutoMapRelationshipAttributeStub("fdaf");
            //---------------Test Result -----------------------
            Assert.AreEqual(RelationshipType.Association, attribute.RelationshipType);
        }
        [Test]
        public void Test_ConstructRelationship_WithRelType_ShouldSetToRSpecified()
        {
            //---------------Set up test pack-------------------            
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var attribute = new AutoMapRelationshipAttributeStub(RelationshipType.Composition);
            //---------------Test Result -----------------------
            Assert.AreEqual(RelationshipType.Composition, attribute.RelationshipType);
        }
        [Test]
        public void Test_ConstructRelationship_WithRelNameAndRelType_ShouldSetToRSpecified()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var attribute = new AutoMapRelationshipAttributeStub("fdaf", RelationshipType.Composition);
            //---------------Test Result -----------------------
            Assert.AreEqual(RelationshipType.Composition, attribute.RelationshipType);
        }

        [Test]
        public void Test_SetAttributeWithRelationshipType_ShouldHaveRelType()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBoWithCompositionMultipleRel);
            const string expectedPropName = "MyMultipleRevRel";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapOneToManyAttribute), true);

            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();
            Assert.AreEqual(1, customAttributes.Count());
            //---------------Execute Test ----------------------
            var onToManyAtt = (AutoMapOneToManyAttribute)customAttributes[0];
            //---------------Test Result -----------------------
            Assert.AreEqual(RelationshipType.Composition, onToManyAtt.RelationshipType);
        }

    }

    public class AutoMapRelationshipAttributeStub : AutoMapRelationshipAttribute
    {
        public AutoMapRelationshipAttributeStub()
        {
        }

        public AutoMapRelationshipAttributeStub(string reverseRelationshipName) : base(reverseRelationshipName)
        {
        }

        public AutoMapRelationshipAttributeStub(RelationshipType relationshipType) : base(relationshipType)
        {
        }

        public AutoMapRelationshipAttributeStub(string reverseRelationshipName, RelationshipType relationshipType) : base(reverseRelationshipName, relationshipType)
        {
        }
    }
}
