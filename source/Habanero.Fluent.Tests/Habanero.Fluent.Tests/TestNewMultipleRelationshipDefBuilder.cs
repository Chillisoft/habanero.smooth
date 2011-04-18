using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.Fluent.Tests.TestStubs;
using Habanero.Smooth.Test;
using NUnit.Framework;
using TestProject.BO;

// ReSharper disable InconsistentNaming
namespace Habanero.Fluent.Tests
{
    [TestFixture]
    public class TestNewMultipleRelationshipDefBuilder
    {


        [Test]
        public void Test_Build_ShouldUseDefaults()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "R" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var multipleRelationshipDef = GetMultipleRelationshipDefBuilder<Car, Driver>(relationshipName)
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, multipleRelationshipDef.RelationshipName);
            Assert.IsTrue(multipleRelationshipDef.KeepReferenceToRelatedObject);
            Assert.AreEqual(DeleteParentAction.DoNothing, multipleRelationshipDef.DeleteParentAction);
            Assert.AreEqual("Driver", multipleRelationshipDef.RelatedObjectClassName);
            Assert.AreEqual("TestProject.BO", multipleRelationshipDef.RelatedObjectAssemblyName);
            Assert.AreEqual(0, multipleRelationshipDef.RelKeyDef.Count);
            Assert.AreEqual(InsertParentAction.InsertRelationship, multipleRelationshipDef.InsertParentAction);
            Assert.AreEqual(RelationshipType.Association, multipleRelationshipDef.RelationshipType);
            Assert.AreEqual("", multipleRelationshipDef.OrderCriteriaString);
            Assert.AreEqual(0, multipleRelationshipDef.TimeOut);
        }

        private static NewMultipleRelationshipDefBuilderSpy<T, TRelatedType> GetMultipleRelationshipDefBuilder<T, TRelatedType>(string relationshipName)
            where T : BusinessObject
            where TRelatedType : BusinessObject
        {
            return new NewMultipleRelationshipDefBuilderSpy<T, TRelatedType>(relationshipName);
        }

        [Test]
        public void Test_Build_NoRelationshipName_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                GetMultipleRelationshipDefBuilder<Car, Driver>("")
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
            var multipleRelationshipDef = GetMultipleRelationshipDefBuilder<Car, Driver>(relationshipName)
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
            var multipleRelationshipDef = GetMultipleRelationshipDefBuilder<Car, Driver>(relationshipName)
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
            var multipleRelationshipDef = GetMultipleRelationshipDefBuilder<Car, Driver>(relationshipName)
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
            var multipleRelationshipDef = GetMultipleRelationshipDefBuilder<Car, Driver>(relationshipName)
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
            const string relationshipName = "Drivers";
            const string propertyName = "VehicleID";
            const string relatedPropName = "CarID";
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var multipleRelationshipDef = new NewRelationshipsBuilderStub<Car>().WithNewMultipleRelationship(c => c.Drivers).WithRelProp(propertyName, relatedPropName);
            IRelationshipDef relationshipDef = multipleRelationshipDef.Build();
            

            //---------------Test Result -----------------------
            Assert.AreEqual(relationshipName, relationshipDef.RelationshipName);
            Assert.AreEqual(1, relationshipDef.RelKeyDef.Count);
            IRelPropDef relPropDef = relationshipDef.RelKeyDef[propertyName];
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

            var multipleRelationshipDef = new NewRelationshipsBuilderStub<Car>().WithNewMultipleRelationship<Car>(relationshipName)
                .WithCompositeRelationshipKey()
                    .WithRelProp(propertyName, relatedPropName)
                    .WithRelProp(propertyName2, relatedPropName2)
                .EndCompositeRelationshipKey()
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
            var relationshipDef = new NewRelationshipsBuilderStub<Car>().WithNewMultipleRelationship(c => c.Drivers)
                .WithRelProp(c=>c.VehicleID,d=>d.CarID)
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual("Drivers", relationshipDef.RelationshipName);
        }

        [Test]
        public void Test_Build_WithLambdaProp_ShouldSetRelationshipProperties()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var relationshipDef = new NewRelationshipsBuilderStub<Car>().WithNewMultipleRelationship(c => c.Drivers)
                .WithRelProp(c => c.VehicleID, d => d.CarID)
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual("Drivers", relationshipDef.RelationshipName);
            var relPropDef = relationshipDef.RelKeyDef["VehicleID"];
            Assert.IsNotNull(relPropDef);
            Assert.AreEqual("VehicleID", relPropDef.OwnerPropertyName);
            Assert.AreEqual("CarID", relPropDef.RelatedClassPropName);
        }

        [Test]
        public void Test_Build_WithLambdaProp_WithfakeBOs_ShouldSetRelationshipProperties()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
           /* var multipleRelationshipDef = new NewMultipleRelationshipDefBuilder<FakeBOWithMultipleRel, FakeBOWithSingleRel>()
                .WithRelationshipName(rel => rel.MyMultipleRel)
                .WithRelProp(car => car.FakeBOWithMultipleRelationshipID, driver => driver.FKID)
                .Build();*/

            IRelationshipDef relationshipDef = new NewRelationshipsBuilderStub<FakeBOWithMultipleRelWithProp>()
                .WithNewMultipleRelationship(c => c.MyMultipleRel)
                //.WithRelProp("FakeBOWithMultipleRelationshipID", "MySingleRelationshipID")
                .WithRelProp(x=> x.FakeBOWithMultipleRelationshipID, n=> n.MySingleRelationshipID)
                .Build();
            //---------------Test Result -----------------------
            var relPropDef = relationshipDef.RelKeyDef["FakeBOWithMultipleRelationshipID"];
            Assert.IsNotNull(relPropDef);
            Assert.AreEqual("FakeBOWithMultipleRelationshipID", relPropDef.OwnerPropertyName);
            Assert.AreEqual("MySingleRelationshipID", relPropDef.RelatedClassPropName);
        }


        private static string GetRandomString()
        {
            return RandomValueGenerator.GetRandomString();
        }
    }
}