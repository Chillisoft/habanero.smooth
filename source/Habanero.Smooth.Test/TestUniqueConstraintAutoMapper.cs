using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Util;
using NUnit.Framework;

namespace Habanero.Smooth.Test
{
    [TestFixture]
    public class TestUniqueConstraintAutoMapper
    {
        [Test]
        public void Test_Construct_WithNullClassDef_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                new UniqueConstraintAutoMapper(null);
                Assert.Fail("expected ArgumentNullException");
            }
            //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("classDef", ex.ParamName);
            }
        }

        [Test]
        public void Test_Map_WhenNullCDef_ShouldReturnEmptyCollection()
        {
            //---------------Set up test pack-------------------
            IClassDef cDef = null;
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            IList<IKeyDef> keyDefs = cDef.MapUniqueConstraints();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, keyDefs.Count);
        }

        [Test]
        public void Test_Map_WhenNoUniqueConstraints_ShouldReturnEmptyCollection()
        {
            //---------------Set up test pack-------------------
            IClassDef cDef = GetClassDefWithPropsMapped(typeof(FakeBOWProps));
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            IList<IKeyDef> keyDefs = cDef.MapUniqueConstraints();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, keyDefs.Count);
        }

        [Test]
        public void Test_Map_WhenOneUniqueConstraintWithOneProp_ShouldReturnOneKeyDefWithOneProp()
        {
            //---------------Set up test pack-------------------
            IClassDef cDef = GetClassDefWithPropsMapped(typeof(FakeBOWithUniqueConstraint_OneProp));
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            IList<IKeyDef> keyDefs = cDef.MapUniqueConstraints();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, keyDefs.Count);
            Assert.AreEqual(1, keyDefs[0].Count);
            Assert.AreSame(cDef.GetPropDef("UCProp"), keyDefs[0][0]);
        }

        [Test]
        public void Test_Map_WhenTwoUniqueConstraintWithOnePropEach_ShouldReturnTwoKeyDefWithOnePropEach()
        {
            //---------------Set up test pack-------------------
            IClassDef cDef = GetClassDefWithPropsMapped(typeof(FakeBOWithTwoUniqueConstraints_OnePropEach));
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            IList<IKeyDef> keyDefs = cDef.MapUniqueConstraints();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, keyDefs.Count);
            Assert.AreEqual(1, keyDefs[0].Count);
            Assert.AreEqual(1, keyDefs[1].Count);
            Assert.AreSame(cDef.GetPropDef("UCProp1"), keyDefs[0][0]);
            Assert.AreSame(cDef.GetPropDef("UCProp2"), keyDefs[1][0]);
        }
              

        [Test]
        public void Test_Map_WhenOneUniqueConstraintWithTwoProps_ShouldReturnOneKeyDefWithTwoProps()
        {
            //---------------Set up test pack-------------------
            IClassDef cDef = GetClassDefWithPropsMapped(typeof(FakeBOWithUniqueConstraint_TwoProps));
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            IList<IKeyDef> keyDefs = cDef.MapUniqueConstraints();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, keyDefs.Count);
            Assert.AreEqual(2, keyDefs[0].Count);
            Assert.AreSame(cDef.GetPropDef("UCProp1"), keyDefs[0][0]);
            Assert.AreSame(cDef.GetPropDef("UCProp2"), keyDefs[0][1]);
        }

        [Test]
        public void Test_Map_WhenTwoUniqueConstraintWithTwoPropsEach_ShouldReturnTwoKeyDefWithTwoProps()
        {
            //---------------Set up test pack-------------------
            IClassDef cDef = GetClassDefWithPropsMapped(typeof(FakeBOWithTwoUniqueConstraints_TwoPropEach));
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            IList<IKeyDef> keyDefs = cDef.MapUniqueConstraints();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, keyDefs.Count);
            Assert.AreEqual(2, keyDefs[0].Count);
            Assert.AreEqual(2, keyDefs[1].Count);
            Assert.AreSame(cDef.GetPropDef("UC1Prop1"), keyDefs[0][0]);
            Assert.AreSame(cDef.GetPropDef("UC1Prop2"), keyDefs[0][1]);
            Assert.AreSame(cDef.GetPropDef("UC2Prop1"), keyDefs[1][0]);
            Assert.AreSame(cDef.GetPropDef("UC2Prop2"), keyDefs[1][1]);
        }


        private static IClassDef GetClassDefWithPropsMapped(Type type)
        {
            ClassAutoMapper classAutoMapper = new ClassAutoMapper(type.ToTypeWrapper());
            IClassDef classDef = CreateClassDef(classAutoMapper);
            //            IClassDef classDef = classAutoMapper.CreateClassDef(type);

            //            classDef.MapProperties();
            ReflectionUtilities.ExecutePrivateMethod(classAutoMapper, "MapProperties");
            return classDef;
        }
        private static IClassDef CreateClassDef(ClassAutoMapper classAutoMapper)
        {
            return ReflectionUtilities.ExecutePrivateMethod(classAutoMapper, "CreateClassDef") as IClassDef;
        }
    }

    
}
