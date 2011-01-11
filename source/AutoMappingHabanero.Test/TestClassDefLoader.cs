using System;
using FakeBosInSeperateAssembly;
using Habanero.BO.Loaders;
using NUnit.Framework;
// ReSharper disable InconsistentNaming
namespace AutoMappingHabanero.Test
{
    [TestFixture]
    public class TestClassDefLoader
    {
        [Test]
        public void Test_Construct_WhenSourceNull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            const ITypeSource source = null;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                new ReflectionClassDefLoader(source);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("source", ex.ParamName);
            }
        }

        [Test]
        public void TestAccept_LoadClassDefs_ShouldLoadAllClassDefs()
        {
            //---------------Set up test pack-------------------
            ITypeSource source = new AssemblyTypeSource(typeof(FakeExtBoShouldBeLoaded));
            IClassDefsLoader loader = new ReflectionClassDefLoader(source);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var defs = loader.LoadClassDefs();
            //---------------Test Result -----------------------
            AllClassesAutoMapper allClassesAutoMapper = new AllClassesAutoMapper(source);
            var expectedDefs = allClassesAutoMapper.Map();
            Assert.IsNotNull(defs);
            defs.ShouldHaveCount(expectedDefs.Count);
            defs.ShouldContain(expectedDefs, "Should contain all elements loaded via the mapper");
        }
    }
    // ReSharper restore InconsistentNaming

}