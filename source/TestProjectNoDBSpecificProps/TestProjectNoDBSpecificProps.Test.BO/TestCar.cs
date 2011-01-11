using System;
using TestProjectNoDBSpecificProps.BO;
using Habanero.BO;
using NUnit.Framework;
using TestProjectNoDBSpecificProps.Test.BO.TestUtils;

namespace TestProjectNoDBSpecificProps.Test.BO
{
    /// <summary>
    /// Provides a place to write custom tests for Car objects.
    /// This file is only written once and can be changed.  The Def file
    /// attached to this as a dependent is rewritten with each regeneration
    /// and contains the standard tests for Car.
    /// Regenerate this test project whenever there have been changes to the
    /// business objects.
    /// If tests are failing due to a unique setup in your application,
    /// you can either override the Create methods in TestUtils, or you
    /// can add the test to the ignore list below and reimplement it here.
    /// </summary>
    public partial class TestCar
    {
        [TestFixtureSetUp]
        public void SetupTestFixture()
        {
            TestBase.SetupTestFixture();

            //------------------------------------------------------------
            // Use this list to ignore generated tests that are failing
            // due to a unique condition in your application.
            // Remember to reimplement the test here.
            //------------------------------------------------------------
            //_ignoreList.Add("TestMethodName", "Reason for ignoring it");
        }

        [SetUp]
        public void Setup()
        {
            TestBase.SetupTest();
        }
        [Ignore("Not Yet Implemented")] //TODO Wajeeda Nabee 15 Feb 2010: Ignored Test - Not Yet Implemented
        [Test]
        public void Test_ToString()
        {
            //---------------Set up test pack-------------------
            BORegistry.DataAccessor = new DataAccessorInMemory();
            Car car = TestUtilsCar.CreateUnsavedValidCar();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            string toStringValue = car.ToString();
            //---------------Test Result -----------------------
            Assert.Fail("Implement ToString() for Car and refine this test");
            //Assert.AreEqual(car.SomeProperty, toStringValue);
        }
    }
}