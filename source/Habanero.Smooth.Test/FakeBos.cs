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
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedTypeParameter
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable ClassNeverInstantiated.Global
namespace Habanero.Smooth.Test
{
	public enum FakeEnum
	{
		SomeNum
	}

	public class FakeAttribute : Attribute
	{
	}

	public class SomeNonBoClass
	{
	}

	public class SomeNonBoClassSubClass : SomeNonBoClass
	{
	}

	public class SuperClassWithNonStandardID : BusinessObject
	{
		/// <summary>
		///
		/// </summary>
		[AutoMapPrimaryKey]
		public virtual Guid? NonStandardID
		{
			get { return ((Guid?)(base.GetPropertyValue("NonStandardID"))); }
			set { base.SetPropertyValue("NonStandardID", value); }
		}
	}
	public class SubClassWithNonStandardID : SuperClassWithNonStandardID{}


	public class SuperClassWithPKFromClassDef : BusinessObject
	{
		public static IClassDef LoadClassDef()
		{
			var itsLoader = new XmlClassLoader(new DtdLoader(), new DefClassFactory());
			var itsClassDef = itsLoader.LoadClass(@"
			  <class name=""SuperClassWithPKFromClassDef"" assembly=""Habanero.Smooth.Test"">
				<property name=""MYPKID"" type=""Guid"" />
				<primaryKey isObjectID=""true"">
				  <prop name=""MYPKID"" />
				</primaryKey>
			  </class>
			");
			return itsClassDef;
		}
	}

	public class SubClassWithPKFromClassDef : SuperClassWithPKFromClassDef
	{
	}

	public class RelatedToSubClassWithPKFromClassDef : BusinessObject
	{

		/// <summary>
		/// The SubClassWithNonStandardID this RelatedToSubClassWithNonStandardID is for.
		/// </summary>
		public virtual SubClassWithPKFromClassDef SubClassWithPKFromClassDefSingleRel
		{
			get { return Relationships.GetRelatedObject<SubClassWithPKFromClassDef>("SubClassWithPKFromClassDefSingleRel"); }
			set { Relationships.SetRelatedObject("SubClassWithPKFromClassDefSingleRel", value); }
		}
	}
	public class RelatedToSubClassWithNonStandardID: BusinessObject
	{
		/// <summary>
		/// The SubClassWithNonStandardID this RelatedToSubClassWithNonStandardID is for.
		/// </summary>
		public virtual SubClassWithNonStandardID SubClassWithNonStandardIDSingleRel
		{
			get { return Relationships.GetRelatedObject<SubClassWithNonStandardID>("SubClassWithNonStandardIDSingleRel"); }
			set { Relationships.SetRelatedObject("SubClassWithNonStandardIDSingleRel", value); }
		}
	}

	public class FakeBOfdafasd : BusinessObject
	{
		public string Someprop { get; set; }
		public FakeBOWProps somerela { get; set; }
	}

	public class FakeBOWProps : BusinessObject
	{
		public Guid FakeBOWPropsID { get; set; }
		public Guid PublicGetGuidProp { get; private set; }
		public Guid? PublicGetNullableGuidProp { get; private set; }
		public string PublicStringProp { get; private set; }
		public int PublicIntProp { get; private set; }
		public short PublicShortProp { get; private set; }
		public FakeEnum PublicEnumProp { get; set; }
		public FakeEnum? PublicNullableEnumProp { get; set; }
		public System.Drawing.Image PublicImageProp { get; set; }
		public byte[] PublicByteArrayProp { get; set; }

		[FakeAttribute]
		public float? PublicPropWithAtt { get; set; }

		[AutoMapIgnore]
		public Guid PublicWithIgnoreAtt { get; set; }

		[AutoMapKeepValuePrivate]
		public Guid PublicWithKeepValuePrivateAtt { get; set; }
	}

	public class FakeBOGeneric : BusinessObject<FakeBOGeneric>
	{
	}

	public class FakeBOGenericSubType : FakeBOGeneric
	{
	}

	public class GenericBOSuperType<T> : BusinessObject<T>
	{
	}

	public class FakeGenericBOSubType : GenericBOSuperType<FakeGenericBOSubType>
	{
	}

	public class FakeBONoPK : BusinessObject
	{
		public Guid PublicGetGuidProp { get; private set; }
	}

	public class FakeBOAttributePK : BusinessObject
	{
		[AutoMapPrimaryKey]
		public Guid PublicGuidProp { get; private set; }

		public Guid AnotherProp { get; private set; }
	}

	public class FakeBOTwoPropsAttributePK : BusinessObject
	{
		[AutoMapPrimaryKey]
		public Guid PublicGuidProp { get; private set; }

		[AutoMapPrimaryKey]
		public Guid AnotherPrimaryKeyProp { get; private set; }

		public Guid AnotherProp { get; private set; }
	}

	public class FakeBOAttributePKAndPKNaming : BusinessObject
	{
		[AutoMapPrimaryKey]
		public Guid PublicGuidProp { get; private set; }

		public Guid FakeBOAttributePKAndPKNamingID { get; private set; }
		public Guid AnotherProp { get; private set; }
		public BusinessObjectCollection<FakeBOWithUndefinableSingleRel> MyMultipleRevRel { get; set; }

		public BusinessObjectCollection<FakeBOWithSingleRel1> MyMultipleRevRel2 { get; set; }
	}

	public class FakeBOWithCompositionSingleRel : BusinessObject
	{
		[AutoMapOneToOne(RelationshipType.Aggregation)]
		public FakeBONoPK MySingleRelationship { get; set; }
	}

	public class FakeBOWithCompositionManyToOneRel : BusinessObject
	{
		[AutoMapManyToOne(RelationshipType.Aggregation)]
		public FakeBONoPK MySingleRelationship { get; set; }
	}

	public class FakeBOWithSingleRel : BusinessObject
	{
		public FakeBONoPK MySingleRelationship { get; set; }

		[AutoMapIgnore]
		public FakeBONoPK MySingleRElationshipWithIgnore { get; set; }
	}

	public interface IFakeNoPK
	{
		Guid PublicGetGuidProp { get; }
	}

	public class FakeBOWithInterface : BusinessObject, IFakeNoPK
	{
		public Guid PublicGetGuidProp { get; private set; }
	}

	public class FakeBOWithSingleRelToInterface : BusinessObject
	{
		[AutoMapManyToOne(RelatedObjectClassType = typeof(FakeBOWithInterface))]
		public IFakeNoPK MySingleRelationship { get; set; }
	}

	public class FakeBOWithSingleRelWithFieldNameOverride : BusinessObject
	{
		[AutoMapFieldName("SingleID")]
		public FakeBONoPK MySingleRelationship { get; set; }
	}

	public class FakeBOWithSingleRelToGenericBO : BusinessObject
	{
		public FakeBOGeneric MySingleRelationship { get; set; }
	}

	public class FakeBOWithSingleRel1 : BusinessObject
	{
		public FakeBOAttributePKAndPKNaming MySingleRelationship2 { get; set; }
	}

	public class FakeBOWithSingleRelAndFKProp : BusinessObject
	{
		public FakeBONoPK MySingleRelationship { get; set; }
		public Guid? MySingleRelationshipID { get; set; }
	}

	public class FakeBOWithOneToOneAttribute : BusinessObject
	{
		[AutoMapOneToOne]
		public FakeBONoPK MySingleRelationship { get; set; }

		public Guid? MySingleRelationshipID { get; set; }
	}

	public class FakeBOWithReverseSingleRel : BusinessObject
	{
		/// <summary>
		/// Mapped to MyReverseSingleRel Relationship.
		/// </summary>
		public FakeBOWithTwoSingleRel MySingleRelationship { get; set; }
	}

	public class FakeBOWithTwoSingleRel : BusinessObject
	{
		public FakeBONoPK MySingleRelationship { get; set; }

		public FakeBOWithReverseSingleRel MyReverseSingleRel { get; set; }
	}

	public class FakeBOWithTwoSingleRelNoRevs : BusinessObject
	{
		public FakeBONoPK MySingleRelationship { get; set; }

		public FakeBONoPK MySecondSingleRelationship { get; set; }
	}

	public class FakeBOWithUndefinableSingleRel : BusinessObject
	{
		public FakeBOWithSingleAndMultipleRelToSameType MySingleRelationship { get; set; }

		[AutoMapOneToOne]
		public FakeBOWithSingleAndMultipleRelToSameType MySingleRelWithOneToOneAttribute { get; set; }

		[AutoMapManyToOne]
		public FakeBOWithSingleAndMultipleRelToSameType MySingleRelWithOneToManyAttribute { get; set; }
	}

	public class FakeBOWithSingleAndMultipleRelToSameType : BusinessObject
	{
		public FakeBOWithUndefinableSingleRel MySingleRevRel { get; set; }

		public BusinessObjectCollection<FakeBOWithUndefinableSingleRel> MyMultipleRevRel { get; set; }
	}

	public class FakeBoWithCompositionMultipleRel : BusinessObject
	{
		[AutoMapOneToMany(RelationshipType = RelationshipType.Composition)]
		public BusinessObjectCollection<FakeBOWithUndefinableSingleRel> MyMultipleRevRel { get; set; }
	}

	public class FakeBoWithDeleteParentActionDeleteRelatedRel : BusinessObject
	{
		[AutoMapOneToMany(DeleteParentAction = DeleteParentAction.DeleteRelated)]
		public BusinessObjectCollection<FakeBOWithUndefinableSingleRel> MyMultipleRevRel { get; set; }
	}

	public class FakeBoWithOneToManyAssociation : BusinessObject
	{
		[AutoMapOneToMany(RelationshipType.Association)]
		public BusinessObjectCollection<FakeBOWithUndefinableSingleRel> MultipleRel { get; set; }
	}

	public class FakeBoWithMultipleRel : BusinessObject
	{
		public BusinessObjectCollection<FakeBOWithUndefinableSingleRel> MyMultipleRevRel { get; set; }

		public IBusinessObjectCollection FakeBOWithUndefinableSingleRels { get; set; }

		[AutoMapIgnore]
		public IBusinessObjectCollection MultipleRevIgnores { get; set; }

		public BusinessObjectCollection<FakeWithTwoSingleReverseRel> MyMultipleWithTwoSingleReverse { get; set; }

		[AutoMapOneToMany("MySingleRelationship2")]
		public BusinessObjectCollection<FakeWithTwoSingleReverseRel> MyMultipleAutoMapWithTwoSingleReverse { get; set; }

		[AutoMapIgnore]
		public BusinessObjectCollection<FakeWithTwoSingleReverseRel> MyMultipleWithTwoSingleReverseThisIgnore { get; set; }

		public BusinessObjectCollection<FakeBOTwoSingleOneIgnore> MyMultipleWithTwoSingleReverseOneIgnore { get; set; }
		public BusinessObjectCollection<FakeBoWithAutoMapOneToOne> ReverseHasAutoMapOneToOne { get; set; }

		public BusinessObjectCollection<FakeWithThreeSingleReverseRelOneAutoMapped> MyMultipleReverseAutoMapped { get; set; }

		public FakeBOWithSingleAttributeDeclaredRevRel SingleRel { get; set; }

		[AutoMapOneToMany("NonExistentReverseRel")]
		public BusinessObjectCollection<FakeWithThreeSingleReverseRelOneAutoMapped> MultipleMappedToNonExistentReverse { get; set; }

		[AutoMapOneToOne]
		public FakeWithTwoSingleReverseRel MySingleWithTwoSingleReverse { get; set; }
	}

	public class FakeBOWithTwoMultipleRelToSameProp : BusinessObject
	{
		[AutoMapOneToMany("FakeBORel1")]
		public BusinessObjectCollection<FakeBOWithTwoRelToSameProp> MyMultipleAutoMapWithTwoSingleReverse { get; set; }

		[AutoMapOneToMany("FakeBORel2")]
		public BusinessObjectCollection<FakeBOWithTwoRelToSameProp> MyMultipleAutoMapWithTwoSingleReverse2 { get; set; }
	}

	public class FakeBOWithTwoSingleRelToSameProp : BusinessObject
	{
		[AutoMapManyToOne("FakeBORel1")]
		public FakeBOWithTwoRelToSameProp SingleRel1 { get; set; }

		[AutoMapManyToOne("FakeBORel2")]
		public FakeBOWithTwoRelToSameProp SingleRel2 { get; set; }
	}

	public class FakeBOWithTwoSingleRelToSamePropWithSameName : BusinessObject
	{
		[AutoMapManyToOne("FakeBOWithTwoSingleRelToSamePropWithSameNames")]
		public FakeBOWithTwoRelToSameProp FakeBOWithTwoRelToSameProp { get; set; }

		[AutoMapManyToOne("FakeBORel2")]
		public FakeBOWithTwoRelToSameProp SingleRel2 { get; set; }
	}

	public class FakeBOWithTwoRelToSameProp:BusinessObject
	{
	}

	public class FakeBoWithAutoMapOneToOne : BusinessObject
	{
		[AutoMapOneToOne]
		public FakeBoWithMultipleRel MySingleRelationship1 { get; set; }
	}

	public class FakeWithTwoSingleReverseRel : BusinessObject
	{
		public FakeBoWithMultipleRel MySingleRelationship1 { get; set; }

		public FakeBoWithMultipleRel MySingleRelationship2 { get; set; }
	}

	public class FakeBOTwoSingleOneIgnore : BusinessObject
	{
		public FakeBoWithMultipleRel MySingleRelationship1 { get; set; }

		[AutoMapIgnore]
		public FakeBoWithMultipleRel MySingleRelationship2 { get; set; }
	}

	public class FakeWithThreeSingleReverseRelOneAutoMapped : BusinessObject
	{
		public FakeBoWithMultipleRel MySingleRelationship1 { get; set; }
		public FakeBoWithMultipleRel MySingleRelationship2 { get; set; }

		[AutoMapManyToOne("MyMultipleReverseAutoMapped")]
		public FakeBoWithMultipleRel MySingleRelationship3 { get; set; }
	}

	public class FakeBOWithSingleAttributeDeclaredRevRel : BusinessObject
	{
		[AutoMapManyToOne(ReverseRelationshipName = "AttributeRevRelName")]
		public FakeBOWithUndefinableSingleRel MySingleRelationship { get; set; }
	}

	public class FakeBOWithReverseSingle : BusinessObject
	{
		public FakeBOWithM1Attribute MySingleRelationship { get; set; }

		[AutoMapOneToOne("MySingleRevRelationship")]
		public FakeBOWith11Attribute MySingleRelationship2 { get; set; }

		[AutoMapOneToOne("NoRevRel")]
		public FakeBOWith11Attribute MySingleWithAutoMapNoReverse { get; set; }

		public FakeBOWith11Attribute MySingleRelationship3 { get; set; }

		[AutoMapIgnore]
		public FakeBOWith11Attribute MySingleIgnorRelationship { get; set; }

		public FakeBOWithIgnoreAttribute MySingleWithReverseIgnore { get; set; }
		public FakeBOWithOneSingleRel SingleWithRevesre { get; set; }
	}

	public class FakeBOWithReverseSingleToInterface : BusinessObject
	{
		[AutoMapOneToOne]
		public IFakeBO MySingleRelationship { get; set; }
	}

	public class FakeBOWithOneSingleRel : BusinessObject
	{
		public FakeBOWithReverseSingle ReverseSingleRel { get; set; }
	}

	public class FakeBOWith11Attribute : BusinessObject
	{
		public FakeBOWithReverseSingle MySingleRevRelationship { get; set; }
		public FakeBOWithReverseSingle MySingleRelationship2 { get; set; }
	}

	public class FakeBOWithTwo11Rels : BusinessObject
	{
		[AutoMapOneToOne]
		public FakeBoNoProps MyOneToOne { get; set; }

		[AutoMapOneToOne]
		public FakeBoNoProps MyOneToOne2 { get; set; }
	}

	public class FakeBOWithIgnoreAttribute : BusinessObject
	{
		[AutoMapIgnore]
		public FakeBOWithReverseSingle MySingleRelationship1 { get; set; }
	}

	public class FakeBOWithM1Attribute : BusinessObject
	{
		[AutoMapManyToOne]
		public FakeBOWithReverseSingle MySingleRelationshipRev { get; set; }
	}

	public class FakeBOWithOneM21_AndOne121 : BusinessObject
	{
		[AutoMapManyToOne(ReverseRelationshipName = "xxxs")]
		public FakeBOWithReverseSingle MySingleRel1 { get; set; }

		[AutoMapOneToOne(ReverseRelationshipName = "yyyy")]
		public FakeBOWithReverseSingle MySingleRel2 { get; set; }
	}

	public class FakeBOWithOne12M : BusinessObject
	{
		[AutoMapOneToMany(ReverseRelationshipName = "xxxs")]
		public BusinessObjectCollection<FakeBOWithReverseSingle> MyMultRel { get; set; }

		[AutoMapOneToMany(ReverseRelationshipName = "yyyy")]
		public BusinessObjectCollection<FakeBOWithReverseSingle> MyMultRell2 { get; set; }
	}

	public class FakeBOWithAllTypesOfRel : BusinessObject
	{
		[AutoMapOneToMany(ReverseRelationshipName = "xxxs")]
		public BusinessObjectCollection<FakeBOWithReverseSingle> MyMultRel { get; set; }

		[AutoMapOneToMany(ReverseRelationshipName = "yyyy")]
		public BusinessObjectCollection<FakeBOWithReverseSingle> MyMultRell2 { get; set; }

		[AutoMapManyToOne(ReverseRelationshipName = "xxxs")]
		public FakeBOWithReverseSingle MySingleRel1 { get; set; }

		[AutoMapOneToOne]
		public FakeBoWithMultipleRel MySingleRelationship1 { get; set; }

		[AutoMapOneToOne]
		public FakeBoWithMultipleRel MySingleRelationship2 { get; set; }

		[AutoMapOneToOne]
		public FakeBoWithMultipleRel MySingleRelationship3 { get; set; }

		[AutoMapIgnore]
		public FakeBOWith11Attribute MySingleIgnoreRelationship { get; set; }

		[AutoMapIgnore]
		public BusinessObjectCollection<FakeBOWithReverseSingle> MyMultIgnoreRel { get; set; }

		public Guid FakeBOWPropsID { get; set; }
		public Guid PublicGetGuidProp { get; private set; }
		public Guid? PublicGetNullableGuidProp { get; private set; }
		public string PublicStringProp { get; private set; }
		public int PublicIntProp { get; private set; }
		public FakeEnum PublicEnumProp { get; set; }
		public FakeEnum? PublicNullableEnumProp { get; set; }
	}

	public class FakeBoWithNoSingleReverse : BusinessObject
	{
		public FakeBOWithUndefinableSingleRel MySingleRelationship { get; set; }
	}

	public class FakeBoWithStaticProperty : BusinessObject
	{
		[AutoMapOneToOne]
		public static FakeBoNoProps MySingleRelationship { get; set; }

		[AutoMapManyToOne]
		public static FakeBoNoProps MySingleRelationship2 { get; set; }

		public static BusinessObjectCollection<FakeBoNoProps> MyMultiple { get; set; }
		public static string PublicStringProp { get; private set; }
	}

	public class FakeBoWithPrivateProps : BusinessObject
	{
		[AutoMapOneToOne]
		private FakeBoNoProps PrivateOneToOneRel { get; set; }

		[AutoMapManyToOne]
		private FakeBoNoProps PrivateManyToOneRel { get; set; }

		private BusinessObjectCollection<FakeBoNoProps> PrivateMultipleRel { get; set; }

		private string PrivateStringProp { get; set; }
		public string PublicStringProp { get; private set; }
		protected string ProtectedStringProp { get; set; }
		internal string InternalStringProp { get; set; }
	}

	[AutoMapIgnore]
	public class FakeBoIgnore : BusinessObject
	{
		public FakeBOWithUndefinableSingleRel MySingleRelationship { get; set; }
	}

	[AutoMapTableName(TableName = "tbMyFakeBo")]
	public class FakeBoWithTableName : BusinessObject
	{
		[AutoMapFieldName("MyFieldName")]
		public virtual string PropWithFieldName
		{
			get { return ((string) (base.GetPropertyValue("PropWithFieldName"))); }
			set { base.SetPropertyValue("PropWithFieldName", value); }
		}
		public virtual string PropNoFieldName
		{
			get { return ((string)(base.GetPropertyValue("PropNoFieldName"))); }
			set { base.SetPropertyValue("PropNoFieldName", value); }
		}
	}

	public class FakeBoWithoutTableName : BusinessObject
	{
	}

	public class FakeManyToOneBoRelNoFK : BusinessObject
	{
		public FakeBOWithNoRelationship MySingleRelationship { get; set; }
	}

	public class FakeBOWithNoRelationship : BusinessObject
	{
	}

	public class FakeBOWithMultipleRel : BusinessObject
	{
		public BusinessObjectCollection<FakeBOWithNoRelationship> MyMultipleRel { get; set; }
	}

	public class FakeBOWithMultipleRelWithProp : BusinessObject
	{
		public Guid? FakeBOWithMultipleRelationshipID { get; set; }
		public BusinessObjectCollection<FakeBOWithSingleRelAndFKProp> MyMultipleRel { get; set; }
	}

	public class FakeBOWithOneToOneRel : BusinessObject
	{
		[AutoMapOneToOne]
		public FakeBOWithNoRelationship MyMultipleRel { get; set; }
	}

	public class FakeBOSuperClass : BusinessObject
	{
	}

	public class FakeBoNoPropsOfT : BusinessObject<FakeBoNoPropsOfT>
	{
	}

	public interface IFakeBO : IBusinessObject
	{
		string FakeBOSuperClassWithDescType { get; set; }
	}

	public class FakeBOSubClass : FakeBOSuperClass
	{
	}

	public class FakeBOSubClassA : FakeBOSuperClass
	{
	}

	public class FakeBOSubSubClass : FakeBOSubClass
	{
	}

	public class FakeBoNoProps : BusinessObject
	{
	}

	public class FakeBOSuperClassWithDesc : BusinessObject, IFakeBO
	{
		public virtual string FakeBOSuperClassWithDescType { get; set; }
//        public virtual string OverriddenProp { get; set; }
//        [AutoMapManyToOne]
//        public virtual FakeBOWithNoRelationship ManyToOneRelationshipOverridden { get; set; }
//        [AutoMapOneToOne]
//        public virtual FakeBOWithNoRelationship OneToOneRelationshipOverridden { get; set; }
//        [AutoMapOneToMany]
//        public virtual BusinessObjectCollection<FakeBOWithNoRelationship> OneToManyRelationshipOverridden { get; set; }
//
//        [AutoMapManyToOne]
//        public virtual FakeBOWithNoRelationship ManyToOneRelationshipInherited { get; set; }
//        [AutoMapOneToOne]
//        public virtual FakeBOWithNoRelationship OneToOneRelationshipInherited{ get; set; }
//        [AutoMapOneToMany]
//        public virtual BusinessObjectCollection<FakeBOWithNoRelationship> OneToManyRelationshipInherited { get; set; }
	}

	public class FakeBOSubClassSuperHasDesc : FakeBOSuperClassWithDesc
	{
	}

	public class FakeBOSubSubClassSuperHasDesc : FakeBOSubClassSuperHasDesc
	{
	}

	public class FakeBOSubClassWithSuperHasUC : FakeBOSuperClassWithUC
	{
	}

	public class FakeBOSuperClassWithUC : BusinessObject
	{
		[AutoMapUniqueConstraint("UC_Fake")]
		public string FakeUCProp { get; set; }
	}

	public class FakeBOSubClassWithRelationships : FakeBOSuperClass
	{
		public BusinessObjectCollection<FakeBoNoProps> MultipleRel { get; set; }
		public FakeBoNoProps SingleRel { get; set; }
	}

	public class FakeBOSuperClassWithVirtualProps : BusinessObject, IFakeBO
	{
		public virtual string FakeBOSuperClassWithDescType { get; set; }
		public virtual string OverriddenProp { get; set; }
		[AutoMapManyToOne]
		public virtual FakeBOWithNoRelationship ManyToOneRelationshipOverridden { get; set; }
		[AutoMapOneToOne]
		public virtual FakeBOWithNoRelationship OneToOneRelationshipOverridden { get; set; }
		[AutoMapOneToMany]
		public virtual BusinessObjectCollection<FakeBOWithNoRelationship> OneToManyRelationshipOverridden { get; set; }

		[AutoMapManyToOne]
		public virtual FakeBOWithNoRelationship ManyToOneRelationshipInherited { get; set; }
		[AutoMapOneToOne]
		public virtual FakeBOWithNoRelationship OneToOneRelationshipInherited { get; set; }
		[AutoMapOneToMany]
		public virtual BusinessObjectCollection<FakeBOWithNoRelationship> OneToManyRelationshipInherited { get; set; }
	}

	public class FakeBOSubClassWithOverridenProps : FakeBOSuperClassWithVirtualProps
	{
		public override string OverriddenProp { get; set; }
		[AutoMapManyToOne]
		public override FakeBOWithNoRelationship ManyToOneRelationshipOverridden { get; set; }
		[AutoMapOneToOne]
		public override FakeBOWithNoRelationship OneToOneRelationshipOverridden { get; set; }
		[AutoMapOneToMany]
		public override BusinessObjectCollection<FakeBOWithNoRelationship> OneToManyRelationshipOverridden { get; set; }
	}

	public class FakeBOWithUniqueConstraint_TwoProps : BusinessObject
	{
		[AutoMapUniqueConstraint("UC1")]
		public virtual string UCProp1 { get; set; }

		[AutoMapUniqueConstraint("UC1")]
		public virtual string UCProp2 { get; set; }
	}

	public class FakeBOWithUniqueConstraint_OneProp : BusinessObject
	{
		[AutoMapUniqueConstraint("UC")]
		public virtual string UCProp { get; set; }
	}

	public class FakeBOWithUniqueConstraint_Relationship : BusinessObject
	{
		[AutoMapUniqueConstraint("UC")]
		[AutoMapManyToOne]
		public FakeBOWithNoRelationship RelatedObject { get; set; }
	}

	public class FakeBOWithUniqueConstraint_TwoRelationship : BusinessObject
	{
		[AutoMapUniqueConstraint("UC")]
		[AutoMapManyToOne]
		public FakeBOWithNoRelationship RelatedObject1 { get; set; }

		[AutoMapUniqueConstraint("UC")]
		[AutoMapManyToOne]
		public FakeBOWithNoRelationship RelatedObject2 { get; set; }
	}

	public class FakeBOWithTwoUniqueConstraints_OnePropEach : BusinessObject
	{
		[AutoMapUniqueConstraint("UC1")]
		public virtual string UCProp1 { get; set; }
		[AutoMapUniqueConstraint("UC2")]
		public virtual string UCProp2 { get; set; }
	}

	public class FakeBOWithTwoUniqueConstraints_TwoPropEach : BusinessObject
	{
		[AutoMapUniqueConstraint("UC1")]
		public virtual string UC1Prop1 { get; set; }
		[AutoMapUniqueConstraint("UC1")]
		public virtual string UC1Prop2 { get; set; }
		[AutoMapUniqueConstraint("UC2")]
		public virtual string UC2Prop1 { get; set; }
		[AutoMapUniqueConstraint("UC2")]
		public virtual string UC2Prop2 { get; set; }
	}

	public class FakeBOImplementingInterface : BusinessObject, IFakeBOInterface
	{
		public string ImplementedProp { get; set; }
	}

	public interface IFakeBOInterface
	{
		string ImplementedProp { get; set; }
	}
}

namespace Habanero.Smooth.Test.ValidFakeBOs
{
	public class FakeBOSuperClass : BusinessObject
	{
	}

	public class FakeBOSubClass : FakeBOSuperClass
	{
	}

	public class FakeBOSubSubClass : FakeBOSubClass
	{
	}

	public class FakeBoNoProps : BusinessObject
	{
	}

	public class FakeBOSubClassWithRelationship : FakeBOSuperClass
	{
		public BusinessObjectCollection<FakeBoNoProps> MultipleRel { get; set; }
		public FakeBoNoProps SingleRel { get; set; }
	}

	//This returns an Error due to the ClassName being pluralised
	//The consequence is that the Reverse Relationship for both the
	// single and multiple is created as FakeBOSubClassWithRelationships
	//this results in both these relationships mapping to
	// Same reverse but reverse has diff relProps and is
	// different cardinality
//    public class FakeBOSubClassWithRelationships : FakeBOSuperClass
//    {
//        public BusinessObjectCollection<FakeBoNoProps> MultipleRel { get; set; }
//        public FakeBoNoProps SingleRel { get; set; }
//    }
	public class FakeBOWithNoRelationship : BusinessObject
	{
	}

	public class FakeBOWithMultipleRel : BusinessObject
	{
		public BusinessObjectCollection<FakeBOWithNoRelationship> MultipleRel { get; set; }
	}

	public class FakeBOWithOneToOneRel : BusinessObject
	{
		[AutoMapOneToOne]
		public FakeBOWithNoRelationship MyMultipleRel { get; set; }
	}

	public class FakeBOWithCompulsoryProp : BusinessObject
	{
		[AutoMapCompulsory]
		public String CompulsoryProp { get; set; }

		public String NonCompulsoryProp { get; set; }

		[AutoMapCompulsory]
		public FakeBOWithMultipleRel CompulsorySingleRelationship { get; set; }

		[AutoMapManyToOne(ReverseRelationshipName = "AnyName")]
		public FakeBOWithMultipleRel NonCompulsorySingleRelationship { get; set; }
	}

	public class FakeBOWithDefaultProp : BusinessObject
	{
		[AutoMapDefault("Today")]
		public DateTime DefaultProp { get; set; }

		public String NonDefaultProp { get; set; }
	}

	public class FakeBoWithIntPK : BusinessObject
	{
		/// <summary>
		///
		/// </summary>
		[AutoMapPrimaryKey]
		public virtual int FakeBoWithIntPKID
		{
			get { return ((int) (base.GetPropertyValue("FakeBoWithIntPKID"))); }
			set { base.SetPropertyValue("FakeBoWithIntPKID", value); }
		}
	}

	public class FakeBOWitDisplayNameProp : BusinessObject
	{
		[AutoMapDisplayName("MyDisplayName")]
		public String DisplayNameProp { get; set; }

		public String NonDisplayNameProp { get; set; }
	}

	public class FakeBOWithAutoIncrementingProp : BusinessObject
	{
		[AutoMapAutoIncrementing]
		public int AutoIncrementingProp { get; set; }

		public int NonAutoIncrementingProp { get; set; }
	}

	public class FakeBOWithReadWriteRuleProp : BusinessObject
	{
		[AutoMapReadWriteRule(PropReadWriteRule.ReadOnly)]
		public DateTime ReadWriteRuleReadOnly { get; set; }

		[AutoMapReadWriteRule(PropReadWriteRule.ReadWrite)]
		public DateTime ReadWriteRuleReadWrite{ get; set; }

		public String ReadWriteRuleDefault { get; set; }
	}

	public class FakeMergeableParent : BusinessObject
	{
		[AutoMapOneToOne("FakeMergeableParentReverse", RelationshipType.Aggregation)]
		public virtual FakeMergeableChild FakeMergeableChild { get; set; }

		[AutoMapOneToOne("FakeMergeableParentReverseNoType", RelationshipType.Composition)]
		public virtual FakeMergeableChild FakeMergeableChildNoType { get; set; }


		[AutoMapOneToOne("FakeMergeableParentReverseFKDefined", RelationshipType.Composition)]
		public virtual FakeMergeableChild FakeMergeableChildNoTypeRelatedFK { get; set; }
		public Guid FakeMergeableChildNoTypeRelatedFKID { get; set; }
	}

	public class FakeMergeableChild : BusinessObject
	{

		[AutoMapOneToOne("FakeMergeableChild", RelationshipType.Association)]
		public virtual FakeMergeableParent FakeMergeableParentReverse { get; set; }

		public virtual FakeMergeableParent FakeMergeableParentReverseNoType { get; set; }

		public virtual FakeMergeableParent FakeMergeableParentReverseFKDefined { get; set; }
	}

	public class FakeBOWithWithOneToOneAssociationRel: BusinessObject
	{
		[AutoMapOneToOneAttribute("FakeMergeableRel", RelationshipType.Association)]
		public virtual AFakeBO2WithOneToOneAssociationRel FakeParentReverse
		{
			get
			{
				return Relationships.GetRelatedObject<AFakeBO2WithOneToOneAssociationRel>("FakeParentReverse");
			}
			set
			{
				Relationships.SetRelatedObject("FakeParentReverse", value);
			}
		}
	}

	public class AFakeBO2WithOneToOneAssociationRel : BusinessObject
	{
		public virtual Guid? FakeMergeableRelID
		{
			get
			{
				return ((Guid?)(base.GetPropertyValue("FakeMergeableRelID")));
			}
			set
			{
				base.SetPropertyValue("FakeMergeableRelID", value);
			}
		}

		[AutoMapOneToOneAttribute("FakeParentReverse", RelationshipType.Association)]
		public virtual FakeBOWithWithOneToOneAssociationRel FakeMergeableRel
		{
			get
			{
				return Relationships.GetRelatedObject<FakeBOWithWithOneToOneAssociationRel>("FakeMergeableRel");
			}
			set
			{
				Relationships.SetRelatedObject("FakeMergeableRel", value);
			}
		}
	}
}
// ReSharper restore UnusedMember.Global
// ReSharper restore UnusedAutoPropertyAccessor.Local
// ReSharper restore UnusedMember.Local
// ReSharper restore ClassNeverInstantiated.Global