using NUnit.Framework;
using TestProject.BO;

namespace Habanero.Fluent.Tests
{
    [TestFixture]
    public class TestNewSingleRelKeyDefBuilder
    {
        [Test]
        public void Test_Construct()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            NewSingleRelKeyDefBuilder<Car, SteeringWheel> singleRelKeyDefBuilder = new NewClassDefBuilder<Car>()
                .WithPrimaryKey(car => car.VehicleID)
                .WithRelationships()
                .WithNewSingleRelationship(c => c.SteeringWheel);
                
            //var relKeyDefBuilder = new NewSingleRelKeyDefBuilder<Car, SteeringWheel>(singleRelationshipDefBuilder);

            //---------------Test Result -----------------------
            Assert.IsNotNull(singleRelKeyDefBuilder);
        }

        [Test]
        public void Test_WithRelProp_WhenCalled_ShouldReturnSingleRelationshipDefBuilder()
        {
            //---------------Set up test pack-------------------
            NewSingleRelKeyDefBuilder<Car, SteeringWheel> singleRelKeyDefBuilder = new NewClassDefBuilder<Car>()
                             .WithPrimaryKey(car => car.VehicleID)
                             .WithRelationships()
                             .WithNewSingleRelationship(c => c.SteeringWheel);
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var singleRelationshipDefBuilder = singleRelKeyDefBuilder.WithRelProp("VehicleID", "CarID");
            //var relationshipDefBuilder = relKeyDefBuilder.WithRelProp("VehicleID", "CarID");
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<NewSingleRelationshipDefBuilder<Car, SteeringWheel>>(singleRelationshipDefBuilder);
        }

        [Test]
        public void Test_CompositeRel_WithRelProp_WhenCalled_ShouldReturnSingleRelationshipDefBuilder()
        {
            var newSingleRelKeyBuilder = new NewClassDefBuilder<Car>()
                .WithPrimaryKey(car => car.VehicleID)
                .WithRelationships()
                .WithNewSingleRelationship(c => c.SteeringWheel)
                .WithCompositeRelationshipKey();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var singleRelationshipDefBuilder = newSingleRelKeyBuilder.WithRelProp("VehicleID", "CarID")
                                                                     .WithRelProp("xxxx","yyyy").EndCompositeRelationshipKey();

            //---------------Test Result -----------------------
            Assert.IsInstanceOf<NewSingleRelationshipDefBuilder<Car, SteeringWheel>>(singleRelationshipDefBuilder);
        }



    }
}