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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Smooth.Test.ExtensionMethods;
using Habanero.Util;
using NUnit.Framework;

namespace Habanero.Smooth.Test
{
    [TestFixture]
    public class TestUniqueConstraintAutoMapper
    {
// ReSharper disable InconsistentNaming
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
        public void Test_Map_WhenOneUniqueConstraintOnRelationship_ShouldCreateConstraintOnRelationshipProperty()
        {
            //---------------Set up test pack-------------------
            ClassDef.ClassDefs.Add(typeof (FakeBOWithNoRelationship).MapClass());
            IClassDef cDef = GetClassDefWithPropsMapped(typeof(FakeBOWithUniqueConstraint_Relationship));
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            IList<IKeyDef> keyDefs = cDef.MapUniqueConstraints();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, keyDefs.Count);
            Assert.AreEqual(1, keyDefs[0].Count);
            Assert.AreSame(cDef.GetPropDef("RelatedObjectID"), keyDefs[0][0]);
        }

        [Test]
        public void Test_Map_WhenOneUniqueConstraintOnTwoRelationship_ShouldCreateConstraintOnBothRelationshipProperties()
        {
            //---------------Set up test pack-------------------
            ClassDef.ClassDefs.Add(typeof (FakeBOWithNoRelationship).MapClass());
            IClassDef cDef = GetClassDefWithPropsMapped(typeof(FakeBOWithUniqueConstraint_TwoRelationship));
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            IList<IKeyDef> keyDefs = cDef.MapUniqueConstraints();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, keyDefs.Count);
            Assert.AreEqual(2, keyDefs[0].Count);
            Assert.AreSame(cDef.GetPropDef("RelatedObject1ID"), keyDefs[0][0]);
            Assert.AreSame(cDef.GetPropDef("RelatedObject2ID"), keyDefs[0][1]);
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
            var cDef = GetClassDefWithPropsMapped(typeof(FakeBOWithTwoUniqueConstraints_TwoPropEach));
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var keyDefs = cDef.MapUniqueConstraints();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, keyDefs.Count);
            Assert.AreEqual(2, keyDefs[0].Count);
            Assert.AreEqual(2, keyDefs[1].Count);
            Assert.AreSame(cDef.GetPropDef("UC1Prop1"), keyDefs[0][0]);
            Assert.AreSame(cDef.GetPropDef("UC1Prop2"), keyDefs[0][1]);
            Assert.AreSame(cDef.GetPropDef("UC2Prop1"), keyDefs[1][0]);
            Assert.AreSame(cDef.GetPropDef("UC2Prop2"), keyDefs[1][1]);
        }



        [Test]
        public void Map_WhenUniqueConstraintOnSuperClass_ShouldNotTryMapTwice()
        {
            //---------------Set up test pack-------------------
            var subClass = typeof(FakeBOSubClassWithSuperHasUC);
            var cDef = GetClassDefWithPropsMapped(subClass);
            //---------------Assert Precondition----------------
            Assert.IsTrue(cDef.HasUniqueConstraintAttribute("FakeUCProp"));
            //---------------Execute Test ----------------------
            var keyDefs = cDef.MapUniqueConstraints();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, keyDefs.Count, "Since the UC attribute is defined on the super class it should not be remapped on the subclass");
        }

        private static IClassDef GetClassDefWithPropsMapped(Type type)
        {
            var classAutoMapper = new ClassAutoMapper(type.ToTypeWrapper());
            var classDef = CreateClassDef(classAutoMapper);
            ReflectionUtilities.ExecutePrivateMethod(classAutoMapper, "MapProperties");
            ReflectionUtilities.ExecutePrivateMethod(classAutoMapper, "MapRelDefs");
            return classDef;
        }
        private static IClassDef CreateClassDef(ClassAutoMapper classAutoMapper)
        {
            return ReflectionUtilities.ExecutePrivateMethod(classAutoMapper, "CreateClassDef") as IClassDef;
        }
    }

    
}
