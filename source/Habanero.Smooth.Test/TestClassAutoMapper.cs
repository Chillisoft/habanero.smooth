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
using System.Linq;
using System.Reflection;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Smooth.Test.ExtensionMethods;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.Test.ValidFakeBOs;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Smooth.Test
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestClassAutoMapper
    {
        [SetUp]
        public void Setup()
        {
            AllClassesAutoMapper.PropNamingConvention = new DefaultPropNamingConventions();
            AllClassesAutoMapper.ClassDefCol = null;
        }

        [Test]
        public void Test_MustBeMapped_WhenHasIgnore_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBoIgnore);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var mustBeMapped = type.MustBeMapped();
            //---------------Test Result -----------------------
            Assert.IsFalse(mustBeMapped);
        }

        [Test]
        public void Test_MustBeMapped_WhenIsIBo_ShouldRetTrue()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBoNoProps);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var mustBeMapped = type.MustBeMapped();
            //---------------Test Result -----------------------
            Assert.IsTrue(mustBeMapped);
        }
                
        [Test]
        public void Test_MustBeMapped_WhenNotIsIBo_ReturnFalse()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var type = typeof(SomeNonBoClass);
            var mustBeMapped = type.MustBeMapped();
            //---------------Test Result -----------------------
            Assert.IsFalse(mustBeMapped);
        }

        [Test]
        public void Test_MustBeMapped_WhenIsInterface_ReturnFalse()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var type = typeof (IFakeBoInterfaceShouldNotBeLoaded);
            var mustBeMapped = type.MustBeMapped();
            //---------------Test Result -----------------------
            Assert.IsFalse(mustBeMapped);
        }

        [Test]
        public void Test_MustBeMapped_WhenIsAbstract_ReturnFalse()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var type = typeof (FakeAbstractBoShouldNotBeLoaded);
            var mustBeMapped = type.MustBeMapped();
            //---------------Test Result -----------------------
            Assert.IsFalse(mustBeMapped);
        }

        [Test]
        public void Test_MustBeMapped_WhenNull_ReturnFalse()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            Type type = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            var mustBeMapped = type.MustBeMapped();
            //---------------Test Result -----------------------
            Assert.IsFalse(mustBeMapped);
        }

        [Test]
        public void Test_Map_WhenHasIgnore_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBoIgnore);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.IsNull(classDef);
        }

        [Test]
        public void Test_MapClass_WhenIsIBo_ShouldMapClassName()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBoNoProps);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.IsNotNull(classDef);
            Assert.AreEqual(type.Name, classDef.ClassName);
            Assert.AreEqual("Habanero.Smooth.Test", classDef.AssemblyName);
        }

        [Test]
        public void Test_MapClass_WhenNotIsIBo_ReturnNullClassDef()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var type = typeof(SomeNonBoClass);
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.IsNull(classDef);
        }

        [Test]
        public void Test_MapClass_WhenIsInterface_ReturnNullClassDef()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var type = typeof (IFakeBoInterfaceShouldNotBeLoaded);
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.IsNull(classDef);
        }

        [Test]
        public void Test_MapClass_WhenIsAbstract_ReturnNullClassDef()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var type = typeof(FakeAbstractBoShouldNotBeLoaded);
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.IsNull(classDef);
        }

        [Test]
        public void Test_Map_WhenHasNoProps_ShouldMapNoProps()
        {
            //---------------Set up test pack-------------------
            var type = typeof (FakeBoNoProps);
            //---------------Assert Precondition----------------
            type.GetProperties().Length.ShouldEqual(7);
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.PropDefcol.Count, "Creates A Primary Key Prop");
        }

        [Test]
        public void Test_Map_WhenIsSubtypeOfBusinessObjectOfT_ShouldNotCreateExtraPrimaryKeyProp()
        {
            //---------------Set up test pack-------------------
            var type = typeof (FakeBoNoPropsOfT);
            //---------------Assert Precondition----------------
            type.GetProperties().Length.ShouldEqual(7);
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.PropDefcol.Count, "Creates A Primary Key Prop");
        }

        [Test]
        public void Test_Map_WhenHasNoMappedTableName_ShouldSetClassName()
        {
            //---------------Set up test pack-------------------
            string expectedTableName = "FakeBoWithoutTableName";
            var type = typeof(FakeBoWithoutTableName);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedTableName, classDef.TableName);
        }

        [Test]
        public void Test_Map_WhenHasMappedTableName_ShouldSetTableNameOnClassDef()
        {
            //---------------Set up test pack-------------------
            string expectedTableName = "tbMyFakeBo";
            var type = typeof(FakeBoWithTableName);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedTableName, classDef.TableName);
        }

        [Test]
        public void Test_MapClass_WhenInheritsFromGenericBO_ReturnMapClass()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBOGeneric);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.IsNotNull(classDef);
        }

        [Test]
        public void Test_MapClass_WhenInheritsFromGenericBO_ReturnMapDefaultPrimaryKeyCorrectly()
        {
            //---------------Set up test pack-------------------
            var type = typeof (FakeBOGeneric);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.IsNotNull(classDef.PrimaryKeyDef);
            Assert.AreEqual(1, classDef.PrimaryKeyDef.Count);
            var pkProp = classDef.PrimaryKeyDef[0];
            Assert.AreEqual("FakeBOGeneric" + "ID", pkProp.PropertyName);
        }

        [TestCase("PublicStringProp")]
        [TestCase("PublicGetNullableGuidProp")]
        [TestCase("PublicGetGuidProp")]
        [TestCase("PublicEnumProp")]
        [TestCase("PublicNullableEnumProp")]
        [TestCase("PublicPropWithAtt")]
        [TestCase("PublicImageProp")]
        [TestCase("PublicByteArrayProp")]
        public void Test_Map_WhenHasProps_ShouldMapProps(string propName)
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBOWProps);
            //---------------Assert Precondition----------------
            type.GetProperties().Length.ShouldBeGreaterThan(5);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.GreaterOrEqual(classDef.PropDefcol.Count, 5);
            classDef.PropDefcol.ShouldContain(propDef => propDef.PropertyName == propName);
        }

        [Test]
        public void Test_MapClass_ShouldAssignPropDefToClassDef()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "PublicGetGuidProp";
            var classType = typeof(FakeBOWProps);
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo);
            Assert.AreEqual(expectedPropName, propertyInfo.Name);
            //---------------Execute Test ----------------------
            var classDef = classType.MapClass();
            //---------------Test Result -----------------------
            Assert.IsNotNull(classDef);
            IPropDef propDef = classDef.GetPropDef(expectedPropName);
            Assert.IsNotNull(propDef, "Prop Def should not be null");
            Assert.IsNotNull(propDef.ClassDef, "Prop Def should be assigned to ClassDef");
        }

        [Test]
        public void Test_Map_ShouldMapPrimaryKey()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBOAttributePK);
            //---------------Assert Precondition----------------
            const string primaryKeyPropName = "PublicGuidProp";
            Assert.IsTrue(type.HasPrimaryKeyAttribute(primaryKeyPropName));
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.IsNotNull(classDef.PrimaryKeyDef);
            Assert.AreEqual(1, classDef.PrimaryKeyDef.Count);
            var propDef = classDef.PrimaryKeyDef[0];
            Assert.AreEqual(primaryKeyPropName, propDef.PropertyName);
        }

        [Test]
        public void Test_Map_ShouldMapManyToOneRelationship()
        {
            //---------------Set up test pack-------------------
            const string expectedPropName = "MySingleRelationship";
            const string expectedOwnerPropName = expectedPropName + "ID";
            var type = typeof(FakeBOWithSingleRel);
            //---------------Assert Precondition----------------
            PropertyInfo propertyInfo = type.GetProperty(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObject>();
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.RelationshipDefCol.Count, "Should have a single relationship");
            Assert.AreEqual(2, classDef.PropDefcol.Count, "Should have FKProp and IDProp");
            var relationshipDef = classDef.RelationshipDefCol[expectedPropName];
            Assert.AreEqual(expectedPropName, relationshipDef.RelationshipName);
            var relPropDef = relationshipDef.RelKeyDef.FirstOrDefault();
            Assert.IsNotNull(relPropDef);
            var fkPropDef = classDef.GetPropDef(relPropDef.OwnerPropertyName);
            Assert.IsNotNull(fkPropDef);
            Assert.AreEqual(expectedOwnerPropName, fkPropDef.PropertyName);
            Assert.AreEqual(expectedOwnerPropName, fkPropDef.DatabaseFieldName);
        }

        [Test]
        public void Test_Map_WhenHasFKProp_ShouldNotCreateFKProp()
        {
            //If you have a relationship 'MySingleRelationship' that
            //  and you can find a FKProp 'MySingleRelationshipID'
            // then you should not create an FKProp
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithSingleRelAndFKProp);
            const string expectedPropName = "MySingleRelationship";
            const string expectedOwnerPropName = expectedPropName + "ID";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            propertyInfo.AssertIsOfType<IBusinessObject>();
            var relatedClassType = propertyInfo.PropertyType;
            Assert.IsFalse(relatedClassType.HasReverseRelationshipOfType<FakeBOWithSingleRel>());
            //---------------Execute Test ----------------------
            var classDef = classType.MapClass();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, classDef.PropDefcol.Count, "Should have FKProp and IDProp");
            var relationshipDef = classDef.RelationshipDefCol[expectedPropName];
            var relPropDef = relationshipDef.RelKeyDef.FirstOrDefault();
            Assert.IsNotNull(relPropDef);
            var fkPropDef = classDef.GetPropDef(relPropDef.OwnerPropertyName);
            Assert.IsNotNull(fkPropDef);
            Assert.AreEqual(expectedOwnerPropName, fkPropDef.PropertyName);
            Assert.AreEqual(expectedOwnerPropName, fkPropDef.DatabaseFieldName);
        }

        [Test]
        public void Test_Map_GivenManyToOne_WhenHasAutoMapFieldNameAttribute_ShouldSetDatabaseFieldName()
        {
            //---------------Set up test pack-------------------
            var classType = typeof (FakeBOWithSingleRelWithFieldNameOverride);
            const string expectedPropName = "MySingleRelationship";
            const string expectedOwnerPropName = expectedPropName + "ID";
            const string expectedDatabaseFieldName = "SingleID";
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObject>();
            //---------------Execute Test ----------------------
            var classDef = classType.MapClass();
            //---------------Test Result -----------------------
            var actualPropDef = classDef.GetPropDef(expectedOwnerPropName);
            Assert.AreEqual(expectedDatabaseFieldName, actualPropDef.DatabaseFieldName);
        }

        [TestCase("MyMultRel", 2)]
        [TestCase("MyMultRell2", 2)]
        public void TestAccept_Map_ShouldMapOneToMany(string expectedPropName, int expectedNoRels)
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBOWithOne12M);
            //---------------Assert Precondition----------------
            PropertyInfo propertyInfo = type.GetProperty(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObjectCollection>();
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedNoRels, classDef.RelationshipDefCol.Count);
            var relationshipDef = classDef.RelationshipDefCol[expectedPropName];
            Assert.AreEqual(expectedPropName, relationshipDef.RelationshipName);
            Assert.IsInstanceOf(typeof(MultipleRelationshipDef), relationshipDef);
        }

        [TestCase("MySingleRelationship", 2)]
        [TestCase("MySecondSingleRelationship", 2)]
        public void TestAccept_Map_WhenTwoM_1Relationships_ShouldMapBothRelationships(string expectedPropName, int expectedNoRels)
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBOWithTwoSingleRelNoRevs);
            //---------------Assert Precondition----------------
            PropertyInfo propertyInfo = type.GetProperty(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObject>();
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedNoRels, classDef.RelationshipDefCol.Count);
            var relationshipDef = classDef.RelationshipDefCol[expectedPropName];
            Assert.AreEqual(expectedPropName, relationshipDef.RelationshipName);
            Assert.IsInstanceOf(typeof (SingleRelationshipDef), relationshipDef);
        }

        [TestCase("MyOneToOne", 2)]
        [TestCase("MyOneToOne2", 2)]
        public void TestAccept_Map_WhenTwo1_1Relationships_ShouldMapBothRelationships(string expectedPropName, int expectedNoRels)
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBOWithTwo11Rels);
            //---------------Assert Precondition----------------
            PropertyInfo propertyInfo = type.GetProperty(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObject>();
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedNoRels, classDef.RelationshipDefCol.Count);
            var relationshipDef = classDef.RelationshipDefCol[expectedPropName];
            Assert.AreEqual(expectedPropName, relationshipDef.RelationshipName);
            Assert.IsInstanceOf(typeof(SingleRelationshipDef), relationshipDef);
        }

        [TestCase("MySingleRel1", 2)]
        [TestCase("MySingleRel2", 2)]
        public void TestAccept_Map_WhenOne1_1AndOneM_1Relationships_ShouldMapBothRelationships(string expectedPropName, int expectedNoRels)
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBOWithOneM21_AndOne121);
            //---------------Assert Precondition----------------
            PropertyInfo propertyInfo = type.GetProperty(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObject>();
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedNoRels, classDef.RelationshipDefCol.Count);
            var relationshipDef = classDef.RelationshipDefCol[expectedPropName];
            Assert.AreEqual(expectedPropName, relationshipDef.RelationshipName);
            Assert.IsInstanceOf(typeof(SingleRelationshipDef), relationshipDef);
        }

        [TestCase("MyMultRel",typeof(MultipleRelationshipDef) , 6)]
        [TestCase("MyMultRell2", typeof(MultipleRelationshipDef), 6)]
        [TestCase("MySingleRel1", typeof(SingleRelationshipDef), 6)]
        [TestCase("MySingleRelationship1", typeof(SingleRelationshipDef), 6)]
        [TestCase("MySingleRelationship2", typeof(SingleRelationshipDef), 6)]
        [TestCase("MySingleRelationship3", typeof(SingleRelationshipDef), 6)]
        public void TestAccept_Map_WhenHasAllTypesOfRel_ShouldMapAll(string expectedPropName, Type relType, int expectedNoRels)
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBOWithAllTypesOfRel);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedNoRels, classDef.RelationshipDefCol.Count);
            var relationshipDef = classDef.RelationshipDefCol[expectedPropName];
            Assert.AreEqual(expectedPropName, relationshipDef.RelationshipName);
            Assert.IsInstanceOf(relType, relationshipDef);
        }
        
        [Test]
        public void TestAccept_Map_WhenHasUniqueConstraint()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBOWithUniqueConstraint_TwoProps);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.KeysCol.Count);
            IKeyDef keyDef = classDef.KeysCol["UC1"];
            Assert.IsNotNull(keyDef);
            Assert.AreEqual(2, keyDef.Count);
            Assert.AreEqual("UCProp1", keyDef[0].PropertyName);
            Assert.AreEqual("UCProp2", keyDef[1].PropertyName);
        }

        [Test]
        public void Test_Map_WhenHasUniqueConstraint_OneProp()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBOWithUniqueConstraint_OneProp);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.KeysCol.Count);
            IKeyDef keyDef = classDef.KeysCol["UC"];
            Assert.IsNotNull(keyDef);
            Assert.AreEqual(1, keyDef.Count);
            Assert.AreEqual("UCProp", keyDef[0].PropertyName);
        }

        [Test]
        public void Test_Map_WhenHasTwoUniqueConstraint_OnePropEach()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBOWithTwoUniqueConstraints_OnePropEach);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, classDef.KeysCol.Count);
            IKeyDef keyDef1 = classDef.KeysCol["UC1"];
            IKeyDef keyDef2 = classDef.KeysCol["UC2"];
            Assert.IsNotNull(keyDef1);
            Assert.IsNotNull(keyDef2);
            Assert.AreEqual(1, keyDef1.Count);
            Assert.AreEqual(1, keyDef2.Count);
            Assert.AreEqual("UCProp1", keyDef1[0].PropertyName);
            Assert.AreEqual("UCProp2", keyDef2[0].PropertyName);
        }

        [Test]
        public void Test_Map_WhenHasTwoUniqueConstraint_TwoPropEach()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBOWithTwoUniqueConstraints_TwoPropEach);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var classDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, classDef.KeysCol.Count);
            IKeyDef keyDef1 = classDef.KeysCol["UC1"];
            IKeyDef keyDef2 = classDef.KeysCol["UC2"];
            Assert.IsNotNull(keyDef1);
            Assert.IsNotNull(keyDef2);
            Assert.AreEqual(2, keyDef1.Count);
            Assert.AreEqual(2, keyDef2.Count);
            Assert.AreEqual("UC1Prop1", keyDef1[0].PropertyName);
            Assert.AreEqual("UC1Prop2", keyDef1[1].PropertyName);
            Assert.AreEqual("UC2Prop1", keyDef2[0].PropertyName);
            Assert.AreEqual("UC2Prop2", keyDef2[1].PropertyName);
        }

        [Test]
        public void Test_Map_WhenOneUniqueConstraintOnRelationship_ShouldCreateConstraintOnRelationshipProperty()
        {
            //---------------Set up test pack-------------------
            ClassDef.ClassDefs.Add(typeof(FakeBOWithNoRelationship).MapClass());
            
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var classDef = typeof(FakeBOWithUniqueConstraint_Relationship).MapClass();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.KeysCol.Count);
            IKeyDef keyDef1 = classDef.KeysCol["UC"];
            Assert.AreEqual(1, keyDef1.Count);
            Assert.AreSame(classDef.GetPropDef("RelatedObjectID"), keyDef1[0]);
        }

        [Test]
        public void Test_Map_WhenInheritance_ShouldMapInheritenceRelationship()
        {
            //---------------Set up test pack-------------------
            Type superClass = typeof(FakeBOSuperClass);
            Type subClass = typeof(FakeBOSubClass);
            //---------------Assert Precondition----------------
            Assert.AreSame(superClass, subClass.BaseType);
            Assert.IsTrue(superClass.ToTypeWrapper().IsBusinessObject);
            //---------------Execute Test ----------------------
            var subClassDef = subClass.MapClass();
            //---------------Test Result -----------------------
            Assert.IsNotNull(subClassDef);
            Assert.AreSame(superClass, subClassDef.SuperClassClassDef.ClassType);
            Assert.AreEqual(ORMapping.SingleTableInheritance, subClassDef.SuperClassDef.ORMapping);
            Assert.AreEqual("FakeBOSuperClassType", subClassDef.SuperClassDef.Discriminator);
        }

        [Test]
        public void Test_Map_WhenInheritance_AndBaseClassAlreadyInClassDefCol_ShouldUseExistingClassDef()
        {
            //---------------Set up test pack-------------------
            Type superClass = typeof(FakeBOSuperClass);
            Type subClass = typeof(FakeBOSubClass);
            var superClassDef = superClass.MapClass();
            AllClassesAutoMapper.ClassDefCol = new ClassDefCol{superClassDef};
            //---------------Assert Precondition----------------
            Assert.AreSame(superClass, subClass.BaseType);
            Assert.IsTrue(superClass.ToTypeWrapper().IsBusinessObject);
            Assert.AreEqual(1, AllClassesAutoMapper.ClassDefCol.Count);
            Assert.IsTrue(AllClassesAutoMapper.ClassDefCol.Contains(superClassDef), "Should Contain");
            //---------------Execute Test ----------------------
            var subClassDef = subClass.MapClass();
            //---------------Test Result -----------------------
            Assert.IsNotNull(subClassDef);
            Assert.AreSame(superClassDef, subClassDef.SuperClassClassDef);
        }

        [Test]
        public void Test_Map_WhenClassAlreadyInClassDefCol_ShouldRetExistingClassDef()
        {
            //---------------Set up test pack-------------------
            Type type = typeof(FakeBOSuperClass);
            var expectedClassDef = type.MapClass();
            AllClassesAutoMapper.ClassDefCol = new ClassDefCol { expectedClassDef }; 
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, AllClassesAutoMapper.ClassDefCol.Count);
            Assert.IsTrue(AllClassesAutoMapper.ClassDefCol.Contains(expectedClassDef), "Should Contain");
            //---------------Execute Test ----------------------
            var returnedClasDef = type.MapClass();
            //---------------Test Result -----------------------
            Assert.AreSame(expectedClassDef, returnedClasDef);
        }

        [Test]
        public void Test_IdentityNameConvention_ShouldBeDefaultIfNotSet()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var idConvention = ClassAutoMapper.PropNamingConvention;
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
            Assert.AreSame(expectedConvention, ClassAutoMapper.PropNamingConvention);
        }

        [Test]
        public void TestAccept_Map_NoFKPropDef_ShouldCreateFKProp()
        {
            //If you have a relationship 'MySingleRelationship' that
            //  you cannot find a Prop Def in 'FakeBOWithSingleRel' that has same name 'MySingleRelationshipID'
            // then you should create a ForeignKey PropDef. 
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithSingleRel);
            const string expectedPropName = "MySingleRelationship";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            propertyInfo.AssertIsOfType<IBusinessObject>();
            var relatedClassType = propertyInfo.PropertyType;
            Assert.IsFalse(relatedClassType.HasReverseRelationshipOfType<FakeBOWithSingleRel>());
            //---------------Execute Test ----------------------
            var classDef = classType.MapClass();
            var relationshipDef = classDef.RelationshipDefCol[expectedPropName];
            //---------------Test Result -----------------------
            var relKeyDef = relationshipDef.RelKeyDef;
            string expectedOwnerPropName = relationshipDef.GetOwningPropName();
            Assert.IsTrue(relKeyDef.Contains(expectedOwnerPropName),
                          "By Convention the RelationshipPropName on the single side of the M-1 Relationship Should be RelationshipName & ID");
            var relPropDef = relKeyDef[expectedOwnerPropName];

            var propDef = classDef.GetPropDef(relPropDef.OwnerPropertyName, false);
            Assert.IsNotNull(propDef, "Foreign Key Prop Def should have been created");


            Assert.AreEqual(expectedOwnerPropName, propDef.PropertyName);
            Assert.AreEqual(PropReadWriteRule.ReadWrite, propDef.ReadWriteRule);
            Assert.AreEqual(typeof(Guid), propDef.PropertyType);
            Assert.AreEqual(null, propDef.DefaultValue);
        }

        [Test]
        public void TestAccept_Map_UsingType_NoFKPropDef_ShouldCreateFKProp()
        {
            //If you have a relationship 'MySingleRelationship' that
            //  you cannot find a Prop Def in 'FakeBOWithSingleRel' that has same name 'MySingleRelationshipID'
            // then you should create a ForeignKey PropDef. 
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithSingleRel);
            const string expectedPropName = "MySingleRelationship";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            propertyInfo.AssertIsOfType<IBusinessObject>();
            var relatedClassType = propertyInfo.PropertyType;
            Assert.IsFalse(relatedClassType.HasReverseRelationshipOfType<FakeBOWithSingleRel>());
            //---------------Execute Test ----------------------
            ClassAutoMapper classAutoMapper = new ClassAutoMapper(classType);
            var classDef = classAutoMapper.Map();
            var relationshipDef = classDef.RelationshipDefCol[expectedPropName];
            //---------------Test Result -----------------------
            var relKeyDef = relationshipDef.RelKeyDef;
            string expectedOwnerPropName = relationshipDef.GetOwningPropName();
            Assert.IsTrue(relKeyDef.Contains(expectedOwnerPropName),
                          "By Convention the RelationshipPropName on the single side of the M-1 Relationship Should be RelationshipName & ID");
            var relPropDef = relKeyDef[expectedOwnerPropName];

            var propDef = classDef.GetPropDef(relPropDef.OwnerPropertyName, false);
            Assert.IsNotNull(propDef, "Foreign Key Prop Def should have been created");


            Assert.AreEqual(expectedOwnerPropName, propDef.PropertyName);
            Assert.AreEqual(PropReadWriteRule.ReadWrite, propDef.ReadWriteRule);
            Assert.AreEqual(typeof(Guid), propDef.PropertyType);
            Assert.AreEqual(null, propDef.DefaultValue);
        }

        [Test]
        public void Test_MapAccept_WhenHasRelationship_ShouldMapRelationshipDefToClassDef()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithCompulsoryProp);
            const string expectedPropName = "CompulsorySingleRelationship";
            var propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsOfType<IBusinessObject>();
            //---------------Execute Test ----------------------
            var classDef = classType.MapClass();
            var relationshipDef = classDef.RelationshipDefCol[expectedPropName];
            //---------------Test Result -----------------------
            Assert.IsNotNull(relationshipDef.OwningClassDef);
            Assert.AreSame(classDef, relationshipDef.OwningClassDef);
        }
    }
}
