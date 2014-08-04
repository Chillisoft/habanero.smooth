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
using System.Linq;
using System.Reflection;
using Habanero.Smooth.Test.ExtensionMethods;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.ReflectionWrappers;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Smooth.Test
{
    [TestFixture]
    public class TestAutoMapperExtensions
    {
        [SetUp]
        public void Setup()
        {
            AllClassesAutoMapper.PropNamingConvention = new DefaultPropNamingConventions();
        }

#region OneToOneReverseRelationship


        [Test]
        public void Test_HasSingleReverseRelationship_WhenRevHasM_1Attr_ShouldBeFalse()
        {
            //FakeBOWithSingleAttributeDeclaredRevRel has a relationship 'MySingleRelationship'
            // that is mapped via an AutoMapOneToMany Attribute to 'FakeBOWithUndefinableSingleRel'
            // AttributeRevRelName relationship. Because it is a single Relationship
            // and is mapped via AutoMap Attribute as a ManyToOne this will always return
            // that there are no single reverse relationships event though 'SingleRel' would 
            // be found if it were not for the autoMapping Prop.
            //---------------Set up test pack-------------------
            const string expectedPropName = "MySingleRelationship";
            var type = typeof(FakeBOWithSingleAttributeDeclaredRevRel).ToTypeWrapper();
            //---------------Assert Precondition----------------
            PropertyWrapper propertyInfo = type.GetProperty(expectedPropName);
            Assert.IsTrue(propertyInfo.IsSingleRelationship);
            //---------------Execute Test ----------------------
            var hasSingleReverseRelationship = propertyInfo.HasSingleReverseRelationship;
            //---------------Test Result -----------------------
            Assert.IsFalse(hasSingleReverseRelationship);
        }

        [Test]
        public void Test_HasSingleReverseRelationship_WhenHasAndOwnerSingle_ShouldBeTrue()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithReverseSingleRel).ToTypeWrapper();
            const string expectedPropName = "MySingleRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<FakeBOWithTwoSingleRel>();
            var reversePropInfo = reverseClassType.GetProperty("MyReverseSingleRel");
            Assert.IsNotNull(reversePropInfo);
            reversePropInfo.AssertIsOfType<FakeBOWithReverseSingleRel>();
            //---------------Execute Test ----------------------
            var hasSingleReverseRelationship = propertyInfo.HasSingleReverseRelationship;
            //---------------Test Result -----------------------
            Assert.IsTrue(hasSingleReverseRelationship);
        }

        [Test]
        public void Test_HasSingleReverseRelationship_WhenHasAndOwnerMultiple_ShouldBeTrue()
        {
            //'FakeBoWithMultipleRel' has relationship 'MyMultipleWithTwoSingleReverse'
            // that is related to 'FakeWithTwoSingleReverseRel' which has two 
            // single relationships that could be mapped back.
            var classType = typeof(FakeBoWithMultipleRel).ToTypeWrapper();
            const string expectedPropName = "MyMultipleWithTwoSingleReverse";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship1");
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship2");
            //---------------Execute Test ----------------------
            var hasSingleReverseRelationship = propertyInfo.HasSingleReverseRelationship;
            //---------------Test Result -----------------------
            Assert.IsTrue(hasSingleReverseRelationship);
        }

        //The GetOneToOneReverseRelationshipInfos for a SingleRelationship should return all Props for SingleRelationships 
        //      that that are for the ownerClass 
        //   ----Unless----
        //1- This Relationship is mapped as a ManyToOneAttribute Or ReverseRelationship MappedVia M:1
        //2 - ReverseRel directly mapped via a OneToOne attribute to this relationship.
        //Or 3 - This Relationship is mapped by a OnetoOne Attribute on reverse relationship.
        //Or 4 - This Relationship has an Ignore Attribute.
        //or 5 - The Reverse Relationship has an Ignore Attribute
        [Test]
        public void Test_Get1_1RevRels_WhenNotHas_ShouldNotHaveItems()
        {
            //You have a single Relationship that is referencing a class
            // that does not have any reverse single relationships back to
            // 'FakeBOWithSingleAttributeDeclaredRevRel'
            //---------------Set up test pack-------------------
            const string expectedPropName = "MySingleRelationship";
            var type = typeof(FakeBoWithNoSingleReverse).ToTypeWrapper();
            //---------------Assert Precondition----------------
            var propertyInfo = type.GetProperty(expectedPropName);
            propertyInfo.AssertIsSingleRelationship();
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }
        [Test]
        public void Test_Get1_1RevRels_WhenRelHasManyToOneAttribute_ShouldNotHaveItems()
        {
            //You have a single Relationship that has a ManyToOne Attribute
            // Its reverse relationship can never be single.
            //---------------Set up test pack-------------------
            const string expectedPropName = "MySingleRelationshipRev";
            var type = typeof(FakeBOWithM1Attribute).ToTypeWrapper();
            //---------------Assert Precondition----------------
            var propertyInfo = type.GetProperty(expectedPropName);
            propertyInfo.AssertIsSingleRelationship();
            propertyInfo.HasAttribute<AutoMapManyToOneAttribute>();
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

        [Test]
        public void Test_Get1_1RevRels_WhenReverseRelHasManyToOne_ShouldNotHaveItems()
        {
            //You have a single Relationship that has a ManyToOne Attribute
            // Its reverse relationship can never be single.
            //---------------Set up test pack-------------------
            const string expectedPropName = "MySingleRelationship";
            var ownerClassType = typeof(FakeBOWithReverseSingle).ToTypeWrapper();
            var propertyInfo = ownerClassType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            propertyInfo.AssertIsSingleRelationship();
            const string reveresRelationshipName = "MySingleRelationshipRev";
            AssertRelationshipIsForOwnerClass(ownerClassType, reverseClassType, reveresRelationshipName);
            var reversePropInfo = reverseClassType.GetProperty(reveresRelationshipName);
            reversePropInfo.HasAttribute<AutoMapManyToOneAttribute>();
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

        [Test]
        public void Test_Get1_1RevRels_WhenHasOneSingleRev_ShouldReturnOneItem()
        {
            //'FakeBOWithReverseSingleRel' has a Relationship 'MySingleRelationship'
            // to 'FakeBOWithTwoSingleRelNoRevs' that has two single relationships.
            // but only 'MyReverseSingleRel' returns the owner Class.
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithReverseSingleRel).ToTypeWrapper();
            const string expectedPropName = "MySingleRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;

            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<FakeBOWithTwoSingleRel>();
            propertyInfo.AssertIsSingleRelationship();
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MyReverseSingleRel");
            AssertReverseRelationshipNotForOwnerClass(classType, reverseClassType, "MySingleRelationship");
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, singleRevRels.Count);
            Assert.AreEqual("MyReverseSingleRel", singleRevRels[0].Name);
        }

        [Test]
        public void Test_Get1_1RevRels_WhenAutoMapOneToOne_ShouldReturnOneItem()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithReverseSingle).ToTypeWrapper();
            const string expectedPropName = "MySingleRelationship2";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;

            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<FakeBOWith11Attribute>();
            propertyInfo.AssertIsSingleRelationship();
            propertyInfo.AssertHasAttribute<AutoMapOneToOneAttribute>();

            const string expectedRevRelName = "MySingleRevRelationship";
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, expectedRevRelName);
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship2");
            AssertHasOneToOneWithReverseRelationship(propertyInfo, expectedRevRelName);
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, singleRevRels.Count);
            Assert.AreEqual(expectedRevRelName, singleRevRels[0].Name);
        }

        [Test]
        public void Test_Get1_1RevRels_WhenReverseAutoMapOneToOne_ShouldReturnOneItem()
        {
            //'FakeBOWith11Attribute' has a Relationship 'MySingleRevRelationship'
            // to 'FakeBOWithReverseSingle' that has two single relationships back.
            // but 'MySingleRelationship2' has a OneToOneAttribute back to 'MySingleRevRelationship'
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWith11Attribute).ToTypeWrapper();
            const string expectedPropName = "MySingleRevRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<FakeBOWithReverseSingle>();
            propertyInfo.AssertIsSingleRelationship();
            
            const string expectedRevRelName = "MySingleRelationship2";
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, expectedRevRelName);
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship2");
            var reversePropertyInfo = reverseClassType.GetProperty(expectedRevRelName);
            AssertHasOneToOneWithReverseRelationship(reversePropertyInfo, expectedPropName);
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, singleRevRels.Count);
            Assert.AreEqual(expectedRevRelName, singleRevRels[0].Name);
        }

        [Test]
        public void Test_Get1_1RevRels_WhenHasIgnore_ShouldReturnNoItems()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithReverseSingle).ToTypeWrapper();
            const string expectedPropName = "MySingleIgnorRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<FakeBOWith11Attribute>();
            propertyInfo.AssertIsSingleRelationship();
            propertyInfo.AssertHasIgnoreAttribute();

            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRevRelationship");
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship2");

            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }
        [Test]
        public void Test_Get1_1RevRels_WhenReverseHasIgnore_ShouldReturnNoItems()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithReverseSingle).ToTypeWrapper();
            const string expectedPropName = "MySingleWithReverseIgnore";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<FakeBOWithIgnoreAttribute>();
            propertyInfo.AssertIsSingleRelationship();

            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship1");
            AssertReverseRelationshipHasIgnoreAttribute(reverseClassType, "MySingleRelationship1");
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }
        [Test]
        public void Test_Get1_1RevRels_WhenHasMultipleReverse_NoAttributes_ShouldReturnBothPotentialReverseRels()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithReverseSingle).ToTypeWrapper();
            const string expectedPropName = "MySingleRelationship3";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<FakeBOWith11Attribute>();
            propertyInfo.AssertIsSingleRelationship();

            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRevRelationship");
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship2");
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, singleRevRels.Count);
        }
        [Test]
        public void Test_Get1_1RevRels_WhenIsMultiple_ShouldReturnNone()
        {
            var classType = typeof(FakeBoWithMultipleRel).ToTypeWrapper();
            const string expectedPropName = "MyMultipleWithTwoSingleReverse";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var relatedClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();

            AssertRelationshipIsForOwnerClass(classType, relatedClassType, "MySingleRelationship1");
            AssertRelationshipIsForOwnerClass(classType, relatedClassType, "MySingleRelationship2");
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

#endregion


#region ManyToOneReverseRelationship


        //The GetManyToOneReverseRelationshipInfos for a Multiple should return ALL Props for SingleRelationships 
        //      that that are for this ownerClass 
        //   ----Unless----
        //1 - ReverseRel directly mapped via a ManyToOne Attribute on this Relationship.
        //Or 2 - This Relationship is mapped by a OneToMany Attribute on reverse relationship.
        //Or 3 - This Relationship has an Ignore Attribute.
        //or 4 - The Reverse Relationship has an Ignore Attribute
        [Test]
        public void Test_GetM1RevRels_WhenHasTwo_ShouldReturnBoth()
        {
            var classType = typeof(FakeBoWithMultipleRel).ToTypeWrapper();
            const string expectedPropName = "MyMultipleWithTwoSingleReverse";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var relatedClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();

            AssertRelationshipIsForOwnerClass(classType, relatedClassType, "MySingleRelationship1");
            AssertRelationshipIsForOwnerClass(classType, relatedClassType, "MySingleRelationship2");
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, singleRevRels.Count);
            singleRevRels.ShouldContain(info => info.Name == "MySingleRelationship1");
            singleRevRels.ShouldContain(info => info.Name == "MySingleRelationship2");
        }

        [Test]
        public void Test_GetM1RevRels_WhenNotHas_ShouldNotHaveItems()
        {
            var classType = typeof(FakeBoWithMultipleRel).ToTypeWrapper();
            const string expectedPropName = "MyMultipleRevRel";
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

        [Test]
        public void Test_GetM1RevRels_WhenOneRevIgnored_ShouldReturnOne()
        {
            //Have a relationship that references a class that has two reverse relationships
            // to this class but one of them has an ignore attribute.
            //Should return one relationship.
            //---------------Set up test pack-------------------
            const string expectedPropName = "MyMultipleWithTwoSingleReverseOneIgnore";
            var classType = typeof(FakeBoWithMultipleRel).ToTypeWrapper();
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;

            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship1");
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship2");
            AssertReverseRelationshipHasIgnoreAttribute(reverseClassType, "MySingleRelationship2");
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, singleRevRels.Count);
            singleRevRels.ShouldContain(info => info.Name == "MySingleRelationship1");
        }

        [Test]
        public void Test_GetSingleRevRels_WhenRevRelHasAutomapRel_ShouldReturnItemWithMappingRel()
        {
            //The Single Reverse Relationship has an Attribute mapping it to this relationship.
            //Even though there are other single reverse relationships to this relationship onle the one should be returned.
            var classType = typeof(FakeBoWithMultipleRel).ToTypeWrapper();

            const string expectedPropName = "MyMultipleReverseAutoMapped";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship1");
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship2");
            const string expectedReverseRel = "MySingleRelationship3";
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, expectedReverseRel);
            AssertReverseRelationshipHasAutoMapToThisRel(reverseClassType, expectedReverseRel, expectedPropName);
            var reversePropInfo = reverseClassType.GetProperty(expectedReverseRel);
            Assert.IsTrue(reversePropInfo.HasAutoMapManyToOneAttribute(expectedPropName), "Should have automap attribute");
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, singleRevRels.Count);
            singleRevRels.ShouldContain(info => info.Name == expectedReverseRel);
        }

        [Test]
        public void Test_GetSingleRels_WhenRevRelHasOneToOneAtt_ShouldReturnNoItems()
        {
            //The Single Reverse Relationship has an Attribute mapping it to this relationship.
            //Even though there are other single reverse relationships to this relationship onle the one should be returned.
            var classType = typeof (FakeBoWithMultipleRel).ToTypeWrapper();

            const string expectedPropName = "ReverseHasAutoMapOneToOne";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship1");

            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

        [Test]
        public void Test_HasSingleReverseRelationship_WhenNotHas_ShouldBeFalse()
        {
            //'FakeBoWithMultipleRel' has relationship 'MyMultipleRevRel'
            // that is related to 'FakeBOWithUndefinableSingleRel' which does not
            // have any single relationships mapped back to this class.
            var classType = typeof(FakeBoWithMultipleRel).ToTypeWrapper();
            const string expectedPropName = "MyMultipleRevRel";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();
            AssertReverseRelationshipNotForOwnerClass(classType, reverseClassType, "MySingleRelationship");
            AssertReverseRelationshipNotForOwnerClass(classType, reverseClassType, "MySingleRelWithOneToOneAttribute");
            AssertReverseRelationshipNotForOwnerClass(classType, reverseClassType, "MySingleRelWithOneToManyAttribute");
            //---------------Execute Test ----------------------
            var hasSingleReverseRelationship = propertyInfo.HasSingleReverseRelationship;
            //---------------Test Result -----------------------
            Assert.IsFalse(hasSingleReverseRelationship);
        }
        [Test]
        public void Test_GetM1RevRels_WhenHasTwo_ThisRelIgnored_ShouldReturnNone()
        {
            var classType = typeof(FakeBoWithMultipleRel).ToTypeWrapper();
            const string expectedPropName = "MyMultipleWithTwoSingleReverseThisIgnore";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var relatedClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();

            AssertRelationshipIsForOwnerClass(classType, relatedClassType, "MySingleRelationship1");
            AssertRelationshipIsForOwnerClass(classType, relatedClassType, "MySingleRelationship2");
            propertyInfo.AssertHasIgnoreAttribute();
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

        [Test]
        public void Test_GetM1RevRels_WhenRelIsSingle_ShouldReturnNone()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithReverseSingle).ToTypeWrapper();
            const string expectedPropName = "MySingleRelationship3";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<FakeBOWith11Attribute>();
            propertyInfo.AssertIsSingleRelationship();

            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRevRelationship");
            AssertRelationshipIsForOwnerClass(classType, reverseClassType, "MySingleRelationship2");
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

        [Test]
        public void Test_GetM1RevRels_WhenHasTwoButRelHasOneToManyMapping_ShouldReturnMappedRel()
        {
            var classType = typeof(FakeBoWithMultipleRel).ToTypeWrapper();
            const string expectedPropName = "MyMultipleAutoMapWithTwoSingleReverse";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var relatedClassType = propertyInfo.RelatedClassType;
            const string expectedMappedReverseRel = "MySingleRelationship2";
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();

            AssertRelationshipIsForOwnerClass(classType, relatedClassType, "MySingleRelationship1");
            AssertRelationshipIsForOwnerClass(classType, relatedClassType, expectedMappedReverseRel);
            AssertReverseRelationshipHasAutoMapToThisRel(classType, expectedPropName, expectedMappedReverseRel);
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, singleRevRels.Count);
            singleRevRels.ShouldContain(info => info.Name == expectedMappedReverseRel);
        }

        [Test]
        public void Test_GetM1RevRels_WhenMappedToNonExistentReverse_ShouldReturnNoItems()
        {
            var classType = typeof(FakeBoWithMultipleRel).ToTypeWrapper();
            const string expectedPropName = "MultipleMappedToNonExistentReverse";
            var propertyInfo = classType.GetProperty(expectedPropName);
            var relatedClassType = propertyInfo.RelatedClassType;
            const string expectedMappedReverseRel = "NonExistentReverseRel";
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();
            relatedClassType.AssertPropertyNotExists(expectedMappedReverseRel);
            AssertRelationshipHasAutoMapToThisRel(classType, expectedPropName, expectedMappedReverseRel);
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetManyToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }
        #endregion

        #region GetMappedReverseRelationshipName

        [Test]
        public void Test_GetMappedReverseRelationshipName_WhenNoAttribute_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            PropertyInfo info = RandomValueGenerator.GetMockPropInfoWithNoAutoMapProp<AutoMapOneToManyAttribute>();
            PropertyWrapper propertyWrapper = info.ToPropertyWrapper();
            object[] customAttributes = info.GetCustomAttributes(typeof(AutoMapOneToManyAttribute), true);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(customAttributes);
            Assert.AreEqual(0, customAttributes.Count());
            //---------------Execute Test ----------------------
            string mappedReverseRelationshipName = propertyWrapper.GetMappedReverseRelationshipName<AutoMapOneToManyAttribute>();
            //---------------Test Result -----------------------
            Assert.IsNull(mappedReverseRelationshipName);
        }
        [Test]
        public void Test_GetMappedReverseRelationshipName_WhenAutoMapPropNoReverseRel_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            PropertyInfo info = RandomValueGenerator.GetMockPropInfoWithAutoMapAttribute<AutoMapOneToManyAttribute>();
            PropertyWrapper propertyWrapper = info.ToPropertyWrapper();
            object[] customAttributes = info.GetCustomAttributes(typeof(AutoMapOneToManyAttribute), true);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(customAttributes);
            Assert.AreEqual(1, customAttributes.Count());
            var autoMapRelationshipAttribute = customAttributes[0] as AutoMapRelationshipAttribute;
            Assert.IsNotNull(autoMapRelationshipAttribute);
            Assert.IsNull(autoMapRelationshipAttribute.ReverseRelationshipName);
            //---------------Execute Test ----------------------
            string mappedReverseRelationshipName = propertyWrapper.GetMappedReverseRelationshipName<AutoMapOneToManyAttribute>();
            //---------------Test Result -----------------------
            Assert.IsNull(mappedReverseRelationshipName);
        }
        [Test]
        public void Test_GetMappedReverseRelationshipName_WhenAutoMapPropWithReverseRel_ShouldReturnReverseRel()
        {
            //---------------Set up test pack-------------------
            const string expectedRevRelName = "MappedRevRel";
            PropertyInfo info = RandomValueGenerator.GetMockPropInfoWithAutoMapAttribute<AutoMapOneToManyAttribute>(expectedRevRelName);
            PropertyWrapper propertyWrapper = info.ToPropertyWrapper();
            object[] customAttributes = info.GetCustomAttributes(typeof(AutoMapOneToManyAttribute), true);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(customAttributes);
            Assert.AreEqual(1, customAttributes.Count());
            var autoMapRelationshipAttribute = customAttributes[0] as AutoMapRelationshipAttribute;
            Assert.IsNotNull(autoMapRelationshipAttribute);
            Assert.IsNotNullOrEmpty(autoMapRelationshipAttribute.ReverseRelationshipName);
            //---------------Execute Test ----------------------
            string mappedReverseRelationshipName = propertyWrapper.GetMappedReverseRelationshipName<AutoMapOneToManyAttribute>();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedRevRelName, mappedReverseRelationshipName);
        }

        #endregion

        #region GetSingleReverseRelationshipName

        [Test]
        public void Test_GetSingleRevRelationshipName_WhenNoRevRel_WhenHasAttribute_WhenHasRevRelName__ShouldBeRevRelName()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithReverseSingle).ToTypeWrapper();
            const string relationshipName = "MySingleWithAutoMapNoReverse";
            var relPropInfo = classType.GetProperty(relationshipName);
            const string mappedRevRelName = "NoRevRel";
            //---------------Assert Precondition----------------
            relPropInfo.AssertHasAttribute<AutoMapOneToOneAttribute>(attribute => attribute.ReverseRelationshipName == mappedRevRelName);
            //---------------Execute Test ----------------------
            string reverseRelationshipName = relPropInfo.GetSingleReverseRelationshipName<AutoMapOneToOneAttribute>();
            //---------------Test Result -----------------------
            Assert.AreEqual(mappedRevRelName, reverseRelationshipName);
        }
        [Test]
        public void Test_GetSingleRevRelationshipName_WhenHasRevRel_ShouldBeFoundRevRelName()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithReverseSingle).ToTypeWrapper();
            const string relationshipName = "SingleWithRevesre";
            PropertyWrapper relPropInfo = classType.GetProperty(relationshipName);
            const string foundRevRelationship = "ReverseSingleRel";
            var relatedClassType = relPropInfo.RelatedClassType;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(relPropInfo);
            relatedClassType.AssertPropertyExists(foundRevRelationship);
            //---------------Execute Test ----------------------
            string reverseRelationshipName = relPropInfo.GetSingleReverseRelationshipName<AutoMapOneToOneAttribute>();
            //---------------Test Result -----------------------
            Assert.AreEqual(foundRevRelationship, reverseRelationshipName);
        }

        [Test]
        public void Test_GetSingleRevRelationshipName_WhenNoRevRel_WhenHasAttribute_WhenNoRevRelName__ShouldBeOwnerClassName()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithOneToOneAttribute).ToTypeWrapper();
            const string relationshipName = "MySingleRelationship";
            var relPropInfo = classType.GetProperty(relationshipName);
            //---------------Assert Precondition----------------
            relPropInfo.AssertHasAttribute<AutoMapOneToOneAttribute>(attribute => attribute.ReverseRelationshipName == null);
            //---------------Execute Test ----------------------
            string reverseRelationshipName = relPropInfo.GetSingleReverseRelationshipName<AutoMapOneToOneAttribute>();
            //---------------Test Result -----------------------
            Assert.AreEqual(classType.Name, reverseRelationshipName);
        }

        [Test]
        public void Test_GetSingleRevRelationshipName_ShouldReturnRelDefWithRevRelNameSet()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOAttributePKAndPKNaming).ToTypeWrapper();
            const string expectedPropName = "MyMultipleRevRel2";
            PropertyWrapper propertyInfo = classType.GetProperty(expectedPropName);
            PropertyWrapper reverseRelPropInfo = propertyInfo.GetSingleReverseRelPropInfos()[0];
            //---------------Assert Precondition----------------
            propertyInfo.AssertIsMultipleRelationship();
            Assert.IsTrue(propertyInfo.HasSingleReverseRelationship, "There is no reverse single rel");

            Assert.AreNotEqual(classType.Name, reverseRelPropInfo.Name);
            //---------------Execute Test ----------------------
            string reverseRelationshipName = propertyInfo.GetSingleReverseRelationshipName<AutoMapOneToManyAttribute>();
            //---------------Test Result -----------------------
            Assert.AreEqual(reverseRelPropInfo.Name, reverseRelationshipName);
        }
        #endregion


        private static void AssertRelationshipHasAutoMapToThisRel(TypeWrapper classType, string relName, string expectedReversRelName)
        {

            var relProp = classType.GetProperty(relName);
            var customRelationship = relProp.GetAttributes<AutoMapRelationshipAttribute>();
            Assert.IsTrue(customRelationship.Any(o => (o.ReverseRelationshipName == expectedReversRelName)),
                          relName + " on " + classType.Name + " should have an AutoMapManyToOne attribute");

        }
        private static void AssertReverseRelationshipHasAutoMapToThisRel(TypeWrapper reverseClassType, string relName, string expectedReversRelName)
        {

            var reverseRelProp = reverseClassType.GetProperty(relName);
            var customRelationship = reverseRelProp.GetAttributes<AutoMapRelationshipAttribute>();
            Assert.IsTrue(customRelationship.Any(o => (o.ReverseRelationshipName == expectedReversRelName)),
                          relName + " on " + reverseClassType.Name + " should have an AutoMapManyToOneAttribute attribute");

        }

        private static void AssertHasOneToOneWithReverseRelationship(PropertyWrapper propertyInfo, string expectedRevRelName)
        {
            Assert.IsTrue(propertyInfo.HasAutoMapOneToOneAttribute(expectedRevRelName), string.Format("Should have AutoMapOneToOne with ReverseRelationship '{0}'", expectedRevRelName));
        }

        private static void AssertReverseRelationshipHasIgnoreAttribute(TypeWrapper reverseClassType, string relName)
        {
            var singleRelProp = reverseClassType.GetProperty(relName);
            Assert.IsTrue(singleRelProp.HasIgnoreAttribute, relName + " on " + reverseClassType.Name + " should have an ignore attribute");
        }

        private static void AssertRelationshipIsForOwnerClass(TypeWrapper ownerClassType, TypeWrapper reverseClassType, string reveresRelationshipName)
        {
            var reversePropInfo = reverseClassType.GetProperty(reveresRelationshipName);
            Assert.IsNotNull(reversePropInfo, reveresRelationshipName + " on " + reverseClassType.Name + " should not be null");
            Assert.AreSame(ownerClassType.UnderlyingType, reversePropInfo.RelatedClassType.UnderlyingType);
        }
        private static void AssertReverseRelationshipNotForOwnerClass(TypeWrapper ownerClassType, TypeWrapper reverseClassType, string reveresRelationshipName)
        {
            var reversePropInfo = reverseClassType.GetProperty(reveresRelationshipName);
            Assert.IsNotNull(reversePropInfo, "No Reverse Relationship found with the name");
            Assert.AreNotSame(ownerClassType, reversePropInfo.RelatedClassType);
        }


    }
}