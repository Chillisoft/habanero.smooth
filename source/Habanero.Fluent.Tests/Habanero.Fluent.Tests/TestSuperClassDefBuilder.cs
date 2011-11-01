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
using Habanero.Base;
using Habanero.BO;
using NUnit.Framework;
using TestProject.BO;

// ReSharper disable InconsistentNaming
namespace Habanero.Fluent.Tests
{
    [TestFixture]
    public class TestSuperClassDefBuilder
    {
        [Test]
        public void Test_Construct_ShouldCreate()
        {
            //---------------Execute Test ----------------------
            var superClassDefBuilder = GetSuperClassDefBuilder<Car>();
            //---------------Test Result -----------------------
            Assert.IsNotNull(superClassDefBuilder);
        }

        [Test]
        public void Test_Build_ShouldBuildSuperClassDef()
        {

            //---------------Execute Test ----------------------
            var superClassDef = GetSuperClassDefBuilder<Car>()
                .Build();
            //---------------Test Result -----------------------
            Assert.IsNotNull(superClassDef);
        }

        [Test]
        public void Test_Build_ShouldBuildSuperClassDef_WithDefaults()
        {
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
            //---------------Execute Test ----------------------
            var superClassDef = GetSuperClassDefBuilder<FakeSubClass>().Build();
            //---------------Test Result -----------------------
            Assert.AreEqual("FakeSuperClass", superClassDef.ClassName);
            Assert.AreEqual("Habanero.Fluent.Tests", superClassDef.AssemblyName);
        }

        [Test]
        public void Test_Build_ShouldBuildSuperClassDef_WithDiscrimatorEQDefault()
        {

            //---------------Execute Test ----------------------
            var superClassDef = GetSuperClassDefBuilder<FakeSubClass>().Build();
            //---------------Test Result -----------------------
            Assert.AreEqual("FakeSuperClassType", superClassDef.Discriminator);
        }

        [Test]
        public void Test_BuildCar_ShouldBuildSuperClassDef_WithCorrectDefault()
        {
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
            const string discriminator = "Prop1";
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

            //---------------Execute Test ----------------------
            var superClassDef = GetSuperClassDefBuilder<Car>()
                .WithDiscriminator(c => c.Model)
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual("Model", superClassDef.Discriminator);
        }


        private static SuperClassDefBuilder<T> GetSuperClassDefBuilder<T>() where T : BusinessObject
        {
            return new SuperClassDefBuilder<T>(new ClassDefBuilder<T>());
        }

    }
}