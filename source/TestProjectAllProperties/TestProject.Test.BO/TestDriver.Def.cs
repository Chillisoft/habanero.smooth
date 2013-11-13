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
    // Provides the part of the test class that tests Driver objects
    [TestFixture]
    public partial class TestDriver
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
        public void Test_CreateDriverWithDefaults()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Driver driver = new Driver();

            //---------------Test Result -----------------------
                        Assert.IsNotNull(driver.DriverID);
                        Assert.IsInstanceOf(driver.Props["DriverID"].PropertyType, driver.DriverID);
                        Assert.IsNull(driver.DriverName);
                        Assert.IsNull(driver.Age);
                        Assert.IsNull(driver.DOB);
                        Assert.IsNull(driver.LicenseRaing);
                        Assert.IsNull(driver.CarID);
        }

        [Test]  // Ensures that a class can be successfully saved
        public void Test_SaveDriver()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Driver driver = TestUtilsDriver.CreateUnsavedValidDriver();

            //---------------Assert Precondition----------------
            Assert.IsTrue(driver.Status.IsNew);
            BusinessObjectCollection<Driver> col = new BusinessObjectCollection<Driver>();
            col.LoadAll();
            Assert.AreEqual(0, col.Count);

            //---------------Execute Test ----------------------
            driver.Save();

            //---------------Test Result -----------------------
            Assert.IsFalse(driver.Status.IsNew);
            col.LoadAll();
            Assert.AreEqual(1, col.Count);
	    
        }
        
        [Test]  // Ensures that a saved class can be loaded
        public void Test_LoadDriver()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Driver driver = TestUtilsDriver.CreateSavedDriver();

            //---------------Execute Test ----------------------
            Driver loadedDriver = Broker.GetBusinessObject<Driver>(driver.ID);

            //---------------Test Result -----------------------
                        Assert.AreEqual(driver.DriverID, loadedDriver.DriverID);
                        Assert.AreEqual(driver.DriverName, loadedDriver.DriverName);
                        Assert.AreEqual(driver.Age, loadedDriver.Age);
                        Assert.AreEqual(driver.DOB, loadedDriver.DOB);
                        Assert.AreEqual(driver.LicenseRaing, loadedDriver.LicenseRaing);
                        Assert.AreEqual(driver.CarID, loadedDriver.CarID);
        }
        
        [Test]  // Ensures that a class can be deleted
        public void Test_DeleteDriver()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Driver driver = TestUtilsDriver.CreateSavedDriver();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            driver.MarkForDelete();
            driver.Save();
            //---------------Test Result -----------------------
            try
            {
                Driver retrievedDriver = Broker.GetBusinessObject<Driver>(driver.ID);
                Assert.Fail("expected Err");
            }
            catch (BusObjDeleteConcurrencyControlException ex)
            {
                StringAssert.Contains("A Error has occured since the object you are trying to refresh has been deleted by another user", ex.Message);
                StringAssert.Contains("There are no records in the database for the Class: Driver", ex.Message);
            }
        }

        [Test]  // Ensures that updates to property values are stored and can be retrieved
        public void Test_UpdateDriver()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Driver driver = TestUtilsDriver.CreateSavedDriver();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            object valueForDriverName = TestUtilsShared.GetRandomString();
            driver.DriverName = (System.String) valueForDriverName;
            object valueForAge = TestUtilsShared.GetRandomInt();
            driver.Age = (System.Int32) valueForAge;
            object valueForDOB = TestUtilsShared.GetRandomDate();
            driver.DOB = (System.DateTime) valueForDOB;
            object valueForLicenseRaing = (double)TestUtilsShared.GetRandomInt();
            driver.LicenseRaing = (System.Double) valueForLicenseRaing;
            object valueForCarID = Guid.NewGuid();
            driver.CarID = (System.Guid) valueForCarID;
            driver.Save();

            //---------------Test Result -----------------------
            BusinessObjectManager.Instance.ClearLoadedObjects();
            GC.Collect();
            TestUtilsShared.WaitForGC();
            Driver retrievedDriver =
                    Broker.GetBusinessObject<Driver>(driver.ID);
            
            Assert.AreEqual(valueForDriverName, retrievedDriver.DriverName);
            Assert.AreEqual(valueForAge, retrievedDriver.Age);
            Assert.AreEqual(valueForDOB, retrievedDriver.DOB);
            Assert.AreEqual(valueForLicenseRaing, retrievedDriver.LicenseRaing);
            Assert.AreEqual(valueForCarID, retrievedDriver.CarID);
        }
        
        [Test]  // Ensures that gets and sets in the code refer to the same property
        public void Test_PropertyGetters()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Driver driver = new Driver();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            object valueForDriverName = TestUtilsShared.GetRandomString();
            driver.DriverName = (System.String) valueForDriverName;
            object valueForAge = TestUtilsShared.GetRandomInt();
            driver.Age = (System.Int32) valueForAge;
            object valueForDOB = TestUtilsShared.GetRandomDate();
            driver.DOB = (System.DateTime) valueForDOB;
            object valueForLicenseRaing = (double)TestUtilsShared.GetRandomInt();
            driver.LicenseRaing = (System.Double) valueForLicenseRaing;
            object valueForCarID = Guid.NewGuid();
            driver.CarID = (System.Guid) valueForCarID;
            
            //---------------Test Result -----------------------
            Assert.AreEqual(valueForDriverName, driver.DriverName);
            Assert.AreEqual(valueForAge, driver.Age);
            Assert.AreEqual(valueForDOB, driver.DOB);
            Assert.AreEqual(valueForLicenseRaing, driver.LicenseRaing);
            Assert.AreEqual(valueForCarID, driver.CarID);
        }
        
        [Test]  // Ensures that property getters in the code point to the correct property
        public void Test_PropertyGettersUseCorrectPropertyNames()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Driver driver = new Driver();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            object valueForDriverName = TestUtilsShared.GetRandomString();
            driver.DriverName = (System.String) valueForDriverName;
            object valueForAge = TestUtilsShared.GetRandomInt();
            driver.Age = (System.Int32) valueForAge;
            object valueForDOB = TestUtilsShared.GetRandomDate();
            driver.DOB = (System.DateTime) valueForDOB;
            object valueForLicenseRaing = (double)TestUtilsShared.GetRandomInt();
            driver.LicenseRaing = (System.Double) valueForLicenseRaing;
            object valueForCarID = TestUtilsCar.CreateSavedCar().VehicleID;
            driver.CarID = (System.Guid) valueForCarID;
            
            //---------------Test Result -----------------------
            Assert.AreEqual(valueForDriverName, driver.GetPropertyValue("DriverName"));
            Assert.AreEqual(valueForAge, driver.GetPropertyValue("Age"));
            Assert.AreEqual(valueForDOB, driver.GetPropertyValue("DOB"));
            Assert.AreEqual(valueForLicenseRaing, driver.GetPropertyValue("LicenseRaing"));
            Assert.AreEqual(valueForCarID, driver.GetPropertyValue("CarID"));
        }
        
        [Test]  // Ensures that property setters in the code point to the correct property
        public void Test_PropertySettersUseCorrectPropertyNames()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Driver driver = new Driver();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            object valueForDriverName = TestUtilsShared.GetRandomString();            
            driver.SetPropertyValue("DriverName", valueForDriverName);
            object valueForAge = TestUtilsShared.GetRandomInt();            
            driver.SetPropertyValue("Age", valueForAge);
            object valueForDOB = TestUtilsShared.GetRandomDate();            
            driver.SetPropertyValue("DOB", valueForDOB);
            object valueForLicenseRaing = (double)TestUtilsShared.GetRandomInt();            
            driver.SetPropertyValue("LicenseRaing", valueForLicenseRaing);
            object valueForCarID = TestUtilsCar.CreateSavedCar().VehicleID;            
            driver.SetPropertyValue("CarID", valueForCarID);
            
            //---------------Test Result -----------------------
            Assert.AreEqual(valueForDriverName, driver.DriverName);
            Assert.AreEqual(valueForAge, driver.Age);
            Assert.AreEqual(valueForDOB, driver.DOB);
            Assert.AreEqual(valueForLicenseRaing, driver.LicenseRaing);
            Assert.AreEqual(valueForCarID, driver.CarID);
        }
        
        [Test]  // Makes sure there are no non-null rules in the database that don't have corresponding compulsory rules
        public void Test_SetPropertyValue_Null()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Driver driver = TestUtilsDriver.CreateSavedDriver();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            driver.DriverName = null;
            driver.Age = null;
            driver.DOB = null;
            driver.LicenseRaing = null;
            driver.CarID = null;
            driver.Save();

            //---------------Test Result -----------------------
            BusinessObjectManager.Instance.ClearLoadedObjects();
            GC.Collect();
            TestUtilsShared.WaitForGC();
            Driver retrievedDriver =
                    Broker.GetBusinessObject<Driver>(driver.ID);
            
            Assert.IsNull(retrievedDriver.DriverName);
            Assert.IsNull(retrievedDriver.Age);
            Assert.IsNull(retrievedDriver.DOB);
            Assert.IsNull(retrievedDriver.LicenseRaing);
            Assert.IsNull(retrievedDriver.CarID);
        }
        

        [Test]
        public void Test_NotSettingCompulsoryPropertiesThrowsException()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            BORegistry.DataAccessor = new DataAccessorInMemory();
            Driver driver = TestUtilsDriver.CreateUnsavedValidDriver();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            driver.DriverID = null;
            
            try
            {
                driver.Save();
                Assert.Fail("Should throw an exception when compulsory properties are null");
            }
            //---------------Test Result -----------------------
            catch (BusObjectInAnInvalidStateException ex)
            {
                StringAssert.Contains("Driver ID' is a compulsory field and has no value", ex.Message);
            }
        }
        
        [Test]  // Checks that the read-write rules have not been changed in the class defs
        public void Test_ReadWriteRules()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            ClassDef classDef = ClassDef.Get<Driver>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            
            //---------------Test Result -----------------------
			Assert.AreEqual("WriteNew",classDef.PropDefColIncludingInheritance["DriverID"].ReadWriteRule.ToString());
			Assert.AreEqual("ReadWrite",classDef.PropDefColIncludingInheritance["DriverName"].ReadWriteRule.ToString());
			Assert.AreEqual("ReadWrite",classDef.PropDefColIncludingInheritance["Age"].ReadWriteRule.ToString());
			Assert.AreEqual("ReadWrite",classDef.PropDefColIncludingInheritance["DOB"].ReadWriteRule.ToString());
			Assert.AreEqual("ReadWrite",classDef.PropDefColIncludingInheritance["LicenseRaing"].ReadWriteRule.ToString());
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
            Driver driver = TestUtilsDriver.CreateSavedDriver();
            
            TestProject.BO.Car boForRelationshipCar = driver.Car;
            
            BusinessObjectManager.Instance.ClearLoadedObjects();
            GC.Collect();
            TestUtilsShared.WaitForGC();
            //---------------Execute Test ----------------------
            TestProject.BO.Car loadedRelatedBO = Broker.GetBusinessObject<TestProject.BO.Car>(boForRelationshipCar.ID);
            Driver loadedDriver = Broker.GetBusinessObject<Driver>(driver.ID);
            //---------------Test Result -----------------------
            Assert.AreEqual(boForRelationshipCar, loadedDriver.Car);
            Assert.AreEqual(loadedRelatedBO, loadedDriver.Car);
            Assert.AreEqual(loadedRelatedBO, driver.Car);
        }
                
        
        
        
        
        
        
        [Test]  // Checks that deleting this instance has no effect in the related class
        public void Test_SingleRelationshipDeletion_DoNothing_Car()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Driver driver = TestUtilsDriver.CreateSavedDriver();

            TestProject.BO.Car boForRelationshipCar = TestUtilsCar.CreateSavedCar();
            driver.Car = boForRelationshipCar;
            driver.Save();

            //---------------Assert Preconditions---------------
            IRelationshipDef relationshipDef = ClassDef.Get<Driver>().RelationshipDefCol["Car"];
            Assert.AreEqual(DeleteParentAction.DoNothing, relationshipDef.DeleteParentAction);
            //---------------Execute Test ----------------------
            driver.MarkForDelete();
            driver.Save();
            //---------------Execute Test ----------------------
            BusinessObjectManager.Instance.ClearLoadedObjects();
            GC.Collect();
            TestUtilsShared.WaitForGC();

            try
            {
                Broker.GetBusinessObject<Driver>(driver.ID);
                Assert.Fail("BO should no longer exist and exception should be thrown");
            }
            catch (BusObjDeleteConcurrencyControlException ex)
            {
                StringAssert.Contains("There are no records in the database for the Class: Driver", ex.Message);
            }            
            
            TestProject.BO.Car relatedBO = Broker.GetBusinessObject<TestProject.BO.Car>(boForRelationshipCar.ID);
            Assert.AreEqual(relatedBO.ID.ToString(),boForRelationshipCar.ID.ToString());
            
        }
       
    }
}
