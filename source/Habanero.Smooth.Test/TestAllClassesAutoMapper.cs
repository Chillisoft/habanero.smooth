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
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.ReflectionWrappers;
using NUnit.Framework;
using Rhino.Mocks;

// ReSharper disable InconsistentNaming
namespace Habanero.Smooth.Test
{
    [TestFixture]
    public class TestAllClassesAutoMapper
    {
        [SetUp]
        public void Setup()
        {
            AllClassesAutoMapper.PropNamingConvention = new DefaultPropNamingConventions();
            AllClassesAutoMapper.ClassDefCol = null;
        }

/*        [Ignore]
        [Test]
        public void Test_TODO_MustParseThroughClassDefCols_CreateReverseRelationshipsETC()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("NYI");
        }*/


        [Test]
        public void Test_Construct_WithNullsource_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            ITypeSource typeSource = null;
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                new AllClassesAutoMapper(typeSource);
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
        public void Test_SetClassDefCol_ShouldSet()
        {
            //---------------Set up test pack-------------------
            var classDefCol = new ClassDefCol();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
             AllClassesAutoMapper.ClassDefCol = classDefCol;
            //---------------Test Result -----------------------
             Assert.AreSame(classDefCol, AllClassesAutoMapper.ClassDefCol);
        }

        [Test]
        public void Test_GetClassDefCol_WhenNonSet_ShouldRetEmptyCol()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefCol = AllClassesAutoMapper.ClassDefCol;
            //---------------Test Result -----------------------
            Assert.IsNotNull(classDefCol);
            Assert.IsEmpty(classDefCol.ToArray());
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
        public void Test_MapWhenHasEnum_ShouldNotMap()
        {
            //---------------Set up test pack-------------------
            FakeTypeSource source = new FakeTypeSource(typeof(FakeEnum));
 
            AllClassesAutoMapper allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            ClassDefCol classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, classDefCol.Count);
        }

        [Test]
        public void Test_MapWhenHasInterface_ShouldNotMap()
        {
            //---------------Set up test pack-------------------
            FakeTypeSource source = new FakeTypeSource();
            source.Add<IFakeBoInterfaceShouldNotBeLoaded>();
            AllClassesAutoMapper allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            ClassDefCol classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, classDefCol.Count);
        }

        [Test]
        public void Test_MapWhenHasAbstract_ShouldNotMap()
        {
            //---------------Set up test pack-------------------
            FakeTypeSource source = new FakeTypeSource(typeof(FakeAbstractBoShouldNotBeLoaded));
            AllClassesAutoMapper allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            ClassDefCol classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, classDefCol.Count);
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
        public void Test_MapClassesForAssembly_ShouldMapAllClassesIntheAssembly()
        {
            //---------------Set up test pack-------------------
            Assembly thisAssembly = typeof(FakeExtBoShouldBeLoaded).Assembly;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            ClassDefCol classDefCol = thisAssembly.MapClasses();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, classDefCol.Count);
            classDefCol.ShouldContain(def => def.ClassName == "FakeExtBoShouldBeLoaded");
            classDefCol.ShouldContain(def => def.ClassName == "FakeExtBoOnlyImplementingInterfaceShouldBeLoaded");
        }

        [Test]
        public void Test_MapClassesForAssembly_ShouldAlwaysResetTheClassDefCol_ShouldMapAllClassesIntheAssembly()
        {
            //---------------Set up test pack-------------------
            Assembly thisAssembly = typeof(FakeExtBoShouldBeLoaded).Assembly;
            ClassDefCol classDefCol = thisAssembly.MapClasses();
            var fakeExtBO = classDefCol.FirstOrDefault(def => def.ClassName == "FakeExtBoShouldBeLoaded");
            //---------------Assert Precondition----------------
            
            //---------------Execute Test ----------------------
            fakeExtBO.TypeParameter = RandomValueGenerator.GetRandomString();
            ClassDefCol classDefCol2 = thisAssembly.MapClasses();
            //---------------Test Result -----------------------
            var fakeExtBO2 = classDefCol2.FirstOrDefault(def => def.ClassName == "FakeExtBoShouldBeLoaded");
            Assert.AreNotSame(fakeExtBO2, fakeExtBO);
            Assert.AreNotEqual(fakeExtBO2.TypeParameter, fakeExtBO.TypeParameter);
        }

        [Test]
        public void Test_MapClassesForType_ShouldMapAllClassesForTheTypesAssembly()
        {
            //---------------Set up test pack-------------------
            Type type = typeof (FakeExtBoShouldBeLoaded);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            ClassDefCol classDefCol = type.MapClasses();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, classDefCol.Count);
            classDefCol.ShouldContain(def => def.ClassName == "FakeExtBoShouldBeLoaded");
            classDefCol.ShouldContain(def => def.ClassName == "FakeExtBoOnlyImplementingInterfaceShouldBeLoaded");
        }

        [Test]
        public void Test_MapClassesForTypeWithWhereClause_ShouldMapAllClassesForTheTypesAssemblyMatchingClause()
        {
            //---------------Set up test pack-------------------
            Type type = typeof(FakeExtBoShouldBeLoaded);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            ClassDefCol classDefCol = type.MapClasses(type1 => !type1.Name.Contains("OnlyImplementing"));
            //---------------Test Result -----------------------
            classDefCol.ShouldContain(def => def.ClassName == "FakeExtBoShouldBeLoaded");
            classDefCol.ShouldNotContain(def => def.ClassName == "FakeExtBoOnlyImplementingInterfaceShouldBeLoaded");
            Assert.AreEqual(1, classDefCol.Count);
        }

        [Test]
        public void Test_Map_WhenNotHasReverseRelDefined_ShouldCreateReverseRel()
        {
            //---------------Set up test pack-------------------
            Type boWithM21 = typeof(FakeManyToOneBoRelNoFK);
            Type boWithNoDefinedRel = typeof(FakeBOWithNoRelationship);
            FakeTypeSource source = new FakeTypeSource(
                new[] { boWithM21, boWithNoDefinedRel });

            AllClassesAutoMapper allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            ClassDefCol classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            IClassDef cDefWithM21 = classDefCol[boWithM21];
            cDefWithM21.RelationshipDefCol.ShouldHaveCount(1);
            IRelationshipDef relationshipDef = cDefWithM21.RelationshipDefCol.FirstOrDefault();
            Assert.IsNotNull(relationshipDef);

            Assert.IsNotNullOrEmpty(relationshipDef.ReverseRelationshipName);

            IClassDef cDefNoDefinedRel = classDefCol[boWithNoDefinedRel];
            cDefNoDefinedRel.RelationshipDefCol.ShouldHaveCount(1);

            IRelationshipDef reverseRelDef = cDefNoDefinedRel.RelationshipDefCol[relationshipDef.ReverseRelationshipName];
            Assert.AreEqual(relationshipDef.ReverseRelationshipName, reverseRelDef.RelationshipName);
            Assert.AreEqual(relationshipDef.RelationshipName, reverseRelDef.ReverseRelationshipName);
            Assert.IsInstanceOf(typeof(MultipleRelationshipDef), reverseRelDef);
            Assert.AreEqual(RelationshipType.Association, reverseRelDef.RelationshipType);
            Assert.AreEqual(DeleteParentAction.Prevent, reverseRelDef.DeleteParentAction);
            Assert.IsFalse(reverseRelDef.OwningBOHasForeignKey);
        }

        [Test]
        public void Test_Map_WhenNotHasReverseRelDefined_ShouldCreateRelProp()
        {

            //---------------Set up test pack-------------------
            Type boWithM21 = typeof(FakeManyToOneBoRelNoFK);
            Type boWithNoDefinedRel = typeof(FakeBOWithNoRelationship);
            FakeTypeSource source = new FakeTypeSource(
                new[] { boWithM21, boWithNoDefinedRel });

            AllClassesAutoMapper allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            ClassDefCol classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            IClassDef cDefWithM21 = classDefCol[boWithM21];
            cDefWithM21.RelationshipDefCol.ShouldHaveCount(1);
            IRelationshipDef relationshipDef = cDefWithM21.RelationshipDefCol.FirstOrDefault();
            Assert.IsNotNull(relationshipDef);
            relationshipDef.RelKeyDef.ShouldHaveCount(1);
            IRelPropDef relPropDef = relationshipDef.RelKeyDef.FirstOrDefault();
            Assert.IsNotNull(relPropDef);

            IClassDef cDefNoDefinedRel = classDefCol[boWithNoDefinedRel];

            IRelationshipDef reverseRelDef = cDefNoDefinedRel.RelationshipDefCol[relationshipDef.ReverseRelationshipName];
            reverseRelDef.RelKeyDef.ShouldHaveCount(1);
            IRelPropDef revereRelPropDef = reverseRelDef.RelKeyDef.FirstOrDefault();
            Assert.IsNotNull(revereRelPropDef, "ReverseRelationship ShouldHave Been Created");
            Assert.AreEqual(relPropDef.OwnerPropertyName, revereRelPropDef.RelatedClassPropName);
            Assert.AreEqual(relPropDef.RelatedClassPropName, revereRelPropDef.OwnerPropertyName);
        }

        /// <summary>
        /// When mapping Related Classes and there is no reverse relationship defined
        /// the autoMapper should created the FKProp if Required.
        /// </summary>
        [Test]
        public void Test_Map_WhenNotHasRevRelDefined_AndIsOneToMany_ShouldCreateSingleRevRel()
        {
            //---------------Set up test pack-------------------
            Type boWith12M = typeof(FakeBOWithMultipleRel);
            Type boWithNoDefinedRel = typeof(FakeBOWithNoRelationship);
            FakeTypeSource source = new FakeTypeSource(
                new[] { boWith12M, boWithNoDefinedRel });

            AllClassesAutoMapper allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            ClassDefCol classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            IClassDef cDefWith12M = classDefCol[boWith12M];
            cDefWith12M.RelationshipDefCol.ShouldHaveCount(1);
            IRelationshipDef relationshipDef = cDefWith12M.RelationshipDefCol.First();
            Assert.IsInstanceOf(typeof(MultipleRelationshipDef), relationshipDef);

            IClassDef cDefNoDefinedRel = classDefCol[boWithNoDefinedRel];

            IRelationshipDef reverseRelDef = cDefNoDefinedRel.RelationshipDefCol[relationshipDef.ReverseRelationshipName];
            Assert.AreEqual(relationshipDef.ReverseRelationshipName, reverseRelDef.RelationshipName);
            Assert.AreEqual(relationshipDef.RelationshipName, reverseRelDef.ReverseRelationshipName);
            Assert.IsInstanceOf(typeof(SingleRelationshipDef), reverseRelDef);
            Assert.AreEqual(RelationshipType.Association, reverseRelDef.RelationshipType);
            Assert.AreEqual(DeleteParentAction.DoNothing, reverseRelDef.DeleteParentAction);
            Assert.IsTrue(reverseRelDef.OwningBOHasForeignKey);
        }

        [Test]
        public void Test_Map_WhenNotHasFKPropDefined_AndIsOneToMany_ShouldFKProp()
        {
            //---------------Set up test pack-------------------
            Type boWith12M = typeof(FakeBOWithMultipleRel);
            Type boWithNoDefinedRel = typeof(FakeBOWithNoRelationship);
            FakeTypeSource source = new FakeTypeSource(
                new[] { boWith12M, boWithNoDefinedRel });

            AllClassesAutoMapper allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            ClassDefCol classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            IClassDef cDefWith12M = classDefCol[boWith12M];
            cDefWith12M.RelationshipDefCol.ShouldHaveCount(1);
            IRelationshipDef relationshipDef = cDefWith12M.RelationshipDefCol.First();
            Assert.IsInstanceOf(typeof(MultipleRelationshipDef), relationshipDef);

            IClassDef cDefNoDefinedRel = classDefCol[boWithNoDefinedRel];

            IRelationshipDef reverseRelDef = cDefNoDefinedRel.RelationshipDefCol[relationshipDef.ReverseRelationshipName];
            IRelPropDef reverseRelPropDef = reverseRelDef.RelKeyDef.First();
            cDefNoDefinedRel.PropDefcol.ShouldContain(propDef => propDef.PropertyName == reverseRelPropDef.OwnerPropertyName);
        }

        /// <summary>
        /// When mapping Related Classes and there is no reverse relationship defined
        /// the autoMapper should created the FKProp if Required.
        /// </summary>
        [Test]
        public void Test_Map_WhenNotHasRevRelDefined_AndIsOneToOne_ShouldCreateSingleRevRel()
        {
            //---------------Set up test pack-------------------
            Type boWith12M = typeof(FakeBOWithOneToOneRel);
            Type boWithNoDefinedRel = typeof(FakeBOWithNoRelationship);
            FakeTypeSource source = new FakeTypeSource(
                new[] { boWith12M, boWithNoDefinedRel });

            AllClassesAutoMapper allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            ClassDefCol classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            IClassDef cDefWith12M = classDefCol[boWith12M];
            cDefWith12M.RelationshipDefCol.ShouldHaveCount(1);
            IRelationshipDef relationshipDef = cDefWith12M.RelationshipDefCol.First();
            Assert.IsInstanceOf(typeof(SingleRelationshipDef), relationshipDef);

            IClassDef cDefNoDefinedRel = classDefCol[boWithNoDefinedRel];

            IRelationshipDef reverseRelDef = cDefNoDefinedRel.RelationshipDefCol[relationshipDef.ReverseRelationshipName];
            Assert.AreEqual(relationshipDef.ReverseRelationshipName, reverseRelDef.RelationshipName);
            Assert.AreEqual(relationshipDef.RelationshipName, reverseRelDef.ReverseRelationshipName);
            Assert.IsInstanceOf(typeof(SingleRelationshipDef), reverseRelDef);
            Assert.AreEqual(RelationshipType.Association, reverseRelDef.RelationshipType);
            Assert.AreEqual(DeleteParentAction.DoNothing, reverseRelDef.DeleteParentAction);
            Assert.IsTrue(reverseRelDef.OwningBOHasForeignKey);
        }

        /// <summary>
        /// When mapping Related Classes and there is no reverse relationship defined
        /// the autoMapper should created the FKProp if Required.
        /// </summary>
        [Test]
        public void Test_Map_WhenTypeInSourceTwice_ShouldAddSingleClassDef()
        {
            //---------------Set up test pack-------------------
            Type typeFirstTime = typeof(FakeBOWithOneToOneRel);
            Type typeSecondTime = typeof(FakeBOWithOneToOneRel);
            FakeTypeSource source = new FakeTypeSource(
                new[] { typeFirstTime, typeSecondTime });

            AllClassesAutoMapper allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            ClassDefCol classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDefCol.Count);
            var classDef = classDefCol.First();
            Assert.AreSame(typeSecondTime, classDef.ClassType);
        }

        /// <summary>
        /// When mapping Related Classes and there is no reverse relationship defined
        /// the autoMapper should created the FKProp if Required.
        /// </summary>
        [Test]
        public void Test_CreateRevRelationship_WhenIsOneToOne_ShouldCreateSingleRevRel()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = new FakeSingleRelationshipDef();
            singleRelationshipDef.SetAsOneToOne();
            IClassDef cDefWith121 = typeof(FakeBoNoProps).MapClass();
            ClassDefCol classDefCol = new ClassDefCol { cDefWith121};
            //---------------Assert Precondition----------------
            Assert.IsTrue(singleRelationshipDef.IsOneToOne);
            //---------------Execute Test ----------------------
            IRelationshipDef reverseRelDef = AllClassesAutoMapper.CreateReverseRelationship(classDefCol, cDefWith121, singleRelationshipDef);
            //---------------Test Result -----------------------

            Assert.AreEqual(singleRelationshipDef.ReverseRelationshipName, reverseRelDef.RelationshipName);
            Assert.AreEqual(singleRelationshipDef.RelationshipName, reverseRelDef.ReverseRelationshipName);
            Assert.IsInstanceOf(typeof(SingleRelationshipDef), reverseRelDef);
            Assert.AreEqual(RelationshipType.Association, reverseRelDef.RelationshipType);
            Assert.AreEqual(DeleteParentAction.DoNothing, reverseRelDef.DeleteParentAction);
            Assert.IsTrue(reverseRelDef.OwningBOHasForeignKey);
        }

        [Test]
        public void Test_Map_WhenRelatedClassNotInClassDefCol_ShouldNotRaiseError()
        {
            //---------------Set up test pack-------------------
            Type boWith12M = typeof(FakeBOWithMultipleRel);
            FakeTypeSource source = new FakeTypeSource(
                new[] { boWith12M});

            AllClassesAutoMapper allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            ClassDefCol classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            classDefCol.ShouldHaveCount(1);
        }

        [Test]
        public void Test_Map_WhenInheritance_ShouldMapInheritanceRelationship()
        {
            //---------------Set up test pack-------------------
            var superClass = typeof(FakeBOSuperClass);
            var subClass = typeof(FakeBOSubClass);
            var source = new FakeTypeSource(new[] {superClass, subClass});
            var allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            var classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            classDefCol.ShouldHaveCount(2);
            var subClassDef = classDefCol.First(def => def.ClassType == subClass);
            var superClassDef = classDefCol.First(def => def.ClassType == superClass);
            Assert.AreEqual(superClassDef.ClassType, subClassDef.SuperClassDef.SuperClassClassDef.ClassType);
            Assert.AreSame(superClassDef, subClassDef.SuperClassDef.SuperClassClassDef);
        }

        [Test]
        public void Test_Map_GivenTypeSourceProvidesSubClassBeforeSuperClass_WhenInheritance_ShouldMapInheritanceRelationship()
        {
            //---------------Set up test pack-------------------
            var superClass = typeof(FakeBOSuperClass);
            var subClass = typeof(FakeBOSubClass);
            var source = new FakeTypeSource(new[] {subClass, superClass});
            var allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            var classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            classDefCol.ShouldHaveCount(2);
            var subClassDef = classDefCol.First(def => def.ClassType == subClass);
            var superClassDef = classDefCol.First(def => def.ClassType == superClass);
            Assert.AreEqual(superClassDef.ClassType, subClassDef.SuperClassDef.SuperClassClassDef.ClassType);
            Assert.AreSame(superClassDef, subClassDef.SuperClassDef.SuperClassClassDef);
        }

        [Test]
        public void Test_Map_When2LayersOfInheritance_ShouldMapInheritanceRelationships()
        {
            //---------------Set up test pack-------------------
            var superSuperClass = typeof(FakeBOSuperClassWithDesc);
            var superClass = typeof(FakeBOSubClassSuperHasDesc);
            var subClass = typeof(FakeBOSubSubClassSuperHasDesc);
            var source = new FakeTypeSource(new[] {superSuperClass, superClass, subClass});
            var allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            var classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            classDefCol.ShouldHaveCount(3);
            var subClassDef = classDefCol.First(def => def.ClassType == subClass);
            var superClassDef = classDefCol.First(def => def.ClassType == superClass);
            var superSuperClassDef = classDefCol.First(def => def.ClassType == superSuperClass);
            Assert.AreEqual(superClassDef.ClassType, subClassDef.SuperClassDef.SuperClassClassDef.ClassType);
            Assert.AreSame(superClassDef, subClassDef.SuperClassDef.SuperClassClassDef);
            Assert.AreEqual(superSuperClassDef.ClassType, superClassDef.SuperClassDef.SuperClassClassDef.ClassType);
            Assert.AreSame(superSuperClassDef, superClassDef.SuperClassDef.SuperClassClassDef);
        }

        [Test]
        public void Test_Map_GivenTypeSourceProvidesSubClassBeforeSuperClasses_When2LayersOfInheritance_ShouldMapInheritanceRelationships()
        {
            //---------------Set up test pack-------------------
            var superSuperClass = typeof(FakeBOSuperClassWithDesc);
            var superClass = typeof(FakeBOSubClassSuperHasDesc);
            var subClass = typeof(FakeBOSubSubClassSuperHasDesc);
            var source = new FakeTypeSource(new[] {subClass, superSuperClass, superClass});
            var allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            var classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            classDefCol.ShouldHaveCount(3);
            var subClassDef = classDefCol.First(def => def.ClassType == subClass);
            var superClassDef = classDefCol.First(def => def.ClassType == superClass);
            var superSuperClassDef = classDefCol.First(def => def.ClassType == superSuperClass);
            Assert.AreEqual(superClassDef.ClassType, subClassDef.SuperClassDef.SuperClassClassDef.ClassType);
            Assert.AreSame(superClassDef, subClassDef.SuperClassDef.SuperClassClassDef);
            Assert.AreEqual(superSuperClassDef.ClassType, superClassDef.SuperClassDef.SuperClassClassDef.ClassType);
            Assert.AreSame(superSuperClassDef, superClassDef.SuperClassDef.SuperClassClassDef);
        }

        [Test]
        public void Test_Map_WhenInheritanceWithMultipleDerivatives_ShouldSameSuperClass()
        {
            //---------------Set up test pack-------------------
            var parentSuperClass = typeof(FakeBOSuperClass);
            var parentSubClassA = typeof(FakeBOSubClass);
            var parentSubClassB = typeof(FakeBOSubClassA);
            var source = new FakeTypeSource(new[] {parentSuperClass, parentSubClassA, parentSubClassB});
            var allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            var classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            classDefCol.ShouldHaveCount(3);
            Assert.AreSame(classDefCol[parentSubClassA].SuperClassClassDef, classDefCol[parentSuperClass]);
            Assert.AreSame(classDefCol[parentSubClassB].SuperClassClassDef, classDefCol[parentSuperClass]);
            Assert.AreSame(classDefCol[parentSubClassA].SuperClassClassDef, classDefCol[parentSubClassB].SuperClassClassDef);
        }

        [Test]
        public void Test_Map_GivenTypeSourceProvidesSubClassesBeforeSuperClass_WhenInheritanceWithMultipleDerivatives_ShouldSameSuperClass()
        {
            //---------------Set up test pack-------------------
            var parentSuperClass = typeof(FakeBOSuperClass);
            var parentSubClassA = typeof(FakeBOSubClass);
            var parentSubClassB = typeof(FakeBOSubClassA);
            var source = new FakeTypeSource(new[] {parentSubClassA, parentSubClassB, parentSuperClass});
            var allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            var classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            classDefCol.ShouldHaveCount(3);
            Assert.AreSame(classDefCol[parentSubClassA].SuperClassClassDef, classDefCol[parentSuperClass]);
            Assert.AreSame(classDefCol[parentSubClassB].SuperClassClassDef, classDefCol[parentSuperClass]);
            Assert.AreSame(classDefCol[parentSubClassA].SuperClassClassDef, classDefCol[parentSubClassB].SuperClassClassDef);
        }

        [Test]
        public void Test_Map_WhenInheritance_ShouldNotCreatePKInSubClass()
        {
            //---------------Set up test pack-------------------
            var superClass = typeof(FakeBOSuperClass);
            var subClass = typeof(FakeBOSubClass);
            var source = new FakeTypeSource(new[] {superClass, subClass});
            var allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            var classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            classDefCol.ShouldHaveCount(2);
            var subClassDef = classDefCol.First(def => def.ClassType == subClass);
            var superClassDef = classDefCol.First(def => def.ClassType == superClass);
            Assert.IsNotNull(superClassDef.PrimaryKeyDef);
            Assert.IsNull(subClassDef.PrimaryKeyDef, "Should Not Create a PK since it will use the super classes PK");

            subClassDef.PropDefcol.ShouldBeEmpty();
            superClassDef.PropDefcol.ShouldNotBeEmpty();
        }

        [Test]
        public void Test_Map_WhenInheritance_ShouldCreateDiscriminatorPropInSuperClass()
        {
            //---------------Set up test pack-------------------
            var superClass = typeof(FakeBOSuperClass);
            var subClass = typeof(FakeBOSubClass);
            var source = new FakeTypeSource(new[] {superClass, subClass});
            var allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            var classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            classDefCol.ShouldHaveCount(2);
            var subClassDef = classDefCol.First(def => def.ClassType == subClass);
            var superClassDef = classDefCol.First(def => def.ClassType == superClass);

            var discriminatorPropName = subClassDef.SuperClassDef.Discriminator;

            var superClassProps = superClassDef.PropDefcol;
            superClassProps.ShouldHaveCount(2);
            superClassProps.ShouldContain(propDef => propDef.PropertyName == discriminatorPropName);
        }

        [Test]
        public void Test_Map_GivenTypeSourceProvidesSubClassBeforeSuperClass_WhenInheritance_ShouldCreateDiscriminatorPropInSuperClass()
        {
            //---------------Set up test pack-------------------
            var superClass = typeof(FakeBOSuperClass);
            var subClass = typeof(FakeBOSubClass);
            var source = new FakeTypeSource(new[] {subClass, superClass});
            var allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            var classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            classDefCol.ShouldHaveCount(2);
            var subClassDef = classDefCol.First(def => def.ClassType == subClass);
            var superClassDef = classDefCol.First(def => def.ClassType == superClass);

            var discriminatorPropName = subClassDef.SuperClassDef.Discriminator;

            var superClassProps = superClassDef.PropDefcol;
            superClassProps.ShouldHaveCount(2);
            superClassProps.ShouldContain(propDef => propDef.PropertyName == discriminatorPropName);
        }

        [Test]
        public void Test_Map_WhenInheritanceAndHasRelationship_ShouldCreateCorrectRelPropDefs()
        {
            //---------------Set up test pack-------------------
            var superClass = typeof(FakeBOSuperClass);
            var subClass = typeof(FakeBOSubClassWithRelationships);
            var source = new FakeTypeSource(new[] {superClass, subClass});
            var allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            var classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            classDefCol.ShouldHaveCount(2);
            var subClassDef = classDefCol.First(def => def.ClassType == subClass);
            var superClassDef = classDefCol.First(def => def.ClassType == superClass);

            subClassDef.PropDefcol.ShouldHaveCount(1, "Should have FK for MultipleRel Created");
            superClassDef.PropDefcol.ShouldHaveCount(2, "Should have PK and Discriminator Created");

            subClassDef.RelationshipDefCol.ShouldHaveCount(2, "Should have Multiple and single Rel");
            var multipleRelDef = subClassDef.RelationshipDefCol.First(relationshipDef => relationshipDef.IsOneToMany);
            var multipleRelPropDef = multipleRelDef.RelKeyDef.First();
            Assert.AreEqual("FakeBOSuperClassID", multipleRelPropDef.OwnerPropertyName);
            Assert.AreEqual("FakeBOSuperClassID", multipleRelPropDef.RelatedClassPropName);

            var singleRelDef = subClassDef.RelationshipDefCol.First(relationshipDef => relationshipDef.IsManyToOne);
            var singleRelPropDef = singleRelDef.RelKeyDef.First();
            Assert.AreEqual("SingleRelID", singleRelPropDef.OwnerPropertyName);
            Assert.AreEqual("FakeBoNoPropsID", singleRelPropDef.RelatedClassPropName);
        }

        [Test]
        public void Test_Map_GivenTypeSourceProvidesSubClassBeforeSuperClass_WhenInheritanceAndHasRelationship_ShouldCreateCorrectRelPropDefs()
        {
            //---------------Set up test pack-------------------
            var superClass = typeof(FakeBOSuperClass);
            var subClass = typeof(FakeBOSubClassWithRelationships);
            var source = new FakeTypeSource(new[] {subClass, superClass});
            var allClassesAutoMapper = new AllClassesAutoMapper(source);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            var classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            classDefCol.ShouldHaveCount(2);
            var subClassDef = classDefCol.First(def => def.ClassType == subClass);
            var superClassDef = classDefCol.First(def => def.ClassType == superClass);

            subClassDef.PropDefcol.ShouldHaveCount(1, "Should have FK for MultipleRel Created");
            superClassDef.PropDefcol.ShouldHaveCount(2, "Should have PK and Discriminator Created");

            subClassDef.RelationshipDefCol.ShouldHaveCount(2, "Should have Multiple and single Rel");
            var multipleRelDef = subClassDef.RelationshipDefCol.First(relationshipDef => relationshipDef.IsOneToMany);
            var multipleRelPropDef = multipleRelDef.RelKeyDef.First();
            Assert.AreEqual("FakeBOSuperClassID", multipleRelPropDef.OwnerPropertyName);
            Assert.AreEqual("FakeBOSuperClassID", multipleRelPropDef.RelatedClassPropName);

            var singleRelDef = subClassDef.RelationshipDefCol.First(relationshipDef => relationshipDef.IsManyToOne);
            var singleRelPropDef = singleRelDef.RelKeyDef.First();
            Assert.AreEqual("SingleRelID", singleRelPropDef.OwnerPropertyName);
            Assert.AreEqual("FakeBoNoPropsID", singleRelPropDef.RelatedClassPropName);
        }

        [Test]
        public void Test_Map_When2LayersOfInheritance_AndHasDiscriminatorProp_ShouldNotCreateDiscriminator()
        {
            //---------------Set up test pack-------------------
            var superSuperClass = typeof(FakeBOSuperClassWithDesc);
            var superClass = typeof(FakeBOSubClassSuperHasDesc);
            var subClass = typeof(FakeBOSubSubClassSuperHasDesc);
            var source = new FakeTypeSource(new[] {superSuperClass, superClass, subClass});
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            var allClassesAutoMapper = new AllClassesAutoMapper(source);
            var classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            var subClassDef = classDefCol.First(def => def.ClassName == "FakeBOSubSubClassSuperHasDesc");
            var inheritanceDef = subClassDef.SuperClassDef;
            var superClassClassDef = inheritanceDef.SuperClassClassDef;
            var superSuperClassClassDef = superClassClassDef.SuperClassDef.SuperClassClassDef;

            superClassClassDef.PropDefcol.ShouldHaveCount(0,
                "No Properties Should be created for SuperClass since ID and Discriminator will be on SuperSuperClass");
            var superSuperClassProps = superSuperClassClassDef.PropDefcol;
            superSuperClassProps.ShouldHaveCount(2, "Discriminator and ID Prop should be created");

            superSuperClassProps.ShouldContain(def => def.PropertyName == "FakeBOSuperClassWithDescType");
        }

        [Test]
        public void Test_Map_WhenSubClassHasSuperClassWithUniqueConstraint_ShouldNotCreateDuplicate()
        {
            //---------------Set up test pack-------------------
            var superClass = typeof(FakeBOSuperClassWithUC);
            var subClass = typeof(FakeBOSubClassWithSuperHasUC);
            var source = new FakeTypeSource(new[] {superClass, subClass});
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, source.GetTypes().Count());
            //---------------Execute Test ----------------------
            var allClassesAutoMapper = new AllClassesAutoMapper(source);
            var classDefCol = allClassesAutoMapper.Map();
            //---------------Test Result -----------------------
            var subClassDef = classDefCol.First(def => def.ClassName == "FakeBOSubClassWithSuperHasUC");
            var inheritanceDef = subClassDef.SuperClassDef;
            var superClassClassDef = inheritanceDef.SuperClassClassDef;

            Assert.AreEqual(0, subClassDef.KeysCol.Count);
            Assert.AreEqual(1, superClassClassDef.KeysCol.Count);
            Assert.AreEqual("UC_Fake", superClassClassDef.KeysCol["UC_Fake"].KeyName);
        }

        [Test]
        public void Test_IdentityNameConvention_ShouldBeDefaultIfNotSet()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var idConvention = AllClassesAutoMapper.PropNamingConvention;
            //---------------Test Result -----------------------
            idConvention.ShouldBeOfType<DefaultPropNamingConventions>();
        }

        [Test]
        public void Test_SetIdentityNameConvention_ShouldSetCustomConvention()
        {
            //---------------Set up test pack-------------------
            INamingConventions expectedConvention = MockRepository.GenerateMock<INamingConventions>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            AllClassesAutoMapper.PropNamingConvention = expectedConvention;
            //---------------Test Result -----------------------
            Assert.AreSame(expectedConvention, AllClassesAutoMapper.PropNamingConvention);
        }

        [Test]
        public void TestAccept_Validate_WhenAllClassDefsLoaded_ShouldBeTrue()
        {
            //---------------Set up test pack-------------------
            Func<TypeWrapper, bool> whereClause = type
                    => (type.Namespace == "Habanero.Smooth.Test.ValidFakeBOs");
            AppDomainTypeSource source = new AppDomainTypeSource(whereClause);
            AllClassesAutoMapper allClassesAutoMapper = new AllClassesAutoMapper(source);
            ClassDefCol classDefCol = allClassesAutoMapper.Map();
            ClassDefValidator validator = new ClassDefValidator(new DefClassFactory());
            //---------------Assert Precondition----------------
            Assert.Greater(classDefCol.Count, 0);
            //---------------Execute Test ----------------------
            validator.ValidateClassDefs(classDefCol);
            //---------------Test Result -----------------------
            Assert.IsNotNull(classDefCol);
            //Should Validate without Error if it gets here then it has validated
        }
    }

    internal class FakeSingleRelationshipDef : SingleRelationshipDef
    {
        public FakeSingleRelationshipDef()
            : base("rel", typeof(FakeBoNoProps), new RelKeyDef(), true, DeleteParentAction.Prevent)
        {
            this.ReverseRelationshipName = "ReverseRelationship";
        }

    }

    internal class FakeTypeSource : CustomTypeSource
    {
        public FakeTypeSource()
        {
        }

        public FakeTypeSource(IEnumerable<Type> types):base(types)
        {
        }

        public FakeTypeSource(Type type)
        {
            Add(type);
        }
    }

    internal class DummyTypeSourceWithMockItems : ITypeSource
    {
        private readonly IEnumerable<TypeWrapper> _typeWrappers;
        private int NoOfItems { get; set; }

        public DummyTypeSourceWithMockItems(int noOfItems)
        {
            NoOfItems = noOfItems;
            _typeWrappers = CreateTypes();
        }

        public DummyTypeSourceWithMockItems():this(4)
        {           
        }

        public IEnumerable<TypeWrapper> GetTypes()
        {
            return _typeWrappers;
        }

        private IEnumerable<TypeWrapper> CreateTypes()
        {
            IList<TypeWrapper> items = new List<TypeWrapper>();
            for (int i = 0; i < NoOfItems; i++)
            {
                items.Add(new FakeTypeWrapper());
            }
            return items;
        }
    }
}
// ReSharper restore InconsistentNaming