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
using Habanero.BO.Loaders;
using Habanero.Smooth.ReflectionWrappers;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Habanero.Smooth.Test
{
    [TestFixture]
    public class TestAcceptance
    {
        [SetUp]
        public void Setup()
        {
            AllClassesAutoMapper.ClassDefCol = null;
        }

        [Test]
        public void Test_Map_WhenHaveTwoClassesShouldMapBoth()
        {
            //---------------Set up test pack-------------------
            Type type1 = typeof(FakeBOWithNoRelationship);
            Type type2 = typeof(FakeBoNoProps);
            FakeTypeSource source = new FakeTypeSource(
                    new[] { type1, type2 });

            AllClassesAutoMapper allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            ClassDefCol classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            Assert.IsNotNull(classDefCol);
            Assert.AreEqual(2, classDefCol.Count);
            classDefCol.ShouldContain(def => def.ClassName == type1.Name);
            classDefCol.ShouldContain(def => def.ClassName == type2.Name);
        }
        [Test]
        public void Test_Map_WhenInheritanceWithNonStandardIDProp_ShouldMapWithCorrectNonStandardIDProp()
        {
            //---------------Set up test pack-------------------
            var subClassWithNonStdID = typeof (SubClassWithNonStandardID);
            var superClassWithNonStdID = typeof(SuperClassWithNonStandardID);
            var source = new FakeTypeSource(new[] { subClassWithNonStdID, superClassWithNonStdID });
            var allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreSame(superClassWithNonStdID, subClassWithNonStdID.BaseType);
            //---------------Execute Test ----------------------
            var classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, classDefCol.Count);
            classDefCol.ShouldContain(def => def.ClassName == subClassWithNonStdID.Name, "Should contain SubClass");
            classDefCol.ShouldContain(def => def.ClassName == superClassWithNonStdID.Name, "Should Contain SuperClass");

            var subClassDef = classDefCol.FindByClassName(subClassWithNonStdID.Name);
            Assert.IsNull(subClassDef.PrimaryKeyDef);
            var superClassDef = classDefCol.FindByClassName(superClassWithNonStdID.Name);
            Assert.IsNotNull(superClassDef.PrimaryKeyDef);
            Assert.AreEqual(1, superClassDef.PrimaryKeyDef.Count, "The PrimaryKey (ObjectID) Should not be composite");
            var propDef = superClassDef.PrimaryKeyDef[0];
            Assert.AreEqual("NonStandardID", propDef.PropertyName);
        }

        [Test]
        public void Test_Map_WhenSingleRelationshipToSubClass_WhenNonStandardIDProp_ShouldSetRelatedPropToNonStandardID_FixBug1355()
        {
            //---------------Set up test pack-------------------
            var subClassWithNonStdID = typeof(SubClassWithNonStandardID);
            var superClassWithNonStdID = typeof(SuperClassWithNonStandardID);
            var relatedToSubClassWithNonStandardID = typeof(RelatedToSubClassWithNonStandardID);

            var source = new FakeTypeSource(new[] { subClassWithNonStdID, superClassWithNonStdID, relatedToSubClassWithNonStandardID });
            var allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            var classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            var relatedToSubClassWithNonStandardIDClassDef = classDefCol.FindByClassName(relatedToSubClassWithNonStandardID.Name);
            Assert.AreEqual(1, relatedToSubClassWithNonStandardIDClassDef.RelationshipDefCol.Count);
            var subClassWithNonStandardIDRelDef = relatedToSubClassWithNonStandardIDClassDef.RelationshipDefCol["SubClassWithNonStandardIDSingleRel"];
            Assert.AreEqual(1, subClassWithNonStandardIDRelDef.RelKeyDef.Count, "Should have a non composite Key");
            var relPropDef = subClassWithNonStandardIDRelDef.RelKeyDef.FirstOrDefault();
            Assert.AreEqual("SubClassWithNonStandardIDSingleRelID", relPropDef.OwnerPropertyName);
            Assert.AreEqual("NonStandardID", relPropDef.RelatedClassPropName);
        }

        [Test]
        public void Test_Map_WhenSingleRelationshipToSubClass_WhenIDPropDeclaredInClassDefXml_ShouldSetRelatedPropToNonStandardID_FixBug1355()
        {
            //---------------Set up test pack-------------------


            var superClassDef = SuperClassWithPKFromClassDef.LoadClassDef();//Loaded from XML
            var defCol = new ClassDefCol {superClassDef};


            var subClassWithPKFromClassDef = typeof(SubClassWithPKFromClassDef);
            var relatedToSubClassWithPKFromClassDefType = typeof(RelatedToSubClassWithPKFromClassDef);

            var source = new FakeTypeSource(new[] {subClassWithPKFromClassDef, relatedToSubClassWithPKFromClassDefType });
            AllClassesAutoMapper.ClassDefCol = defCol;
            var allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            var relatedToSubClassWithPKFromClassDef = classDefCol.FindByClassName(relatedToSubClassWithPKFromClassDefType.Name);
            Assert.AreEqual(1, relatedToSubClassWithPKFromClassDef.RelationshipDefCol.Count);
            var subClassWithPKFromClassDefRelDef = relatedToSubClassWithPKFromClassDef.RelationshipDefCol["SubClassWithPKFromClassDefSingleRel"];
            Assert.AreEqual(1, subClassWithPKFromClassDefRelDef.RelKeyDef.Count, "Should have a non composite Key");
            var relPropDef = subClassWithPKFromClassDefRelDef.RelKeyDef.FirstOrDefault();
            Assert.AreEqual("SubClassWithPKFromClassDefSingleRelID", relPropDef.OwnerPropertyName);
            Assert.AreEqual("MYPKID", relPropDef.RelatedClassPropName);
    }
        [Test]
        public void Test_Map_WhenSingleRelationshipToSubClass_WhenIDPropDeclaredInClassDefXml_ShouldNotCreatePropDefOnSubClass()
        {
            //---------------Set up test pack-------------------

            var superClassDef = SuperClassWithPKFromClassDef.LoadClassDef();//Loaded from XML
            var defCol = new ClassDefCol {superClassDef};

            var subClassWithPKFromClassDef = typeof(SubClassWithPKFromClassDef);
            var relatedToSubClassWithPKFromClassDefType = typeof(RelatedToSubClassWithPKFromClassDef);

            var source = new FakeTypeSource(new[] {subClassWithPKFromClassDef, relatedToSubClassWithPKFromClassDefType });
            AllClassesAutoMapper.ClassDefCol = defCol;
            var allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            var relatedToSubClassWithPKFromClassDef = classDefCol.FindByClassName(subClassWithPKFromClassDef.Name);
            relatedToSubClassWithPKFromClassDef.PropDefcol.ShouldNotContain(def => def.PropertyName == "MYPKID");
        }

    }
    // ReSharper restore InconsistentNaming
}