using System;
using System.Collections.Generic;
using System.Linq;
using AutoMappingHabanero.ReflectionWrappers;
using Habanero.Base;
using NUnit.Framework;

namespace AutoMappingHabanero.Test
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