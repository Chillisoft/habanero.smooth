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
using System.Reflection;
using FakeBosInSeperateAssembly;
using Habanero.Base;
using Habanero.Smooth.ReflectionWrappers;
using NUnit.Framework;

namespace Habanero.Smooth.Test
{
    [TestFixture]
    public class TestMutiTypeSource
    {
        [Test]
        public void Test_GetTypes_ShouldReturnAllTypesImplementingIBoInterface()
        {
            //---------------Set up test pack-------------------
            var typeSource = new DummyTypeSourceWithMockItems();
            IEnumerable<TypeWrapper> types1 = typeSource.GetTypes();
            var typeSource2 = new DummyTypeSourceWithMockItems(1);
            IEnumerable<TypeWrapper> types2 = typeSource2.GetTypes();
            var multiTypeSource = new MultiTypeSource(new []{typeSource, typeSource2});
            //---------------Assert Precondition----------------
            types1.ShouldNotBeEmpty();
            types2.ShouldNotBeEmpty();
            //---------------Execute Test ----------------------
            var types = multiTypeSource.GetTypes();
            //---------------Test Result -----------------------
            Assert.IsNotNull(types);
            types.ShouldHaveCount(types1.Count() + types2.Count());
            types.ShouldContain(types1, "Should Contain all Items in Type One");
            types.ShouldContain(types2, "Should Contain all Items in Type Two");
        }
        [Test]
        public void Test_GetTypes_WhenAddToTypeSource_ShouldReturnAllTypesImplementingIBoInterface()
        {
            //---------------Set up test pack-------------------
            var typeSource = new DummyTypeSourceWithMockItems();
            IEnumerable<TypeWrapper> types1 = typeSource.GetTypes();
            var typeSource2 = new DummyTypeSourceWithMockItems(1);
            IEnumerable<TypeWrapper> types2 = typeSource2.GetTypes();
            var multiTypeSource = new MultiTypeSource(new []{typeSource});
            //---------------Assert Precondition----------------
            multiTypeSource.GetTypes().ShouldHaveCount(4, "The firstType Should be loaded");
            //---------------Execute Test ----------------------
            multiTypeSource.TypeSources.Add(typeSource2);
            var types = multiTypeSource.GetTypes();
            //---------------Test Result -----------------------
            Assert.IsNotNull(types);
            types.ShouldHaveCount(types1.Count() + types2.Count());
            types.ShouldContain(types1, "Should Contain all Items in Type One");
            types.ShouldContain(types2, "Should Contain all Items in Type Two");
        }

    }

    public class MultiTypeSource:ITypeSource
    {
        public IList<ITypeSource> TypeSources { get; private set; }

        public MultiTypeSource(IEnumerable<ITypeSource> typeSources)
        {
            TypeSources = typeSources.ToList();
        }

        public IEnumerable<TypeWrapper> GetTypes()
        {
            return TypeSources.SelectMany(x => x.GetTypes());
        }
    }
}