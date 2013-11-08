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
using TestProject.BO;
//using TestProjectNoDBSpecificProps.BO
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using NUnit.Framework;

namespace TestProject.Test.BO
{
    // Provides the part of the test class that tests SteeringWheel objects
    [TestFixture]
    public partial class TestSteeringWheel
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
        public void Test_CreateSteeringWheelWithDefaults()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            SteeringWheel steeringWheel = new SteeringWheel();

            //---------------Test Result -----------------------
                        Assert.IsNotNull(steeringWheel.SteeringWheelID);
            Assert.IsInstanceOfType(steeringWheel.Props["SteeringWheelID"].PropertyType, steeringWheel.SteeringWheelID);
                        Assert.IsNull(steeringWheel.CarID);
        }

        [Test]  // Ensures that a class can be successfully saved
        public void Test_SaveSteeringWheel()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            SteeringWheel steeringWheel = TestUtilsSteeringWheel.CreateUnsavedValidSteeringWheel();

            //---------------Assert Precondition----------------
            Assert.IsTrue(steeringWheel.Status.IsNew);
            BusinessObjectCollection<SteeringWheel> col = new BusinessObjectCollection<SteeringWheel>();
            col.LoadAll();
            Assert.AreEqual(0, col.Count);

            //---------------Execute Test ----------------------
            steeringWheel.Save();

            //---------------Test Result -----------------------
            Assert.IsFalse(steeringWheel.Status.IsNew);
            col.LoadAll();
            Assert.AreEqual(1, col.Count);
	    
        }
        
        [Test]  // Ensures that a saved class can be loaded
        public void Test_LoadSteeringWheel()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            SteeringWheel steeringWheel = TestUtilsSteeringWheel.CreateSavedSteeringWheel();

            //---------------Execute Test ----------------------
            SteeringWheel loadedSteeringWheel = Broker.GetBusinessObject<SteeringWheel>(steeringWheel.ID);

            //---------------Test Result -----------------------
                        Assert.AreEqual(steeringWheel.SteeringWheelID, loadedSteeringWheel.SteeringWheelID);
                        Assert.AreEqual(steeringWheel.CarID, loadedSteeringWheel.CarID);
        }
        
        [Test]  // Ensures that a class can be deleted
        public void Test_DeleteSteeringWheel()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            SteeringWheel steeringWheel = TestUtilsSteeringWheel.CreateSavedSteeringWheel();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            steeringWheel.MarkForDelete();
            steeringWheel.Save();
            //---------------Test Result -----------------------
            try
            {
                SteeringWheel retrievedSteeringWheel = Broker.GetBusinessObject<SteeringWheel>(steeringWheel.ID);
                Assert.Fail("expected Err");
            }
            catch (BusObjDeleteConcurrencyControlException ex)
            {
                StringAssert.Contains("A Error has occured since the object you are trying to refresh has been deleted by another user", ex.Message);
                StringAssert.Contains("There are no records in the database for the Class: SteeringWheel", ex.Message);
            }
        }

        [Test]  // Ensures that updates to property values are stored and can be retrieved
        public void Test_UpdateSteeringWheel()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            SteeringWheel steeringWheel = TestUtilsSteeringWheel.CreateSavedSteeringWheel();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            object valueForCarID = Guid.NewGuid();
            steeringWheel.CarID = (System.Guid) valueForCarID;
            steeringWheel.Save();

            //---------------Test Result -----------------------
            BusinessObjectManager.Instance.ClearLoadedObjects();
            GC.Collect();
            TestUtilsShared.WaitForGC();
            SteeringWheel retrievedSteeringWheel =
                    Broker.GetBusinessObject<SteeringWheel>(steeringWheel.ID);
            
            Assert.AreEqual(valueForCarID, retrievedSteeringWheel.CarID);
        }
        
        [Test]  // Ensures that gets and sets in the code refer to the same property
        public void Test_PropertyGetters()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            SteeringWheel steeringWheel = new SteeringWheel();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            object valueForCarID = Guid.NewGuid();
            steeringWheel.CarID = (System.Guid) valueForCarID;
            
            //---------------Test Result -----------------------
            Assert.AreEqual(valueForCarID, steeringWheel.CarID);
        }
        
        [Test]  // Ensures that property getters in the code point to the correct property
        public void Test_PropertyGettersUseCorrectPropertyNames()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            SteeringWheel steeringWheel = new SteeringWheel();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            object valueForCarID = TestUtilsCar.CreateSavedCar().VehicleID;
            steeringWheel.CarID = (System.Guid) valueForCarID;
            
            //---------------Test Result -----------------------
            Assert.AreEqual(valueForCarID, steeringWheel.GetPropertyValue("CarID"));
        }
        
        [Test]  // Ensures that property setters in the code point to the correct property
        public void Test_PropertySettersUseCorrectPropertyNames()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            SteeringWheel steeringWheel = new SteeringWheel();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            object valueForCarID = TestUtilsCar.CreateSavedCar().VehicleID;            
            steeringWheel.SetPropertyValue("CarID", valueForCarID);
            
            //---------------Test Result -----------------------
            Assert.AreEqual(valueForCarID, steeringWheel.CarID);
        }
        
        [Test]  // Makes sure there are no non-null rules in the database that don't have corresponding compulsory rules
        public void Test_SetPropertyValue_Null()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            SteeringWheel steeringWheel = TestUtilsSteeringWheel.CreateSavedSteeringWheel();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            steeringWheel.CarID = null;
            steeringWheel.Save();

            //---------------Test Result -----------------------
            BusinessObjectManager.Instance.ClearLoadedObjects();
            GC.Collect();
            TestUtilsShared.WaitForGC();
            SteeringWheel retrievedSteeringWheel =
                    Broker.GetBusinessObject<SteeringWheel>(steeringWheel.ID);
            
            Assert.IsNull(retrievedSteeringWheel.CarID);
        }
        

        [Test]
        public void Test_NotSettingCompulsoryPropertiesThrowsException()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            BORegistry.DataAccessor = new DataAccessorInMemory();
            SteeringWheel steeringWheel = TestUtilsSteeringWheel.CreateUnsavedValidSteeringWheel();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            steeringWheel.SteeringWheelID = null;
            
            try
            {
                steeringWheel.Save();
                Assert.Fail("Should throw an exception when compulsory properties are null");
            }
            //---------------Test Result -----------------------
            catch (BusObjectInAnInvalidStateException ex)
            {
                StringAssert.Contains("Steering Wheel ID' is a compulsory field and has no value", ex.Message);
            }
        }
        
        [Test]  // Checks that the read-write rules have not been changed in the class defs
        public void Test_ReadWriteRules()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            ClassDef classDef = ClassDef.Get<SteeringWheel>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            
            //---------------Test Result -----------------------
			Assert.AreEqual("WriteNew",classDef.PropDefColIncludingInheritance["SteeringWheelID"].ReadWriteRule.ToString());
			Assert.AreEqual("ReadWrite",classDef.PropDefColIncludingInheritance["CarID"].ReadWriteRule.ToString());
        }
        
        
        
        
        [Test]  // Checks that classes using primary keys that are not an ID cannot have duplicate primary key values
        public void Test_NonIDPrimaryKey_ChecksForUniqueness()
        {
            CheckIfTestShouldBeIgnored();
            // Test does not apply to this class since the primary key is an ID
        }
        
        
        [Test]  // Checks that BOs in a single relationship load correctly (no tampering with class defs)
        public void Test_LoadThroughSingleRelationship_Car()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            BORegistry.DataAccessor = new DataAccessorInMemory();
            SteeringWheel steeringWheel = TestUtilsSteeringWheel.CreateSavedSteeringWheel();
            
            TestProject.BO.Car boForRelationshipCar = steeringWheel.Car;
            
            BusinessObjectManager.Instance.ClearLoadedObjects();
            GC.Collect();
            TestUtilsShared.WaitForGC();
            //---------------Execute Test ----------------------
            TestProject.BO.Car loadedRelatedBO = Broker.GetBusinessObject<TestProject.BO.Car>(boForRelationshipCar.ID);
            SteeringWheel loadedSteeringWheel = Broker.GetBusinessObject<SteeringWheel>(steeringWheel.ID);
            //---------------Test Result -----------------------
            Assert.AreEqual(boForRelationshipCar, loadedSteeringWheel.Car);
            Assert.AreEqual(loadedRelatedBO, loadedSteeringWheel.Car);
            Assert.AreEqual(loadedRelatedBO, steeringWheel.Car);
        }
                
        
        
        
        
        
        
        [Test]  // Checks that deleting this instance has no effect in the related class
        public void Test_SingleRelationshipDeletion_DoNothing_Car()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            SteeringWheel steeringWheel = TestUtilsSteeringWheel.CreateSavedSteeringWheel();

            TestProject.BO.Car boForRelationshipCar = TestUtilsCar.CreateSavedCar();
            steeringWheel.Car = boForRelationshipCar;
            steeringWheel.Save();

            //---------------Assert Preconditions---------------
            IRelationshipDef relationshipDef = ClassDef.Get<SteeringWheel>().RelationshipDefCol["Car"];
            Assert.AreEqual(DeleteParentAction.DoNothing, relationshipDef.DeleteParentAction);
            //---------------Execute Test ----------------------
            steeringWheel.MarkForDelete();
            steeringWheel.Save();
            //---------------Execute Test ----------------------
            BusinessObjectManager.Instance.ClearLoadedObjects();
            GC.Collect();
            TestUtilsShared.WaitForGC();

            try
            {
                Broker.GetBusinessObject<SteeringWheel>(steeringWheel.ID);
                Assert.Fail("BO should no longer exist and exception should be thrown");
            }
            catch (BusObjDeleteConcurrencyControlException ex)
            {
                StringAssert.Contains("There are no records in the database for the Class: SteeringWheel", ex.Message);
            }            
            
            TestProject.BO.Car relatedBO = Broker.GetBusinessObject<TestProject.BO.Car>(boForRelationshipCar.ID);
            Assert.AreEqual(relatedBO.ID.ToString(),boForRelationshipCar.ID.ToString());
            
        }
       
    }
}
