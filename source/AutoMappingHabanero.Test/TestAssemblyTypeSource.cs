using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMappingHabanero.ReflectionWrappers;
using FakeBosInSeperateAssembly;
using Habanero.Base;
using NUnit.Framework;

namespace AutoMappingHabanero.Test
{
    [TestFixture]
    public class TestAssemblyTypeSource
    {
        [Test]
        public void Test_GetTypes_ShouldReturnAllTypesImplementingIBoInterface()
        {
            //---------------Set up test pack-------------------
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            AssemblyTypeSource typeSource = new AssemblyTypeSource(thisAssembly);
            var expectedBos = TypesImplementing<IBusinessObject>();
            //---------------Assert Precondition----------------
            Assert.GreaterOrEqual(expectedBos.Count(), 2);
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            Assert.IsNotNull(bos);
            bos.ItemsShouldBeEqual(expectedBos);
        }

        [Test]
        public void Test_GetTypes_ShouldReturnObjectThatOnlyImplementsIBo()
        {
            //---------------Set up test pack-------------------
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            AssemblyTypeSource typeSource = new AssemblyTypeSource(thisAssembly);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            bos.ShouldContain(typeof(FakeBoShouldBeLoadedOnlyImplementingInterfaceShouldBeLoaded).ToTypeWrapper(), "Object Implementing IBO should be loaded");
        }
        
        [Test]
        public void Test_GetTypes_ShouldNotReturnInterface()
        {
            //---------------Set up test pack-------------------
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            AssemblyTypeSource typeSource = new AssemblyTypeSource(thisAssembly);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            bos.ShouldContain(typeof(FakeBoShouldBeLoaded).ToTypeWrapper());
            bos.ShouldNotContain(typeof(IFakeBoInterfaceShouldNotBeLoaded).ToTypeWrapper(), "Interface should not be loaded");
        }
        
        [Test]
        public void Test_GetAllTypesImplementingIBo_ShouldNotReturnAbstractClasses()
        {
            //---------------Set up test pack-------------------
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            AssemblyTypeSource typeSource = new AssemblyTypeSource(thisAssembly);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            Assert.IsNotNull(bos);
            Assert.GreaterOrEqual(bos.Count(), 5);
            bos.ShouldContain(typeof (FakeBoShouldBeLoaded).ToTypeWrapper());
            bos.ShouldNotContain(typeof (FakeAbstractBoShouldNotBeLoaded).ToTypeWrapper(),
                                 "Abstract classes should not be loaded");
        }

        [Test]
        public void Test_GetAllTypesImplementingIBo_ShouldNotReturnGenericClasses()
        {
            //---------------Set up test pack-------------------
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            AssemblyTypeSource typeSource = new AssemblyTypeSource(thisAssembly);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            Assert.IsNotNull(bos);
            Assert.GreaterOrEqual(bos.Count(), 5);
            bos.ShouldContain(typeof (FakeBoShouldBeLoaded).ToTypeWrapper());
            bos.ShouldNotContain(typeof (FakeGenericBOShouldNotBeLoaded<string>).ToTypeWrapper(),
                                 "Generic classes should not be loaded");

        }

        #region External Interface

        [Test]
        public void Test_GetTypes_WhenExtDll_ShouldReturnNormalBo()
        {
            //---------------Set up test pack-------------------
            Assembly thisAssembly = typeof(FakeExtBoShouldBeLoaded).Assembly;
            AssemblyTypeSource typeSource = new AssemblyTypeSource(thisAssembly);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            bos.ShouldContain(typeof(FakeExtBoShouldBeLoaded).ToTypeWrapper(), "Object Implementing IBO should be loaded");
        }
        [Test]
        public void Test_GetTypes_WhenExtDll_ShouldReturnAllTypesImplementingIBoInterface()
        {
            //---------------Set up test pack-------------------
            Assembly thisAssembly = typeof(FakeExtBoShouldBeLoaded).Assembly;
            AssemblyTypeSource typeSource = new AssemblyTypeSource(thisAssembly);
            var expectedBos = TypesImplementing<IBusinessObject>(thisAssembly);
            //---------------Assert Precondition----------------
            Assert.GreaterOrEqual(expectedBos.Count(), 2);
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            Assert.IsNotNull(bos);
            bos.ItemsShouldBeEqual(expectedBos);
        }

        [Test]
        public void Test_GetTypes_WhenExtDll_ShouldReturnObjectThatOnlyImplementsIBo()
        {
            //---------------Set up test pack-------------------
            Assembly thisAssembly = typeof(FakeExtBoShouldBeLoaded).Assembly;
            AssemblyTypeSource typeSource = new AssemblyTypeSource(thisAssembly);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            bos.ShouldContain(typeof(FakeExtBoOnlyImplementingInterfaceShouldBeLoaded).ToTypeWrapper(), "Object Implementing IBO should be loaded");
        }

        [Test]
        public void Test_GetTypes_WhenExtDll_ShouldNotReturnInterface()
        {
            //---------------Set up test pack-------------------
            Assembly thisAssembly = typeof(FakeExtBoShouldBeLoaded).Assembly;
            AssemblyTypeSource typeSource = new AssemblyTypeSource(thisAssembly);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            bos.ShouldNotContain(typeof(IFakeExtBoInterfaceShouldNotBeLoaded).ToTypeWrapper(), "Interface should not be loaded");
        }

        [Test]
        public void Test_GetTypes_WhenExtDll_ShouldNotReturnAbstractClasses()
        {
            //---------------Set up test pack-------------------
            Assembly thisAssembly = typeof(FakeExtBoShouldBeLoaded).Assembly;
            AssemblyTypeSource typeSource = new AssemblyTypeSource(thisAssembly);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            Assert.IsNotNull(bos);
            Assert.GreaterOrEqual(bos.Count(), 2);
            bos.ShouldNotContain(typeof(FakeExtAbstractBoInterfaceShouldNotBeLoaded).ToTypeWrapper(),
                                 "Abstract classes should not be loaded");
        }
        [Test]
        public void Test_GetTypes_WhenExtDll_WithWhereClause_ShouldNotReturnExcludedClasses()
        {
            //---------------Set up test pack-------------------
            Assembly thisAssembly = typeof(FakeExtBoShouldBeLoaded).Assembly;
            AssemblyTypeSource typeSource = new AssemblyTypeSource(thisAssembly, type => !type.Name.Contains("FakeExtBoOnlyImplementingInterfaceShouldBeLoaded"));
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            Assert.IsNotNull(bos);
            Assert.AreEqual(1, bos.Count());
            bos.ShouldContain(typeof (FakeExtBoShouldBeLoaded).ToTypeWrapper());
            bos.ShouldNotContain(typeof(FakeExtBoOnlyImplementingInterfaceShouldBeLoaded).ToTypeWrapper(),
                                 "Abstract classes should not be loaded");
        }
        [Test]
        public void Test_GetTypes_WhenExtDll_WithNullWhereClause_ShouldRetAllClasses()
        {
            //---------------Set up test pack-------------------
            Assembly thisAssembly = typeof(FakeExtBoShouldBeLoaded).Assembly;
            AssemblyTypeSource typeSource = new AssemblyTypeSource(thisAssembly, null);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            Assert.IsNotNull(bos);
            Assert.AreEqual(2, bos.Count());
            bos.ShouldContain(typeof (FakeExtBoShouldBeLoaded).ToTypeWrapper());
            bos.ShouldContain(typeof(FakeExtBoOnlyImplementingInterfaceShouldBeLoaded).ToTypeWrapper(),
                                 "Abstract classes should be loaded");
        }

        [Test]
        public void Test_GetTypes_WhenExtDll_ShouldNotReturnGenericClasses()
        {
            //---------------Set up test pack-------------------
            AssemblyTypeSource typeSource = new AssemblyTypeSource(typeof(FakeExtBoShouldBeLoaded));
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            Assert.IsNotNull(bos);
            Assert.GreaterOrEqual(bos.Count(), 2);
            bos.ShouldNotContain(typeof(FakeExtGenericBOShouldNotBeLoaded<string>).ToTypeWrapper(),
                                 "Generic classes should not be loaded");
        }

        [Test]
        public void Test_GetTypes_WhenExtDll_WhenConstructWithType_ShouldReturnNormalBo()
        {
            //---------------Set up test pack-------------------
            Assembly thisAssembly = typeof(FakeExtBoShouldBeLoaded).Assembly;
            AssemblyTypeSource typeSource = new AssemblyTypeSource(typeof(FakeExtBoShouldBeLoaded));
            var expectedBos = TypesImplementing<IBusinessObject>(thisAssembly);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bos = typeSource.GetTypes();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, bos.Count());
            bos.ShouldContain(typeof(FakeExtBoShouldBeLoaded).ToTypeWrapper(), "Object Implementing IBO should be loaded");
            bos.ItemsShouldBeEqual(expectedBos);
        }
        #endregion


        private static IEnumerable<TypeWrapper> TypesImplementing<T>()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            return TypesImplementing(executingAssembly, typeof(T));
        }
        private static IEnumerable<TypeWrapper> TypesImplementing<T>(Assembly assembly)
        {
            return TypesImplementing(assembly, typeof(T));
        }
//
//        private static IEnumerable<TypeWrapper> TypesImplementing(Type desiredType)
//        {
//            var executingAssembly = Assembly.GetExecutingAssembly();
//            return TypesImplementing(executingAssembly, desiredType);
//        }

        private static IEnumerable<TypeWrapper> TypesImplementing(Assembly assembly, Type desiredType)
        {
            return assembly.GetTypes().Select(type1 => type1.ToTypeWrapper())
                .Where(type => desiredType.IsAssignableFrom(type.GetUnderlyingType()) && type.IsRealClass);
        }


//        var disposableTypes = TypesImplementing(typeof(IDisposable));
//
    }
}