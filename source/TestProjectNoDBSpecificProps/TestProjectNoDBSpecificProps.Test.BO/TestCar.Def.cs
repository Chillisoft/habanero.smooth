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
    // Provides the part of the test class that tests Car objects
    [TestFixture]
    public partial class TestCar
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
        public void Test_CreateCarWithDefaults()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Car car = new Car();

            //---------------Test Result -----------------------
            Assert.IsNull(car.Make);
            Assert.IsNull(car.Model);
            Assert.IsNull(car.MaxSpeed);
        }

        [Test]  // Ensures that a class can be successfully saved
        public void Test_SaveCar()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Car car = TestUtilsCar.CreateUnsavedValidCar();

            //---------------Assert Precondition----------------
            Assert.IsTrue(car.Status.IsNew);
            BusinessObjectCollection<Car> col = new BusinessObjectCollection<Car>();
            col.LoadAll();
            Assert.AreEqual(0, col.Count);

            //---------------Execute Test ----------------------
            car.Save();

            //---------------Test Result -----------------------
            Assert.IsFalse(car.Status.IsNew);
            col.LoadAll();
            Assert.AreEqual(1, col.Count);
	    
        }
        
        [Test]  // Ensures that a saved class can be loaded
        public void Test_LoadCar()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Car car = TestUtilsCar.CreateSavedCar();

            //---------------Execute Test ----------------------
            Car loadedCar = Broker.GetBusinessObject<Car>(car.ID);

            //---------------Test Result -----------------------
            Assert.AreEqual(car.Make, loadedCar.Make);
            Assert.AreEqual(car.Model, loadedCar.Model);
            Assert.AreEqual(car.MaxSpeed, loadedCar.MaxSpeed);
        }
        
        [Test]  // Ensures that a class can be deleted
        public void Test_DeleteCar()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Car car = TestUtilsCar.CreateSavedCar();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            car.MarkForDelete();
            car.Save();
            //---------------Test Result -----------------------
            try
            {
                Broker.GetBusinessObject<Car>(car.ID);
                Assert.Fail("expected Err");
            }
            catch (BusObjDeleteConcurrencyControlException ex)
            {
                StringAssert.Contains("A Error has occured since the object you are trying to refresh has been deleted by another user", ex.Message);
                StringAssert.Contains("There are no records in the database for the Class: Car", ex.Message);
            }
        }

        [Test]  // Ensures that updates to property values are stored and can be retrieved
        public void Test_UpdateCar()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Car car = TestUtilsCar.CreateSavedCar();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            object valueForMake = TestUtilsShared.GetRandomString();
            car.Make = (System.String) valueForMake;
            object valueForModel = TestUtilsShared.GetRandomString();
            car.Model = (System.String) valueForModel;
            object valueForMaxSpeed = (double)TestUtilsShared.GetRandomInt();
            car.MaxSpeed = (System.Double) valueForMaxSpeed;
            car.Save();

            //---------------Test Result -----------------------
            BusinessObjectManager.Instance.ClearLoadedObjects();
            GC.Collect();
            TestUtilsShared.WaitForGC();
            Car retrievedCar = Broker.GetBusinessObject<Car>(car.ID);
            
            Assert.AreEqual(valueForMake, retrievedCar.Make);
            Assert.AreEqual(valueForModel, retrievedCar.Model);
            Assert.AreEqual(valueForMaxSpeed, retrievedCar.MaxSpeed);
        }
        
        [Test]  // Ensures that gets and sets in the code refer to the same property
        public void Test_PropertyGetters()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Car car = new Car();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            object valueForMake = TestUtilsShared.GetRandomString();
            car.Make = (System.String) valueForMake;
            object valueForModel = TestUtilsShared.GetRandomString();
            car.Model = (System.String) valueForModel;
            object valueForMaxSpeed = (double)TestUtilsShared.GetRandomInt();
            car.MaxSpeed = (System.Double) valueForMaxSpeed;
            
            //---------------Test Result -----------------------
            Assert.AreEqual(valueForMake, car.Make);
            Assert.AreEqual(valueForModel, car.Model);
            Assert.AreEqual(valueForMaxSpeed, car.MaxSpeed);
        }
        
        [Test]  // Ensures that property getters in the code point to the correct property
        public void Test_PropertyGettersUseCorrectPropertyNames()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Car car = new Car();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            object valueForMake = TestUtilsShared.GetRandomString();
            car.Make = (System.String) valueForMake;
            object valueForModel = TestUtilsShared.GetRandomString();
            car.Model = (System.String) valueForModel;
            
            //---------------Test Result -----------------------
            Assert.AreEqual(valueForMake, car.GetPropertyValue("Make"));
            Assert.AreEqual(valueForModel, car.GetPropertyValue("Model"));
        }
        
        [Test]  // Ensures that property setters in the code point to the correct property
        public void Test_PropertySettersUseCorrectPropertyNames()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Car car = new Car();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            object valueForMake = TestUtilsShared.GetRandomString();            
            car.SetPropertyValue("Make", valueForMake);
            object valueForModel = TestUtilsShared.GetRandomString();            
            car.SetPropertyValue("Model", valueForModel);
            
            //---------------Test Result -----------------------
            Assert.AreEqual(valueForMake, car.Make);
            Assert.AreEqual(valueForModel, car.Model);
        }
        
        [Test]  // Makes sure there are no non-null rules in the database that don't have corresponding compulsory rules
        public void Test_SetPropertyValue_Null()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Car car = TestUtilsCar.CreateSavedCar();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            car.Make = null;
            car.Model = null;
            car.Save();

            //---------------Test Result -----------------------
            BusinessObjectManager.Instance.ClearLoadedObjects();
            GC.Collect();
            TestUtilsShared.WaitForGC();
            Car retrievedCar =
                    Broker.GetBusinessObject<Car>(car.ID);
            
            Assert.IsNull(retrievedCar.Make);
            Assert.IsNull(retrievedCar.Model);
        }
        

        [Test]
        public void Test_NotSettingCompulsoryPropertiesThrowsException()
        {
            CheckIfTestShouldBeIgnored();
            // There are no compulsory properties
        }
        
        [Test]  // Checks that the read-write rules have not been changed in the class defs
        public void Test_ReadWriteRules()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            ClassDef classDef = ClassDef.Get<Car>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            
            //---------------Test Result -----------------------
			Assert.AreEqual("ReadWrite",classDef.PropDefColIncludingInheritance["Make"].ReadWriteRule.ToString());
			Assert.AreEqual("ReadWrite",classDef.PropDefColIncludingInheritance["Model"].ReadWriteRule.ToString());
			Assert.AreEqual("WriteNew",classDef.PropDefColIncludingInheritance["VehicleID"].ReadWriteRule.ToString());
            Assert.AreEqual("WriteNew", classDef.PropDefColIncludingInheritance["VehicleType"].ReadWriteRule.ToString());
			Assert.AreEqual("ReadWrite",classDef.PropDefColIncludingInheritance["MaxSpeed"].ReadWriteRule.ToString());
        }
        
        [Test]  // Checks that classes using primary keys that are not an ID cannot have duplicate primary key values
        public void Test_NonIDPrimaryKey_ChecksForUniqueness()
        {
            CheckIfTestShouldBeIgnored();
            // Test does not apply to this class since there is no primary key defined
        }
        
        [Test]  // Checks that BOs in a single relationship load correctly (no tampering with class defs)
        public void Test_LoadThroughSingleRelationship_SteeringWheel()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            BORegistry.DataAccessor = new DataAccessorInMemory();
            
            SteeringWheel wheel = TestUtilsSteeringWheel.CreateSavedSteeringWheel();
            Car car = wheel.Car;
            TestProjectNoDBSpecificProps.BO.SteeringWheel boForRelationshipSteeringWheel = car.SteeringWheel;
            
            BusinessObjectManager.Instance.ClearLoadedObjects();
            GC.Collect();
            TestUtilsShared.WaitForGC();
            //---------------Execute Test ----------------------
            TestProjectNoDBSpecificProps.BO.SteeringWheel loadedRelatedBO = Broker.GetBusinessObject<TestProjectNoDBSpecificProps.BO.SteeringWheel>(boForRelationshipSteeringWheel.ID);
            Car loadedCar = Broker.GetBusinessObject<Car>(car.ID);
            //---------------Test Result -----------------------
            Assert.AreEqual(boForRelationshipSteeringWheel, loadedCar.SteeringWheel);
            Assert.AreEqual(loadedRelatedBO, loadedCar.SteeringWheel);
            Assert.AreEqual(loadedRelatedBO, car.SteeringWheel);
        }
                
        [Test]  // Checks that a related collection loads correctly (no tampering with class defs)
        public void Test_LoadThroughMultipleRelationship_Drivers()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            BORegistry.DataAccessor = new DataAccessorInMemory();
            Car car = TestUtilsCar.CreateSavedCar();

            TestProjectNoDBSpecificProps.BO.Driver boForRelationshipDrivers = TestUtilsDriver.CreateUnsavedValidDriver();
            boForRelationshipDrivers.Car = car;
            boForRelationshipDrivers.Save();

            BusinessObjectManager.Instance.ClearLoadedObjects();
            GC.Collect();
            TestUtilsShared.WaitForGC();
            //---------------Assert Preconditions---------------
            Assert.AreEqual(1, car.Drivers.Count);
            //---------------Execute Test ----------------------
            TestProjectNoDBSpecificProps.BO.Driver loadedRelatedBO = Broker.GetBusinessObject<TestProjectNoDBSpecificProps.BO.Driver>(boForRelationshipDrivers.ID);
            Car loadedCar = Broker.GetBusinessObject<Car>(car.ID);
            //---------------Test Result -----------------------
            Assert.AreEqual(1, loadedCar.Drivers.Count);
            Assert.AreEqual(boForRelationshipDrivers, loadedCar.Drivers[0]);
            Assert.AreEqual(loadedRelatedBO, loadedCar.Drivers[0]);
            Assert.AreEqual(loadedRelatedBO, car.Drivers[0]);
        }
        
        [Test]  // Checks that deletion is prevented when a child exists
        public void Test_MultipleRelationshipDeletion_PreventDelete_Drivers()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Car car = TestUtilsCar.CreateSavedCar();

            TestProjectNoDBSpecificProps.BO.Driver boForRelationshipDrivers = TestUtilsDriver.CreateUnsavedValidDriver();
            boForRelationshipDrivers.Car = car;
            boForRelationshipDrivers.Save();

            //---------------Assert Preconditions---------------
            Assert.AreEqual(1, car.Drivers.Count);
            IRelationshipDef relationshipDef = ClassDef.Get<Car>().RelationshipDefCol["Drivers"];
            Assert.AreEqual(DeleteParentAction.Prevent, relationshipDef.DeleteParentAction);
            //---------------Execute Test ----------------------
            try
            {
                car.MarkForDelete();
                car.Save();
                Assert.Fail("Should have thrown exception due to deletion prevention");
            }
            //---------------Test Result -----------------------
            catch (BusObjDeleteException ex)
            {
                StringAssert.Contains("You cannot delete Car identified by ", ex.Message);
                StringAssert.Contains("via the Drivers relationship", ex.Message);
            }
        }
        
        [Test]
        public void Test_Inheritance_SuperClassDef_Exists()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            ISuperClassDef superClassDef = ClassDef.Get<Car>().SuperClassDef;
            //---------------Test Result -----------------------
            Assert.IsNotNull(superClassDef);
        }

        [Test]  
        public void Test_Inheritance_SuperClassDef_PropertiesCorrect()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            ISuperClassDef superClassDef = ClassDef.Get<Car>().SuperClassDef;
            //---------------Test Result -----------------------
            Assert.AreEqual("TestProjectNoDBSpecificProps.BO", superClassDef.AssemblyName);
            Assert.AreEqual("Vehicle", superClassDef.ClassName);
            Assert.AreEqual("VehicleType", superClassDef.Discriminator);
        }
		
        [Test]
        public void Test_Inheritance_SuperClassDef_RelatedClassIsCorrect()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------

            IClassDef classDef = ClassDef.Get<Car>();
            ISuperClassDef superClassDef = classDef.SuperClassDef;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            IClassDef relatedClassDef = ClassDef.ClassDefs[superClassDef.AssemblyName, superClassDef.ClassName];
            //---------------Test Result -----------------------
            Assert.IsNotNull(relatedClassDef);
            IPropDef propDef = relatedClassDef.PropDefcol[superClassDef.Discriminator];
            Assert.IsNotNull(propDef, "The discriminator must be a property on the parent class");
        }
		
        [Test]
        public void Test_Inheritance_CodeClass_InheritsCorrectly()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            BORegistry.DataAccessor = new DataAccessorInMemory();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Car car = TestUtilsCar.CreateUnsavedValidCar();
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<Vehicle>(car);
        }        
    }
}
