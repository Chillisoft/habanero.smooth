﻿using System;
using System.Linq;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.Test;
using NUnit.Framework;
using TestProject.BO;

namespace Habanero.Fluent.Tests
{
    [TestFixture]
    public class TestNewClassDefBuilder
    {

               private string GetRandomString()
        {
            return RandomValueGenerator.GetRandomString();
        }

        [Test]
// ReSharper disable InconsistentNaming
        public void Test_WithRelDef_WithSingleRelKey_ShouldHaveOneRelProp()
        {
            //---------------Set up test pack-------------------
            NewClassDefBuilder<Car> classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDef = classDefBuilder
                                .WithPrimaryKey(c => c.VehicleID)
                                    .WithRelationships()
                                        .WithNewSingleRelationship(car => car.SteeringWheel)
                                        .WithRelProp(GetRandomString(), GetRandomString())

                                    .EndSingleRelationship()
                                .EndRelationships()
                .Build();

            //---------------Test Result -----------------------
            Assert.IsNotNull(classDef);
            Assert.AreEqual(1, classDef.RelationshipDefCol.Count);
            var relationshipDef = classDef.RelationshipDefCol["SteeringWheel"];
            Assert.IsNotNull(relationshipDef);
            Assert.AreEqual(1, relationshipDef.RelKeyDef.Count);
        }



        [Test]
        public void Test_WithRelDef_WithSingleRelKey_ShouldSetDefaults()
        {
            //---------------Set up test pack-------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDef = classDefBuilder
                                .WithPrimaryKey(c => c.VehicleID)
                                .WithRelationships()
                                    .WithNewSingleRelationship(car => car.SteeringWheel)
                                        .WithRelProp(GetRandomString(), GetRandomString())
                                    .EndSingleRelationship()
                                .EndRelationships()
                .Build();

            //---------------Test Result -----------------------
            var relationshipDef = classDef.RelationshipDefCol["SteeringWheel"];
            Assert.IsTrue(relationshipDef.KeepReferenceToRelatedObject);
            Assert.AreEqual(DeleteParentAction.DoNothing, relationshipDef.DeleteParentAction);
            Assert.AreEqual("TestProject.BO", relationshipDef.RelatedObjectAssemblyName);
            Assert.AreEqual("SteeringWheel", relationshipDef.RelatedObjectClassName);
            Assert.AreEqual(InsertParentAction.InsertRelationship, relationshipDef.InsertParentAction);
            Assert.AreEqual(RelationshipType.Association, relationshipDef.RelationshipType);
        }


        [Test]
        public void Test_WithRelDef_WithCompositeRelKey_ShouldRequired()
        {
            //---------------Set up test pack-------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDef = classDefBuilder.WithPrimaryKey(c => c.VehicleID)
                                .WithRelationships()
                                    .WithNewSingleRelationship(car => car.SteeringWheel)
                                        .WithCompositeRelationshipKey()
                                            .WithRelProp(GetRandomString(), GetRandomString())
                                            .WithRelProp(GetRandomString(), GetRandomString())
                                        .EndCompositeRelationshipKey()
                                    .EndSingleRelationship()
                                .EndRelationships()
                        .Build();

            //---------------Test Result -----------------------
            Assert.IsNotNull(classDef);
            Assert.AreEqual(1, classDef.RelationshipDefCol.Count);
            var relationshipDef = classDef.RelationshipDefCol["SteeringWheel"];
            Assert.IsNotNull(relationshipDef);
            Assert.AreEqual(2, relationshipDef.RelKeyDef.Count);
        }


        [Test]
        public void Test_CreateClassDef_ShouldUseDefaults()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDef = GetClassDefBuilderForTypeOf_Car().WithPrimaryKey(c => c.VehicleID).Build();
            //---------------Test Result -----------------------
            Assert.AreEqual("TestProject.BO", classDef.AssemblyName);
            Assert.AreEqual("Car", classDef.ClassName);
            Assert.AreEqual(1, classDef.PropDefcol.Count, "Should contain at least one property that was created when creating the pk");
            Assert.AreEqual(1, classDef.PrimaryKeyDef.Count);//Should Be Empty
            Assert.AreEqual(0, classDef.KeysCol.Count);//Should Be Empty
            Assert.AreEqual(0, classDef.RelationshipDefCol.Count);//Should Be Empty
            Assert.AreEqual(0, classDef.UIDefCol.Count);
        }

        [Test]
        public void Test_Build_WithPropertyLambda_ShouldBuildProp()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            var classDef = classDefBuilder
                            .WithPrimaryKey(c => c.VehicleID)
                            .WithProperties()
                                .Property(c => c.Make).EndProperty()
                            .EndProperties()
                .Build();
            //---------------Test Result -----------------------
            Assert.GreaterOrEqual(classDef.PropDefcol.Count, 1, "Should have prop");
            var propDef = classDef.PropDefcol.FirstOrDefault();
            Assert.AreEqual("Make", propDef.PropertyName);
            Assert.AreSame(typeof(string), propDef.PropertyType);
        }

        [Test]
        public void Test_CreateClassDef_WithProperty_WithIsCompulsory_ShouldSetAsCompulsory()
        {
            //---------------Set up test pack-------------------
            string propertyName1 = "A" + GetRandomString();
            string propertyName2 = "B" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            var classDef = classDefBuilder
                                .WithPrimaryKey(c=>c.VehicleID)
                                .WithProperties()
                                    .Property<int>(propertyName1)
                                        .IsCompulsory()
                                    .EndProperty()
                                    .Property(propertyName2)
                                        .WithReadWriteRule(PropReadWriteRule.ReadWrite)
                                    .EndProperty()
                                .EndProperties()
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(3, classDef.PropDefcol.Count, "The primarykey prop will also automatically be built");//Should not Be Empty
            var propDef = classDef.PropDefcol.FirstOrDefault();
            Assert.IsTrue(propDef.Compulsory, "Should be compulsory");
            Assert.AreSame(typeof(int), propDef.PropertyType);
        }

        [Ignore("Need to work out why propertyTypeNameAssembly is being returned as CommonLanguageRuntime")] //TODO Andrew Russell 03 Feb 2011: Ignored Test - Need to work out why propertyTypeNameAssembly is being returned as CommonLanguageRuntime
        [Test]
        public void Test_CreateClassDef_WithLambda_GuidProp_ShouldCreatePropTypeGuid()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            var classDef = classDefBuilder
                            .WithPrimaryKey(c => c.VehicleID)
                            .WithProperties()
                                .Property(c => c.VehicleID).EndProperty()
                            .EndProperties()
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.PropDefcol.Count);//Should not Be Empty
            var propDef = classDef.PropDefcol.FirstOrDefault();
            Assert.AreEqual("VehicleID", propDef.PropertyName);
            Assert.AreEqual("System", propDef.PropertyTypeAssemblyName);
            Assert.AreEqual("Guid", propDef.PropertyTypeName);
            Assert.AreSame(typeof(Guid), propDef.PropertyType);
        }

        [Test]
        public void Test_CreateClassDef_WithSingleRelationship_Lambda_ShouldBuildRelationships()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            var classDef = classDefBuilder
                            .WithPrimaryKey(c => c.VehicleID)
                            .WithRelationships()
                                .WithNewSingleRelationship(c => c.SteeringWheel)
                                    .WithRelProp("VehicleID", "CarID")
                                .EndSingleRelationship()
                            .EndRelationships()
                    .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.RelationshipDefCol.Count);

            var relationshipDef1 = classDef.RelationshipDefCol["SteeringWheel"];
            Assert.IsNotNull(relationshipDef1.RelationshipName);
            Assert.AreEqual("SteeringWheel", relationshipDef1.RelatedObjectClassName);
            Assert.AreEqual(RelationshipType.Association, relationshipDef1.RelationshipType);
            //var relationshipDef2 = classDef.RelationshipDefCol["Drivers"];
            //Assert.IsNotNull(relationshipDef2.RelationshipName);
            //Assert.AreEqual("Driver", relationshipDef2.RelatedObjectClassName);
        }

        [Test]
        public void Test_CreateClassDef_WithMultipleRelationship_Lambda_ShouldBuildRelationships()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            var classDef = classDefBuilder
                            .WithPrimaryKey(c => c.VehicleID)
                            .WithRelationships()
                                .WithNewMultipleRelationship(car => car.Drivers)
                                        .WithRelProp("VehicleID", "CarID")
                                .EndMultipleRelationship()
                            .EndRelationships()
                    .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.RelationshipDefCol.Count);


            var relationshipDef1 = classDef.RelationshipDefCol["Drivers"];
            Assert.IsNotNull(relationshipDef1.RelationshipName);
            Assert.AreEqual("Driver", relationshipDef1.RelatedObjectClassName);
            Assert.AreEqual(RelationshipType.Association, relationshipDef1.RelationshipType);

        }

        [Ignore("Currently this test is not testing anything as you can no longer create a relationship without a relprop")] //TODO Andrew Russell 07 Feb 2011: Ignored Test - Currently this test is not testing anything as you can no longer create a relationship without a relprop
        [Test]
        public void Test_Validate_With_NoRelProps_ShouldBeInvalid()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "R" + GetRandomString();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var classDefBuilder = new NewClassDefBuilder<Car>();
            var classDef = classDefBuilder
                    .WithPrimaryKey(car1 => car1.VehicleID)
                    .WithRelationships()
                        .WithNewSingleRelationship<SteeringWheel>(relationshipName)
                            .WithRelProp("VehicleID", "CarID")
                        .EndSingleRelationship()
                    .EndRelationships()
                    .WithProperties()
                        .Property(car1 => car1.VehicleID).EndProperty()
                    .EndProperties()
                .Build();
            var classDefSteeringWheel = new NewClassDefBuilder<SteeringWheel>()
                    .WithPrimaryKey(w => w.SteeringWheelID)
                    .WithProperties()
                        .Property(wheel => wheel.CarID).EndProperty()
                        //.Property(wheel => wheel.SteeringWheelID).EndProperty()
                    .EndProperties()
                    .WithRelationships()
                        .WithNewSingleRelationship(wheel => wheel.Car)
                            .WithRelProp("CarID", "VehicleID")
                        .EndSingleRelationship()
                    .EndRelationships()
                .Build();
            var classDefCol = new ClassDefCol { classDef, classDefSteeringWheel };
            //---------------Test Result -----------------------
            var classDefValidator = new ClassDefValidator(new DefClassFactory());
            classDefValidator.ValidateClassDefs(classDefCol);
        }


        [Test]
        public void Test_CreateClassDef_WithRelationships_ShouldBuildRelationships()
        {
            //---------------Set up test pack-------------------
            var relationshipName1 = "C" + GetRandomString();
            var relationshipName2 = "F" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            var classDef = classDefBuilder
                    .WithPrimaryKey(c => c.VehicleID)
                    .WithRelationships()
                        .WithNewSingleRelationship<SteeringWheel>(relationshipName1)
                            .WithRelProp(c => c.VehicleID, wheel => wheel.CarID)
                        .EndSingleRelationship()
                        .WithNewMultipleRelationship<Driver>(relationshipName2)
                            .WithRelProp(c => c.VehicleID, driver => driver.CarID)
                        .EndMultipleRelationship()
                    .EndRelationships()
            .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, classDef.RelationshipDefCol.Count);
            classDef.RelationshipDefCol.ShouldContain(def => def.RelationshipName == relationshipName1);
            classDef.RelationshipDefCol.ShouldContain(def => def.RelationshipName == relationshipName2);
        }


        [Test]
        public void Test_Build_WithLambdaProp_ShouldSetRelationshipProperties()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var multipleRelationshipDef = GetClassDefBuilderForTypeOf_Car()
                                                .WithPrimaryKey(c => c.VehicleID)
                                                .WithRelationships()
                                                    .WithNewMultipleRelationship(c => c.Drivers)
                                                            .WithRelProp(car => car.VehicleID, driver => driver.CarID)
                                                    .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual("Drivers", multipleRelationshipDef.RelationshipName);
            var relPropDef = multipleRelationshipDef.RelKeyDef["VehicleID"];
            Assert.IsNotNull(relPropDef);
            Assert.AreEqual("VehicleID", relPropDef.OwnerPropertyName);
            Assert.AreEqual("CarID", relPropDef.RelatedClassPropName);
        }

        [Test]
        public void Test_CreateClassDef_WithPrimaryKey_IntProp_ShouldBuildPrimary_WithIsGuidObjectID_False()
        {
            //---------------Set up test pack-------------------
            string propertyName1 = "A" + GetRandomString();
            string propertyName2 = "B" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            var classDef = classDefBuilder
                 .WithPrimaryKey(propertyName1)
                 .WithProperties()
                    .Property<int>(propertyName1).EndProperty()
                    .Property(propertyName2).EndProperty()
                .EndProperties()
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.PrimaryKeyDef.Count);
            Assert.IsFalse(classDef.PrimaryKeyDef.IsGuidObjectID);
            var def = classDef.PrimaryKeyDef[0];
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.AreEqual(propertyName1, def.PropertyName);
        }

        [Test]
        public void Test_CreateClassDef_WithPrimaryKeyWithLamba_ShouldBuildPrimary_WithPropName()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            var classDef = classDefBuilder
                    .WithPrimaryKey(car => car.Make)
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.PrimaryKeyDef.Count);
            Assert.IsFalse(classDef.PrimaryKeyDef.IsGuidObjectID);
            var def = classDef.PrimaryKeyDef[0];
            Assert.AreEqual("Make", def.PropertyName);
        }

        [Test]
        public void Test_CreateClassDef_WithPrimaryKeyWithLamba_WhenGuidID_ShouldBuildPrimary_WithGuidID()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            var classDef = classDefBuilder
                    .WithPrimaryKey(n => n.VehicleID)
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.PrimaryKeyDef.Count);
            var def = classDef.PrimaryKeyDef[0];
            Assert.AreEqual("VehicleID", def.PropertyName);
            Assert.AreSame(typeof(Guid), def.PropertyType);
            Assert.IsTrue(classDef.PrimaryKeyDef.IsGuidObjectID, "Should set to ISGuidObjectID since NonCompositeGuid");
        }

        [Test]
        public void Test_CreateClassDef_WithStringPrimaryKey_ShouldBuildPrimaryKey_WithIsGuidObjectID_True_ByDefault()
        {
            //---------------Set up test pack-------------------
            string propertyName1 = "A" + GetRandomString();
            string propertyName2 = "B" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            var classDef = classDefBuilder
                    .WithPrimaryKey(propertyName1)
                    .WithProperties()
                        .Property(propertyName2).EndProperty()
                    .EndProperties()
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.PrimaryKeyDef.Count);
            var def = classDef.PrimaryKeyDef[0];
            Assert.AreSame(typeof(Guid), def.PropertyType);
            Assert.AreEqual(propertyName1, def.PropertyName);
            Assert.IsTrue(classDef.PrimaryKeyDef.IsGuidObjectID);
        }

        [Test]
        public void Test_CreateClassDef_WithPrimaryKeyComposite_ShouldBuildPrimary_WithIsGuidObjectID_False()
        {
            //---------------Set up test pack-------------------
            string propertyName1 = "A" + GetRandomString();
            string propertyName2 = "B" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            var classDef = classDefBuilder
                .WithCompositePrimaryKey()
                    .WithPrimaryKeyProperty(propertyName1)
                    .WithPrimaryKeyProperty(propertyName2)
                .EndCompositePrimaryKey()
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, classDef.PrimaryKeyDef.Count);
            Assert.IsFalse(classDef.PrimaryKeyDef.IsGuidObjectID);
            Assert.AreEqual(propertyName1, classDef.PrimaryKeyDef[0].PropertyName);
            Assert.AreEqual(propertyName2, classDef.PrimaryKeyDef[1].PropertyName);
        }


        [Test]
        public void Test_CreateClassDef_WithUniqueConstraint_ShouldBuildKeyDef()
        {
            //---------------Set up test pack-------------------
            string propertyName1 = "A" + GetRandomString();
            string propertyName2 = "B" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            var classDef = classDefBuilder
                 .WithPrimaryKey(car => car.VehicleID)
                 .WithProperties()
                    .Property<int>(propertyName1).EndProperty()
                    .Property(propertyName2).EndProperty()
                .EndProperties()
                .WithUniqueConstraints()
                    .UniqueConstraint()
                        .AddProperty(propertyName1)
                    .EndUniqueConstraint()
                .EndUniqueConstraints()
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.KeysCol.Count);//Should Be Empty
            IKeyDef keyDef = classDef.KeysCol.FirstOrDefault();
            Assert.AreEqual(propertyName1, keyDef.KeyName);
            Assert.AreEqual(1, keyDef.Count);
            Assert.AreEqual(propertyName1, keyDef[0].PropertyName);
        }

        [Test]
        public void Test_CreateClassDef_WithUniqueConstraint_TwoProps_ShouldBuildKeyDefWithTwoProps()
        {
            //---------------Set up test pack-------------------
            string propertyName1 = "A" + GetRandomString();
            string propertyName2 = "B" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            var classDef = classDefBuilder
                .WithPrimaryKey(car => car.VehicleID)
                .WithProperties()
                    .Property(propertyName1).EndProperty()
                    .Property(propertyName2).EndProperty()
                .EndProperties()
                .WithUniqueConstraints()
                    .UniqueConstraint()
                        .AddProperty(propertyName1)
                        .AddProperty(propertyName2)
                    .EndUniqueConstraint()
                .EndUniqueConstraints()
            .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.KeysCol.Count);//Should Be Empty
            IKeyDef keyDef = classDef.KeysCol.FirstOrDefault();
            Assert.AreEqual(2, keyDef.Count);
            Assert.AreEqual(propertyName1, keyDef[0].PropertyName);
            Assert.AreEqual(propertyName2, keyDef[1].PropertyName);
        }


        [Test]
        public void Test_CreateClassDef_WithUniqueConstraint_WithKeyName_ShouldBuildKeyDefWithKeyName()
        {
            //---------------Set up test pack-------------------
            string propertyName1 = "A" + GetRandomString();
            string propertyName2 = "B" + GetRandomString();
            string keyName = "K" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            var classDef = classDefBuilder
                .WithPrimaryKey(car => car.VehicleID)
                .WithProperties()
                    .Property<int>(propertyName1).EndProperty()
                    .Property(propertyName2).EndProperty()
                .EndProperties()
                .WithUniqueConstraints()
                    .UniqueConstraint(keyName).EndUniqueConstraint()
                .EndUniqueConstraints()
            .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.PrimaryKeyDef.Count);//PrimaryKey is required
            Assert.AreEqual(1, classDef.KeysCol.Count);//Should Be Empty
            IKeyDef keyDef = classDef.KeysCol.FirstOrDefault();
            Assert.AreEqual(keyName, keyDef.KeyName);
        }

        [Test]
        public void Test_CreateClassDef_WithUniqueConstraintWithLambaProp_ShouldBuildKeyDefWithProp()
        {
            //---------------Set up test pack-------------------
            string keyName = "K" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            var classDef = classDefBuilder
                .WithPrimaryKey(car => car.VehicleID)
                .WithProperties()
                    .Property(o => o.Make).EndProperty()
                .EndProperties()
                .WithUniqueConstraints()
                    .UniqueConstraint(keyName)
                        .AddProperty(o => o.Make)
                    .EndUniqueConstraint()
                .EndUniqueConstraints()
                .Build();
            //---------------Test Result -----------------------
            var keyDef = classDef.KeysCol.FirstOrDefault();
            Assert.AreEqual(1, keyDef.Count);
            Assert.AreEqual("Make", keyDef[0].PropertyName);
        }

        [Test]
        public void Test_CreateClassDef_WithSuperClass_ShouldBuildClassDefWithSuperClassDef()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            IClassDef classDef = classDefBuilder
                .WithSuperClass().EndSuperClass()
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual("Vehicle", classDef.SuperClassDef.ClassName);
            Assert.AreEqual("TestProject.BO", classDef.SuperClassDef.AssemblyName);
            Assert.AreEqual("VehicleType", classDef.SuperClassDef.Discriminator);
        }


        [Test]
        public void Test_CreateClassDef_WithSuperClass_WithLambdaDiscriminator_ShouldBuildClassDefWithSuperClassDef()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            var classDef = classDefBuilder
                .WithSuperClass()
                    .WithDiscriminator(vehicle => vehicle.StringProp)
                .EndSuperClass()
            .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual("Vehicle", classDef.SuperClassDef.ClassName);
            Assert.AreEqual("TestProject.BO", classDef.SuperClassDef.AssemblyName);
            Assert.AreEqual("StringProp", classDef.SuperClassDef.Discriminator);
        }

        [Test]
        public void Test_CreateClassDef_WithSuperClass_WithDiscriminator_ShouldBuildClassDefWithSuperClassDefWithDefinedDescriminator()

        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = GetClassDefBuilderForTypeOf_Car();
            var classDef = classDefBuilder
                .WithSuperClass()
                    .WithDiscriminator("SomeProp")
                .EndSuperClass()
            .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual("Vehicle", classDef.SuperClassDef.ClassName);
            Assert.AreEqual("TestProject.BO", classDef.SuperClassDef.AssemblyName);
            Assert.AreEqual("SomeProp", classDef.SuperClassDef.Discriminator);
        }





///*    public class CarBuilder: ClassDefBuilder<Car>
//    {
//        public CarBuilder()
//        {
//            WithProperty();
//            WithProperty();

//        }
//    }*/


//    //public  class Car : BusinessObject
//    //{

//    //    #region Properties
//    //    public virtual String NoOfSeats
//    //    {
//    //        get
//    //        {
//    //            return ((String)(base.GetPropertyValue("NoOfSeats")));
//    //        }
//    //        set
//    //        {
//    //            base.SetPropertyValue("NoOfSeats", value);
//    //        }
//    //    }
//    //    #endregion
//    //}



//        [Test]
//        [Ignore("not yet implemented")]
//        public void Test_WithPrimaryKey()
//        {
//            //---------------Set up test pack-------------------
            
//            //---------------Assert Precondition----------------

//            //---------------Execute Test ----------------------
//            new NewClassDefBuilder<Car>()
//            .WithPrimaryKey(c => c.VehicleID)
//            .Build();

//            //---------------Test Result -----------------------
//            Assert.Fail("Test Not Yet Implemented");
//        }


//        [Test]
//        [Ignore("not yet implemented")]
//        public void Test_WithCompositePrimaryKey()
//        {
//            //---------------Set up test pack-------------------
            
//            //---------------Assert Precondition----------------

//            //---------------Execute Test ----------------------
//            new NewClassDefBuilder<Car>()
//                .WithCompositePrimaryKey()
//                    .WithPrimaryKeyProperty(c => c.Make)
//                    .WithPrimaryKeyProperty(c => c.Model)
//                    .Return()
//                .Build();
                    
//            //---------------Test Result -----------------------
//            Assert.Fail("Test Not Yet Implemented");
//        }

//        [Test]
//        [Ignore("not yet implemented")]
//        public void Test_WithSuperClass()
//        {
//            //---------------Set up test pack-------------------
            
//            //---------------Assert Precondition----------------

//            //---------------Execute Test ----------------------
//            new NewClassDefBuilder<Car>()
//                .WithSuperClass()
//                    .Return()
//                .Build();
//            //---------------Test Result -----------------------
//            Assert.Fail("Test Not Yet Implemented");
//        }

//        [Test]
//        [Ignore("not yet implemented")]
//        public void Test_WithX()
//        {
//            //---------------Set up test pack-------------------
            
//            //---------------Assert Precondition----------------

//            //---------------Execute Test ----------------------
//            var classDef = new NewClassDefBuilder<Car>()
//                .WithPrimaryKey("gogoID")
//                .WithProperties()
//                    .Property("sdf")
//                        .WithDatabaseFieldName("gogo")
//                        .WithDefaultValue("99")
//                        .WithReadWriteRule(PropReadWriteRule.ReadWrite)
//                    .EndProperty()
//                    .Property(c => c.Make)
//                        .WithReadWriteRule(PropReadWriteRule.ReadOnly)
//                        .WithDisplayName("MMMM Make")
//                        .WithDatabaseFieldName("cMAKE")
//                    .EndProperty()
//                    .Property(c => c.Model).EndProperty()
//                .EndProperties()
//                .WithRelationships()
//                    .WithNewSingleRelationship(c => c.SteeringWheel)
//                            .WithRelProp("asdsfa","asfsda")
//                    .EndSingleRelationship()
//                    .WithNewMultipleRelationship(d => d.Drivers)
//                        .WithCompositeRelationshipKey()
//                                .WithRelProp("x","y")
//                                .WithRelProp("x1", "y1")
//                            .EndRelProps()
//                        .EndCompositeRelationship()
//                .EndRelationships()
                
//            .Build();
            
//            //---------------Test Result -----------------------
//            Assert.Fail("Test Not Yet Implemented");
//        }


        private static NewClassDefBuilder<Car> GetClassDefBuilderForTypeOf_Car()
        {
            return new NewClassDefBuilder<Car>();
        }
    }
}
