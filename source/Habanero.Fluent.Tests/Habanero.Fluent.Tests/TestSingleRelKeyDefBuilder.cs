using System.Collections.Generic;
using Habanero.Smooth.Test;
using NUnit.Framework;
using TestProject.BO;

namespace Habanero.Fluent.Tests
{
    [TestFixture]
    public class TestSingleRelKeyDefBuilder
    {
        [Test]
        public void Test_Construct()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var singleRelKeyDefBuilder = GetSingleRelKeyDefBuilder();

            //---------------Test Result -----------------------
            Assert.IsNotNull(singleRelKeyDefBuilder);
            Assert.IsInstanceOf<NewSingleRelKeyDefBuilder<Car, SteeringWheel>>(singleRelKeyDefBuilder);
        }

        [Test]
        public void Test_WithRelProp_WhenCalled_ShouldReturnSingleRelationshipDefBuilder()
        {
            //---------------Set up test pack-------------------
            var singleRelKeyDefBuilder = GetSingleRelKeyDefBuilder();
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var singleRelationshipDefBuilder = singleRelKeyDefBuilder.WithRelProp("VehicleID", "CarID");
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<NewSingleRelationshipDefBuilder<Car,SteeringWheel>>(singleRelationshipDefBuilder);
        }

        [Test]
        public void Test_WithRelPropAndEndSingleRelationship_ShouldReturnNewRelationshipBuilder()
        {
            //---------------Set up test pack-------------------
            var singleRelKeyDefBuilder = GetSingleRelKeyDefBuilder();
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            var relationshipsBuilder = singleRelKeyDefBuilder.WithRelProp("VehicleID", "CarID").EndSingleRelationship();
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<NewRelationshipsBuilder<Car>>(relationshipsBuilder);
        }



        private static NewSingleRelKeyDefBuilder<Car, SteeringWheel> GetSingleRelKeyDefBuilder()
        {
            return new NewClassDefBuilder2<Car>(new NewClassDefBuilder<Car>(), new List<string> { RandomValueGenerator.GetRandomString() }).WithRelationships().WithNewSingleRelationship(c => c.SteeringWheel);
        }

    }
}