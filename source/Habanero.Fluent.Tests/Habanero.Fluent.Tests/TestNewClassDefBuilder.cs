using Habanero.Base;
using Habanero.BO.ClassDefinition;
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
            var classDef = new NewClassDefBuilder<Car>()
                .WithPrimaryKey("gogoID")
                .WithProperties()
                    .Property("sdf")
                        .WithDatabaseFieldName("gogo")
                        .WithDefaultValue("99")
                        .WithReadWriteRule(PropReadWriteRule.ReadWrite)
                    .Return()
                    .Property(c => c.Make).Return()
                    .Property(c => c.Model).Return()
                .Return()
                .WithRelationships()
                    .WithNewSingleRelationship(c => c.SteeringWheel)
            .Build();

            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }



    }
}
