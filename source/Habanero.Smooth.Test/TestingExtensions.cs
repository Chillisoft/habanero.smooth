using System.Linq;
using Habanero.Smooth.ReflectionWrappers;
using NUnit.Framework;

namespace Habanero.Smooth.Test
{
    public static class TestingExtensions
    {
        public static void AssertRelationshipHasAutoMapToThisRel(this TypeWrapper classType, string relName, string expectedReversRelName)
        {

            var relProp = classType.GetProperty(relName);
            var customRelationship = relProp.GetAttributes<AutoMapRelationshipAttribute>();
            Assert.IsTrue(customRelationship.Any(o => (o.ReverseRelationshipName == expectedReversRelName)),
                          relName + " on " + classType.Name + " should have an AutoMapManyToOne attribute");

        }

        public static void AssertReverseRelationshipHasAutoMapToThisRel(this TypeWrapper reverseClassType, string relName, string expectedReversRelName)
        {

            var reverseRelProp = reverseClassType.GetProperty(relName);
            var customRelationship = reverseRelProp.GetAttributes<AutoMapRelationshipAttribute>();
            Assert.IsTrue(customRelationship.Any(o => (o.ReverseRelationshipName == expectedReversRelName)),
                          relName + " on " + reverseClassType.Name + " should have an AutoMapManyToOneAttribute attribute");

        }

        public static void AssertHasOneToOneWithReverseRelationship(this PropertyWrapper propertyWrapper, string expectedRevRelName)
        {
            Assert.IsTrue(propertyWrapper.HasAutoMapOneToOneAttribute(expectedRevRelName), string.Format("Should have AutoMapOneToOne with ReverseRelationship '{0}'", expectedRevRelName));
        }

        

        public static void AssertReverseRelationshipHasIgnoreAttribute(this TypeWrapper reverseClassType, string relName)
        {
            var singleRelProp = reverseClassType.GetProperty(relName);
            Assert.IsTrue(singleRelProp.HasIgnoreAttribute, relName + " on " + reverseClassType.Name + " should have an ignore attribute");
        }

        public static void AssertRelationshipIsForOwnerClass(this TypeWrapper ownerClassType, TypeWrapper reverseClassType, string reveresRelationshipName)
        {
            var reversePropInfo = reverseClassType.GetProperty(reveresRelationshipName);
            Assert.IsNotNull(reversePropInfo, reveresRelationshipName + " on " + reverseClassType.Name + " should not be null");
            Assert.AreSame(ownerClassType.UnderlyingType, reversePropInfo.RelatedClassType.UnderlyingType);
        }

        public static void AssertReverseRelationshipNotForOwnerClass(this TypeWrapper ownerClassType, TypeWrapper reverseClassType, string reveresRelationshipName)
        {
            var reversePropInfo = reverseClassType.GetProperty(reveresRelationshipName);
            Assert.IsNotNull(reversePropInfo, "No Reverse Relationship found with the name");
            Assert.AreNotSame(ownerClassType, reversePropInfo.RelatedClassType);
        }
    }
}