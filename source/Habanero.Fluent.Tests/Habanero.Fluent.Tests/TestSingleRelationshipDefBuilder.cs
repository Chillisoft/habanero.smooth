using System;
using System.Linq.Expressions;
using Habanero.Base;
using Habanero.Fluent.Tests.TestStubs;
using Habanero.Smooth.Test;
using NUnit.Framework;
using TestProject.BO;

// ReSharper disable InconsistentNaming
namespace Habanero.Fluent.Tests
{
    public class TestSingleRelationshipDefBuilder
    {

        [Test]
        public void Test_Build_ShouldConstructCorrectly()
        {
            //---------------Set up test pack-------------------
            const string relationshipName = "SteeringWheel"; 
            SingleRelationshipDefBuilder<Car, SteeringWheel> singleRelationshipDefBuilder = GetNewSingleRelationshipDefBuilder(car => car.SteeringWheel);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var singleRelationshipDef = singleRelationshipDefBuilder.Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, singleRelationshipDef.RelationshipName);
            Assert.IsTrue(singleRelationshipDef.KeepReferenceToRelatedObject);
            Assert.AreEqual(DeleteParentAction.DoNothing, singleRelationshipDef.DeleteParentAction);
            // This will only be available once the classdef has been built i.e. using new ClassDef
            //Assert.AreEqual("Car", singleRelationshipDef.OwningClassName);
            Assert.AreEqual("TestProject.BO", singleRelationshipDef.RelatedObjectAssemblyName);
            Assert.AreEqual("SteeringWheel", singleRelationshipDef.RelatedObjectClassName);
            Assert.AreEqual(0, singleRelationshipDef.RelKeyDef.Count);
            Assert.AreEqual(InsertParentAction.InsertRelationship, singleRelationshipDef.InsertParentAction);
            Assert.AreEqual(RelationshipType.Association, singleRelationshipDef.RelationshipType);
        }

        [Test]
        public void Test_Build_ShouldUseDefaults()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "R" + GetRandomString(); 
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            // Note the Factory method creates the relationship between car and steeringwheel, the relationship name can be random
            var singleRelationshipDef = GetNewSingleRelationshipDefBuilder(relationshipName)
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, singleRelationshipDef.RelationshipName);
            Assert.IsTrue(singleRelationshipDef.KeepReferenceToRelatedObject);
            Assert.AreEqual(DeleteParentAction.DoNothing, singleRelationshipDef.DeleteParentAction);
            Assert.AreEqual("TestProject.BO", singleRelationshipDef.RelatedObjectAssemblyName);
            Assert.AreEqual("SteeringWheel", singleRelationshipDef.RelatedObjectClassName);
            Assert.AreEqual(0, singleRelationshipDef.RelKeyDef.Count);
            Assert.AreEqual(InsertParentAction.InsertRelationship, singleRelationshipDef.InsertParentAction);
            Assert.AreEqual(RelationshipType.Association, singleRelationshipDef.RelationshipType);
        }

        [Test]
        public void Test_NoRelationshipName_ShouldRaiseError()
        {
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

        [Test]
        public void Test_Build_WithLambdaProp_ShouldSetRelationshipName()
        {
            //---------------Set up test pack-------------------
            var newRelationshipsBuilder = new RelationshipsBuilderStub<Car>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var newSingleRelationshipDefBuilder = new SingleRelationshipDefBuilder<Car, SteeringWheel>(newRelationshipsBuilder,c=> c.SteeringWheel);
            var singleRelationshipDef = newSingleRelationshipDefBuilder.Build();

            //---------------Test Result -----------------------
            Assert.AreEqual("SteeringWheel", singleRelationshipDef.RelationshipName);
        }


        [Test]
        public void Test_Build_WithLambdaProp_ShouldSetRelationshipProperties()
        {
            //---------------Set up test pack-------------------
            var newSingleRelationshipDefBuilder = new RelationshipsBuilderStub<Car>().WithSingleRelationship(c => c.SteeringWheel).WithRelProp("VehicleID", "CarID");
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var singleRelationshipDef = newSingleRelationshipDefBuilder.Build();


            //---------------Test Result -----------------------
            Assert.AreEqual("SteeringWheel", singleRelationshipDef.RelationshipName);
            var relPropDef = singleRelationshipDef.RelKeyDef["VehicleID"];
            Assert.IsNotNull(relPropDef);
            Assert.AreEqual("VehicleID", relPropDef.OwnerPropertyName);
            Assert.AreEqual("CarID", relPropDef.RelatedClassPropName);
        }

        private static SingleRelationshipDefBuilder<Car, SteeringWheel> GetNewSingleRelationshipDefBuilder(string relationshipName)
        {
            var newRelationshipsBuilder = new RelationshipsBuilderStub<Car>();
            return new SingleRelationshipDefBuilder<Car, SteeringWheel>(newRelationshipsBuilder, relationshipName);
        }

        private static SingleRelationshipDefBuilder<Car, SteeringWheel> GetNewSingleRelationshipDefBuilder(Expression<Func<Car, SteeringWheel>> relationshipExpression)
        {
            var newRelationshipsBuilder = new RelationshipsBuilderStub<Car>();
            return new SingleRelationshipDefBuilder<Car, SteeringWheel>(newRelationshipsBuilder, relationshipExpression);
        }

        private static string GetRandomString()
        {
            return RandomValueGenerator.GetRandomString();
        }
    }
}