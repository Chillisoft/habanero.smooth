// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2010 Chillisoft Solutions
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
using System;
using System.Collections.Generic;
using System.Linq;
using Habanero.Smooth.ReflectionWrappers;
using FakeBosInSeperateAssembly;
using Habanero.Base;
using NUnit.Framework;

namespace Habanero.Smooth.Test
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