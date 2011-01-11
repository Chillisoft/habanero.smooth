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
    public class TestSingleRelationshipDefBuilder
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
            SingleRelationshipDef singleRelationshipDef = GetSingleRelationshipDefBuilder<Car>(relationshipName)
                .WithRelatedType<Car>()
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, singleRelationshipDef.RelationshipName);
            Assert.IsTrue(singleRelationshipDef.KeepReferenceToRelatedObject);
            Assert.AreEqual(DeleteParentAction.DoNothing, singleRelationshipDef.DeleteParentAction);
            Assert.AreEqual("TestProject.BO", singleRelationshipDef.RelatedObjectAssemblyName);
            Assert.AreEqual("Car", singleRelationshipDef.RelatedObjectClassName);
            Assert.AreEqual(0, singleRelationshipDef.RelKeyDef.Count);
            Assert.AreEqual(InsertParentAction.InsertRelationship, singleRelationshipDef.InsertParentAction);
            Assert.AreEqual(RelationshipType.Association, singleRelationshipDef.RelationshipType);
        }

        private SingleRelationshipDefBuilder<T> GetSingleRelationshipDefBuilder<T>(string relationshipName) where T : BusinessObject
        {
            return new SingleRelationshipDefBuilder<T>()
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
                GetSingleRelationshipDefBuilder<Car>("")
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
            SingleRelationshipDef singleRelationshipDef = GetSingleRelationshipDefBuilder<Car>(relationshipName)
                .WithInsertParentAction(InsertParentAction.DoNothing)
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, singleRelationshipDef.RelationshipName);
            Assert.AreEqual(InsertParentAction.DoNothing, singleRelationshipDef.InsertParentAction);
        }

        [Test]
        public void Test_Build_WithRelationshipType_ShouldSetRelationshipType()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "R" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            SingleRelationshipDef singleRelationshipDef = GetSingleRelationshipDefBuilder<Car>(relationshipName)
                .WithRelationshipType(RelationshipType.Aggregation)
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, singleRelationshipDef.RelationshipName);
            Assert.AreEqual(RelationshipType.Aggregation, singleRelationshipDef.RelationshipType);
        }

        [Test]
        public void Test_Build_WithRelProp__ShouldCreateSingleRelDefWithOneProp()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "R" + GetRandomString();
            string propertyName = "P" + GetRandomString();
            string relatedPropName = "P" + GetRandomString();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            SingleRelationshipDef singleRelationshipDef = GetSingleRelationshipDefBuilder<Car>(relationshipName)
                .WithRelProp(propertyName, relatedPropName)
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, singleRelationshipDef.RelationshipName);
            Assert.AreEqual(1, singleRelationshipDef.RelKeyDef.Count);
            IRelPropDef relPropDef = singleRelationshipDef.RelKeyDef[propertyName];
            Assert.IsNotNull(relPropDef);
            Assert.AreEqual(propertyName, relPropDef.OwnerPropertyName);
            Assert.AreEqual(relatedPropName, relPropDef.RelatedClassPropName);
        }

        [Test]
        public void Test_Build_With_2RelProps_ShouldCreateSingleRelDefWithTwoProps()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "R" + GetRandomString();
            string propertyName = "P" + GetRandomString();
            string relatedPropName = "P" + GetRandomString();
            string propertyName2 = "P" + GetRandomString();
            string relatedPropName2 = "P" + GetRandomString();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            SingleRelationshipDef singleRelationshipDef = GetSingleRelationshipDefBuilder<Car>(relationshipName)
                .WithRelProp(propertyName, relatedPropName)
                .WithRelProp(propertyName2, relatedPropName2)
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, singleRelationshipDef.RelationshipName);
            Assert.AreEqual(2, singleRelationshipDef.RelKeyDef.Count);
            IRelPropDef relPropDef = singleRelationshipDef.RelKeyDef[propertyName];
            Assert.IsNotNull(relPropDef);
            Assert.AreEqual(propertyName, relPropDef.OwnerPropertyName);
            Assert.AreEqual(relatedPropName, relPropDef.RelatedClassPropName);  
            relPropDef = singleRelationshipDef.RelKeyDef[propertyName2];
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
            SingleRelationshipDef singleRelationshipDef = new SingleRelationshipDefBuilder<Car>()
                                                            .WithRelationshipName(c => c.SteeringWheel)
                                                            .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual("SteeringWheel", singleRelationshipDef.RelationshipName);
        }
    }
}