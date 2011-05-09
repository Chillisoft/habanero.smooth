using NUnit.Framework;
using TestProject.BO;

// ReSharper disable InconsistentNaming
namespace Habanero.Fluent.Tests
{
    [TestFixture]
    public class TestSingleRelKeyDefBuilder
    {
        //[Test]
        //public void Test_Construct()
        //{
        //    //---------------Execute Test ----------------------
        //    var singleRelKeyDefBuilder = new ClassDefBuilder<Car>()
        //        .WithPrimaryKey(car => car.VehicleID)
        //        .WithRelationships()
        //        .WithNewSingleRelationship(c => c.SteeringWheel);
                

        //    //---------------Test Result -----------------------
        //    Assert.IsNotNull(singleRelKeyDefBuilder);
        //}

        //[Test]
        //public void Test_WithRelProp_WhenCalled_ShouldReturnSingleRelationshipDefBuilder()
        //{
        //    //---------------Set up test pack-------------------
        //    var singleRelKeyDefBuilder = new ClassDefBuilder<Car>()
        //                     .WithPrimaryKey(car => car.VehicleID)
        //                     .WithRelationships()
        //                     .WithNewSingleRelationship(c => c.SteeringWheel);
            
        //    //---------------Assert Precondition----------------

        //    //---------------Execute Test ----------------------
        //    var singleRelationshipDefBuilder = singleRelKeyDefBuilder.WithRelProp("VehicleID", "CarID");
        //    //---------------Test Result -----------------------
        //    Assert.IsInstanceOf<SingleRelationshipDefBuilder<Car, SteeringWheel>>(singleRelationshipDefBuilder);
        //}

        //[Test]
        //public void Test_CompositeRel_WithRelProp_WhenCalled_ShouldReturnSingleRelationshipDefBuilder()
        //{
        //    var newSingleRelKeyBuilder = new ClassDefBuilder<Car>()
        //        .WithPrimaryKey(car => car.VehicleID)
        //        .WithRelationships()
        //        .WithNewSingleRelationship(c => c.SteeringWheel)
        //        .WithCompositeRelationshipKey();

        //    //---------------Assert Precondition----------------

        //    //---------------Execute Test ----------------------
        //    var singleRelationshipDefBuilder = newSingleRelKeyBuilder.WithRelProp("VehicleID", "CarID")
        //                                                             .WithRelProp("xxxx","yyyy").EndCompositeRelationshipKey();

        //    //---------------Test Result -----------------------
        //    Assert.IsInstanceOf<SingleRelationshipDefBuilder<Car, SteeringWheel>>(singleRelationshipDefBuilder);
        //}

    }
}