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
using Habanero.Base;
using Habanero.Smooth.ReflectionWrappers;
using NUnit.Framework;

namespace Habanero.Smooth.Test
{
    [TestFixture]
    public class TestAppDomainTypeSource
    {
        [Test]
        public void Test_GetTypes_ShouldReturnAllTypesImplementingIBoInterface()
        {
            //---------------Set up test pack-------------------
            AppDomainTypeSource typeSource = new AppDomainTypeSource();
            var expectedBos = TypesImplementingInterface<IBusinessObject>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            Assert.IsNotNull(bos);
            bos.ItemsShouldBeEqual(expectedBos);
        }

        [Test]
        public void Test_GetTypes_WhenHasWhereClause_ShouldReturnItemsMatchingWhereClause()
        {
            //---------------Set up test pack-------------------
            TypeWrapper boToBeExcluded = typeof(FakeBOTwoPropsAttributePK).ToTypeWrapper();
            Func<TypeWrapper, bool> whereClause = type => type.Name != boToBeExcluded.Name;
            AppDomainTypeSource typeSource = new AppDomainTypeSource(whereClause);
            var expectedBos = TypesImplementingInterface<IBusinessObject>().Where(whereClause);
            //---------------Assert Precondition----------------
            expectedBos.ShouldNotContain(boToBeExcluded);
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            Assert.IsNotNull(bos);
            bos.ItemsShouldBeEqual(expectedBos);
            bos.ShouldNotContain(wrapper => wrapper.Name == boToBeExcluded.Name);
        }

        [Test]
        public void Test_GetTypes_ShouldNotReturnInterface()
        {
            //---------------Set up test pack-------------------
            AppDomainTypeSource typeSource = new AppDomainTypeSource();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            Assert.IsNotNull(bos);
            Assert.GreaterOrEqual(bos.Count(), 5);
            bos.ShouldContain(typeof(FakeBoShouldBeLoaded).ToTypeWrapper());
            bos.ShouldNotContain(typeof(IFakeBoInterfaceShouldNotBeLoaded).ToTypeWrapper(), "Interface should not be loaded");
        }

        [Test]
        public void Test_GetTypes_ShouldNotReturnAbstractClasses()
        {
            //---------------Set up test pack-------------------
            AppDomainTypeSource typeSource = new AppDomainTypeSource();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            Assert.IsNotNull(bos);
            Assert.GreaterOrEqual(bos.Count(), 5);
            bos.ShouldContain(typeof(FakeBoShouldBeLoaded).ToTypeWrapper());
            bos.ShouldNotContain(typeof(FakeAbstractBoShouldNotBeLoaded).ToTypeWrapper(),
                                 "Abstract classes should not be loaded");
        }
        [Test]
        public void Test_GetTypes_ShouldNotReturnGenericClasses()
        {
            //---------------Set up test pack-------------------
            AppDomainTypeSource typeSource = new AppDomainTypeSource();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            Assert.IsNotNull(bos);
            Assert.GreaterOrEqual(bos.Count(), 5);
            bos.ShouldContain(typeof (FakeBoShouldBeLoaded).ToTypeWrapper());
            bos.ShouldNotContain(typeof(FakeGenericBOShouldNotBeLoaded<string>).ToTypeWrapper(),
                                 "Generic classes should not be loaded");
        }

        private static IEnumerable<TypeWrapper> TypesImplementingInterface<T>()
        {
            return TypesImplementingInterface(typeof (T));
        }

        private static IEnumerable<TypeWrapper> TypesImplementingInterface(Type desiredType)
        {
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Select(type1 => type1.ToTypeWrapper())
                .Where(type => desiredType.IsAssignableFrom(type.GetUnderlyingType()) && type.IsRealClass);
        }
    }


}