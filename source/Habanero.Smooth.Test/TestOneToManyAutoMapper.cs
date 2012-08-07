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
using System.Reflection;
using Habanero.BO.Loaders;
using Habanero.Smooth.Test.ExtensionMethods;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.ReflectionWrappers;
using NUnit.Framework;
using Rhino.Mocks;
using Habanero.Smooth.Test;

namespace Habanero.Smooth.Test
{
	[TestFixture]
	public class TestOneToManyAutoMapper
	{
		// ReSharper disable InconsistentNaming
		[SetUp]
		public void Setup()
		{
			AllClassesAutoMapper.PropNamingConvention = new DefaultPropNamingConventions();
			AllClassesAutoMapper.ClassDefCol = null;
		}

		#region CreateRelPropDef

		[Test]
		public void Test_GetRelatedPropName_WhenRelatedClassHasAttributeDeclaredIdProp_ShouldUseDeclaredIDPropName()
		{
			//---------------Set up test pack-------------------
			SetFakeNameConvention();

			var classType = typeof(FakeBOAttributePKAndPKNaming);
			PropertyInfo propertyInfo = classType.GetProperty("MyMultipleRevRel");

			string expectedRelatedPropName = classType.ToTypeWrapper().GetPKPropName();
			//---------------Assert Precondition----------------
			Assert.AreEqual("PublicGuidProp", expectedRelatedPropName);
			//---------------Execute Test ----------------------
			OneToManyAutoMapper autoMapper = new OneToManyAutoMapper(propertyInfo);
			var relatedPropName = autoMapper.GetRelatedPropName();
			//---------------Test Result -----------------------
			Assert.AreEqual(expectedRelatedPropName, relatedPropName);
		}

		[Test]
		public void Test_GetRelatatedPropName_WhenStdNamingPropAndRelDeclaredProp_ShouldReturnDeclaredPropName()
		{
			//---------------Set up test pack-------------------
			var classType = typeof(FakeBOAttributePKAndPKNaming);
			const string expectedPropName = "MyMultipleRevRel2";
			PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
			PropertyWrapper propertyWrapper = propertyInfo.ToPropertyWrapper();
			OneToManyAutoMapper autoMapper = new OneToManyAutoMapper(propertyInfo);
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsMultipleRelationship();
			Assert.IsTrue(propertyWrapper.HasSingleReverseRelationship, "There is no reverse single rel");

			PropertyWrapper reverseRelPropInfo = propertyWrapper.GetSingleReverseRelPropInfos()[0];
			string expectedRelatedPropName = reverseRelPropInfo.Name + "ID";
			//---------------Execute Test ----------------------
			var relatedPropName = autoMapper.GetRelatedPropName();
			//---------------Test Result -----------------------
			Assert.AreEqual(expectedRelatedPropName, relatedPropName);
		}
		[Test]
		public void Test_GetRelatatedPropName_WhenNoRevNonDefaultNaming_ShouldReturnNamingProp()
		{
			//---------------Set up test pack-------------------
			SetFakeNameConvention();

			var classType = typeof(FakeBoWithMultipleRel);
			const string expectedPropName = "MyMultipleRevRel";
			PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
			OneToManyAutoMapper autoMapper = new OneToManyAutoMapper(propertyInfo);
			//---------------Assert Precondition----------------
			Assert.AreEqual(0, propertyInfo.ToPropertyWrapper().GetSingleReverseRelPropInfos().Count);
			//---------------Execute Test ----------------------

			var relatedPropName = autoMapper.GetRelatedPropName();
			//---------------Test Result -----------------------
			Assert.AreEqual(ClassAutoMapper.PropNamingConvention.GetIDPropertyName(classType.ToTypeWrapper()), relatedPropName);
		}

		[Test]
		public void Test_GetOwningPropName_ShouldReturnClassTypeID()
		{
			//---------------Set up test pack-------------------
			var classType = typeof(FakeBoWithMultipleRel);
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			string owningPropName = OneToManyAutoMapper.GetOwningPropName(classType.ToTypeWrapper());
			//---------------Test Result -----------------------
			Assert.AreEqual(classType.ToTypeWrapper().GetPKPropName(), owningPropName);
		}
		[Test]
		public void Test_GetOwningPropName_WhenAutoMapPK_ShouldReturnClassTypeID()
		{
			//---------------Set up test pack-------------------
			var classType = typeof(FakeBOAttributePKAndPKNaming);
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			string owningPropName = OneToManyAutoMapper.GetOwningPropName(classType.ToTypeWrapper());
			//---------------Test Result -----------------------
			Assert.AreEqual(classType.ToTypeWrapper().GetPKPropName(), owningPropName);
		}

		[Test]
		public void Test_CreateRelPropDef_WhenNoReverseRel_WithDefaultIDConventionName()
		{
			//---------------Set up test pack-------------------
			var classType = typeof(FakeBoWithMultipleRel);
			const string expectedPropName = "MyMultipleRevRel";
			var propWrapper = classType.GetPropertyWrapper(expectedPropName);
			OneToManyAutoMapper autoMapper = new OneToManyAutoMapper(propWrapper);
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propWrapper.AssertIsMultipleRelationship();
			Assert.IsFalse(propWrapper.HasSingleReverseRelationship, "There is no reverse single rel");
			//---------------Execute Test ----------------------
			var relPropDef = autoMapper.CreateRelPropDef();
			//---------------Test Result -----------------------
			const string expectedOwnerPropName = "FakeBoWithMultipleRel" + "ID";
			Assert.AreEqual(expectedOwnerPropName, relPropDef.OwnerPropertyName);

			//Need to do reverse rel before can do this the RelatedProp should 
			const string expectedRelatedPropName = expectedOwnerPropName;
			Assert.AreEqual(expectedRelatedPropName, relPropDef.RelatedClassPropName);
		}

		[Test]
		public void Test_CreateRelPropDef__WhenNonDefaultNameConventionSet_WhenNoReverseRel_ShouldCreateRelPropWithConventionName()
		{
			//---------------Set up test pack-------------------
			INamingConventions nameConvention = SetFakeNameConvention();

			var classType = typeof(FakeBoWithMultipleRel);
			const string expectedPropName = "MyMultipleRevRel";
			var propWrapper = classType.GetPropertyWrapper(expectedPropName);
			OneToManyAutoMapper autoMapper = new OneToManyAutoMapper(propWrapper);
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propWrapper.AssertIsMultipleRelationship();
			Assert.IsFalse(propWrapper.HasSingleReverseRelationship, "There is no reverse single rel");
			//---------------Execute Test ----------------------
			var relPropDef = autoMapper.CreateRelPropDef();
			//---------------Test Result -----------------------
			string expectedOwnerPropName = nameConvention.GetIDPropertyName(classType.ToTypeWrapper());
			Assert.AreEqual(expectedOwnerPropName, relPropDef.OwnerPropertyName);

			//Need to do reverse rel before can do this the RelatedProp should 
			string expectedRelatedPropName = expectedOwnerPropName;
			Assert.AreEqual(expectedRelatedPropName, relPropDef.RelatedClassPropName);
		}

		private static INamingConventions SetFakeNameConvention()
		{
			INamingConventions nameConvention = new FakeConvetion();
			AllClassesAutoMapper.PropNamingConvention = nameConvention;
			return nameConvention;
		}

		[Test]
		public void Test_CreateRelPropDef_WhenAutoMapPrimaryKey_WhenNoReverse_ShouldCreateRelProp_WithPrimaryKeyName()
		{
			//---------------Set up test pack-------------------
			SetFakeNameConvention();

			var classType = typeof(FakeBOAttributePKAndPKNaming);
			const string expectedPropName = "MyMultipleRevRel";
			var propertyInfo = classType.GetPropertyWrapper(expectedPropName);

			string expectedOwnerPropName = classType.ToTypeWrapper().GetPKPropName();
			string expectedRelatedPropName = expectedOwnerPropName;
			OneToManyAutoMapper autoMapper = new OneToManyAutoMapper(propertyInfo);
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsMultipleRelationship();
			Assert.IsFalse(propertyInfo.HasSingleReverseRelationship, "There is no reverse single rel");
			//---------------Execute Test ----------------------
			var relPropDef = autoMapper.CreateRelPropDef();
			//---------------Test Result -----------------------
			Assert.AreEqual(expectedOwnerPropName, relPropDef.OwnerPropertyName);

			Assert.AreEqual(expectedRelatedPropName, relPropDef.RelatedClassPropName);
		}

		[Test]
		public void Test_CreateRelPropDef_WhenHasReverse_ShouldCreateRelProp_WithReverseRelNameID()
		{
			//---------------Set up test pack-------------------
			var classType = typeof(FakeBOAttributePKAndPKNaming);
			const string expectedPropName = "MyMultipleRevRel2";
			var propertyInfo = classType.GetPropertyWrapper(expectedPropName);
			OneToManyAutoMapper autoMapper = new OneToManyAutoMapper(propertyInfo);
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsMultipleRelationship();
			Assert.IsTrue(propertyInfo.HasSingleReverseRelationship, "There is no reverse single rel");

			PropertyWrapper reverseRelPropInfo = propertyInfo.GetSingleReverseRelPropInfos()[0];
			string expectedOwnerPropName = classType.ToTypeWrapper().GetPKPropName();
			string expectedRelatedPropName = reverseRelPropInfo.Name + "ID";
			//---------------Execute Test ----------------------
			var relPropDef = autoMapper.CreateRelPropDef();
			//---------------Test Result -----------------------
			Assert.AreEqual(expectedOwnerPropName, relPropDef.OwnerPropertyName);

			//Need to do reverse rel before can do this the RelatedProp should 
			Assert.AreEqual(expectedRelatedPropName, relPropDef.RelatedClassPropName);
		}
		[Test]
		public void Test_CreateRelPropDef_WhenHasReverse_ShouldCreateRelProp_WithNonStdNaming_WithNonStdReverseRelName()
		{
			//---------------Set up test pack-------------------
			var nameConvention = SetFakeNameConvention();
			var classType = typeof(FakeBOAttributePKAndPKNaming);
			const string expectedPropName = "MyMultipleRevRel2";
			var propertyInfo = classType.GetPropertyWrapper(expectedPropName);
			OneToManyAutoMapper autoMapper = new OneToManyAutoMapper(propertyInfo);
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsMultipleRelationship();
			Assert.IsTrue(propertyInfo.HasSingleReverseRelationship, "There is no reverse single rel");

			PropertyWrapper reverseRelPropInfo = propertyInfo.GetSingleReverseRelPropInfos()[0];
			string expectedOwnerPropName = classType.ToTypeWrapper().GetPKPropName();
			//---------------Execute Test ----------------------
			var relPropDef = autoMapper.CreateRelPropDef();
			//---------------Test Result -----------------------
			Assert.AreEqual(expectedOwnerPropName, relPropDef.OwnerPropertyName);

			//Need to do reverse rel before can do this the RelatedProp should 
			string expectedRelatedPropName = nameConvention.GetSingleRelOwningPropName(reverseRelPropInfo.Name);
			Assert.AreEqual(expectedRelatedPropName, relPropDef.RelatedClassPropName);
		}

		#endregion

		[Test]
		public void Test_Construct_WithNullPropInfo_ShouldRaiseError()
		{
			//---------------Set up test pack-------------------
			PropertyInfo propertyInfo = null;
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			try
			{
				new OneToManyAutoMapper(propertyInfo);
				Assert.Fail("expected ArgumentNullException");
			}
				//---------------Test Result -----------------------
			catch (ArgumentNullException ex)
			{
				StringAssert.Contains("Value cannot be null", ex.Message);
				StringAssert.Contains("propInfo", ex.ParamName);
			}
		}

		[Test]
		public void Test_Map_WhenNullPropInfo_ShouldReturnNull()
		{
			//---------------Set up test pack-------------------
			PropertyInfo info = null;
			//---------------Assert Precondition----------------
			//---------------Execute Test ----------------------
			var relationshipDefCol = info.MapOneToMany();
			//---------------Test Result -----------------------
			Assert.IsNull(relationshipDefCol);
		}

		[Test]
		public void Test_Map_WhenNotRelToIBoCol_ShouldReturnNull()
		{
			//---------------Set up test pack-------------------
			var classType = typeof(FakeBOWProps);
			const string expectedPropName = "PublicGetGuidProp";
			var propertyInfo = classType.GetProperty(expectedPropName);
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsNotOfType<IBusinessObjectCollection>();
			//---------------Execute Test ----------------------
			var relationshipDef = propertyInfo.MapOneToMany();
			//---------------Test Result -----------------------
			Assert.IsNull(relationshipDef);
		}

		[Test]
		public void Test_Map_PropWithIgnoreAttribute_ShouldReturnNull()
		{
			//---------------Set up test pack-------------------
			var classType = typeof(FakeBoWithMultipleRel);
			const string expectedPropName = "MultipleRevIgnores";
			var propertyInfo = classType.GetProperty(expectedPropName);
			var propertyWrapper = propertyInfo.ToPropertyWrapper();
			//---------------Assert Precondition----------------
			Assert.IsNotNull(propertyInfo);
			Assert.IsTrue(propertyWrapper.HasIgnoreAttribute);
			//---------------Execute Test ----------------------
			var relationshipDef = propertyInfo.MapOneToMany();
			//---------------Test Result -----------------------
			Assert.IsNull(relationshipDef);
		}
		[Test]
		public void Test_MapWithPropWrapper_PropWithIgnoreAttribute_ShouldReturnNull()
		{
			//---------------Set up test pack-------------------
			var classType = typeof(FakeBoWithMultipleRel);
			const string expectedPropName = "MultipleRevIgnores";
			PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
			var propertyWrapper = propertyInfo.ToPropertyWrapper();
			//---------------Assert Precondition----------------
			Assert.IsNotNull(propertyInfo);
			Assert.IsTrue(propertyWrapper.HasIgnoreAttribute);
			//---------------Execute Test ----------------------
			var relationshipDef = propertyWrapper.MapOneToMany();
			//---------------Test Result -----------------------
			Assert.IsNull(relationshipDef);
		}

		[Test]
		public void Test_MustBeMapped_WhenPropIsInherited_ShouldRetFalse()
		{
			//---------------Set up test pack-------------------

			var propertyWrapper = MockRepository.GenerateStub<FakePropertyWrapper>();
			propertyWrapper.Stub(wrapper => wrapper.IsPublic).Return(true);
			propertyWrapper.Stub(wrapper => wrapper.IsInherited).Return(true);
			propertyWrapper.Stub(wrapper => wrapper.IsMultipleRelationship).Return(true);
//            propertyWrapper.Stub(wrapper => wrapper.PropertyInfo).Return(GetFakePropertyInfo());
			propertyWrapper.Stub(wrapper => wrapper.DeclaringType).Return(GetFakeTypeWrapper());
			var mapper = new OneToManyAutoMapper(propertyWrapper);
			//---------------Assert Precondition----------------

			Assert.IsFalse(propertyWrapper.IsStatic);
			Assert.IsTrue(propertyWrapper.IsPublic);
			Assert.IsTrue(propertyWrapper.IsMultipleRelationship);
			Assert.IsFalse(propertyWrapper.HasIgnoreAttribute);
//            Assert.IsTrue(propertyWrapper.HasOneToOneAttribute);
//            Assert.IsNotNull(propertyWrapper.PropertyInfo);
			Assert.IsNotNull(propertyWrapper.DeclaringType);

			Assert.IsTrue(propertyWrapper.IsInherited);
			//---------------Execute Test ----------------------
			var mustBeMapped = mapper.MustBeMapped();
			//---------------Test Result -----------------------
			Assert.IsFalse(mustBeMapped);
		}

		private TypeWrapper GetFakeTypeWrapper()
		{
			return MockRepository.GenerateMock<FakeTypeWrapper>();
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
			const string expectedPropName = "MyMultipleWithTwoSingleReverse";
			var propertyInfo = classType.GetProperty(expectedPropName);

			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsMultipleRelationship();
			//Assert
			var reversePropInfo = reverseClassType.GetProperty("MySingleRelationship1");
			Assert.IsNotNull(reversePropInfo);
			reversePropInfo.AssertIsOfType(classType);

			Assert.AreEqual(2, propertyInfo.ToPropertyWrapper().GetSingleReverseRelPropInfos().Count);
			//---------------Execute Test ----------------------
			try
			{
				propertyInfo.MapOneToMany();
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
		public void Test_Map_ShouldSetDeleteActionPrevent_OwningBOHasFKFalse()
		{
			//---------------Set up test pack-------------------
			var classType = typeof(FakeBoWithMultipleRel);
			const string expectedPropName = "MyMultipleRevRel";
			var propertyInfo = classType.GetProperty(expectedPropName);
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsMultipleRelationship();
			//---------------Execute Test ----------------------
			var relationshipDef = propertyInfo.MapOneToMany();
			//---------------Test Result -----------------------
			Assert.IsFalse(relationshipDef.OwningBOHasForeignKey);
			Assert.AreEqual(DeleteParentAction.Prevent, relationshipDef.DeleteParentAction);
		}

		[Test]
		public void Test_Map_WhenIsMultipleRel_BoCol_NotReverse_ShouldReturnRelDefWithConventionRevName()
		{
			//---------------Set up test pack-------------------
			var classType = typeof(FakeBoWithMultipleRel);
			const string expectedPropName = "MyMultipleRevRel";
			PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsMultipleRelationship();
			Assert.AreEqual(0, propertyInfo.ToPropertyWrapper().GetSingleReverseRelPropInfos().Count);
			//---------------Execute Test ----------------------
			var relationshipDef = propertyInfo.MapOneToMany();
			//---------------Test Result -----------------------
			Assert.IsNotNull(relationshipDef);
			Assert.IsInstanceOf(typeof(MultipleRelationshipDef), relationshipDef);
			Assert.AreEqual(expectedPropName, relationshipDef.RelationshipName);
			Assert.AreEqual(typeof(FakeBOWithUndefinableSingleRel).FullName, relationshipDef.RelatedObjectClassName);
			propertyInfo.PropertyType.AssemblyQualifiedName.StartsWith(relationshipDef.RelatedObjectAssemblyName);
			Assert.AreEqual(DeleteParentAction.Prevent, relationshipDef.DeleteParentAction);
			Assert.IsNotNull(relationshipDef.RelKeyDef);
			Assert.AreEqual(classType.Name, relationshipDef.ReverseRelationshipName);
		}

		[Test]
		public void Test_Map_WhenIs_IBoCol_ShouldReturnRelDefWitConventionForRevName()
		{
			//If the relationship is defined as a IBOCol instead of a 
			// BusinessObjectCollection<Person> then should still detect as
			// a multiple relationship. Should use Heuristic for determining
			// related class i.e. (Singularise FakeBOWithUndefinableSingleRels to obtain
			// relatedClassName).
			//---------------Set up test pack-------------------
			var classType = typeof(FakeBoWithMultipleRel);
			const string expectedPropName = "FakeBOWithUndefinableSingleRels";
			PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsMultipleRelationship();
			//---------------Execute Test ----------------------
			var relationshipDef = propertyInfo.MapOneToMany();
			//---------------Test Result -----------------------
			Assert.IsNotNull(relationshipDef);
			Assert.IsInstanceOf(typeof(MultipleRelationshipDef), relationshipDef);
			Assert.AreEqual(expectedPropName, relationshipDef.RelationshipName);
			Assert.AreEqual(typeof(FakeBOWithUndefinableSingleRel).Name, relationshipDef.RelatedObjectClassName);
			propertyInfo.PropertyType.AssemblyQualifiedName.StartsWith(relationshipDef.RelatedObjectAssemblyName);
			Assert.AreEqual(DeleteParentAction.Prevent, relationshipDef.DeleteParentAction);
			Assert.IsNotNull(relationshipDef.RelKeyDef);
			Assert.AreEqual(classType.Name, relationshipDef.ReverseRelationshipName);
		}

		[Test]
		public void Test_Map_WhenHasSingleReverseRel_ShouldReturnRelDefWithRevRelNameSet()
		{
			//---------------Set up test pack-------------------
			var classType = typeof(FakeBOAttributePKAndPKNaming);
			const string expectedPropName = "MyMultipleRevRel2";
			PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsMultipleRelationship();
			Assert.IsTrue(propertyInfo.ToPropertyWrapper().HasSingleReverseRelationship, "There is no reverse single rel");

			PropertyWrapper reverseRelPropInfo = propertyInfo.ToPropertyWrapper().GetSingleReverseRelPropInfos()[0];
			Assert.AreNotEqual(classType.Name, reverseRelPropInfo.Name);
			//---------------Execute Test ----------------------
			var relationshipDef = propertyInfo.MapOneToMany();
			//---------------Test Result -----------------------
			//Need to do reverse rel before can do this the RelatedProp should 
			Assert.IsNotNull(relationshipDef.RelKeyDef);
			Assert.AreEqual(reverseRelPropInfo.Name, relationshipDef.ReverseRelationshipName);
		}

		[Test]
		public void Test_Map_WhenMappedToRelViaAttributes_WhouldSetReverseRelName()
		{
			//---------------Set up test pack-------------------
			var classType = typeof(FakeBoWithMultipleRel);
			const string expectedPropName = "MyMultipleAutoMapWithTwoSingleReverse";
			PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
			const string expectedMappedReverseRel = "MySingleRelationship2";
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsMultipleRelationship();
			var singleRevRels = propertyInfo.ToPropertyWrapper().GetManyToOneReverseRelationshipInfos();
			Assert.AreEqual(1, singleRevRels.Count);
			singleRevRels.ShouldContain(info => info.Name == expectedMappedReverseRel);
			//---------------Execute Test ----------------------
			var relationshipDef = propertyInfo.MapOneToMany();
			//---------------Test Result -----------------------
			//Need to do reverse rel before can do this the RelatedProp should 
			Assert.IsNotNull(relationshipDef.RelKeyDef);
			Assert.AreEqual(expectedMappedReverseRel, relationshipDef.ReverseRelationshipName);
		}

	    [Test]
	    public void Map_WhenHasOneToManyAssociation_ShouldMapPreventDelete()
	    {
	        //---------------Set up test pack-------------------
	        //FakeBoWithOneToManyAssociation

            var classType = typeof(FakeBoWithOneToManyAssociation);
            const string expectedPropName = "MultipleRel";
            var propertyInfo = classType.GetProperty(expectedPropName);
	        //---------------Assert Precondition----------------
            classType.AssertPropertyExists(expectedPropName);
            propertyInfo.AssertIsMultipleRelationship();
	        //---------------Execute Test ----------------------
            var relationshipDef = propertyInfo.MapOneToMany();
	        //---------------Test Result -----------------------
	        Assert.IsNotNull(relationshipDef);
	        Assert.AreEqual(DeleteParentAction.Prevent, relationshipDef.DeleteParentAction);
	    }

	    [Test]
	    public void GetRelationshipAttributeDef_WhenOneToManyAssociation_ShouldSetDeleteParentActionPrevent()
	    {
	        //---------------Set up test pack-------------------
            var classType = typeof(FakeBoWithOneToManyAssociation);
            const string expectedPropName = "MultipleRel";
            var propertyInfo = classType.GetProperty(expectedPropName);
	        var propertyWrapper = propertyInfo.ToPropertyWrapper();
	        //---------------Assert Precondition----------------

	        //---------------Execute Test ----------------------
            var relationshipAttribute = propertyWrapper.GetAttribute<AutoMapOneToManyAttribute>();
	        //---------------Test Result -----------------------
            Assert.AreEqual(DeleteParentAction.Prevent, relationshipAttribute.DeleteParentAction);
	    }


		[Test]
		public void Test_Map_WhenMappedViaAttributesWithNoRevRel_WhouldSetReverseRelNameToMappedName()
		{
			//---------------Set up test pack-------------------
			var classType = typeof(FakeBoWithMultipleRel);
			const string expectedPropName = "MultipleMappedToNonExistentReverse";
			PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
			const string expectedMappedReverseRel = "NonExistentReverseRel";
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsMultipleRelationship();
			Assert.AreEqual(0, propertyInfo.ToPropertyWrapper().GetManyToOneReverseRelationshipInfos().Count);
			//---------------Execute Test ----------------------
			var relationshipDef = propertyInfo.MapOneToMany();
			//---------------Test Result -----------------------
			//Need to do reverse rel before can do this the RelatedProp should 
			Assert.IsNotNull(relationshipDef.RelKeyDef);
			Assert.AreEqual(expectedMappedReverseRel, relationshipDef.ReverseRelationshipName);
		}

		[Test]
		public void Test_Map_WhenIsMultiple_ShouldCreateRelProp_WhenNoReverseRel_WithDefaultConventionName()
		{
			//---------------Set up test pack-------------------
			var classType = typeof(FakeBoWithMultipleRel);
			const string expectedPropName = "MyMultipleRevRel";
			PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsMultipleRelationship();
			Assert.IsFalse(propertyInfo.ToPropertyWrapper().HasSingleReverseRelationship, "There is no reverse single rel");
			//---------------Execute Test ----------------------
			var relationshipDef = propertyInfo.MapOneToMany();
			//---------------Test Result -----------------------
			var relKeyDef = relationshipDef.RelKeyDef;
			Assert.IsNotNull(relKeyDef);
			Assert.AreEqual(1, relKeyDef.Count);
			const string expectedOwnerPropName = "FakeBoWithMultipleRel" + "ID";
			Assert.IsTrue(relKeyDef.Contains(expectedOwnerPropName),
						  "By Convention the RelationshipPropName on the single side of the 1-M Relationship Should be RelationshipName & ID");
			var relPropDef = relKeyDef[expectedOwnerPropName];
			Assert.AreEqual(expectedOwnerPropName, relPropDef.OwnerPropertyName);

			//Need to do reverse rel before can do this the RelatedProp should 
			const string expectedRelatedPropName = expectedOwnerPropName;
			Assert.AreEqual(expectedRelatedPropName, relPropDef.RelatedClassPropName);
		}

		[Test]
		public void Test_Map_WhenNonDefaultNameConventionSet_WhenNoReverseRel_ShouldCreateRelPropWithConventionName()
		{
			//---------------Set up test pack-------------------
			SetFakeNameConvention();
			var classType = typeof(FakeBoWithMultipleRel);
			const string expectedPropName = "MyMultipleRevRel";
			PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsMultipleRelationship();
			Assert.IsFalse(propertyInfo.ToPropertyWrapper().HasSingleReverseRelationship, "There is no reverse single rel");
			//---------------Execute Test ----------------------
			var relationshipDef = propertyInfo.MapOneToMany();
			//---------------Test Result -----------------------
			var relKeyDef = relationshipDef.RelKeyDef;
			Assert.IsNotNull(relKeyDef);
			Assert.AreEqual(1, relKeyDef.Count);
			string expectedOwnerPropName = classType.ToTypeWrapper().GetPKPropName();
			Assert.IsTrue(relKeyDef.Contains(expectedOwnerPropName),
						  "By Convention the RelationshipPropName on the single side of the 1-M Relationship Should be RelationshipName & ID");
			var relPropDef = relKeyDef[expectedOwnerPropName];
			Assert.AreEqual(expectedOwnerPropName, relPropDef.OwnerPropertyName);

			//Need to do reverse rel before can do this the RelatedProp should 
			string expectedRelatedPropName = expectedOwnerPropName;
			Assert.AreEqual(expectedRelatedPropName, relPropDef.RelatedClassPropName);
		}

		[Test]
		public void Test_Map_WhenNoRelationshipTypeDefined_ShouldSetToAssociation()
		{
			//---------------Set up test pack-------------------
			SetFakeNameConvention();
			var classType = typeof(FakeBoWithMultipleRel);
			const string expectedPropName = "MyMultipleRevRel";
			PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsMultipleRelationship();
			Assert.IsFalse(propertyInfo.ToPropertyWrapper().HasSingleReverseRelationship, "There is no reverse single rel");
			//---------------Execute Test ----------------------
			var relationshipDef = propertyInfo.MapOneToMany();
			//---------------Test Result -----------------------
			Assert.AreEqual(RelationshipType.Association, relationshipDef.RelationshipType);
		}
		[Test]
		public void Test_Map_WhenRelationshipTypeDefined_ShouldSetToComposition()
		{
			//---------------Set up test pack-------------------
			SetFakeNameConvention();
			var classType = typeof(FakeBoWithCompositionMultipleRel);
			const string expectedPropName = "MyMultipleRevRel";
			PropertyInfo propertyInfo = classType.GetProperty(expectedPropName);
   //         propertyInfo.SetCustomAttribute(new AutoMapOneToManyAttribute(RelationshipType.Composition));
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsMultipleRelationship();
			var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapOneToManyAttribute), true);
			var onToManyAtt = (AutoMapOneToManyAttribute)customAttributes[0];
			Assert.AreEqual(RelationshipType.Composition, onToManyAtt.RelationshipType);
			//---------------Execute Test ----------------------
			var relationshipDef = propertyInfo.MapOneToMany();
			//---------------Test Result -----------------------
			Assert.AreEqual(RelationshipType.Composition, relationshipDef.RelationshipType);
		}		
        
        [Test]
		public void Test_Map_WhenDeleteParentActionDefined_ShouldSetToDeleteRelated()
		{
			//---------------Set up test pack-------------------
			SetFakeNameConvention();
            var classType = typeof(FakeBoWithDeleteParentActionDeleteRelatedRel);
			const string expectedPropName = "MyMultipleRevRel";
			var propertyInfo = classType.GetProperty(expectedPropName);
   //         propertyInfo.SetCustomAttribute(new AutoMapOneToManyAttribute(RelationshipType.Composition));
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsMultipleRelationship();
			var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapOneToManyAttribute), true);
			var onToManyAtt = (AutoMapOneToManyAttribute)customAttributes[0];
            Assert.AreEqual(DeleteParentAction.DeleteRelated, onToManyAtt.DeleteParentAction);
			//---------------Execute Test ----------------------
			var relationshipDef = propertyInfo.MapOneToMany();
			//---------------Test Result -----------------------
			Assert.AreEqual(DeleteParentAction.DeleteRelated, relationshipDef.DeleteParentAction);
		}   
     
        [Test]
		public void Test_Map_WhenNoDeleteParentActionDefined_ShouldSetToPrevent()
		{
			//---------------Set up test pack-------------------
			SetFakeNameConvention();
            var classType = typeof(FakeBoWithMultipleRel);
			const string expectedPropName = "MyMultipleRevRel";
			var propertyInfo = classType.GetProperty(expectedPropName);
   //         propertyInfo.SetCustomAttribute(new AutoMapOneToManyAttribute(RelationshipType.Composition));
			//---------------Assert Precondition----------------
			classType.AssertPropertyExists(expectedPropName);
			propertyInfo.AssertIsMultipleRelationship();
			var customAttributes = propertyInfo.GetCustomAttributes(typeof(AutoMapOneToManyAttribute), true);
			//---------------Execute Test ----------------------
			var relationshipDef = propertyInfo.MapOneToMany();
			//---------------Test Result -----------------------
			Assert.AreEqual(DeleteParentAction.Prevent, relationshipDef.DeleteParentAction);
		}

		[Test]
		public void TestAccept_Map_WhenIsStaticProperty_ShouldReturnNull()
		{
			//---------------Set up test pack-------------------
			var classType = typeof(FakeBoWithStaticProperty);
			const string expectedPropName = "MyMultiple";
			var propertyInfo = classType.GetProperty(expectedPropName);
			var propertyWrapper = propertyInfo.ToPropertyWrapper();
			//---------------Assert Precondition----------------
			Assert.IsTrue(propertyWrapper.IsMultipleRelationship);
			Assert.IsTrue(propertyWrapper.IsStatic);
			//---------------Execute Test ----------------------
			var relationshipDefCol = propertyWrapper.MapOneToMany();
			//---------------Test Result -----------------------
			Assert.IsNull(relationshipDefCol);
		}

		[Test]
		public void TestAccept_Map_WhenIsPrivateProperty_ShouldReturnNull()
		{
			//---------------Set up test pack-------------------
			var classType = typeof(FakeBoWithPrivateProps);
			const string expectedPropName = "PrivateMultipleRel";
			var propertyInfo = classType.GetProperty(expectedPropName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
			var propertyWrapper = propertyInfo.ToPropertyWrapper();
			//---------------Assert Precondition----------------
			Assert.IsTrue(propertyWrapper.IsMultipleRelationship);
			Assert.IsFalse(propertyWrapper.IsPublic);
			//---------------Execute Test ----------------------
			var relationshipDefCol = propertyWrapper.MapOneToMany();
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
			Assert.AreSame(nameConvention, OneToManyAutoMapper.PropNamingConvention);
		}

		[Test]
		public void Test_DefaultNameConventionIfNoneSet()
		{
			//---------------Set up test pack-------------------
			//---------------Assert Precondition----------------
			//---------------Execute Test ----------------------
			var nameConvention = OneToManyAutoMapper.PropNamingConvention;
			//---------------Test Result -----------------------
			Assert.IsInstanceOf(typeof(DefaultPropNamingConventions), nameConvention);
		}

	    [Ignore("Working on this test")] //TODO Brett 06 Jun 2011: Ignored Test - working on this test
		[Test]
		public void TestAutoMap_WithTwoMultipleRelationshipsToTheSameBO()
		{
			//---------------Set up test pack-------------------
			const string parentClassDefXML = @"<classes> 
						<class name=""FakeBOWithTwoRelToSameProp"" assembly=""Habanero.Smooth.Test"">
							<property name =""FakeBOWithTwoRelToSamePropID"" type=""Guid""/>
							<primaryKey isObjectID=""true"">
								<prop name=""FakeBOWithTwoRelToSamePropID"" />
							</primaryKey>
								<relationship name=""FakeBORel1"" type=""single"" relatedClass=""FakeBOWithTwoMultipleRelToSameProp"" reverseRelationship=""MyMultipleAutoMapWithTwoSingleReverse"" relatedAssembly=""Habanero.Smooth.Test"">
								<relatedProperty property=""FakeBOWithTwoRelToSamePropID"" relatedProperty=""MyMultipleAutoMapWithTwoSingleReverseID"" />
								</relationship>
								<relationship name=""FakeBORel2"" type=""single"" relatedClass=""FakeBOWithTwoMultipleRelToSameProp"" reverseRelationship=""MyMultipleAutoMapWithTwoSingleReverse2"" relatedAssembly=""Habanero.Smooth.Test"">
								<relatedProperty property=""FakeBOWithTwoRelToSamePropID"" relatedProperty=""MyMultipleAutoMapWithTwoSingleReverse2ID"" />
								</relationship>
						</class>
					</classes>";
			//---------------Assert Precondition----------------
			//---------------Execute Test ----------------------
			var xmlClassDefsLoader = new XmlClassDefsLoader(parentClassDefXML, new DtdLoader());
			var loadedClassDefs = xmlClassDefsLoader.LoadClassDefs();
			var allClassesAutoMapper = new AllClassesAutoMapper(new CustomTypeSource( new []{typeof(FakeBOWithTwoMultipleRelToSameProp)}));
			AllClassesAutoMapper.ClassDefCol = loadedClassDefs;
			allClassesAutoMapper.Map();
			//---------------Test Result -----------------------
			Assert.AreEqual(2, AllClassesAutoMapper.ClassDefCol.Count);
			var relationshipDefCol = AllClassesAutoMapper.ClassDefCol[typeof (FakeBOWithTwoMultipleRelToSameProp)].RelationshipDefCol;
			Assert.AreEqual(2,relationshipDefCol.Count);
			var relationshipDef = relationshipDefCol["MyMultipleAutoMapWithTwoSingleReverse"];
			Assert.AreEqual("FakeBORel1",relationshipDef.ReverseRelationshipName);
			var relationshipDef2 = relationshipDefCol["MyMultipleAutoMapWithTwoSingleReverse2"];
			Assert.AreEqual("FakeBORel2", relationshipDef2.ReverseRelationshipName);
			var validator = new ClassDefValidator(new DefClassFactory());
			validator.ValidateClassDefs(AllClassesAutoMapper.ClassDefCol);
		}


		[Test]
		public void TestAutoMap_WithTwoSingleRelationshipToTheSameBO()
		{
			//---------------Set up test pack-------------------
			const string parentClassDefXML = @"<classes> 
						<class name=""FakeBOWithTwoRelToSameProp"" assembly=""Habanero.Smooth.Test"">
							<property name =""FakeBOWithTwoRelToSamePropID"" type=""Guid""/>
							<primaryKey isObjectID=""true"">
								<prop name=""FakeBOWithTwoRelToSamePropID"" />
							</primaryKey>
								<relationship name=""FakeBORel1"" type=""multiple"" relatedClass=""FakeBOWithTwoSingleRelToSameProp"" reverseRelationship=""SingleRel1"" relatedAssembly=""Habanero.Smooth.Test"">
								<relatedProperty property=""FakeBOWithTwoRelToSamePropID"" relatedProperty=""SingleRel1ID"" />
								</relationship>
								<relationship name=""FakeBORel2"" type=""multiple"" relatedClass=""FakeBOWithTwoSingleRelToSameProp"" reverseRelationship=""SingleRel2"" relatedAssembly=""Habanero.Smooth.Test"">
								<relatedProperty property=""FakeBOWithTwoRelToSamePropID"" relatedProperty=""SingleRel2ID"" />
								</relationship>
						</class>
					</classes>";
			//---------------Assert Precondition----------------
			//---------------Execute Test ----------------------
			var xmlClassDefsLoader = new XmlClassDefsLoader(parentClassDefXML, new DtdLoader());
			var loadedClassDefs = xmlClassDefsLoader.LoadClassDefs();
			var allClassesAutoMapper = new AllClassesAutoMapper(new CustomTypeSource(new[] { typeof(FakeBOWithTwoSingleRelToSameProp) }));
			AllClassesAutoMapper.ClassDefCol = loadedClassDefs;
			allClassesAutoMapper.Map();
			//---------------Test Result -----------------------
			Assert.AreEqual(2, AllClassesAutoMapper.ClassDefCol.Count);
			var relationshipDefCol = AllClassesAutoMapper.ClassDefCol[typeof(FakeBOWithTwoSingleRelToSameProp)].RelationshipDefCol;
			Assert.AreEqual(2,relationshipDefCol.Count);
			var relationshipDef = relationshipDefCol["SingleRel1"];
			Assert.AreEqual("FakeBORel1",relationshipDef.ReverseRelationshipName);
			var relationshipDef2 = relationshipDefCol["SingleRel2"];
			Assert.AreEqual("FakeBORel2", relationshipDef2.ReverseRelationshipName);
			var validator = new ClassDefValidator(new DefClassFactory());
			validator.ValidateClassDefs(AllClassesAutoMapper.ClassDefCol);
		}

		[Test]
		public void TestAutoMap_WithTwoSingleRelationshipToTheSameBO_WithPropAndRelNameMatching()
		{
			//---------------Set up test pack-------------------
			const string parentClassDefXML = @"<classes> 
						<class name=""FakeBOWithTwoRelToSameProp"" assembly=""Habanero.Smooth.Test"">
							<property name =""FakeBOWithTwoRelToSamePropID"" type=""Guid""/>
							<primaryKey isObjectID=""true"">
								<prop name=""FakeBOWithTwoRelToSamePropID"" />
							</primaryKey>
						</class>
					</classes>";
			//---------------Assert Precondition----------------
			//---------------Execute Test ----------------------
			var xmlClassDefsLoader = new XmlClassDefsLoader(parentClassDefXML, new DtdLoader());
			var loadedClassDefs = xmlClassDefsLoader.LoadClassDefs();
			var relatedClassType = typeof(FakeBOWithTwoSingleRelToSamePropWithSameName);
			var allClassesAutoMapper = new AllClassesAutoMapper(new CustomTypeSource(new[] { relatedClassType }));
			AllClassesAutoMapper.ClassDefCol = loadedClassDefs;
			allClassesAutoMapper.Map();
			//---------------Test Result -----------------------
			Assert.AreEqual(2, AllClassesAutoMapper.ClassDefCol.Count);
			var relationshipDefCol = AllClassesAutoMapper.ClassDefCol[relatedClassType].RelationshipDefCol;
			Assert.AreEqual(2,relationshipDefCol.Count);
			var relationshipDef = relationshipDefCol["FakeBOWithTwoRelToSameProp"];
			Assert.AreEqual("FakeBOWithTwoSingleRelToSamePropWithSameNames", relationshipDef.ReverseRelationshipName);
			var relationshipDef2 = relationshipDefCol["SingleRel2"];
			Assert.AreEqual("FakeBORel2", relationshipDef2.ReverseRelationshipName);
			var validator = new ClassDefValidator(new DefClassFactory());
			validator.ValidateClassDefs(AllClassesAutoMapper.ClassDefCol);
		}

		[Test]
		public void TestAutoMap_WithTwoSingleRelationshipToTheSameBO_WhereReverseRelsAreDefined_WithPropAndRelNameMatching()
		{
			//---------------Set up test pack-------------------
			const string parentClassDefXML = @"<classes> 
						<class name=""FakeBOWithTwoRelToSameProp"" assembly=""Habanero.Smooth.Test"">
							<property name =""FakeBOWithTwoRelToSamePropID"" type=""Guid""/>
							<primaryKey isObjectID=""true"">
								<prop name=""FakeBOWithTwoRelToSamePropID"" />
							</primaryKey>
							<relationship name=""FakeBOWithTwoSingleRelToSamePropWithSameNames"" type=""multiple"" relatedClass=""FakeBOWithTwoSingleRelToSamePropWithSameName"" reverseRelationship=""FakeBOWithTwoRelToSameProp"" relatedAssembly=""Habanero.Smooth.Test"">
							<relatedProperty property=""FakeBOWithTwoRelToSamePropID"" relatedProperty=""FakeBOWithTwoRelToSamePropID"" />
							</relationship>
							<relationship name=""FakeBORel2"" type=""multiple"" relatedClass=""FakeBOWithTwoSingleRelToSamePropWithSameName"" reverseRelationship=""SingleRel2"" relatedAssembly=""Habanero.Smooth.Test"">
							<relatedProperty property=""FakeBOWithTwoRelToSamePropID"" relatedProperty=""SingleRel2ID"" />
							</relationship>
						</class>
					</classes>";	

			//---------------Assert Precondition----------------
			//---------------Execute Test ----------------------
			var xmlClassDefsLoader = new XmlClassDefsLoader(parentClassDefXML, new DtdLoader());
			var loadedClassDefs = xmlClassDefsLoader.LoadClassDefs();
			var relatedClassType = typeof(FakeBOWithTwoSingleRelToSamePropWithSameName);
			var allClassesAutoMapper = new AllClassesAutoMapper(new CustomTypeSource(new[] { relatedClassType }));
			AllClassesAutoMapper.ClassDefCol = loadedClassDefs;
			allClassesAutoMapper.Map();
			//---------------Test Result -----------------------
			Assert.AreEqual(2, AllClassesAutoMapper.ClassDefCol.Count);
			var relationshipDefCol = AllClassesAutoMapper.ClassDefCol[relatedClassType].RelationshipDefCol;
			Assert.AreEqual(2, relationshipDefCol.Count);
			var relationshipDef = relationshipDefCol["FakeBOWithTwoRelToSameProp"];
			Assert.AreEqual("FakeBOWithTwoSingleRelToSamePropWithSameNames", relationshipDef.ReverseRelationshipName);
			var relationshipDef2 = relationshipDefCol["SingleRel2"];
			Assert.AreEqual("FakeBORel2", relationshipDef2.ReverseRelationshipName);
			var validator = new ClassDefValidator(new DefClassFactory());
			validator.ValidateClassDefs(AllClassesAutoMapper.ClassDefCol);
		}
	}
}