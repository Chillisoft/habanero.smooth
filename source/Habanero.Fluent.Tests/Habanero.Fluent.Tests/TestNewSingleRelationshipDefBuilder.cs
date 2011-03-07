using System;
using System.Linq.Expressions;
using Habanero.Base;
using Habanero.Fluent.Tests.TestStubs;
using Habanero.Smooth.Test;
using NUnit.Framework;
using TestProject.BO;

namespace Habanero.Fluent.Tests
{
    public class TestNewSingleRelationshipDefBuilder
    {
        private string GetRandomString()
        {
            return RandomValueGenerator.GetRandomString();
        }

        [Ignore("Work out why the OwningClassName is not being set")] //TODO Andrew Russell 21 Feb 2011: Ignored Test - Work out why the OwningClassName is not being set
        [Test]
        public void Test_Build_ShouldConstructCorrectly()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "SteeringWheel"; 
            NewSingleRelationshipDefBuilder<Car, SteeringWheel> newSingleRelationshipDefBuilder = GetNewSingleRelationshipDefBuilder(car => car.SteeringWheel);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var singleRelationshipDef = newSingleRelationshipDefBuilder.Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, singleRelationshipDef.RelationshipName);
            Assert.IsTrue(singleRelationshipDef.KeepReferenceToRelatedObject);
            Assert.AreEqual(DeleteParentAction.DoNothing, singleRelationshipDef.DeleteParentAction);
            Assert.AreEqual("Car", singleRelationshipDef.OwningClassName);
            Assert.AreEqual("TestProject.BO", singleRelationshipDef.RelatedObjectAssemblyName);
            Assert.AreEqual("SteeringWheel", singleRelationshipDef.RelatedObjectClassName);
            Assert.AreEqual(0, singleRelationshipDef.RelKeyDef.Count);
            Assert.AreEqual(InsertParentAction.InsertRelationship, singleRelationshipDef.InsertParentAction);
            Assert.AreEqual(RelationshipType.Association, singleRelationshipDef.RelationshipType);
        }

        [Ignore("Work out why the OwningClassName is not being set")] //TODO Andrew Russell 21 Feb 2011: Ignored Test - Work out why the OwningClassName is not being set
        [Test]
        public void Test_Build_ShouldUseDefaults()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "R" + GetRandomString(); 
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var singleRelationshipDef = GetNewSingleRelationshipDefBuilder(relationshipName)
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

        [Test]
        public void Test_NoRelationshipName_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                GetNewSingleRelationshipDefBuilder("");
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
            var singleRelationshipDef = GetNewSingleRelationshipDefBuilder(relationshipName)
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
            var singleRelationshipDef = GetNewSingleRelationshipDefBuilder(relationshipName)
            .WithRelationshipType(RelationshipType.Aggregation)
            .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, singleRelationshipDef.RelationshipName);
            Assert.AreEqual(RelationshipType.Aggregation, singleRelationshipDef.RelationshipType);
        }

        // This test is no longer relevant as the WithRelProp is on the SingleRelKeyDefBuilder
        //[Test]
        //public void Test_Build_WithRelProp__ShouldCreateSingleRelDefWithOneProp()
        //{
        //    //---------------Set up test pack-------------------
        //    string relationshipName = "R" + GetRandomString();
        //    string propertyName = "P" + GetRandomString();
        //    string relatedPropName = "P" + GetRandomString();
        //    //---------------Assert Precondition----------------
        //    //---------------Execute Test ----------------------
        //    var singleRelationshipDef = GetNewSingleRelationshipDefBuilder(relationshipName)

        //    .WithRelProp(propertyName, relatedPropName)
        //    .Build();
        //    //---------------Test Result -----------------------
        //    Assert.AreEqual(relationshipName, singleRelationshipDef.RelationshipName);
        //    Assert.AreEqual(1, singleRelationshipDef.RelKeyDef.Count);
        //    var relPropDef = singleRelationshipDef.RelKeyDef[propertyName];
        //    Assert.IsNotNull(relPropDef);
        //    Assert.AreEqual(propertyName, relPropDef.OwnerPropertyName);
        //    Assert.AreEqual(relatedPropName, relPropDef.RelatedClassPropName);
        //}

        // This test is no longer relevant as the WithRelProp is on the SingleRelKeyDefBuilder
        //[Test]
        //public void Test_Build_With_2RelProps_ShouldCreateSingleRelDefWithTwoProps()
        //{
        //    //---------------Set up test pack-------------------
        //    string relationshipName = "R" + GetRandomString();
        //    string propertyName = "P" + GetRandomString();
        //    string relatedPropName = "P" + GetRandomString();
        //    string propertyName2 = "P" + GetRandomString();
        //    string relatedPropName2 = "P" + GetRandomString();
        //    //---------------Assert Precondition----------------
        //    //---------------Execute Test ----------------------
        //    var singleRelationshipDef = GetSingleRelationshipDefBuilder<Car, Car>(relationshipName)
        //    .WithRelProp(propertyName, relatedPropName)
        //    .WithRelProp(propertyName2, relatedPropName2)
        //    .Build();
        //    //---------------Test Result -----------------------
        //    Assert.AreEqual(relationshipName, singleRelationshipDef.RelationshipName);
        //    Assert.AreEqual(2, singleRelationshipDef.RelKeyDef.Count);
        //    var relPropDef = singleRelationshipDef.RelKeyDef[propertyName];
        //    Assert.IsNotNull(relPropDef);
        //    Assert.AreEqual(propertyName, relPropDef.OwnerPropertyName);
        //    Assert.AreEqual(relatedPropName, relPropDef.RelatedClassPropName);
        //    relPropDef = singleRelationshipDef.RelKeyDef[propertyName2];
        //    Assert.IsNotNull(relPropDef);
        //    Assert.AreEqual(propertyName2, relPropDef.OwnerPropertyName);
        //    Assert.AreEqual(relatedPropName2, relPropDef.RelatedClassPropName);
        //}

        //[Test]
        //public void Test_Build_WithLambdaProp_ShouldSetRelationshipName()
        //{
        //    //---------------Set up test pack-------------------
        //    var newRelationshipsBuilder = new NewRelationshipsBuilderStub<Car>();

        //    //---------------Assert Precondition----------------

        //    //---------------Execute Test ----------------------
        //    var newSingleRelationshipDefBuilder = new NewSingleRelationshipDefBuilder<Car, Car>(newRelationshipsBuilder,);
        //    newSingleRelationshipDefBuilder.Build();

        //    //---------------Test Result -----------------------
        //    Assert.AreEqual("SteeringWheel", newSingleRelationshipDefBuilder.RelationshipName);
        //}

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

        private static NewSingleRelationshipDefBuilder<Car, SteeringWheel> GetNewSingleRelationshipDefBuilder(string relationshipName)
        {
            var newRelationshipsBuilder = new NewRelationshipsBuilderStub<Car>();
            return new NewSingleRelationshipDefBuilder<Car, SteeringWheel>(newRelationshipsBuilder, relationshipName);
        }

        private static NewSingleRelationshipDefBuilder<Car, SteeringWheel> GetNewSingleRelationshipDefBuilder(Expression<Func<Car, SteeringWheel>> relationshipExpression)
        {
            var newRelationshipsBuilder = new NewRelationshipsBuilderStub<Car>();
            return new NewSingleRelationshipDefBuilder<Car, SteeringWheel>(newRelationshipsBuilder, relationshipExpression);
        }


    }
}