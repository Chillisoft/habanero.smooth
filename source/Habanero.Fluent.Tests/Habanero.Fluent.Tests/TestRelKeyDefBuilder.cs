using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Habanero.BO;
using NUnit.Framework;
using TestProject.BO;

namespace Habanero.Fluent.Tests
{
    [TestFixture]
    public class TestRelKeyDefBuilder
    {
        [Test]
        public void Test_Construct()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var singleRelationshipDefBuilder = new OldClassDefBuilder<Car>().WithSingleRelationship(c => c.SteeringWheel);
            var relKeyDefBuilder = new OldRelKeyDefBuilder<Car, SteeringWheel>(singleRelationshipDefBuilder);

            //---------------Test Result -----------------------
            Assert.IsNotNull(relKeyDefBuilder);
        }

        [Test]
        public void Test_WithRelProp_WhenCalled_ShouldReturnSingleRelationshipDefBuilder()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDefBuilder = new OldClassDefBuilder<Car>().WithSingleRelationship(c => c.SteeringWheel);
            var relKeyDefBuilder = new OldRelKeyDefBuilder<Car, SteeringWheel>(singleRelationshipDefBuilder);
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var relationshipDefBuilder = relKeyDefBuilder.WithRelProp("VehicleID", "CarID");
            //---------------Test Result -----------------------
            Assert.AreSame(singleRelationshipDefBuilder, relationshipDefBuilder);
        }



    }

}
