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
