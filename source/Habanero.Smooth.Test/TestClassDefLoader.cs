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
using System;
using FakeBosInSeperateAssembly;
using Habanero.BO.Loaders;
using NUnit.Framework;
// ReSharper disable InconsistentNaming
namespace Habanero.Smooth.Test
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