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
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Smooth.Test.ValidFakeBOs;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Smooth.Test
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestManyToOneAutoMapper
    {
        [SetUp]
        public void Setup()
        {
            AllClassesAutoMapper.PropNamingConvention = new DefaultPropNamingConventions();
            AllClassesAutoMapper.ClassDefCol = null;
        }

        [Test]
        public void Test_Construct_WhenNullPropWrapper_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            PropertyWrapper propertyWrapper = null;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                new ManyToOneAutoMapper(propertyWrapper);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("propWrapper", ex.ParamName);
            }
        }

        [Test]
        public void Test_ConstructShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            PropertyInfo propertyInfo = null;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                new ManyToOneAutoMapper(propertyInfo);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("propWrapper", ex.ParamName);
            }
        }

        [Test]
        public void Test_Map_WhenNullPropInfo_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            PropertyInfo info = null;
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var relationshipDefCol = info.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(relationshipDefCol);
        }
        [Test]
        public void Test_Map_WhenNullPropWrapper_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            PropertyWrapper propertyWrapper = null;
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var relationshipDefCol = propertyWrapper.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(relationshipDefCol);
        }
        [Test]
        public void Test_Map_WhenNotSingleRel_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWProps);
            const string expectedPropName = "PublicGetGuidProp";
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsNotOfType<IBusinessObject>();
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(relationshipDef);
        }

        [Test]
        public void Test_Map_PropWithIgnoreAttribute_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithSingleRel);
            const string expectedPropName = "MySingleRElationshipWithIgnore";
            var propertyInfo = classType.GetProperty(expectedPropName);
            PropertyWrapper propWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObject>();
            Assert.AreEqual(1, propertyInfo.GetCustomAttributes(true).Length);
            Assert.IsTrue(propWrapper.HasIgnoreAttribute);
            Assert.IsNotNull(propertyInfo);
            Assert.AreEqual(1, propertyInfo.GetCustomAttributes(true).Length);
            Assert.IsTrue(propWrapper.HasIgnoreAttribute);
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(relationshipDef);
        }

        [Test]
        public void Test_Map_WhenInheritedProp_ShouldReturnNull()
        {
            //When Prop inherits from another class the prop (relationship
            // will be mapped as part of the base class
            // and should not be mapped as part of this class.
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOSubClassWithOverridenProps);
            PropertyInfo propertyInfo = classType.GetProperty("ManyToOneRelationshipInherited");
            var autoMapper = new ManyToOneAutoMapper(propertyInfo);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.IsFalse(propertyWrapper.HasIgnoreAttribute);

            Assert.IsTrue(propertyWrapper.IsPublic);
            Assert.IsTrue(propertyWrapper.IsInherited);
            //---------------Execute Test ----------------------
            var propDef = autoMapper.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(propDef);
        }

        [Test]
        public void Test_Map_WhenOverridenProp_ShouldReturnNull()
        {
            //When Prop overrides from another class the base
            // declaration of the prop will be mapped as part of the base class
            // and should not be mapped as part of this class.
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOSubClassWithOverridenProps);
            PropertyInfo propertyInfo = classType.GetProperty("ManyToOneRelationshipOverridden");
            var autoMapper = new ManyToOneAutoMapper(propertyInfo);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.IsFalse(propertyWrapper.HasIgnoreAttribute);

            Assert.IsTrue(propertyWrapper.IsPublic);
            Assert.IsTrue(propertyWrapper.IsOverridden);
            //---------------Execute Test ----------------------
            var propDef = autoMapper.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(propDef);
        }

        [Test]
        public void Test_Map_WhenIsSingleRel_ShouldReturnRelDef()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithSingleRel);
            const string expectedPropName = "MySingleRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObject>();
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNotNull(relationshipDef);
            Assert.IsInstanceOf(typeof(SingleRelationshipDef), relationshipDef);
            Assert.AreEqual(expectedPropName, relationshipDef.RelationshipName);
            Assert.AreEqual(propertyInfo.PropertyType.FullName, relationshipDef.RelatedObjectClassName);
            Assert.IsNotNull(propertyInfo.PropertyType.AssemblyQualifiedName);
            propertyInfo.PropertyType.AssemblyQualifiedName.StartsWith(relationshipDef.RelatedObjectAssemblyName);
            Assert.AreEqual(DeleteParentAction.DoNothing, relationshipDef.DeleteParentAction);
            Assert.IsNotNull(relationshipDef.RelKeyDef);
        }
        [Test]
        public void Test_Map_WhenIsSingleRel_ShouldSetDeleteParentActionToDoNothing()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithSingleRel);
            const string expectedPropName = "MySingleRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObject>();
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNotNull(relationshipDef);
            Assert.AreEqual(DeleteParentAction.DoNothing, relationshipDef.DeleteParentAction);
        }

        [Test]
        public void Test_Map_WhenIsSingleRel_ShouldCreateRelProp()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithSingleRel);
            const string expectedPropName = "MySingleRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObject>();
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.MapManyToOne();
            //---------------Test Result -----------------------
            var relKeyDef = relationshipDef.RelKeyDef;
            Assert.IsNotNull(relKeyDef);
            Assert.AreEqual(1, relKeyDef.Count);
            const string expectedOwnerPropName = expectedPropName + "ID";
            Assert.IsTrue(relKeyDef.Contains(expectedOwnerPropName),
                "By Convention the RelationshipPropName on the single side of the M-1 Relationship Should be RelationshipName & ID");
            var relPropDef = relKeyDef[expectedOwnerPropName];
            Assert.AreEqual(expectedOwnerPropName, relPropDef.OwnerPropertyName);
            string expectedRelatedClassName = propertyInfo.PropertyType.Name + "ID";
            Assert.AreEqual(expectedRelatedClassName, relPropDef.RelatedClassPropName);
        }

        [Test]
        public void Test_Map_WhenIsSingleRel_WhenRelatedClassInheritsFromGenericBO_ShouldCreateRelProp()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithSingleRelToGenericBO);
            const string expectedPropName = "MySingleRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObject>();
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.MapManyToOne();
            //---------------Test Result -----------------------
            var relKeyDef = relationshipDef.RelKeyDef;
            Assert.IsNotNull(relKeyDef);
            Assert.AreEqual(1, relKeyDef.Count);
            const string expectedOwnerPropName = expectedPropName + "ID";
            Assert.IsTrue(relKeyDef.Contains(expectedOwnerPropName),
                "By Convention the RelationshipPropName on the single side of the M-1 Relationship Should be RelationshipName & ID");
            var relPropDef = relKeyDef[expectedOwnerPropName];
            Assert.AreEqual(expectedOwnerPropName, relPropDef.OwnerPropertyName);
            string expectedRelatedClassName = "FakeBOGenericID";
            Assert.AreEqual(expectedRelatedClassName, relPropDef.RelatedClassPropName);
        }

        [Test]
        public void Test_Map_When_WhenHasCompulsoryAttribute_ShouldSetIsCompulsoryToTrue()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithCompulsoryProp);
            const string expectedPropName = "CompulsorySingleRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObject>();
            propertyInfo.AssertHasAttribute<AutoMapCompulsoryAttribute>();
            //---------------Execute Test ----------------------
            var classDef = classType.MapClass();
            var relationshipDef = classDef.RelationshipDefCol[expectedPropName];
            //---------------Test Result -----------------------
            Assert.IsTrue(relationshipDef.IsCompulsory);
        }

        [Test]
        public void Test_Map_When_WhenNotHasCompulsoryAttribute_ShouldSetIsCompulsoryToFalse()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithCompulsoryProp);
            const string expectedPropName = "NonCompulsorySingleRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObject>();
            //---------------Execute Test ----------------------
            var classDef = classType.MapClass();
            var relationshipDef = classDef.RelationshipDefCol[expectedPropName];
            //---------------Test Result -----------------------
            Assert.IsFalse(relationshipDef.IsCompulsory);
        }

        [Test]
        public void Test_Map_WhenIsSingleRel_WithCompulsoryAttribute_ShouldCreateRelProp_WithCompulsoryProp()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithCompulsoryProp);
            const string expectedPropName = "CompulsorySingleRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObject>();
            propertyInfo.AssertHasAttribute<AutoMapCompulsoryAttribute>();
            //---------------Execute Test ----------------------
            var classDef = classType.MapClass();
            var relationshipDef = classDef.RelationshipDefCol[expectedPropName];
            //---------------Test Result -----------------------
            Assert.IsTrue(relationshipDef.IsCompulsory);
            const string expectedOwnerPropName = expectedPropName + "ID";
            Assert.IsNotNull(relationshipDef.OwningClassDef);
            var ownerPropDef = relationshipDef.OwningClassDef.GetPropDef(expectedOwnerPropName);
            Assert.IsNotNull(ownerPropDef, "Owner Prop Def cannot be null");
            Assert.IsTrue(ownerPropDef.Compulsory, "Owner Prop for a compulsory Relationship should be set to true.");
        }

        [Test]
        public void Test_DefaultNameConventionIfNoneSet()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var nameConvention = ManyToOneAutoMapper.PropNamingConvention;
            //---------------Test Result -----------------------
            Assert.IsInstanceOf(typeof (DefaultPropNamingConventions), nameConvention);
        }

        [Test]
        public void Test_Map_WhenNonDefaultNameConventionSet_ShouldCreateRelProp()
        {
            //---------------Set up test pack-------------------
            INamingConventions nameConvention = new FakeConvention();
            AllClassesAutoMapper.PropNamingConvention = nameConvention;
            var classType = typeof(FakeBOWithSingleRel);
            const string expectedPropName = "MySingleRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObject>();
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.MapManyToOne();
            //---------------Test Result -----------------------
            var relKeyDef = relationshipDef.RelKeyDef;
            Assert.IsNotNull(relKeyDef);
            Assert.AreEqual(1, relKeyDef.Count);
            string expectedOwnerPropName = nameConvention.GetSingleRelOwningPropName(expectedPropName);
            Assert.IsTrue(relKeyDef.Contains(expectedOwnerPropName),
                "By Convention the RelationshipPropName on the single side of the M-1 Relationship Should be RelationshipName & ID");
            var relPropDef = relKeyDef[expectedOwnerPropName];
            Assert.AreEqual(expectedOwnerPropName, relPropDef.OwnerPropertyName);
            string expectedRelatedClassName = nameConvention.GetIDPropertyName(propertyInfo.PropertyType.ToTypeWrapper());
            Assert.AreEqual(expectedRelatedClassName, relPropDef.RelatedClassPropName);
        }

        [Test]
        public void Test_SetPropNamingConvention_ShouldSetConvention()
        {
            //---------------Set up test pack-------------------
            INamingConventions nameConvention = MockRepository.GenerateMock<INamingConventions>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            AllClassesAutoMapper.PropNamingConvention = nameConvention;
            //---------------Test Result -----------------------
            Assert.AreSame(nameConvention, ManyToOneAutoMapper.PropNamingConvention);
        }

        [Test]
        public void Test_Map_ShouldSetDeleteActionDoNothing_OwningBOHasFKTrue()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithSingleRel);
            const string expectedPropName = "MySingleRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObject>();
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsTrue(relationshipDef.OwningBOHasForeignKey);
            Assert.AreEqual(DeleteParentAction.DoNothing, relationshipDef.DeleteParentAction);
        }

        [Test]
        public void Test_Map_WhenHasOneToOneAttribute_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithOneToOneAttribute);
            const string expectedPropName = "MySingleRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            PropertyWrapper propWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObject>();
            propWrapper.HasAttribute<AutoMapOneToOneAttribute>();
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(relationshipDef);
        }

        [Test]
        public void Test_Map_WhenHasSingleReverseRel_ShouldReturnNull()
        {
            //For the BusinessObject 'FakeBOWithReverseSingleRel'
            //If the Related Type (in this case 'FakeBOWithTwoSingleRel')
            // for the Relatiosnhip 'MySingleRelationship'
            // has a Single Relationship back 'MyReverseSingleRel'
            // then this should not be mapped as a M:1.
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithReverseSingleRel);
            var reverseClassType = typeof (FakeBOWithTwoSingleRel);
            const string expectedPropName = "MySingleRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            PropertyWrapper propWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<FakeBOWithTwoSingleRel>();
            var reversePropInfo = reverseClassType.GetProperty("MyReverseSingleRel");
            Assert.IsNotNull(reversePropInfo);
            reversePropInfo.AssertIsOfType<FakeBOWithReverseSingleRel>();

            Assert.IsTrue(propWrapper.HasSingleReverseRelationship);
            Assert.IsFalse(propWrapper.HasMultipleReverseRelationship);
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(relationshipDef);
        }

        [Test]
        public void TestAccept_Map_WhenHasSingleReverseRelAndMultipleReverseRel_ShouldRaiseError()
        {
            //For the BusinessObject 'FakeBOWithUndefinableSingleRel'
            //If the Related Type (in this case 'FakeBOWithSingleAndMultipleRelToSameType')
            // for the Relatiosnhip 'MySingleRelationship'
            // has a Single Relationship back 'MySingleRevRel'
            // and has a Multiple Reverse Relationship 'MyMultipleRevRel'
            // and 'MySingleRelationship' does not have an 'AutoMapOneToOne' attribute
            // then it cannot be automapped and should raise an error.
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithUndefinableSingleRel);
            var reverseClassType = typeof(FakeBOWithSingleAndMultipleRelToSameType);
            const string expectedPropName = "MySingleRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            PropertyWrapper propWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType(reverseClassType);
            var reversePropInfo = reverseClassType.GetProperty("MySingleRevRel");
            Assert.IsNotNull(reversePropInfo);
            reversePropInfo.AssertIsOfType<FakeBOWithUndefinableSingleRel>();

            Assert.IsTrue(propWrapper.HasSingleReverseRelationship);
            Assert.IsTrue(propWrapper.HasMultipleReverseRelationship);
            //---------------Execute Test ----------------------
            try
            {
                propertyInfo.MapManyToOne();
                Assert.Fail("Expected to throw an InvalidDefinitionException");
            }
            //---------------Test Result -----------------------
            catch (InvalidDefinitionException ex)
            {
                StringAssert.Contains("The Relationship '" + expectedPropName
                        + "' could not be automapped since there are multiple relationships on class '"
                        + "FakeBOWithSingleAndMultipleRelToSameType' that reference the BusinessObject Class '"
                        + "FakeBOWithUndefinableSingleRel'. Please map using ClassDef.XML or Attributes", ex.Message);
            }
        }

        [Test]
        public void TestAccept_Map_WhenHasSingleReverseRelAndMultipleReverseRel_HasOneToOneAttribute_ShouldReturnNull()
        {
            //For the BusinessObject 'FakeBOWithUndefinableSingleRel'
            //If the Related Type (in this case 'FakeBOWithSingleAndMultipleRelToSameType')
            // for the Relatiosnhip 'MySingleRelWithOneToOneAttribute'
            // has a Single Relationship back 'MySingleRevRel'
            // and has a Multiple Reverse Relationship 'MyMultipleRevRel'
            // and 'MySingleRelWithOneToOneAttribute' has a 'AutoMapOneToOne' attribute
            // then it should Return null.
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithUndefinableSingleRel);
            var reverseClassType = typeof(FakeBOWithSingleAndMultipleRelToSameType);
            const string expectedPropName = "MySingleRelWithOneToOneAttribute";
            var propertyInfo = classType.GetProperty(expectedPropName);
            PropertyWrapper propWrapper = propertyInfo.ToPropertyWrapper();

            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType(reverseClassType);
            var reversePropInfo = reverseClassType.GetProperty("MySingleRevRel");
            Assert.IsNotNull(reversePropInfo);
            reversePropInfo.AssertIsOfType<FakeBOWithUndefinableSingleRel>();

            Assert.IsTrue(propWrapper.HasSingleReverseRelationship);
            Assert.IsTrue(propWrapper.HasMultipleReverseRelationship);
            Assert.IsTrue(propWrapper.HasAttribute<AutoMapOneToOneAttribute>());
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(relationshipDef);
        }

        [Test]
        public void TestAccept_Map_WhenHasSingleReverseRelAndMultipleReverseRel_WhenHasOneToManyAttribute_ShouldReturnRel()
        {
            //For the BusinessObject 'FakeBOWithUndefinableSingleRel'
            //If the Related Type (in this case 'FakeBOWithSingleAndMultipleRelToSameType')
            // for the Relatiosnhip 'MySingleRelWithOneToManyAttribute'
            // has a Single Relationship back 'MySingleRevRel'
            // and has a Multiple Reverse Relationship 'MyMultipleRevRel'
            // and 'MySingleRelWithOneToManyAttribute' has 'AutoMapManyToOne'
            // then it should create the Relationship
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithUndefinableSingleRel);
            var reverseClassType = typeof(FakeBOWithSingleAndMultipleRelToSameType);
            const string expectedPropName = "MySingleRelWithOneToManyAttribute";
            var propertyInfo = classType.GetProperty(expectedPropName);
            PropertyWrapper propWrapper = propertyInfo.ToPropertyWrapper();

            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType(reverseClassType);
            var reversePropInfo = reverseClassType.GetProperty("MySingleRevRel");
            Assert.IsNotNull(reversePropInfo);
            reversePropInfo.AssertIsOfType<FakeBOWithUndefinableSingleRel>();
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRevRel");
//            Assert.IsTrue(propertyInfo.HasSingleReverseRelationship());
            Assert.IsTrue(propWrapper.HasMultipleReverseRelationship);
            Assert.IsTrue(propWrapper.HasAttribute<AutoMapManyToOneAttribute>());
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNotNull(relationshipDef);
        }
        [Test]
        public void Test_Map_WhenHasMultipleReverseRel_ShouldReturnRel_WithReverseRelNameSet()
        {
            //For the BusinessObject 'FakeBOWithUndefinableSingleRel'
            //If the Related Type (in this case 'FakeBOWithSingleAndMultipleRelToSameType')
            // for the Relatiosnhip 'MySingleRelWithOneToManyAttribute'
            // and has a Multiple Reverse Relationship 'MyMultipleRevRel'
            // then it should create the Relationship
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithUndefinableSingleRel);
            var reverseClassType = typeof(FakeBOWithSingleAndMultipleRelToSameType);
            const string expectedPropName = "MySingleRelWithOneToManyAttribute";
            var propertyInfo = classType.GetProperty(expectedPropName);
            PropertyWrapper propWrapper = propertyInfo.ToPropertyWrapper();

            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType(reverseClassType);
            var reversePropInfo = reverseClassType.GetProperty("MySingleRevRel");
            Assert.IsNotNull(reversePropInfo);
            reversePropInfo.AssertIsOfType<FakeBOWithUndefinableSingleRel>();

            Assert.IsTrue(propWrapper.HasMultipleReverseRelationship);
            Assert.IsTrue(propWrapper.HasAttribute<AutoMapManyToOneAttribute>());
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNotNull(relationshipDef);
            Assert.AreEqual("MyMultipleRevRel", relationshipDef.ReverseRelationshipName);
        }

        [Test]
        public void Test_Map_WhenNoReverseRelationship_NoAttributes_ShouldReturnRel_WithConventionMultipleReverseRel()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "MySingleRelationship";
            var type = typeof(FakeBOWithSingleRel);
            PropertyInfo propertyInfo = type.GetProperty(expectedPropName);
            PropertyWrapper propWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            propertyInfo.AssertIsOfType<IBusinessObject>();
            Assert.IsFalse(propWrapper.HasSingleReverseRelationship);
            Assert.IsFalse(propWrapper.HasMultipleReverseRelationship);
            Assert.IsFalse(propWrapper.HasAttribute<AutoMapManyToOneAttribute>());
            Assert.IsFalse(propWrapper.HasAttribute<AutoMapOneToOneAttribute>());
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNotNull(relationshipDef);
            Assert.AreEqual("FakeBOWithSingleRels", relationshipDef.ReverseRelationshipName);
        }

        [Test]
        public void Test_Map_WhenHasAttributeDeclareRevRelName_ShouldReturnRel_WithDeclaredRevRelName()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "MySingleRelationship";
            var type = typeof(FakeBOWithSingleAttributeDeclaredRevRel);
            const string reverseRelName = "AttributeRevRelName";
            PropertyInfo propertyInfo = type.GetProperty(expectedPropName);
            PropertyWrapper propWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(propWrapper.IsSingleRelationship);
            Assert.IsFalse(propWrapper.HasSingleReverseRelationship);
            Assert.IsFalse(propWrapper.HasMultipleReverseRelationship);
            Assert.IsTrue(propWrapper.HasAttribute<AutoMapManyToOneAttribute>());
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNotNull(relationshipDef);
            Assert.AreEqual(reverseRelName, relationshipDef.ReverseRelationshipName);
        }

        [Test]
        public void Test_Map_WhenNotRelToIBoCol_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWProps);
            const string expectedPropName = "PublicGetGuidProp";
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsNotOfType<IBusinessObject>();
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(relationshipDef);
        }

//        private static PropertyInfo GetMockPropInfoWithAutoMapAttribute(Type type, string expectedPropName, string reverseRelName)
//        {
//            PropertyInfo propertyInfo = MockRepository.GenerateMock<PropertyInfo>();
//            propertyInfo.Stub(propertyInfo1 => propertyInfo1.Name).Return(expectedPropName);
//            propertyInfo.Stub(info => info.DeclaringType).Return(type);
//            propertyInfo.SetCustomAutoMapRelationshipAttribute<AutoMapManyToOne>(reverseRelName);
//            propertyInfo.Stub(info1 => info1.PropertyType).Return(typeof(FakeBONoPK));
//            return propertyInfo;
//        }

        [Test]
        public void Test_HasMultipleReverseRelationshipWhenHasShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithUndefinableSingleRel);
            const string expectedPropName = "MySingleRelationship";
            var propertyInfo = classType.GetPropertyWrapper(expectedPropName);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            //---------------Execute Test ----------------------
            var hasMultipleReverseRelationship = propertyInfo.HasMultipleReverseRelationship;
            //---------------Test Result -----------------------
            Assert.IsTrue(hasMultipleReverseRelationship);
        }

        [Test]
        public void Test_HasMultipleReverseRelationship_WhenNotHasShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithTwoSingleRel);
            const string expectedPropName = "MySingleRelationship";
            var propertyInfo = classType.GetPropertyWrapper(expectedPropName);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            //---------------Execute Test ----------------------
            var hasMultipleReverseRelationship = propertyInfo.HasMultipleReverseRelationship;
            //---------------Test Result -----------------------
            Assert.IsFalse(hasMultipleReverseRelationship);
        }

        [Test]
        public void Test_GetRelatedPropName_WhenRelatedClassHasAttributeDeclaredIdProp_ShouldUseDeclaredIDPropName()
        {
            //---------------Set up test pack-------------------
            var type = typeof (FakeBOAttributePK);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var relatedPropName = ManyToOneAutoMapper.GetRelatedPropName(type.ToTypeWrapper());
            //---------------Test Result -----------------------
            Assert.AreEqual("PublicGuidProp", relatedPropName);
        }

        [Test]
        public void Test_GetRelatatedPropName_WhenStdNamingPropAndRelDeclaredProp_ShouldReturnDeclaredPropName()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBOAttributePKAndPKNaming);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var relatedPropName = ManyToOneAutoMapper.GetRelatedPropName(type.ToTypeWrapper());
            //---------------Test Result -----------------------
            Assert.AreEqual("PublicGuidProp", relatedPropName);
        }
        [Test]
        public void Test_GetRelatatedPropName_WhenNoProp_ShouldReturnStdNamingProp()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBONoPK);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var relatedPropName = ManyToOneAutoMapper.GetRelatedPropName(type.ToTypeWrapper());
            //---------------Test Result -----------------------
            Assert.AreEqual("FakeBONoPKID", relatedPropName);
        }

        [Test]
        public void Test_GetRelatedPropName_WhenRelatedClassInheritsFromGenericBO_ShouldUseGenericTypeToDetermineName()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBOGeneric);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var relatedPropName = ManyToOneAutoMapper.GetRelatedPropName(type.ToTypeWrapper());
            //---------------Test Result -----------------------
            Assert.AreEqual("FakeBOGenericID", relatedPropName);
        }
        [Test]
        public void TestAccept_Map_WhenIsPrivateProperty_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBoWithPrivateProps);
            const string expectedPropName = "PrivateManyToOneRel";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(propertyWrapper.IsSingleRelationship);
            Assert.IsFalse(propertyWrapper.IsPublic);
            //---------------Execute Test ----------------------
            var relationshipDefCol = propertyWrapper.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(relationshipDefCol);
        }

        [Test]
        public void TestAccept_Map_WhenIsStaticProperty_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var classType = typeof (FakeBoWithStaticProperty);
            const string expectedPropName = "MySingleRelationship2";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(propertyWrapper.IsSingleRelationship);
            Assert.IsTrue(propertyWrapper.IsStatic);
            //---------------Execute Test ----------------------
            var relationshipDefCol = propertyWrapper.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(relationshipDefCol);
        }

        [Test]
        public void Test_MustBeMapped_WhenPropIsInherited_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            var propertyWrapper = MockRepository.GenerateStub<FakePropertyWrapper>();
            propertyWrapper.Stub(wrapper => wrapper.IsPublic).Return(true);
            propertyWrapper.Stub(wrapper => wrapper.IsInherited).Return(true);
            propertyWrapper.Stub(wrapper => wrapper.IsSingleRelationship).Return(true);
            ManyToOneAutoMapper mapper = new ManyToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(propertyWrapper.IsStatic);
            Assert.IsTrue(propertyWrapper.IsPublic);
            Assert.IsFalse(propertyWrapper.HasManyToOneAttribute);
            Assert.IsTrue(propertyWrapper.IsSingleRelationship);
            Assert.IsFalse(propertyWrapper.HasIgnoreAttribute);
            Assert.IsFalse(propertyWrapper.HasOneToOneAttribute);

            Assert.IsTrue(propertyWrapper.IsInherited);
            //---------------Execute Test ----------------------
            var mustBeMapped = mapper.MustBeMapped();
            //---------------Test Result -----------------------
            Assert.IsFalse(mustBeMapped);
        }

        [Test]
        public void TestAccept_Map_WhenRelationshipTypeDefinedAsAggregation_ShouldSetToAggregation()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithCompositionManyToOneRel);
            const string expectedPropName = "MySingleRelationship";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsSingleRelationship();
            var oneToOneAtt = propertyWrapper.GetAttribute<AutoMapManyToOneAttribute>();
            Assert.AreEqual(RelationshipType.Aggregation, oneToOneAtt.RelationshipType);
            //---------------Execute Test ----------------------
            var relationshipDef = propertyWrapper.MapManyToOne();
            //---------------Test Result -----------------------
            Assert.AreEqual(RelationshipType.Aggregation, relationshipDef.RelationshipType);
        }

        private static void AssertRelationshipIsForOwnerClass(Type ownerClassType, Type reverseClassType, string reveresRelationshipName)
        {
            var reversePropInfo = reverseClassType.GetPropertyWrapper(reveresRelationshipName);
            Assert.IsNotNull(reversePropInfo, reveresRelationshipName + " on " + reverseClassType.Name + " should not be null");
            Assert.AreSame(ownerClassType, reversePropInfo.RelatedClassType.UnderlyingType);
        }
    }

    public class FakeConvention : INamingConventions
    {
        public string GetIDPropertyName<T>()
        {
            return GetIDPropertyName(typeof(T).ToTypeWrapper());
        }

        public string GetIDPropertyName(TypeWrapper t)
        {
            return "XXX" + t.Name;
        }

        public string GetSingleRelOwningPropName(string relationshipName)
        {
            return "XXX" + relationshipName;
        }
    }
}
