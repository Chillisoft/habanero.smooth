using System;
using System.Linq;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.Test;
using NUnit.Framework;
using TestProject.BO;


// ReSharper disable InconsistentNaming
namespace Habanero.Fluent.Tests
{
   
    [TestFixture]
    public class TestClassDefBuilder
    {

        private string GetRandomString()
        {
            return RandomValueGenerator.GetRandomString();
        }

        [Test]
        public void Test_WithRelDef_WithSingleRelKey_ShouldHaveOneRelProp()
        {
            //---------------Set up test pack-------------------
            var classDefBuilder = new OldClassDefBuilder<Car>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDef = classDefBuilder.WithNewSingleRelationship(car => car.SteeringWheel)
                        .WithRelProp(GetRandomString(), GetRandomString())
                    .Return()
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
            var classDefBuilder = new OldClassDefBuilder<Car>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDef = classDefBuilder.WithNewSingleRelationship(car => car.SteeringWheel)
                        .WithRelProp(GetRandomString(), GetRandomString())
                    .Return()
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
            var classDefBuilder = new OldClassDefBuilder<Car>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDef = classDefBuilder.WithNewSingleRelationship(car => car.SteeringWheel)
                            .WithCompositeRelationshipKey()
                                .WithRelProp(GetRandomString(), GetRandomString())
                                .WithRelProp(GetRandomString(), GetRandomString())
                            .Return()  // from compositerelationshiprelprops
                        .Return()  // from singlerelationship
                        .Build();

            //---------------Test Result -----------------------
            Assert.IsNotNull(classDef);
            Assert.AreEqual(1, classDef.RelationshipDefCol.Count);
            var relationshipDef = classDef.RelationshipDefCol["SteeringWheel"];
            Assert.IsNotNull(relationshipDef);
            Assert.AreEqual(2, relationshipDef.RelKeyDef.Count);
        }

//        [Test]
//        public void Test_CreateClassDef()
//        {
//            //---------------Set up test pack-------------------
//            //---------------Assert Precondition----------------

//            //---------------Execute Test ----------------------
//            /*            ClassDef classDef = new ClassDefBuilder<Car>()
//                            .WithProperty<int>("SomeProp")
//                               .WithDefault(fdfasdfds)
//                               .WithReadWrite(PropReadWriteRule.WriteOnce)
//                               .WithDataBaseFieldName("fdafdas").Build()
//                            .WithProperty("AnotherProp").Build()
//                            .WithProperty("sss").Build()
//                            .WithSingleRelationship("fdafasd")
//                            .WithMultipleRelationship("fdafds")
//                            .Build();

//                         ClassDef classDef = new ClassDefBuilder<Car>()
//                            .WithProperty(a => a.PropName)
//                              .WithRule(afdafasdfasd)
//                            .WithSingleRelationship("fdafasd")
//                            .WithMultipleRelationship*/
//            //---------------Test Result -----------------------
///*            Assert.AreEqual("Habanero.Fluent.Tests", classDef.AssemblyName);
//            Assert.AreEqual("Car", classDef.ClassName);
//            Assert.IsNotNull(classDef.PropDefcol);//Should Be Empty
//            Assert.IsNotNull(classDef.PrimaryKeyDef);//Should Be Empty
//            Assert.IsNotNull(classDef.KeysCol);//Should Be Empty
//            Assert.IsNotNull(classDef.RelationshipDefCol);//Should Be Empty
//            Assert.AreEqual(0, classDef.UIDefCol.Count());*/
//        }

        [Test]
        public void Test_CreateClassDef_ShouldUseDefaults()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDef = new OldClassDefBuilder<Car>().Build();
            //---------------Test Result -----------------------
            Assert.AreEqual("TestProject.BO", classDef.AssemblyName);
            Assert.AreEqual("Car", classDef.ClassName);
            Assert.AreEqual(0, classDef.PropDefcol.Count);//Should Be Empty
            Assert.AreEqual(0, classDef.PrimaryKeyDef.Count);//Should Be Empty
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
            var classDefBuilder = new OldClassDefBuilder<Car>();
            var classDef = classDefBuilder
                .WithProperty(c => c.Make).Return()
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
            var classDefBuilder = new OldClassDefBuilder<Car>();
            var classDef = classDefBuilder
                .WithProperty<int>(propertyName1)
                    .IsCompulsory().Return()
                .WithProperty(propertyName2)
                    .WithReadWriteRule(PropReadWriteRule.ReadWrite).Return()
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, classDef.PropDefcol.Count);//Should not Be Empty
            var propDef = classDef.PropDefcol.FirstOrDefault();
            Assert.IsTrue(propDef.Compulsory, "Should be compulsory");
            Assert.AreSame(typeof(int), propDef.PropertyType);
        }
        [Test]
        public void Test_CreateClassDef_WithLambda_GuidProp_ShouldCreatePropTypeGuid()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
                    var classDefBuilder = new OldClassDefBuilder<Car>();
            var classDef = classDefBuilder
                .WithProperty(c => c.VehicleID).Return()
                .Build();

            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.PropDefcol.Count);//Should not Be Empty
            var propDef = classDef.PropDefcol.FirstOrDefault();
            Assert.AreEqual("VehicleID", propDef.PropertyName);
            Assert.AreEqual("System", propDef.PropertyTypeAssemblyName);
            Assert.AreEqual("Guid", propDef.PropertyTypeName);
            Assert.AreSame(typeof(Guid), propDef.PropertyType);
        }

        [Ignore("This test is no longer valid as you can't define a singlerelationship without relprops")] //TODO Andrew Russell 20 Dec 2010: Ignored Test - This test is no longer valid as you can't define a singlerelationship without relprops
        [Test]
        public void Test_CreateClassDef_WithRelationshipsLambda_ShouldBuildRelationships()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = new OldClassDefBuilder<Car>();
            var classDef = classDefBuilder
                    //.WithSingleRelationship(c => c.SteeringWheel).Return()
                    .WithMultipleRelationship(c => c.Drivers).Return()
                    .Build();
            //---------------Test Result -----------------------
            //Assert.AreEqual(2, classDef.RelationshipDefCol.Count);

            //var relationshipDef1 = classDef.RelationshipDefCol["SteeringWheel"];
            //Assert.IsNotNull(relationshipDef1.RelationshipName);
            //Assert.AreEqual("SteeringWheel", relationshipDef1.RelatedObjectClassName);

            var relationshipDef2 = classDef.RelationshipDefCol["Drivers"];
            Assert.IsNotNull(relationshipDef2.RelationshipName);
            Assert.AreEqual("Driver", relationshipDef2.RelatedObjectClassName);
        }
/*

        [Test]
        public void Test_Validate_With_NoRelProps_ShouldBeInvalid()
        {
            //---------------Set up test pack-------------------
            string relationshipName = "R" + GetRandomString();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var classDefBuilder = new ClassDefBuilder<Car>();
            var classDef = classDefBuilder
                .WithPrimaryKeyProp(car1 => car1.VehicleID)
                .WithSingleRelationship<SteeringWheel>(relationshipName).Return()
                .WithProperty(car1 => car1.VehicleID).Return()
                .Build();
            var classDefSteeringWheel = new ClassDefBuilder<SteeringWheel>()
                .WithProperty(wheel => wheel.CarID).Return()
                .WithProperty(wheel => wheel.SteeringWheelID).Return()
                .WithPrimaryKeyProp(wheel => wheel.SteeringWheelID)
                .WithSingleRelationship(wheel => wheel.Car)
                    .WithRelProp(wheel => wheel.CarID, car => car.VehicleID).Return()
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
            var classDefBuilder = new ClassDefBuilder<Car>();
            var classDef = classDefBuilder
                    .WithSingleRelationship<SteeringWheel>(relationshipName1)
                            .WithRelProp(car => car.VehicleID, steeringWheel => steeringWheel.CarID)
                            .Return()
                    .WithMultipleRelationship<Car>(relationshipName2).Return()
                    .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, classDef.RelationshipDefCol.Count);
            classDef.RelationshipDefCol.ShouldContain(def => def.RelationshipName == relationshipName1);
            classDef.RelationshipDefCol.ShouldContain(def => def.RelationshipName == relationshipName2);
        }
*/
        //TODO andrew 20 Dec 2010: 

        /*
         * if not composite then we want
                         .WithSingleRelationship<SteeringWheel>(relationshipName1)
                            .WithRelProp(car => car.VehicleID, steeringWheel => steeringWheel.CarID).Return()
                         .WithProperty(efadsfads)
         
         * 
         *          * if not composite then we want
                         .WithSingleRelationship<SteeringWheel>(relationshipName1)
         *                  .WithCompositeKey()
                              .WithRelProp(car => car.VehicleID, steeringWheel => steeringWheel.CarID)
         *                    .WithRelProp(fdafdfasd).Return
                         .WithProperty(efadsfads)
         */

        [Ignore("No longer able to define a singlerelartionship with no relprop")] //TODO Andrew Russell 20 Dec 2010: Ignored Test - No longer able to define a singlerelartionship with no relprop
                [Test]
                public void Test_Build_WithRelationships_WhenRelPropsNotDefined_ShouldBuildRelationships_WithStandardRelProps()
                {
                    //---------------Set up test pack-------------------
                    string relationshipName1 = "C" + GetRandomString();
                    string relationshipName2 = "F" + GetRandomString();
                    //---------------Assert Precondition----------------

                    //---------------Execute Test ----------------------
                    var classDefBuilder = new OldClassDefBuilder<Car>();
                    var classDef = classDefBuilder
                            .WithSingleRelationship<Car>(relationshipName1).Return()
                            .WithMultipleRelationship<Car>(relationshipName2).Return()
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
            var multipleRelationshipDef = new OldMultipleRelationshipDefBuilder<Car, Driver>()
                                                            .WithRelationshipName(c => c.Drivers)
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
            var classDefBuilder = new OldClassDefBuilder<Car>();
            var classDef = classDefBuilder
                .WithProperty<int>(propertyName1).Return()
                .WithProperty(propertyName2).Return()
                .WithPrimaryKeyProp(propertyName1)
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
            var classDefBuilder = new OldClassDefBuilder<Car>();
            var classDef = classDefBuilder
                .WithProperty(n => n.Make).Return()
                .WithPrimaryKeyProp(n => n.Make)
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
            var classDefBuilder = new OldClassDefBuilder<Car>();
            var classDef = classDefBuilder
                .WithProperty(n => n.VehicleID).Return()
                .WithPrimaryKeyProp(n => n.VehicleID)
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, classDef.PrimaryKeyDef.Count);
            var def = classDef.PrimaryKeyDef[0];
            Assert.AreEqual("VehicleID", def.PropertyName);
            Assert.AreSame(typeof(Guid), def.PropertyType);
            Assert.IsTrue(classDef.PrimaryKeyDef.IsGuidObjectID, "Should set to ISGuidObjectID since NonCompositeGuid");
        }

        [Test]
        public void Test_CreateClassDef_WithPrimaryKey_GuidProp_ShouldBuildPrimary_WithIsGuidObjectID_True()
        {
            //---------------Set up test pack-------------------
            string propertyName1 = "A" + GetRandomString();
            string propertyName2 = "B" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = new OldClassDefBuilder<Car>();
            var classDef = classDefBuilder
                .WithProperty<Guid>(propertyName1).Return()
                .WithProperty(propertyName2).Return()
                .WithPrimaryKeyProp(propertyName1)
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
            var classDefBuilder = new OldClassDefBuilder<Car>();
            var classDef = classDefBuilder
                .WithProperty<Guid>(propertyName1).Return()
                .WithProperty(propertyName2).Return()
                .WithPrimaryKeyProp(propertyName1)
                .WithPrimaryKeyProp(propertyName2)
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
            var classDefBuilder = new OldClassDefBuilder<Car>();
            var classDef = classDefBuilder
                .WithProperty<int>(propertyName1).Return()
                .WithProperty(propertyName2).Return()
                .WithUniqueConstraint()
                    .AddProperty(propertyName1).Return()
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
            var classDefBuilder = new OldClassDefBuilder<Car>();
            var classDef = classDefBuilder
                .WithProperty<int>(propertyName1).Return()
                .WithProperty(propertyName2).Return()
                .WithUniqueConstraint()
                    .AddProperty(propertyName1)
                    .AddProperty(propertyName2).Return()
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
            var classDefBuilder = new OldClassDefBuilder<Car>();
            var classDef = classDefBuilder
                .WithProperty<int>(propertyName1).Return()
                .WithProperty(propertyName2).Return()
                .WithUniqueConstraint(keyName).Return()
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, classDef.PrimaryKeyDef.Count);//Should Be Empty
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
            var classDefBuilder = new OldClassDefBuilder<Car>();
            var classDef = classDefBuilder
                .WithProperty(o => o.Make).Return()
                .WithUniqueConstraint(keyName)
                    .AddProperty(o => o.Make).Return()
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
            var classDefBuilder = new OldClassDefBuilder<Car>();
            IClassDef classDef = classDefBuilder
                .WithSuperClass().Return()
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
            var classDefBuilder = new OldClassDefBuilder<Car>();
            var classDef = classDefBuilder
                .WithSuperClass()
                    .WithDiscriminator(vehicle => vehicle.StringProp).Return()
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
            var classDefBuilder = new OldClassDefBuilder<Car>();
            var classDef = classDefBuilder
                .WithSuperClass()
                    .WithDiscriminator("SomeProp").Return()
                .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual("Vehicle", classDef.SuperClassDef.ClassName);
            Assert.AreEqual("TestProject.BO", classDef.SuperClassDef.AssemblyName);
            Assert.AreEqual("SomeProp", classDef.SuperClassDef.Discriminator);
        }


    }


/*    public class CarBuilder: ClassDefBuilder<Car>
    {
        public CarBuilder()
        {
            WithProperty();
            WithProperty();

        }
    }*/


    //public  class Car : BusinessObject
    //{

    //    #region Properties
    //    public virtual String NoOfSeats
    //    {
    //        get
    //        {
    //            return ((String)(base.GetPropertyValue("NoOfSeats")));
    //        }
    //        set
    //        {
    //            base.SetPropertyValue("NoOfSeats", value);
    //        }
    //    }
    //    #endregion
    //}
}
// ReSharper restore InconsistentNaming