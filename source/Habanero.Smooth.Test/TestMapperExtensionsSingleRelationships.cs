using Habanero.Smooth.ReflectionWrappers;
using NUnit.Framework;

namespace Habanero.Smooth.Test
{
            //The GetOneToOneReverseRelationshipInfos for a SingleRelationship should return all Props for SingleRelationships 
        //      that that are for the ownerClass 
        //   ----Unless----
        //1- This Relationship is mapped as a ManyToOneAttribute Or ReverseRelationship MappedVia M:1
        //2 - ReverseRel directly mapped via a OneToOne attribute to this relationship.
        //Or 3 - This Relationship is mapped by a OnetoOne Attribute on reverse relationship.
        //Or 4 - This Relationship has an Ignore Attribute.
        //or 5 - The Reverse Relationship has an Ignore Attribute
    [TestFixture]
    public class TestMapperExtensionsSingleRelationships
    {
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
            Assert.IsTrue(propertyInfo.IsSingleRelationhip);
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
            classType.AssertRelationshipIsForOwnerClass(reverseClassType, "MySingleRelationship1");
            classType.AssertRelationshipIsForOwnerClass(reverseClassType, "MySingleRelationship2");
            //---------------Execute Test ----------------------
            var hasSingleReverseRelationship = propertyInfo.HasSingleReverseRelationship;
            //---------------Test Result -----------------------
            Assert.IsTrue(hasSingleReverseRelationship);
        }

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
            ownerClassType.AssertRelationshipIsForOwnerClass(reverseClassType, reveresRelationshipName);
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
            classType.AssertRelationshipIsForOwnerClass(reverseClassType, "MyReverseSingleRel");
            classType.AssertReverseRelationshipNotForOwnerClass(reverseClassType, "MySingleRelationship");
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
            classType.AssertRelationshipIsForOwnerClass(reverseClassType, expectedRevRelName);
            classType.AssertRelationshipIsForOwnerClass(reverseClassType, "MySingleRelationship2");
            propertyInfo.AssertHasOneToOneWithReverseRelationship(expectedRevRelName);
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
            classType.AssertRelationshipIsForOwnerClass(reverseClassType, expectedRevRelName);
            classType.AssertRelationshipIsForOwnerClass(reverseClassType, "MySingleRelationship2");
            var reversePropertyInfo = reverseClassType.GetProperty(expectedRevRelName);
            reversePropertyInfo.AssertHasOneToOneWithReverseRelationship(expectedPropName);
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

            classType.AssertRelationshipIsForOwnerClass(reverseClassType, "MySingleRevRelationship");
            classType.AssertRelationshipIsForOwnerClass(reverseClassType, "MySingleRelationship2");

            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }

        [Test]
        public void Test_Get1_1RevRels_GivenHasReverseRelationshipMapped_ShouldSetRelKeyDef()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWith11Attribute).ToTypeWrapper();
            const string expectedPropName = "MySingleRevRelationship";
            var propertyWrapper = classType.GetProperty(expectedPropName);
            var reverseClassType = propertyWrapper.RelatedClassType;
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyWrapper.AssertIsOfType<FakeBOWithReverseSingle>();
            propertyWrapper.AssertIsSingleRelationship();

            classType.AssertRelationshipIsForOwnerClass(reverseClassType, "MySingleRelationship2");
            reverseClassType.AssertRelationshipIsForOwnerClass(classType, "MySingleRevRelationship");

            //---------------Execute Test ----------------------
            var singleRevRels = propertyWrapper.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, singleRevRels.Count);
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

            classType.AssertRelationshipIsForOwnerClass(reverseClassType, "MySingleRelationship1");
            reverseClassType.AssertReverseRelationshipHasIgnoreAttribute("MySingleRelationship1");
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

            classType.AssertRelationshipIsForOwnerClass(reverseClassType, "MySingleRevRelationship");
            classType.AssertRelationshipIsForOwnerClass(reverseClassType, "MySingleRelationship2");
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

            classType.AssertRelationshipIsForOwnerClass(relatedClassType, "MySingleRelationship1");
            classType.AssertRelationshipIsForOwnerClass(relatedClassType, "MySingleRelationship2");
            //---------------Execute Test ----------------------
            var singleRevRels = propertyInfo.GetOneToOneReverseRelationshipInfos();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, singleRevRels.Count);
        }
    }
}