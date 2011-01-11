using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.Test;
using NUnit.Framework;
using TestProject.BO;

namespace Habanero.Fluent.Tests
{
    [TestFixture]
    public class TestMultipleRelationshipDefBuilder
    {
        private string GetRandomString()
        {
            return RandomValueGenerator.GetRandomString();
        }

        [Test]
        public void Test_Build_ShouldUseDefaults()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "R" + GetRandomString(); 
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var multipleRelationshipDef = GetMultipleRelationshipDefBuilder<Car>(relationshipName)
                                                                .WithRelatedType<Car>()
                                                                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, multipleRelationshipDef.RelationshipName);
            Assert.IsTrue(multipleRelationshipDef.KeepReferenceToRelatedObject);
            Assert.AreEqual(DeleteParentAction.DoNothing, multipleRelationshipDef.DeleteParentAction);
            Assert.AreEqual("TestProject.BO", multipleRelationshipDef.RelatedObjectAssemblyName);
            Assert.AreEqual("Car", multipleRelationshipDef.RelatedObjectClassName);
            Assert.AreEqual(0, multipleRelationshipDef.RelKeyDef.Count);
            Assert.AreEqual(InsertParentAction.InsertRelationship, multipleRelationshipDef.InsertParentAction);
            Assert.AreEqual(RelationshipType.Association, multipleRelationshipDef.RelationshipType);
            Assert.AreEqual("", multipleRelationshipDef.OrderCriteriaString);
            Assert.AreEqual(0, multipleRelationshipDef.TimeOut);
        }

        private static MultipleRelationshipDefBuilder<T> GetMultipleRelationshipDefBuilder<T>(string relationshipName) where T : BusinessObject
        {
            return new MultipleRelationshipDefBuilder<T>()
                .WithRelationshipName(relationshipName);
        }

        [Test]
        public void Test_Build_NoRelationshipName_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                GetMultipleRelationshipDefBuilder<Car>("")
                    .Build();
                Assert.Fail("Expected to throw an HabaneroArgumentException");
            }
                //---------------Test Result -----------------------
            catch (HabaneroArgumentException ex)
            {
                StringAssert.Contains(
                    "The argument 'relationshipName' is not valid. Argument cannot be a zero length string or null",
                    ex.Message);
            }
        }

        [Test]
        public void Test_Build_WithInsertParentAction_ShouldSetInsertParentAction()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "R" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var multipleRelationshipDef = GetMultipleRelationshipDefBuilder<Car>(relationshipName)
                                                                .WithInsertParentAction(InsertParentAction.DoNothing)
                                                                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(InsertParentAction.DoNothing, multipleRelationshipDef.InsertParentAction);
        }



        [Test]
        public void Test_Build_WithRelationshipType_ShouldSetRelationshipType()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "R" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var multipleRelationshipDef = GetMultipleRelationshipDefBuilder<Car>(relationshipName)
                                                                .WithRelationshipType(RelationshipType.Aggregation)
                                                                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, multipleRelationshipDef.RelationshipName);
            Assert.AreEqual(RelationshipType.Aggregation, multipleRelationshipDef.RelationshipType);
        }

        [Test]
        public void Test_Build_WithOrderBy_ShouldSetOrderBy()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "R" + GetRandomString();
            string orderBy = "O" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var multipleRelationshipDef = GetMultipleRelationshipDefBuilder<Car>(relationshipName)
                                                                .WithOrderBy(orderBy)
                                                                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, multipleRelationshipDef.RelationshipName);
            Assert.AreEqual(orderBy, multipleRelationshipDef.OrderCriteriaString);
        }

        [Test]
        public void Test_Build_WithTimeOut_ShouldSetTimeout()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "R" + GetRandomString();
            int timeout = 254;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var multipleRelationshipDef = GetMultipleRelationshipDefBuilder<Car>(relationshipName)
                                                                .WithTimeout(timeout)
                                                                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, multipleRelationshipDef.RelationshipName);
            Assert.AreEqual(timeout, multipleRelationshipDef.TimeOut);
        }

        [Test]
        public void Test_Build_WithRelProp_ShouldCreateRelDefWithOneProp()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "R" + GetRandomString();
            string propertyName = "P" + GetRandomString();
            string relatedPropName = "P" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            var multipleRelationshipDef = GetMultipleRelationshipDefBuilder<Car>(relationshipName)
                                                                .WithRelProp(propertyName, relatedPropName)
                                                                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, multipleRelationshipDef.RelationshipName);
            Assert.AreEqual(1, multipleRelationshipDef.RelKeyDef.Count);
            IRelPropDef relPropDef = multipleRelationshipDef.RelKeyDef[propertyName];
            Assert.IsNotNull(relPropDef);
            Assert.AreEqual(propertyName, relPropDef.OwnerPropertyName);
            Assert.AreEqual(relatedPropName, relPropDef.RelatedClassPropName);
        }

        [Test]
        public void Test_Build_With_2RelProps_ShouldCreateRelDefWithTwoProps()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "R" + GetRandomString();
            string propertyName = "P" + GetRandomString();
            string relatedPropName = "P" + GetRandomString();
            string propertyName2 = "P" + GetRandomString();
            string relatedPropName2 = "P" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            var multipleRelationshipDef = GetMultipleRelationshipDefBuilder<Car>(relationshipName)
                                                                .WithRelProp(propertyName, relatedPropName)
                                                                .WithRelProp(propertyName2, relatedPropName2)
                                                                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, multipleRelationshipDef.RelationshipName);
            Assert.AreEqual(2, multipleRelationshipDef.RelKeyDef.Count);
            IRelPropDef relPropDef = multipleRelationshipDef.RelKeyDef[propertyName];
            Assert.IsNotNull(relPropDef);
            Assert.AreEqual(propertyName, relPropDef.OwnerPropertyName);
            Assert.AreEqual(relatedPropName, relPropDef.RelatedClassPropName);
            relPropDef = multipleRelationshipDef.RelKeyDef[propertyName2];
            Assert.IsNotNull(relPropDef);
            Assert.AreEqual(propertyName2, relPropDef.OwnerPropertyName);
            Assert.AreEqual(relatedPropName2, relPropDef.RelatedClassPropName);
        }

        [Test]
        public void Test_Build_WithLambdaProp_ShouldSetRelationshipName()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var multipleRelationshipDef = new MultipleRelationshipDefBuilder<Car>()
                                                            .WithRelationshipName(c => c.Drivers)
                                                            .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual("Drivers", multipleRelationshipDef.RelationshipName);
        }

    }
}