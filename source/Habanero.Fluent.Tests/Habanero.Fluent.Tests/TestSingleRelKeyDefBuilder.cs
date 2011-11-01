#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
//  
//  This file is part of the Habanero framework.
//  
//      Habanero is a free framework: you can redistribute it and/or modify
//      it under the terms of the GNU Lesser General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      The Habanero framework is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU Lesser General Public License for more details.
//  
//      You should have received a copy of the GNU Lesser General Public License
//      along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------
#endregion
using System.Xml;
using NUnit.Framework;
using TestProject.BO;

// ReSharper disable InconsistentNaming
namespace Habanero.Fluent.Tests
{
    [TestFixture]
    public class TestSingleRelKeyDefBuilder
    {

        [Test]
        public void Test_Construct()
        {
            //---------------Execute Test ----------------------
            var singleRelKeyDefBuilder = new ClassDefBuilder<Car>()
                .WithPrimaryKey(car => car.VehicleID)
                .WithProperties()
                    .Property(car1 => car1.Make).EndProperty()
                .EndProperties()
                .WithRelationships()
                .WithSingleRelationship(c => c.SteeringWheel);


            //---------------Test Result -----------------------
            Assert.IsNotNull(singleRelKeyDefBuilder);
        }

        [Test]
        public void Test_WithRelProp_WhenCalled_ShouldReturnSingleRelationshipDefBuilder()
        {
            //---------------Set up test pack-------------------
            var singleRelKeyDefBuilder = new ClassDefBuilder<Car>()
                             .WithPrimaryKey(car => car.VehicleID)
                             .WithProperties()
                                .Property(car1 => car1.Make).EndProperty()
                             .EndProperties()
                             .WithRelationships()
                             .WithSingleRelationship(c => c.SteeringWheel);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var singleRelationshipDefBuilder = singleRelKeyDefBuilder.WithRelProp("VehicleID", "CarID");
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<SingleRelationshipDefBuilder<Car, SteeringWheel>>(singleRelationshipDefBuilder);
        }

        [Test]
        public void Test_CompositeRel_WithRelProp_WhenCalled_ShouldReturnSingleRelationshipDefBuilder()
        {
            var newSingleRelKeyBuilder = new ClassDefBuilder<Car>()
                .WithPrimaryKey(car => car.VehicleID)
                .WithProperties()
                    .Property(car1 => car1.Make).EndProperty()
                .EndProperties()
                .WithRelationships()
                .WithSingleRelationship(c => c.SteeringWheel)
                .WithCompositeRelationshipKey();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var singleRelationshipDefBuilder = newSingleRelKeyBuilder.WithRelProp("VehicleID", "CarID")
                                                                     .WithRelProp("xxxx", "yyyy").EndCompositeRelationshipKey();

            //---------------Test Result -----------------------
            Assert.IsInstanceOf<SingleRelationshipDefBuilder<Car, SteeringWheel>>(singleRelationshipDefBuilder);
        }

    }
}