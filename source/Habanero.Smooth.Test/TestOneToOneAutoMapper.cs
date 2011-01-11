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
using System.Reflection;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Smooth.Test.ValidFakeBOs;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Smooth.Test
{
    [TestFixture]
    public class TestOneToOneAutoMapper
    {
        //Mapping One To One is more complicated than OneToMany or ManyToOne.
        //Conventions
        // 1) If no single reverse relationship and no Attribute is found then it is assumed that relationship is a M:1 i.e. its rev is a 1:M
        // 2) If no single rev rel and 1:1Attribute with no RevRelName then RevRelName = ClassName
        // 3) If no single rev rel and 1:1Att with RevRelName then RevRelName = DeclaredRevRelName
        // 4) If has single rev rel then RevRelName = foundRevRelationshipName 
        // Determing RelatedProps 
        // if ownerClass.HasProp(RelName+ID) and relatedClass.NotHasProp(RevRelName+ID) 
        //      ownerProp = foundOwnerPropName
        //      relatedProp = RelatedClassID.
        // if relatedClass.HasProp(reverseRel+ID) and ownerClass.NotHasProp(RelName+ID)
        //      ownerProp = OwnerClassId
        //      relatedProp = foundRelatedPropName
        // if ownerClass.HasProp(RelName+ID) and relatedClass.HasProp(RevRelName+ID)
        //      ownerProp = foundOwnerClassId
        //      relatedProp = foundRelatedPropName
        // Else if ownerClassName LT relatedClassName
        //     ownerProp = OwnerClassId
        //     relatedProp = reverseRelationshipName+ID
        // Else 
        //    ownerProp = RelationshipName+ID
        //    relatedProp = RelatedClassID
        [SetUp]
        public void Setup()
        {
            SetPropNamingConventionToDefault();
        }

        private static void SetPropNamingConventionToDefault()
        {
            AllClassesAutoMapper.PropNamingConvention = new DefaultPropNamingConventions();
            AllClassesAutoMapper.ClassDefCol = null;
        }

        #region CreateRelPropDef

        #region OwningBoHasForeignKey
               
        //Conventions
        // 1) If no single reverse relationship and no Attribute is found then it is assumed that relationship is a M:1 i.e. its rev is a 1:M
        // 2) If no single rev rel and 1:1Attribute with no RevRelName then RevRelName = ClassName
        // 3) If no single rev rel and 1:1Att with RevRelName then RevRelName = DeclaredRevRelName
        // 4) If has single rev rel then RevRelName = foundRevRelationshipName 
        // Determing RelatedProps 
        // if ownerClass.HasProp(RelName+ID) and relatedClass.NotHasProp(RevRelName+ID) 
        //      owningBOHasForeignKey = true;
        // if relatedClass.HasProp(reverseRel+ID) and ownerClass.NotHasProp(RelName+ID)
        //      owningBOHasForeignKey = false;
        // if ownerClass.HasProp(RelName+ID) and relatedClass.HasProp(RevRelName+ID)
        //      if(RelName == relatedClassName) owningBOHasForeignKey = true;
        //      Else owningBOHasForeignKey = false;
        // Else if ownerClassName LT relatedClassName
        //     owningBOHasForeignKey = false;
        // Else
        //    owningBOHasForeignKey = true;
        
        /// <summary>
        /// if ownerClass.HasProp(RelName+ID) and relatedClass.NotHasProp(RevRelName+ID) 
        ///      owningBOHasForeignKey = true;
        /// </summary>
        [Test]
        public void Test_OwningBoHasForeignKey_OwnerHasProp_RelatedNotHasProp_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);

            ownerType.SetHasProperty(owningFKPropName, true);
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsTrue(ownerType.HasProperty(owningFKPropName));
            Assert.IsFalse(relatedType.HasProperty(relatedFKPropName));

            //---------------Execute Test ----------------------
            var owningBoHasForeignKey = autoMapper.OwningBoHasForeignKey;
            //---------------Test Result -----------------------
            Assert.IsTrue(owningBoHasForeignKey);
        }
        /// <summary>
        /// if ownerClass.NotHasProp(RelName+ID) and relatedClass.HasProp(reverseRel+ID)
        ///      owningBOHasForeignKey = false;
        /// </summary>
        [Test]
        public void Test_OwningBoHasForeignKey_OwnerNotHasProp_RelatedHasProp_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);

            relatedType.SetHasProperty(relatedFKPropName, true);
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(ownerType.HasProperty(owningFKPropName));
            Assert.IsTrue(relatedType.HasProperty(relatedFKPropName));

            //---------------Execute Test ----------------------
            var owningBoHasForeignKey = autoMapper.OwningBoHasForeignKey;
            //---------------Test Result -----------------------
            Assert.IsFalse(owningBoHasForeignKey);
        }
        /// <summary>
        /// if ownerClass.HasProp(RelName+ID) and relatedClass.HasProp(RevRelName+ID)
        /// owningBOHasForeignKey = false;
        /// </summary>
        [Test]
        public void Test_OwningBoHasForeignKey_BothHaveProp_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
            ownerType.SetHasProperty(owningFKPropName, true);
            relatedType.SetHasProperty(relatedFKPropName, true);
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsTrue(ownerType.HasProperty(owningFKPropName));
            Assert.IsTrue(relatedType.HasProperty(relatedFKPropName));
            Assert.AreNotEqual(propertyWrapper.Name, relatedType.Name);
            //---------------Execute Test ----------------------
            var owningBoHasForeignKey = autoMapper.OwningBoHasForeignKey;
            //---------------Test Result -----------------------
            Assert.IsTrue(owningBoHasForeignKey);
        }
        /// <summary>
        ///if ownerClassName LT relatedClassName
        ///     owningBOHasForeignKey = false;;
        /// </summary>
        [Test]
        public void Test_OwningBoHasForeignKey_NeitherHaveProps_OwnerLTRelated_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            TypeWrapper ownerType;
            string relatedFKPropName; string owningFKPropName;
            ownerType = GetMockTypeWrapper();


            FakeTypeWrapper relatedType = MockRepository.GenerateMock<FakeTypeWrapper>();
            relatedType.SetName("zzzzz");
            var reverseRelName = GetRandomString();
            var relationshipName = GetRandomString();
            owningFKPropName = GetFKPropName(relationshipName);
            relatedFKPropName = GetFKPropName(reverseRelName);

            PropertyWrapper propertyWrapper1 = MockRepository.GenerateMock<FakePropertyWrapper>();
            SetupMockPropWrapper(propertyWrapper1, ownerType, relatedType, relationshipName, reverseRelName);
            var propertyWrapper
                    = propertyWrapper1;
            ownerType.ClearReturnValue();
            ownerType.SetName("aaaaa");
            relatedType.SetName("zzzzz");
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(ownerType.HasProperty(owningFKPropName));
            Assert.IsFalse(relatedType.HasProperty(relatedFKPropName));
            Assert.AreEqual(1, relatedType.Name.CompareTo(ownerType.Name));

            //---------------Execute Test ----------------------
            var owningBoHasForeignKey = autoMapper.OwningBoHasForeignKey;
            //---------------Test Result -----------------------
            Assert.IsFalse(owningBoHasForeignKey);
        }
        /// <summary>
        //if ownerClassName GT relatedClassName
        //    owningBOHasForeignKey = true;
        /// </summary>
        [Test]
        public void Test_OwningBoHasForeignKey_WhenNeitherHaveProps_WhenOwnerGTRelated_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
            ownerType.ClearReturnValue();
            ownerType.SetName("zzzzz");
            relatedType.SetName("aaaa");

            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(ownerType.HasProperty(owningFKPropName));
            Assert.IsFalse(relatedType.HasProperty(relatedFKPropName));
            Assert.AreEqual(1, ownerType.Name.CompareTo(relatedType.Name));
//            AssertMockPropSetupCorrectly(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);

            //---------------Execute Test ----------------------
            var owningBoHasForeignKey = autoMapper.OwningBoHasForeignKey;
            //---------------Test Result -----------------------
            Assert.IsTrue(owningBoHasForeignKey);
        }
        /// <summary>
        //if ownerClassName LT relatedClassName
        //     owningBOHasForeignKey = false;;    
        /// </summary>   
        [Test]
        public void Test_OwningBoHasForeignKey_NeitherHaveProps_NonDefaultNaming_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            SetNonDefaultNamingConvention();

            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
            ownerType.ClearReturnValue();

            ownerType.SetName("1111");
            relatedType.SetName("2222");
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(ownerType.HasProperty(owningFKPropName));
            Assert.IsFalse(relatedType.HasProperty(relatedFKPropName));
            Assert.AreEqual(-1, ownerType.Name.CompareTo(relatedType.Name));
//            AssertMockPropSetupCorrectly(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            //---------------Execute Test ----------------------
            var owningBoHasForeignKey = autoMapper.OwningBoHasForeignKey;
            //---------------Test Result -----------------------
            Assert.IsFalse(owningBoHasForeignKey);
        }
        /// <summary>
        /// if ownerClass.HasProp(RelName+ID) and relatedClass.HasProp(RevRelName+ID)
        ///   owningBoHasForeignKey = false
        /// </summary>
        [Test]
        public void Test_OwningBoHasForeignKey_BothHave_FakeNameConvention_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            SetNonDefaultNamingConvention();

            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);

            ownerType.SetHasProperty(owningFKPropName, true);
            relatedType.SetHasProperty(relatedFKPropName, true);
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsTrue(ownerType.HasProperty(owningFKPropName));
            Assert.IsTrue(relatedType.HasProperty(relatedFKPropName));
            Assert.AreNotEqual(propertyWrapper.Name, relatedType.Name);
            //---------------Execute Test ----------------------
            var owningBoHasForeignKey = autoMapper.OwningBoHasForeignKey;
            //---------------Test Result -----------------------
            Assert.IsTrue(owningBoHasForeignKey);
        }

        /// <summary>
        /// If relationship is Aggregation or Composition and
        /// no RelProps defined then should set OwningBOHasForeignKey = False.
        /// </summary>
        [Test]
        public void Test_OwningBOHasForeignKey_WhenIsAggregation_AndNeitherHaveProps_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            SetNonDefaultNamingConvention();

            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
            ownerType.ClearReturnValue();
            ownerType.SetName("ZZZZZ" + relatedType.Name);
            propertyWrapper.Stub(pw => pw.GetAttribute<AutoMapOneToOneAttribute>()).Return(
                new AutoMapOneToOneAttribute(RelationshipType.Aggregation));

            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(ownerType.HasProperty(owningFKPropName));
            Assert.IsFalse(relatedType.HasProperty(relatedFKPropName));
            Assert.AreEqual(RelationshipType.Aggregation, GetAutomapAttribute(propertyWrapper).RelationshipType);
            //---------------Execute Test ----------------------
            var owningBoHasForeignKey = autoMapper.OwningBoHasForeignKey;
            //---------------Test Result -----------------------
            Assert.IsFalse(owningBoHasForeignKey);
        }
        /// <summary>
        /// If relationship is Aggregation or Composition and
        /// no RelProps defined then should set OwningBOHasForeignKey = false.
        /// </summary>
        [Test]
        public void Test_OwningBOHasForeignKey_WhenIsComposition_AndNeitherHaveProps_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            SetNonDefaultNamingConvention();

            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
            ownerType.ClearReturnValue();
            ownerType.SetName("ZZZZZ" + relatedType.Name);
            propertyWrapper.Stub(pw => pw.GetAttribute<AutoMapOneToOneAttribute>()).Return(
                    new AutoMapOneToOneAttribute(RelationshipType.Composition));
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(ownerType.HasProperty(owningFKPropName));
            Assert.IsFalse(relatedType.HasProperty(relatedFKPropName));
            Assert.AreEqual(RelationshipType.Composition, GetAutomapAttribute(propertyWrapper).RelationshipType);
            //---------------Execute Test ----------------------
            var owningBoHasForeignKey = autoMapper.OwningBoHasForeignKey;
            //---------------Test Result -----------------------
            Assert.IsFalse(owningBoHasForeignKey);
        }

        /// <summary>
        /// If relationship is Aggregation or Composition and
        /// no RelProps defined then should set OwningBOHasForeignKey = False.
        /// </summary>
        [Test]
        public void Test_OwningBOHasForeignKey_WhenIsAssociation_AndNeitherHaveProps_ShouldRetTrue()
        {
            //---------------Set up test pack-------------------
            SetNonDefaultNamingConvention();

            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
            ownerType.ClearReturnValue();
            ownerType.SetName("ZZZZZZ" + relatedType.Name);
            propertyWrapper.Stub(pw => pw.GetAttribute<AutoMapOneToOneAttribute>()).Return(
                        new AutoMapOneToOneAttribute(RelationshipType.Association));
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(ownerType.HasProperty(owningFKPropName));
            Assert.IsFalse(relatedType.HasProperty(relatedFKPropName));
            Assert.AreEqual(RelationshipType.Association, GetAutomapAttribute(propertyWrapper).RelationshipType);
            //---------------Execute Test ----------------------
            var owningBoHasForeignKey = autoMapper.OwningBoHasForeignKey;
            //---------------Test Result -----------------------
            Assert.IsTrue(owningBoHasForeignKey);
        }

        private AutoMapOneToOneAttribute GetAutomapAttribute(PropertyWrapper propertyWrapper)
        {
            return propertyWrapper.GetAttribute<AutoMapOneToOneAttribute>();
        }

        #endregion

        /// <summary>
        /// if ownerClass.HasProp(RelName+ID) and relatedClass.NotHasProp(RevRelName+ID) 
        ///      ownerProp = foundOwnerPropName
        ///      relatedProp = RelatedClass.ID.
        /// </summary>
        [Test]
        public void Test_GetRelatedPropName_OwnerHasProp_RelatedNotHasProp_ShouldReturnRelatedTypePkName()
        {
            //---------------Set up test pack-------------------
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
            ownerType.SetHasProperty(owningFKPropName, true);
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsTrue(ownerType.HasProperty(owningFKPropName));
            Assert.IsFalse(relatedType.HasProperty(relatedFKPropName));
            Assert.AreSame(ownerType, propertyWrapper.DeclaringType);
            Assert.AreSame(relatedType, propertyWrapper.RelatedClassType);
            
            //---------------Execute Test ----------------------
            var relatedPropName = autoMapper.GetRelatedPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual(relatedType.GetPKPropName(), relatedPropName);
        }
        /// if relatedClass.HasProp(reverseRel+ID) and ownerClass.NotHasProp(RelName+ID)
        ///      ownerProp = OwnerClass.Id
        ///      relatedProp = foundRelatedPropName
        [Test]
        public void Test_GetRelatedPropName_OwnerNotHasProp_RelatedHasProp_ShouldReturnFoundRelName()
        {
            //---------------Set up test pack-------------------
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);

            relatedType.SetHasProperty(relatedFKPropName, true);
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(ownerType.HasProperty(owningFKPropName));
            Assert.IsTrue(relatedType.HasProperty(relatedFKPropName));
            
            //---------------Execute Test ----------------------
            var relatedPropName = autoMapper.GetRelatedPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual(relatedFKPropName, relatedPropName);
        }
        /// if ownerClass.HasProp(RelName+ID) and relatedClass.HasProp(RevRelName+ID)
        ///      ownerProp = foundOwnerClassId
        ///      relatedProp = foundRelatedPropName
        [Test]
        public void Test_GetRelatedPropName_OwnerHasProp_RelatedHasProp_ShouldReturnFoundRelName()
        {
            //---------------Set up test pack-------------------
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
            
            ownerType.SetHasProperty(owningFKPropName, true);
            relatedType.SetHasProperty(relatedFKPropName, true);
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsTrue(ownerType.HasProperty(owningFKPropName));
            Assert.IsTrue(relatedType.HasProperty(relatedFKPropName));
//            AssertMockPropSetupCorrectly(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            
            //---------------Execute Test ----------------------
            var relatedPropName = autoMapper.GetRelatedPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual(relatedFKPropName, relatedPropName);
        }
        ///if ownerClassName LT relatedClassName
        ///     ownerProp = OwnerClassId
        ///     relatedProp = reverseRelationshipName+ID        
        [Test]
        public void Test_GetRelatedPropName_NeitherHaveProps_ShouldReturnRevRelNameID()
        {
            //---------------Set up test pack-------------------
            var ownerType = MockRepository.GenerateMock<FakeTypeWrapper>();
            var relatedType = MockRepository.GenerateMock<FakeTypeWrapper>();
            var reverseRelName = GetRandomString();
            var relationshipName = GetRandomString();
            string owningFKPropName = relationshipName + "ID";
            string relatedFKPropName = reverseRelName + "ID";
            PropertyWrapper propertyWrapper = MockRepository.GenerateStub<FakePropertyWrapper>();
            SetupMockPropWrapper(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            ownerType.SetName("1111");
            relatedType.SetName("2222");
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(ownerType.HasProperty(owningFKPropName));
            Assert.IsFalse(relatedType.HasProperty(relatedFKPropName));
            Assert.AreEqual(-1, ownerType.Name.CompareTo(relatedType.Name));
            AssertMockPropSetupCorrectly(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            
            //---------------Execute Test ----------------------
            var relatedPropName = autoMapper.GetRelatedPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual(relatedFKPropName, relatedPropName);
        }
        ///if ownerClassName GT relatedClassName
        ///    ownerProp = RelationshipName+ID
        ///    relatedProp = RelatedClass.ID    
        [Test]
        public void Test_GetRelatedPropName_WhenNeitherHaveProps_WhenOwnerGTRelated_ShouldReturnRevRelNameID()
        {
            //---------------Set up test pack-------------------
            var ownerType = MockRepository.GenerateMock<FakeTypeWrapper>();
            var relatedType = MockRepository.GenerateMock<FakeTypeWrapper>();
            relatedType.SetPKPropName(GetRandomString());
            var reverseRelName = GetRandomString();
            var relationshipName = GetRandomString();
            string owningFKPropName = relationshipName + "ID";
            string relatedFKPropName = reverseRelName + "ID";
            PropertyWrapper propertyWrapper = MockRepository.GenerateStub<FakePropertyWrapper>();
            SetupMockPropWrapper(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            ownerType.SetName("zzzzz");
            relatedType.SetName("aaaa");
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(ownerType.HasProperty(owningFKPropName));
            Assert.IsFalse(relatedType.HasProperty(relatedFKPropName));
            Assert.AreEqual(1, ownerType.Name.CompareTo(relatedType.Name));
            AssertMockPropSetupCorrectly(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            
            //---------------Execute Test ----------------------
            var relatedPropName = autoMapper.GetRelatedPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual(relatedType.GetPKPropName(), relatedPropName);
        }

        ///if ownerClassName LT relatedClassName
        ///     ownerProp = OwnerClassId
        ///     relatedProp = reverseRelationshipName+ID        
        [Test]
        public void Test_GetRelatedPropName_NeitherHaveProps_NonDefaultNaming_ShouldReturnRevRelNameID()
        {
            //---------------Set up test pack-------------------
            SetNonDefaultNamingConvention();
            var ownerType = MockRepository.GenerateMock<FakeTypeWrapper>();
            var relatedType = MockRepository.GenerateMock<FakeTypeWrapper>();
            var reverseRelName = GetRandomString();
            var relationshipName = GetRandomString();
            var owningFKPropName = GetFKPropName(relationshipName);
            var relatedFKPropName = GetFKPropName(reverseRelName);
            PropertyWrapper propertyWrapper = MockRepository.GenerateStub<FakePropertyWrapper>();
            SetupMockPropWrapper(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            ownerType.SetName("1111");
            relatedType.SetName("2222");
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(ownerType.HasProperty(owningFKPropName));
            Assert.IsFalse(relatedType.HasProperty(relatedFKPropName));
            Assert.AreEqual(-1, ownerType.Name.CompareTo(relatedType.Name));
            AssertMockPropSetupCorrectly(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            //---------------Execute Test ----------------------
            var relatedPropName = autoMapper.GetRelatedPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual(relatedFKPropName, relatedPropName);
        }

        /// if ownerClass.HasProp(RelName+ID) and relatedClass.HasProp(RevRelName+ID)
        ///      ownerProp = foundOwnerClassId
        ///      relatedProp = foundRelatedPropName
        [Test]
        public void Test_GetRelatedPropName_OwnerHasProp_RelatedHasProp_FakeNameConvention_ShouldReturnFoundRelName()
        {
            //---------------Set up test pack-------------------
            SetNonDefaultNamingConvention();
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
            ownerType.SetHasProperty(owningFKPropName, true);
            relatedType.SetHasProperty(relatedFKPropName, true);

            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsTrue(ownerType.HasProperty(owningFKPropName));
            Assert.IsTrue(relatedType.HasProperty(relatedFKPropName));
//            AssertMockPropSetupCorrectly(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            //---------------Execute Test ----------------------
            var relatedPropName = autoMapper.GetRelatedPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual(relatedFKPropName, relatedPropName);
        }

        /// <summary>
        /// if ownerClass.HasProp(RelName+ID) and relatedClass.NotHasProp(RevRelName+ID) 
        ///      ownerProp = foundOwnerPropName
        ///      relatedProp = RelatedClass.ID.
        /// </summary>
        [Test]
        public void Test_GetOwningPropName_OwnerHasProp_RelatedNotHasProp_ShouldReturnRelatedTypePkName()
        {
            //---------------Set up test pack-------------------
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
            //SetupMockPropWrapper(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            ownerType.SetHasProperty(owningFKPropName, true);
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsTrue(ownerType.HasProperty(owningFKPropName));
            Assert.IsFalse(relatedType.HasProperty(relatedFKPropName));
            //Assert.AreEqual(reverseRelName, propertyWrapper.GetSingleReverseRelationshipName<AutoMapOneToOne>());
            Assert.AreSame(ownerType, propertyWrapper.DeclaringType);
            Assert.AreSame(relatedType, propertyWrapper.RelatedClassType);
//            Assert.AreEqual(relationshipName, propertyWrapper.Name);
            //---------------Execute Test ----------------------
            var relatedPropName = autoMapper.GetOwningPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual(owningFKPropName, relatedPropName);
        }
        /// if relatedClass.HasProp(reverseRel+ID) and ownerClass.NotHasProp(RelName+ID)
        ///      ownerProp = OwnerClass.Id
        ///      relatedProp = foundRelatedPropName
        [Test]
        public void Test_GetOwningPropName_OwnerNotHasProp_RelatedHasProp_ShouldReturnFoundRelName()
        {
            //---------------Set up test pack-------------------
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
            relatedType.SetHasProperty(relatedFKPropName, true);
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(ownerType.HasProperty(owningFKPropName));
            Assert.IsTrue(relatedType.HasProperty(relatedFKPropName));
//            AssertMockPropSetupCorrectly(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            //---------------Execute Test ----------------------
            var relatedPropName = autoMapper.GetOwningPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual(ownerType.GetPKPropName(), relatedPropName);
        }
        /// if relatedClass.HasProp(reverseRel+ID) and ownerClass.NotHasProp(RelName+ID)
        ///      ownerProp = OwnerClass.Id
        ///      relatedProp = foundRelatedPropName
        [Test]
        public void Test_GetOwningPropName_OwnerNotHasProp_RelatedHasProp_NonStandardNaming_ShouldReturnFoundRelName()
        {
            //---------------Set up test pack-------------------
            SetNonDefaultNamingConvention();
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
            relatedType.SetHasProperty(relatedFKPropName, true);
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(ownerType.HasProperty(owningFKPropName));
            Assert.IsTrue(relatedType.HasProperty(relatedFKPropName));
//            AssertMockPropSetupCorrectly(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            //---------------Execute Test ----------------------
            var relatedPropName = autoMapper.GetOwningPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual(ownerType.GetPKPropName(), relatedPropName);
        }
        /// if ownerClass.HasProp(RelName+ID) and relatedClass.HasProp(RevRelName+ID)
        ///      ownerProp = foundOwnerClassId
        ///      relatedProp = foundRelatedPropName
        [Test]
        public void Test_GetOwningPropName_OwnerHasProp_RelatedHasProp_ShouldReturnFoundRelName()
        {
            //---------------Set up test pack-------------------
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
//            SetupMockPropWrapper(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            ownerType.SetHasProperty(owningFKPropName, true);
            relatedType.SetHasProperty(relatedFKPropName, true);
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsTrue(ownerType.HasProperty(owningFKPropName));
            Assert.IsTrue(relatedType.HasProperty(relatedFKPropName));
//            AssertMockPropSetupCorrectly(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            //---------------Execute Test ----------------------
            var relatedPropName = autoMapper.GetOwningPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual(owningFKPropName, relatedPropName);
        }
        /// if ownerClass.HasProp(RelName+ID) and relatedClass.HasProp(RevRelName+ID)
        ///      ownerProp = foundOwnerClassId
        ///      relatedProp = foundRelatedPropName
        [Test]
        public void Test_GetOwningPropName_OwnerHasProp_RelatedHasProp_NonStandardNaming_ShouldReturnFoundRelName()
        {
            //---------------Set up test pack-------------------
            SetNonDefaultNamingConvention();
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);

            ownerType.SetHasProperty(owningFKPropName, true);
            relatedType.SetHasProperty(relatedFKPropName, true);
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsTrue(ownerType.HasProperty(owningFKPropName));
            Assert.IsTrue(relatedType.HasProperty(relatedFKPropName));
//            Assert.AreNotEqual(relationshipName, relatedType.Name);
//            AssertMockPropSetupCorrectly(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            
            //---------------Execute Test ----------------------
            var relatedPropName = autoMapper.GetOwningPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual(owningFKPropName, relatedPropName);
        }


        ///if ownerClassName LT relatedClassName
        ///     ownerProp = OwnerClass.Id
        ///     relatedProp = reverseRelationshipName+ID        
        [Test]
        public void Test_GetOwningPropName_NeitherHaveProps_ShouldReturnRevRelNameID()
        {
            //---------------Set up test pack-------------------
            var ownerType = MockRepository.GenerateMock<FakeTypeWrapper>();
            var relatedType = MockRepository.GenerateMock<FakeTypeWrapper>();
            var reverseRelName = GetRandomString();
            var relationshipName = GetRandomString();
            string owningFKPropName = relationshipName + "ID";
            string relatedFKPropName = reverseRelName + "ID";
            PropertyWrapper propertyWrapper = MockRepository.GenerateStub<FakePropertyWrapper>();
            SetupMockPropWrapper(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            ownerType.SetName("1111");
            relatedType.SetName("2222");
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(ownerType.HasProperty(owningFKPropName));
            Assert.IsFalse(relatedType.HasProperty(relatedFKPropName));
            Assert.AreEqual(-1, ownerType.Name.CompareTo(relatedType.Name));
            AssertMockPropSetupCorrectly(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            
            //---------------Execute Test ----------------------
            var relatedPropName = autoMapper.GetOwningPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual(ownerType.GetPKPropName(), relatedPropName);
        }
        ///if ownerClassName GT relatedClassName
        ///    ownerProp = RelationshipName+ID
        ///    relatedProp = RelatedClass.ID    
        [Test]
        public void Test_GetOwningPropName_WhenNeitherHaveProps_WhenOwnerGTRelated_ShouldReturnRevRelNameID()
        {
            //---------------Set up test pack-------------------
            var ownerType = MockRepository.GenerateMock<FakeTypeWrapper>();
            var relatedType = MockRepository.GenerateMock<FakeTypeWrapper>();
            var reverseRelName = GetRandomString();
            var relationshipName = GetRandomString();
            string owningFKPropName = relationshipName + "ID";
            string relatedFKPropName = reverseRelName + "ID";

            PropertyWrapper propertyWrapper = MockRepository.GenerateStub<FakePropertyWrapper>();
            SetupMockPropWrapper(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);

            ownerType.SetName("zzzzz");
            relatedType.SetName("aaaa");
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(ownerType.HasProperty(owningFKPropName));
            Assert.IsFalse(relatedType.HasProperty(relatedFKPropName));
            Assert.AreEqual(1, ownerType.Name.CompareTo(relatedType.Name));
            AssertMockPropSetupCorrectly(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            
            //---------------Execute Test ----------------------
            var relatedPropName = autoMapper.GetOwningPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual(owningFKPropName, relatedPropName);
        }
        [Test]
        public void Test_CreateRelPropDef_WithNonDefaultIDConventionName_ShouldCreateWithCorrectOwningAndRelatedPropNames()
        {
            //---------------Set up test pack-------------------
            SetNonDefaultNamingConvention();
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper 
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
            relatedType.SetHasProperty(relatedFKPropName, true);
            var autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(ownerType.HasProperty(owningFKPropName));
            Assert.IsTrue(relatedType.HasProperty(relatedFKPropName));
            Assert.IsNotNullOrEmpty(autoMapper.GetOwningPropName());
            Assert.IsNotNullOrEmpty(autoMapper.GetRelatedPropName());
            //---------------Execute Test ----------------------
            var relPropDef = autoMapper.CreateRelPropDef();
            //---------------Test Result -----------------------
            Assert.IsNotNull(relPropDef);
            Assert.AreEqual(autoMapper.GetOwningPropName(), relPropDef.OwnerPropertyName);
            Assert.AreEqual(autoMapper.GetRelatedPropName(), relPropDef.RelatedClassPropName);
        }
        [Test]
        public void Test_CreateRelPropDef_WithDefaultIDConventionName_ShouldCreateWithCorrectOwningAndRelatedPropNames()
        {
            //---------------Set up test pack-------------------
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper 
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
            ownerType.SetHasProperty(owningFKPropName, true);
            relatedType.SetHasProperty(relatedFKPropName, true);
            var autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsTrue(ownerType.HasProperty(owningFKPropName));
            Assert.IsTrue(relatedType.HasProperty(relatedFKPropName));
            Assert.IsNotNullOrEmpty(autoMapper.GetOwningPropName());
            Assert.IsNotNullOrEmpty(autoMapper.GetRelatedPropName());
            //---------------Execute Test ----------------------
            var relPropDef = autoMapper.CreateRelPropDef();
            //---------------Test Result -----------------------
            Assert.IsNotNull(relPropDef);
            Assert.AreEqual(autoMapper.GetOwningPropName(), relPropDef.OwnerPropertyName);
            Assert.AreEqual(autoMapper.GetRelatedPropName(), relPropDef.RelatedClassPropName);
        }

        private static PropertyWrapper GetPropertyWrapper(out TypeWrapper ownerType, out TypeWrapper relatedType, out string relatedFKPropName, out string owningFKPropName)
        {
            ownerType = GetMockTypeWrapper();
            relatedType = GetMockTypeWrapper();
            relatedType.Stub(wrapper => wrapper.UnderlyingType).Return(typeof (FakeBOWithSingleRel1));
            var reverseRelName = GetRandomString();
            var relationshipName = GetRandomString();
            owningFKPropName = GetFKPropName(relationshipName);
            relatedFKPropName = GetFKPropName(reverseRelName);

            PropertyWrapper propertyWrapper = MockRepository.GenerateMock<FakePropertyWrapper>();
            SetupMockPropWrapper(propertyWrapper, ownerType, relatedType, relationshipName, reverseRelName);
            return propertyWrapper;
        }

        private static FakeTypeWrapper GetMockTypeWrapper()
        {
            FakeTypeWrapper classType = MockRepository.GenerateMock<FakeTypeWrapper>();
            classType.SetName(GetRandomString());
            classType.SetPKPropName(GetRandomString());
            return classType;
        }

        private static void SetNonDefaultNamingConvention()
        {
            var namingConvention = new FakeConvetion();
            AllClassesAutoMapper.PropNamingConvention = namingConvention;
            return;
        }

        private static string GetFKPropName(string relationshipName)
        {
            return OneToOneAutoMapper.PropNamingConvention.GetSingleRelOwningPropName(relationshipName);
        }

        private static void AssertMockPropSetupCorrectly(PropertyWrapper propertyWrapper, TypeWrapper ownerType, TypeWrapper relatedType, string relationshipName, string reverseRelName)
        {
            Assert.AreEqual(reverseRelName, propertyWrapper.GetSingleReverseRelationshipName<AutoMapOneToOneAttribute>());
            Assert.AreSame(ownerType, propertyWrapper.DeclaringType);
            Assert.AreSame(relatedType, propertyWrapper.RelatedClassType);
            Assert.AreEqual(relationshipName, propertyWrapper.Name);
        }

        private static void SetupMockPropWrapper(PropertyWrapper propertyWrapper, TypeWrapper ownerType, TypeWrapper relatedType, string relationshipName, string reverseRelName)
        {
            propertyWrapper.SetName(relationshipName);
            propertyWrapper.SetDeclaringType(ownerType);
            propertyWrapper.SetOneToOneReverseRelName(reverseRelName);
            propertyWrapper.SetRelatedType(relatedType);
            propertyWrapper.Stub(wrapper => wrapper.HasSingleReverseRelationship).Return(true);
            propertyWrapper.Stub(wrapper1 => wrapper1.IsPublic).Return(true);

            propertyWrapper.Stub(wrapper => wrapper.PropertyInfo).Return(MockRepository.GenerateMock<FakePropertyInfo>());
        }


        private static string GetRandomString()
        {
            return RandomValueGenerator.GetRandomString();
        }

        #endregion

        [Test]
        public void Test_ReverseRelationshipName_ShouldBeSameAsPropReverseRelName()
        {
            //---------------Set up test pack-------------------
            PropertyWrapper propertyWrapper = MockRepository.GenerateMock<FakePropertyWrapper>();
            var expectedRevRelName = GetRandomString();
            propertyWrapper.SetOneToOneReverseRelName(expectedRevRelName);
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedRevRelName, propertyWrapper.GetSingleReverseRelationshipName<AutoMapOneToOneAttribute>());
            //---------------Execute Test ----------------------
            var revRelName = autoMapper.ReverseRelationshipName;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedRevRelName, revRelName);
        }

        [Test]
        public void Test_MustBeMapped_WhenPropIsInherited_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------

            var propertyWrapper = MockRepository.GenerateStub<FakePropertyWrapper>();
            propertyWrapper.Stub(wrapper => wrapper.IsPublic).Return(true);
            propertyWrapper.Stub(wrapper => wrapper.IsInherited).Return(true);
            propertyWrapper.Stub(wrapper => wrapper.IsSingleRelationhip).Return(true);
            propertyWrapper.Stub(wrapper => wrapper.PropertyInfo).Return(GetFakePropertyInfo());
            propertyWrapper.Stub(wrapper => wrapper.HasOneToOneAttribute).Return(true);
            propertyWrapper.Stub(wrapper => wrapper.DeclaringType).Return(GetFakeTypeWrapper());
            OneToOneAutoMapper mapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------

            Assert.IsFalse(propertyWrapper.IsStatic);
            Assert.IsTrue(propertyWrapper.IsPublic);
            Assert.IsFalse(propertyWrapper.HasManyToOneAttribute);
            Assert.IsTrue(propertyWrapper.IsSingleRelationhip);
            Assert.IsFalse(propertyWrapper.HasIgnoreAttribute);
            Assert.IsTrue(propertyWrapper.HasOneToOneAttribute);
            Assert.IsNotNull(propertyWrapper.PropertyInfo);
            Assert.IsNotNull(propertyWrapper.DeclaringType);

            Assert.IsTrue(propertyWrapper.IsInherited);
            //---------------Execute Test ----------------------
            var mustBeMapped = mapper.MustBeMapped();
            //---------------Test Result -----------------------
            Assert.IsFalse(mustBeMapped);
        }

        [Test]
        public void Test_Map_WhenNullPropInfo_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            PropertyWrapper info = null;
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var relationshipDefCol = info.MapOneToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(relationshipDefCol);
        }

        [Test]
        public void Test_Map_WhenNotSingleRel_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            PropertyWrapper propertyWrapper = MockRepository.GenerateStub<FakePropertyWrapper>();
            propertyWrapper.SetIsSingleRelationship(false);
            propertyWrapper.SetIgnoreAttribute(false);
            //---------------Assert Precondition----------------
            Assert.IsFalse(propertyWrapper.IsSingleRelationhip);
            Assert.IsFalse(propertyWrapper.HasIgnoreAttribute);
            //---------------Execute Test ----------------------
            var relationshipDef = propertyWrapper.MapOneToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(relationshipDef);
        }

        [Test]
        public void Test_Map_PropWithIgnoreAttribute_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            PropertyWrapper propertyWrapper = MockRepository.GenerateStub<FakePropertyWrapper>();
            propertyWrapper.SetIsSingleRelationship(false);
            propertyWrapper.SetIgnoreAttribute(true);
            //---------------Assert Precondition----------------
            Assert.IsFalse(propertyWrapper.IsSingleRelationhip);
            Assert.IsTrue(propertyWrapper.HasIgnoreAttribute);
            //---------------Execute Test ----------------------
            var relationshipDef = propertyWrapper.MapOneToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(relationshipDef);
        }

        /// <summary>
        /// ownerClass.HasProp(RelName+ID) and relatedClass.NotHasProp(RevRelName+ID) 
        /// </summary>
        [Test]
        public void Test_Map_WhenOwningBoHasFK_ShouldSetOwningBoHasFKTrue()
        {
            //---------------Set up test pack-------------------
            TypeWrapper ownerType;
            string owningFKPropName;
            PropertyWrapper propertyWrapper = GetPropertyWrapper(out ownerType, out owningFKPropName);

            ownerType.SetHasProperty(owningFKPropName, true);
            propertyWrapper.SetIsSingleRelationship(true);
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsTrue(autoMapper.OwningBoHasForeignKey);
            Assert.IsTrue(propertyWrapper.IsSingleRelationhip);
            //---------------Execute Test ----------------------
            var relationshipDef = autoMapper.MapOneToOne();
            //---------------Test Result -----------------------
            Assert.IsNotNull(relationshipDef);
            Assert.IsInstanceOf(typeof (SingleRelationshipDef), relationshipDef);
            Assert.IsTrue(relationshipDef.OwningBOHasForeignKey);
            Assert.AreEqual(DeleteParentAction.DoNothing, relationshipDef.DeleteParentAction);
        }


        /// <summary>
        /// if ownerClass.NotHasProp(RelName+ID) and relatedClass.HasProp(RevRelName+ID) 
        /// </summary>
        [Test]
        public void Test_Map_OwningBoHasFK_ShouldReturnRelatedTypePkName()
        {
            //---------------Set up test pack-------------------
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
            relatedType.SetHasProperty(relatedFKPropName, true);
            propertyWrapper.SetIsSingleRelationship(true);
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsFalse(autoMapper.OwningBoHasForeignKey);
            Assert.IsTrue(propertyWrapper.IsSingleRelationhip);
            Assert.IsTrue(propertyWrapper.IsPublic);
            Assert.IsFalse(propertyWrapper.IsInherited);
            Assert.IsTrue(autoMapper.MustBeMapped());
            //---------------Execute Test ----------------------
            var relationshipDef = autoMapper.MapOneToOne();
            //---------------Test Result -----------------------
            Assert.IsNotNull(relationshipDef);
            Assert.IsInstanceOf(typeof (SingleRelationshipDef), relationshipDef);
            Assert.IsFalse(relationshipDef.OwningBOHasForeignKey);
            Assert.AreEqual(DeleteParentAction.Prevent, relationshipDef.DeleteParentAction);
        }

        [Test]
        public void Test_Map_ShouldSetReverseRelationshipName()
        {
            //---------------Set up test pack-------------------
            PropertyWrapper propertyWrapper = GetPropertyWrapper(); 
            propertyWrapper.SetIsSingleRelationship(true);
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            var expectedReverseRelName = propertyWrapper.GetSingleReverseRelationshipName<AutoMapOneToOneAttribute>();
            //---------------Assert Precondition----------------
            Assert.IsTrue(propertyWrapper.IsSingleRelationhip);
            Assert.AreEqual(expectedReverseRelName, autoMapper.ReverseRelationshipName);
            //---------------Execute Test ----------------------
            var relationshipDef = autoMapper.MapOneToOne();
            //---------------Test Result -----------------------
            Assert.IsNotNull(relationshipDef);
            Assert.IsInstanceOf(typeof (SingleRelationshipDef), relationshipDef);
            Assert.AreEqual(expectedReverseRelName, relationshipDef.ReverseRelationshipName);
        }

        [Test]
        public void Test_Map_ShouldSetRelationshipAsOneToOne()
        {
            //---------------Set up test pack-------------------
            PropertyWrapper propertyWrapper = GetPropertyWrapper();
            propertyWrapper.SetIsSingleRelationship(true);

            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            //---------------Assert Precondition----------------
            Assert.IsTrue(propertyWrapper.IsSingleRelationhip);
            //---------------Execute Test ----------------------
            var relationshipDef = autoMapper.MapOneToOne();
            //---------------Test Result -----------------------
            Assert.IsNotNull(relationshipDef);
            Assert.IsTrue(relationshipDef.IsOneToOne);
        }

        private static PropertyWrapper GetPropertyWrapper()
        {
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            return GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
        }

        [Test]
        public void Test_Map_ShouldCreateRelProp_WithCorrectNames()
        {
            //---------------Set up test pack-------------------
            TypeWrapper ownerType; TypeWrapper relatedType;
            string relatedFKPropName; string owningFKPropName;
            var propertyWrapper
                    = GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
            propertyWrapper.SetIsSingleRelationship(true);
            OneToOneAutoMapper autoMapper = new OneToOneAutoMapper(propertyWrapper);
            var expectedReverseRelName = propertyWrapper.GetSingleReverseRelationshipName<AutoMapOneToOneAttribute>();
            //---------------Assert Precondition----------------
            Assert.IsTrue(propertyWrapper.IsSingleRelationhip);
            Assert.AreEqual(expectedReverseRelName, autoMapper.ReverseRelationshipName);
            //---------------Execute Test ----------------------
            var relationshipDef = autoMapper.MapOneToOne();
            //---------------Test Result -----------------------
            Assert.IsNotNull(relationshipDef);
            Assert.IsInstanceOf(typeof (SingleRelationshipDef), relationshipDef);
            Assert.AreEqual(1, relationshipDef.RelKeyDef.Count);
            var relPropDef = relationshipDef.RelKeyDef[autoMapper.GetOwningPropName()];
            Assert.AreEqual(autoMapper.GetRelatedPropName(), relPropDef.RelatedClassPropName);
        }

        [Test]
        public void TestAccept_Map_WhenRelationshipTypeDefinedAsAggregation_ShouldSetToAggregation()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithCompositionSingleRel);
            const string expectedPropName = "MySingleRelationship";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsSingleRelationship();
            var oneToOneAtt = propertyWrapper.GetAttribute<AutoMapOneToOneAttribute>();
            Assert.AreEqual(RelationshipType.Aggregation, oneToOneAtt.RelationshipType);
            //---------------Execute Test ----------------------
            var relationshipDef = propertyWrapper.MapOneToOne();
            //---------------Test Result -----------------------
            Assert.AreEqual(RelationshipType.Aggregation, relationshipDef.RelationshipType);
            Assert.AreEqual(DeleteParentAction.Prevent, relationshipDef.DeleteParentAction);
        }

        /// <summary>
        /// If relationship is Aggregation or Composition then DeleteParentAction should be set to Prevent.
        /// </summary>
        [Test]
        public void TestAccept_Map_WhenRelationshipTypeDefinedAsAggregation_ShouldSetDeleteActionPrevent()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeMergeableParent);
            const string expectedPropName = "FakeMergeableChild";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsSingleRelationship();
            var oneToOneAtt = propertyWrapper.GetAttribute<AutoMapOneToOneAttribute>();
            Assert.AreEqual(RelationshipType.Aggregation, oneToOneAtt.RelationshipType);
            //---------------Execute Test ----------------------
            var relationshipDef = propertyWrapper.MapOneToOne();
            //---------------Test Result -----------------------
            Assert.AreEqual(RelationshipType.Aggregation, relationshipDef.RelationshipType);
            Assert.AreEqual(DeleteParentAction.Prevent, relationshipDef.DeleteParentAction);
        }
        /// <summary>
        /// If defined as Association or reverse relationship defined as Composition or Aggregation then
        /// DeleteParentAction = DoNothing.
        /// </summary>
        [Test]
        public void TestAccept_Map_WhenRelationshipTypeDefinedAsAssociation_ShouldSetDeleteActionDoNothing()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeMergeableChild);
            const string expectedPropName = "FakeMergeableParentReverse";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsSingleRelationship();
            var oneToOneAtt = propertyWrapper.GetAttribute<AutoMapOneToOneAttribute>();
            Assert.AreEqual(RelationshipType.Association, oneToOneAtt.RelationshipType);
            //---------------Execute Test ----------------------
            var relationshipDef = propertyWrapper.MapOneToOne();
            //---------------Test Result -----------------------
            Assert.AreEqual(RelationshipType.Association, relationshipDef.RelationshipType);
            Assert.AreEqual(DeleteParentAction.DoNothing, relationshipDef.DeleteParentAction);
        }
        /// <summary>
        ///If there is a single Relationship that does not 
        /// have an attribute and does not have a single reverse rel
        /// then it is assumed to be ManyToOne.
        /// </summary>
        [Test]
        public void TestAccept_Map_WhenNoReverseRelNoAttribute_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithSingleRel);
            const string expectedPropName = "MySingleRelationship";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsSingleRelationship();
            //---------------Execute Test ----------------------
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            var relationshipDef = propertyWrapper.MapOneToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(relationshipDef);
        }

        [Test]
        public void TestAccept_Map_WhenRelTypeCompos_AndRelPropsDefined_ShouldSetDeleteActionPrevent()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeMergeableParent);
            const string expectedPropName = "FakeMergeableChildNoTypeRelatedFK";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsSingleRelationship();
            var oneToOneAtt = propertyWrapper.GetAttribute<AutoMapOneToOneAttribute>();
            Assert.AreEqual(RelationshipType.Composition, oneToOneAtt.RelationshipType);
            //---------------Execute Test ----------------------
            var relationshipDef = propertyWrapper.MapOneToOne();
            //---------------Test Result -----------------------
            Assert.AreEqual(RelationshipType.Composition, relationshipDef.RelationshipType);
            Assert.AreEqual(DeleteParentAction.Prevent, relationshipDef.DeleteParentAction);
        }
        [Test]
        public void TestAccept_Map_WhenRevRelTypeCompos_AndRelPropsDefined_ShouldSetDeleteActionDoNothing()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeMergeableChild);
            const string expectedPropName = "FakeMergeableParentReverseFKDefined";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsSingleRelationship();
            var oneToOneAtt = propertyWrapper.GetAttribute<AutoMapOneToOneAttribute>();
            Assert.IsNull(oneToOneAtt);
            //---------------Execute Test ----------------------
            var relationshipDef = propertyWrapper.MapOneToOne();
            //---------------Test Result -----------------------
            Assert.AreEqual(RelationshipType.Association, relationshipDef.RelationshipType);
            Assert.AreEqual(DeleteParentAction.DoNothing, relationshipDef.DeleteParentAction);
        }

        [Test]
        public void TestAccept_Map_WhenRelationshipTypeNotDefined_ButRevRelComposition_ShouldSetDeleteActionDoNothing()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeMergeableChild);
            const string expectedPropName = "FakeMergeableParentReverseNoType";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsSingleRelationship();
            var oneToOneAtt = propertyWrapper.GetAttribute<AutoMapOneToOneAttribute>();
            Assert.IsNull(oneToOneAtt);
            //---------------Execute Test ----------------------
            var relationshipDef = propertyWrapper.MapOneToOne();
            //---------------Test Result -----------------------
            Assert.AreEqual(RelationshipType.Association, relationshipDef.RelationshipType);
            Assert.AreEqual(DeleteParentAction.DoNothing, relationshipDef.DeleteParentAction);
        }
        [Test]
        public void TestAccept_Map_WhenMappedViaAttributesWithNoRevRel_WhouldSetReverseRelNameToMappedName()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithReverseSingle);
            const string expectedPropName = "MySingleWithAutoMapNoReverse";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            const string expectedMappedReverseRel = "NoRevRel";
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsSingleRelationship();
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.ToPropertyWrapper().MapOneToOne();
            //---------------Test Result -----------------------
            //Need to do reverse rel before can do this the RelatedProp should 
            Assert.AreEqual(expectedMappedReverseRel, relationshipDef.ReverseRelationshipName);
            Assert.AreEqual(1, relationshipDef.RelKeyDef.Count);
            var relPropDef = relationshipDef.RelKeyDef["MySingleWithAutoMapNoReverseID"];
            Assert.AreEqual("FakeBOWith11AttributeID", relPropDef.RelatedClassPropName);
        }

        [Test]
        public void TestAccept_Map_WhenMappedViaAttributes_ButTypeIsInterface_ShouldNotMap()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOWithReverseSingleToInterface);
            const string expectedPropName = "MySingleRelationship";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsSingleRelationship();
            //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.ToPropertyWrapper().MapOneToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(relationshipDef);   
        }
        [Test]
        public void TestAccept_Map_WhenHasTwoSingleReverseRel_WithNoAttributes_ShouldRaiseError()
        {
            //For the BusinessObject 'FakeBOWithUndefinableSingleRel'
            //If the Related Type (in this case 'FakeBOWithSingleAndMultipleRelToSameType')
            // for the Relatiosnhip 'MySingleRelationship'
            // has a Single Relationship back 'MySingleRevRel'
            // and has a Multiple Reverse Relationship 'MyMultipleRevRel'
            // and 'MySingleRelationship' does not have an 'AutoMapOneToOne' attribute
            // then it cannot be automapped and should raise an error.
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBoWithMultipleRel);
            var reverseClassType = typeof(FakeWithTwoSingleReverseRel);
            const string expectedPropName = "MySingleWithTwoSingleReverse";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);

            //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            
            var reversePropInfo = reverseClassType.GetProperty("MySingleRelationship1");
            Assert.IsNotNull(reversePropInfo);
            reversePropInfo.AssertIsOfType(classType);

            Assert.IsTrue(propertyInfo.ToPropertyWrapper().IsSingleRelationhip);
            Assert.IsFalse(propertyInfo.ToPropertyWrapper().HasIgnoreAttribute);

            Assert.AreEqual(2, propertyInfo.ToPropertyWrapper().GetSingleReverseRelPropInfos().Count);
            //---------------Execute Test ----------------------
            try
            {
                propertyInfo.ToPropertyWrapper().MapOneToOne();
                Assert.Fail("Expected to throw an InvalidDefinitionException");
            }
            //---------------Test Result -----------------------
            catch (InvalidDefinitionException ex)
            {
                StringAssert.Contains("The Relationship '" + expectedPropName
                                      + "' could not be automapped since there are multiple Single relationships on class '", ex.Message);
                StringAssert.Contains("that reference the BusinessObject Class '", ex.Message);
                StringAssert.Contains("Please map using ClassDef.XML or Attributes", ex.Message);
            }
        }

        [Test]
        public void TestAccept_Map_WhenIsStaticProperty_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBoWithStaticProperty);
            const string expectedPropName = "MySingleRelationship";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------           
            Assert.IsTrue(propertyWrapper.IsSingleRelationhip);
            Assert.IsTrue(propertyWrapper.IsStatic);
            //---------------Execute Test ----------------------
            var relationshipDefCol = propertyWrapper.MapOneToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(relationshipDefCol);
        }
        [Test]
        public void TestAccept_Map_WhenIsPrivateProperty_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBoWithPrivateProps);
            const string expectedPropName = "PrivateOneToOneRel";
            PropertyInfo propertyInfo = classType.GetProperty(expectedPropName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------           
            Assert.IsTrue(propertyWrapper.IsSingleRelationhip);
            Assert.IsFalse(propertyWrapper.IsPublic);
            //---------------Execute Test ----------------------
            var relationshipDefCol = propertyWrapper.MapOneToOne();
            //---------------Test Result -----------------------
            Assert.IsNull(relationshipDefCol);
        }

        [Test]
        public void Test_SetPropNamingConvention_ShouldSetConvention()
        {
            //---------------Set up test pack-------------------
            INamingConventions nameConvention = MockRepository.GenerateMock<INamingConventions>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            AllClassesAutoMapper.PropNamingConvention = nameConvention;
            //---------------Test Result -----------------------
            Assert.AreSame(nameConvention, OneToOneAutoMapper.PropNamingConvention);
        }

        [Test]
        public void Test_DefaultNameConventionIfNoneSet()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var nameConvention = OneToOneAutoMapper.PropNamingConvention;
            //---------------Test Result -----------------------
            Assert.IsInstanceOf(typeof(DefaultPropNamingConventions), nameConvention);
        }
        private TypeWrapper GetFakeTypeWrapper()
        {
            return MockRepository.GenerateMock<FakeTypeWrapper>();
        }

        private static PropertyWrapper GetPropertyWrapper(out TypeWrapper ownerType, out string owningFKPropName)
        {
            TypeWrapper relatedType;
            string relatedFKPropName;
            return GetPropertyWrapper(out ownerType, out relatedType, out relatedFKPropName, out owningFKPropName);
        }
        private FakePropertyInfo GetFakePropertyInfo()
        {
            return MockRepository.GenerateMock<FakePropertyInfo>();
        }
    }
}