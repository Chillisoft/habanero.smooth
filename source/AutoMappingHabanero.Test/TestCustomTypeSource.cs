using System;
using System.Collections.Generic;
using System.Linq;
using AutoMappingHabanero.ReflectionWrappers;
using FakeBosInSeperateAssembly;
using Habanero.Base;
using NUnit.Framework;

namespace AutoMappingHabanero.Test
{
    [TestFixture]
    public class TestCustomTypeSource
    {
        [Test]
        public void Test_AddGeneric_ShouldReturnAllTypesAdded()
        {
            //---------------Set up test pack-------------------
            CustomTypeSource typeSource = new CustomTypeSource();
           
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            typeSource.Add <FakeExtBoShouldBeLoaded>();
            //---------------Test Result -----------------------
            var bos = typeSource.GetTypes();
            Assert.AreEqual(1, bos.Count());
            bos.ShouldContain(wrapper => wrapper.Name == "FakeExtBoShouldBeLoaded");
        }

        [Test]
        public void Test_Add_ShouldReturnAllTypesAdded()
        {
            //---------------Set up test pack-------------------
            CustomTypeSource typeSource = new CustomTypeSource();
            Type type1 = typeof (FakeExtBoShouldBeLoaded);
            Type type2 = typeof (FakeExtAbstractBoInterfaceShouldNotBeLoaded);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            typeSource.Add(type1);
            typeSource.Add(type2);
            //---------------Test Result -----------------------
            var bos = typeSource.GetTypes();
            Assert.AreEqual(2, bos.Count());
            bos.ShouldContain(wrapper => wrapper.Name == type1.Name);
            bos.ShouldContain(wrapper => wrapper.Name == type2.Name);
        }

        [Test]
        public void Test_AddList_ShouldReturnAllTypesAdded()
        {
            //---------------Set up test pack-------------------
            CustomTypeSource typeSource = new CustomTypeSource();
            Type type1 = typeof(FakeExtBoShouldBeLoaded);
            Type type2 = typeof(FakeExtAbstractBoInterfaceShouldNotBeLoaded);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            typeSource.Add(new []{type1, type2});
            //---------------Test Result -----------------------
            var bos = typeSource.GetTypes();
            Assert.AreEqual(2, bos.Count());
            bos.ShouldContain(wrapper => wrapper.Name == type1.Name);
            bos.ShouldContain(wrapper => wrapper.Name == type2.Name);
        }

        [Test]
        public void Test_ConstructWithList_ShouldReturnAllTypesAdded()
        {
            //---------------Set up test pack-------------------
            Type type1 = typeof(FakeExtBoShouldBeLoaded);
            Type type2 = typeof(FakeExtGenericBOShouldNotBeLoaded<string>);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            CustomTypeSource typeSource = new CustomTypeSource(new[] { type1, type2 });
            //---------------Test Result -----------------------
            var bos = typeSource.GetTypes();
            Assert.AreEqual(2, bos.Count());
            bos.ShouldContain(wrapper => wrapper.Name == type1.Name);
            bos.ShouldContain(wrapper => wrapper.Name == type2.Name);
        }
    }
}