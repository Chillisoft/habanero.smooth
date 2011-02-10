using System;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.Test;
using NUnit.Framework;
using TestProject.BO;

namespace Habanero.Fluent.Tests
{
    [TestFixture, Ignore("No longer valid as you cannot build a SingleRelationshipDef without relprops")]
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
            var singleRelationshipDef = GetSingleRelationshipDefBuilder<Car, Car>(relationshipName)
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

        private OldSingleRelationshipDefBuilder<T, TRelatedType> GetSingleRelationshipDefBuilder<T, TRelatedType>(string relationshipName) where T : BusinessObject where TRelatedType : BusinessObject
        {
            var singleRelationshipDefBuilder = new OldSingleRelationshipDefBuilder<T, TRelatedType>(relationshipName);
            return singleRelationshipDefBuilder;
        }

        [Test]
        public void Test_NoRelationshipName_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                GetSingleRelationshipDefBuilder<Car, Car>("");
                    //.Build();
                Assert.Fail("Expected to throw an ArgumentNullException");
            }
            //---------------Test Result -----------------------
            catch (ArgumentException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                Assert.AreEqual("relationshipName", ex.ParamName);
            }
        }

        [Test]
        public void Test_Build_WithInsertParentAction_ShouldSetInsertParentAction()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "R" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var singleRelationshipDef = GetSingleRelationshipDefBuilder<Car, Car>(relationshipName)
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
            var singleRelationshipDef = GetSingleRelationshipDefBuilder<Car, Car>(relationshipName)
                .WithRelationshipType(RelationshipType.Aggregation)
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, singleRelationshipDef.RelationshipName);
            Assert.AreEqual(RelationshipType.Aggregation, singleRelationshipDef.RelationshipType);
        }
/*

        [Test]
        public void Test_Build_WithRelProp__ShouldCreateSingleRelDefWithOneProp()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "R" + GetRandomString();
            string propertyName = "P" + GetRandomString();
            string relatedPropName = "P" + GetRandomString();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var singleRelationshipDef = GetSingleRelationshipDefBuilder<Car, Car>(relationshipName)
                .WithRelProp(propertyName, relatedPropName)
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, singleRelationshipDef.RelationshipName);
            Assert.AreEqual(1, singleRelationshipDef.RelKeyDef.Count);
            var relPropDef = singleRelationshipDef.RelKeyDef[propertyName];
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
            var singleRelationshipDef = GetSingleRelationshipDefBuilder<Car, Car>(relationshipName)
                .WithRelProp(propertyName, relatedPropName)
                .WithRelProp(propertyName2, relatedPropName2)
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, singleRelationshipDef.RelationshipName);
            Assert.AreEqual(2, singleRelationshipDef.RelKeyDef.Count);
            var relPropDef = singleRelationshipDef.RelKeyDef[propertyName];
            Assert.IsNotNull(relPropDef);
            Assert.AreEqual(propertyName, relPropDef.OwnerPropertyName);
            Assert.AreEqual(relatedPropName, relPropDef.RelatedClassPropName);  
            relPropDef = singleRelationshipDef.RelKeyDef[propertyName2];
            Assert.IsNotNull(relPropDef);
            Assert.AreEqual(propertyName2, relPropDef.OwnerPropertyName);
            Assert.AreEqual(relatedPropName2, relPropDef.RelatedClassPropName);
        }
*/
        //TODO andrew 20 Dec 2010: 

        [Test]
        public void Test_Build_WithLambdaProp_ShouldSetRelationshipName()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var singleRelationshipDef = new OldSingleRelationshipDefBuilder<Car, SteeringWheel>(c => c.SteeringWheel)
                                                            .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual("SteeringWheel", singleRelationshipDef.RelationshipName);
        }
/*        [Test]
        public void Test_Build_WithLambdaProp_ShouldSetRelationshipProperties()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var singleRelationshipDef = new SingleRelationshipDefBuilder<Car, SteeringWheel>(c => c.SteeringWheel)
                                                            .WithRelProp(car => car.VehicleID, wheel => wheel.CarID)
                                                            .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual("SteeringWheel", singleRelationshipDef.RelationshipName);
            var relPropDef = singleRelationshipDef.RelKeyDef["VehicleID"];
            Assert.IsNotNull(relPropDef);
            Assert.AreEqual("VehicleID", relPropDef.OwnerPropertyName);
            Assert.AreEqual("CarID", relPropDef.RelatedClassPropName);
        }*/
        //TODO andrew 20 Dec 2010: 
    }
}