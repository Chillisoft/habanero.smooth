using Habanero.Base;
using NUnit.Framework;
using TestProject.BO;

namespace Habanero.Fluent.Tests
{
    [TestFixture]
    public class TestNewClassDefBuilder
    {
        [Test]
        [Ignore("not yet implemented")]
        public void Test_WithPrimaryKey()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            new NewClassDefBuilder<Car>()
            .WithPrimaryKey(c => c.VehicleID)
            .Build();

            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }


        [Test]
        [Ignore("not yet implemented")]
        public void Test_WithCompositePrimaryKey()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            new NewClassDefBuilder<Car>()
                .WithCompositePrimaryKey()
                    .WithPrimaryKeyProperty(c => c.Make)
                    .WithPrimaryKeyProperty(c => c.Model)
                    .Return()
                .Build();
                    
            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }

        [Test]
        [Ignore("not yet implemented")]
        public void Test_WithSuperClass()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            new NewClassDefBuilder<Car>()
                .WithSuperClass()
                    .Return()
                .Build();
            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }

        [Test]
        [Ignore("not yet implemented")]
        public void Test_WithX()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            new NewClassDefBuilder<Car>()
                .WithPrimaryKey(c => c.VehicleID)
                .WithProperties()
                    .Property(c => c.Make)
                        .WithDatabaseFieldName("gogo")
                        .WithDefaultValue("99")
                        .WithReadWriteRule(PropReadWriteRule.ReadWrite)
                    .Return()
                    .Property(c => c.Model).Return()
                .Return()

                .Build();

            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }



    }
}
