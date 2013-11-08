// ------------------------------------------------------------------------------
// This partial class was auto-generated for use with the Habanero Architecture.
// NB Custom code should be placed in the provided stub class.
// Please do not modify this class directly!
//
// If tests are failing due to a unique condition in your application, use the
// ignore feature in the stub class SetupTestFixture() method.  Reimplement the
// test in the stub class.
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Habanero.BO.Exceptions;
using TestProjectNoDBSpecificProps.BO;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using NUnit.Framework;

namespace TestProjectNoDBSpecificProps.Test.BO
{
    // Provides the part of the test class that tests Vehicle objects
    [TestFixture]
    public partial class TestVehicle
    {
        private readonly Dictionary<string, string> _ignoreList = new Dictionary<string, string>();

        /// <summary>
        /// Checks if the developer has put this test on the ignore list.
        /// If your application has a unique condition that is causing a
        /// generated test to fail, you would lose test repairs when this
        /// class is regenerated.
        /// Simply add the test name to the ignore list in the TestFixtureSetup
        /// of the once-off-generated part of this test class, and then
        /// reimplement the test in that class.
        /// </summary>
        private void CheckIfTestShouldBeIgnored()
        {
            string methodName = new StackTrace().GetFrame(1).GetMethod().Name;
            if (_ignoreList.ContainsKey(methodName))
            {
                Assert.Ignore("The developer has chosen to ignore this test: " + methodName +
                    ", Reason: " + _ignoreList[methodName]);
            }
        }
        
        [Test]  // Ensures that the defaults have not been tampered
        public void Test_CreateVehicleWithDefaults()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Vehicle vehicle = new Vehicle();

            //---------------Test Result -----------------------
            Assert.IsNull(vehicle.MaxSpeed);
        }

        [Test]  // Ensures that a class can be successfully saved
        public void Test_SaveVehicle()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Vehicle vehicle = TestUtilsVehicle.CreateUnsavedValidVehicle();

            //---------------Assert Precondition----------------
            Assert.IsTrue(vehicle.Status.IsNew);
            BusinessObjectCollection<Vehicle> col = new BusinessObjectCollection<Vehicle>();
            col.LoadAll();
            Assert.AreEqual(0, col.Count);

            //---------------Execute Test ----------------------
            vehicle.Save();

            //---------------Test Result -----------------------
            Assert.IsFalse(vehicle.Status.IsNew);
            col.LoadAll();
            Assert.AreEqual(1, col.Count);
	    
        }
        
        [Test]  // Ensures that a saved class can be loaded
        public void Test_LoadVehicle()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Vehicle vehicle = TestUtilsVehicle.CreateSavedVehicle();

            //---------------Execute Test ----------------------
            Vehicle loadedVehicle = Broker.GetBusinessObject<Vehicle>(vehicle.ID);

            //---------------Test Result -----------------------
            Assert.AreEqual(vehicle.MaxSpeed, loadedVehicle.MaxSpeed);
        }
        
        [Test]  // Ensures that a class can be deleted
        public void Test_DeleteVehicle()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Vehicle vehicle = TestUtilsVehicle.CreateSavedVehicle();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            vehicle.MarkForDelete();
            vehicle.Save();
            //---------------Test Result -----------------------
            try
            {
                Vehicle retrievedVehicle = Broker.GetBusinessObject<Vehicle>(vehicle.ID);
                Assert.Fail("expected Err");
            }
            catch (BusObjDeleteConcurrencyControlException ex)
            {
                StringAssert.Contains("A Error has occured since the object you are trying to refresh has been deleted by another user", ex.Message);
                StringAssert.Contains("There are no records in the database for the Class: Vehicle", ex.Message);
            }
        }

        [Test]  // Ensures that updates to property values are stored and can be retrieved
        public void Test_UpdateVehicle()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Vehicle vehicle = TestUtilsVehicle.CreateSavedVehicle();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            object valueForMaxSpeed = (double)TestUtilsShared.GetRandomInt();
            vehicle.MaxSpeed = (System.Double) valueForMaxSpeed;
            vehicle.Save();

            //---------------Test Result -----------------------
            BusinessObjectManager.Instance.ClearLoadedObjects();
            GC.Collect();
            TestUtilsShared.WaitForGC();
            Vehicle retrievedVehicle =
                    Broker.GetBusinessObject<Vehicle>(vehicle.ID);
            Assert.AreEqual(valueForMaxSpeed, retrievedVehicle.MaxSpeed);
        }
        
        [Test]  // Ensures that gets and sets in the code refer to the same property
        public void Test_PropertyGetters()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Vehicle vehicle = new Vehicle();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            double valueForMaxSpeed = (double)TestUtilsShared.GetRandomInt();
            vehicle.MaxSpeed = (System.Double) valueForMaxSpeed;
            
            //---------------Test Result -----------------------
            Assert.AreEqual(valueForMaxSpeed, vehicle.MaxSpeed);
        }
        
        [Test]  // Ensures that property getters in the code point to the correct property
        public void Test_PropertyGettersUseCorrectPropertyNames()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Vehicle vehicle = new Vehicle();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            object valueForMaxSpeed = (double)TestUtilsShared.GetRandomInt();
            vehicle.MaxSpeed = (System.Double) valueForMaxSpeed;
            
            //---------------Test Result -----------------------
            Assert.AreEqual(valueForMaxSpeed, vehicle.GetPropertyValue("MaxSpeed"));
        }
        
        [Test]  // Ensures that property setters in the code point to the correct property
        public void Test_PropertySettersUseCorrectPropertyNames()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Vehicle vehicle = new Vehicle();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            object valueForVehicleType = TestUtilsShared.GetRandomString();            
            vehicle.SetPropertyValue("VehicleType", valueForVehicleType);
            object valueForMaxSpeed = (double)TestUtilsShared.GetRandomInt();            
            vehicle.SetPropertyValue("MaxSpeed", valueForMaxSpeed);
            
            //---------------Test Result -----------------------
            Assert.AreEqual(valueForVehicleType, vehicle.GetPropertyValue("VehicleType"));
            Assert.AreEqual(valueForMaxSpeed, vehicle.MaxSpeed);
        }
        
        [Test]  // Makes sure there are no non-null rules in the database that don't have corresponding compulsory rules
        public void Test_SetPropertyValue_Null()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Vehicle vehicle = TestUtilsVehicle.CreateSavedVehicle();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            vehicle.MaxSpeed = null;
            vehicle.Save();

            //---------------Test Result -----------------------
            BusinessObjectManager.Instance.ClearLoadedObjects();
            GC.Collect();
            TestUtilsShared.WaitForGC();
            Vehicle retrievedVehicle =
                    Broker.GetBusinessObject<Vehicle>(vehicle.ID);
            
            Assert.IsNull(retrievedVehicle.MaxSpeed);
        }
        
        
        [Test]  // Checks that the read-write rules have not been changed in the class defs
        public void Test_ReadWriteRules()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            ClassDef classDef = ClassDef.Get<Vehicle>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            
            //---------------Test Result -----------------------
			Assert.AreEqual("WriteNew",classDef.PropDefColIncludingInheritance["VehicleID"].ReadWriteRule.ToString());
            Assert.AreEqual("WriteNew", classDef.PropDefColIncludingInheritance["VehicleType"].ReadWriteRule.ToString());
			Assert.AreEqual("ReadWrite",classDef.PropDefColIncludingInheritance["MaxSpeed"].ReadWriteRule.ToString());
        }
        

        [Test]  // Checks that classes using primary keys that are not an ID cannot have duplicate primary key values
        public void Test_NonIDPrimaryKey_ChecksForUniqueness()
        {
            CheckIfTestShouldBeIgnored();
            // Test does not apply to this class since the primary key is an ID
        }
       
    }
}
