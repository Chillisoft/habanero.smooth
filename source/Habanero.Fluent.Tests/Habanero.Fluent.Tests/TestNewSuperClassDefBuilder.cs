using System.Collections.Generic;
using Habanero.Base;
using Habanero.BO;
using Habanero.Smooth.Test;
using NUnit.Framework;
using TestProject.BO;

namespace Habanero.Fluent.Tests
{
    [TestFixture]
    public class TestNewSuperClassDefBuilder
    {
        private string GetRandomString()
        {
            return RandomValueGenerator.GetRandomString();
        }

        [Test]
        public void Test_Construct_ShouldCreate()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var superClassDefBuilder = GetSuperClassDefBuilder<Car>();
            //---------------Test Result -----------------------
            Assert.IsNotNull(superClassDefBuilder);
        }

        [Test]
        public void Test_Build_ShouldBuildSuperClassDef()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var superClassDef = GetSuperClassDefBuilder<Car>()
                .Build();
            //---------------Test Result -----------------------
            Assert.IsNotNull(superClassDef);
        }
        [Test]
        public void Test_Build_ShouldBuildSuperClassDef_WithDefaults()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var superClassDef = GetSuperClassDefBuilder<Car>()
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(ORMapping.SingleTableInheritance, superClassDef.ORMapping);
            Assert.IsNull(superClassDef.ID);
        }

        [Test]
        public void Test_Build_ShouldBuildSuperClassDef_WithCorrectSuperType()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var superClassDef = GetSuperClassDefBuilder<FakeSubClass>().Build();
            //---------------Test Result -----------------------
            Assert.AreEqual("FakeSuperClass", superClassDef.ClassName);
            Assert.AreEqual("Habanero.Fluent.Tests", superClassDef.AssemblyName);
        }

        [Test]
        public void Test_Build_ShouldBuildSuperClassDef_WithDiscrimatorEQDefault()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var superClassDef = GetSuperClassDefBuilder<FakeSubClass>().Build();
            //---------------Test Result -----------------------
            Assert.AreEqual("FakeSuperClassType", superClassDef.Discriminator);
        }

        [Test]
        public void Test_BuildCar_ShouldBuildSuperClassDef_WithCorrectDefault()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var superClassDef = GetSuperClassDefBuilder<Car>()
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual("Vehicle", superClassDef.ClassName);
            Assert.AreEqual("TestProject.BO", superClassDef.AssemblyName);
            Assert.AreEqual("VehicleType", superClassDef.Discriminator);
        }

        [Test]
        public void Test_BuildCar_WithDiscriminator_ShouldBuildSuperClassDef_WithCorrectDiscriminator()
        {
            //---------------Set up test pack-------------------
            string discriminator = "Prop1";
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var superClassDef = GetSuperClassDefBuilder<Car>()
                .WithDiscriminator(discriminator)
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual("Vehicle", superClassDef.ClassName);
            Assert.AreEqual("TestProject.BO", superClassDef.AssemblyName);
            Assert.AreEqual(discriminator, superClassDef.Discriminator);
        }

        [Test]
        public void Test_BuildCar_WithLamdaDiscriminator_ShouldBuildSuperClassDef_WithCorrectDiscriminator()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var superClassDef = GetSuperClassDefBuilder<Car>()
                .WithDiscriminator(c => c.Model)
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual("Model", superClassDef.Discriminator);
        }


        private NewSuperClassDefBuilder<T> GetSuperClassDefBuilder<T>() where T : BusinessObject
        {
            var primaryKeyPropNames = new List<string> { GetRandomString() };
            return new NewSuperClassDefBuilder<T>(new NewClassDefBuilder2<T>(new NewClassDefBuilder<T>(), primaryKeyPropNames) );
        }
    }
}