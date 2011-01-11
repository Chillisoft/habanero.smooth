using System;
using System.Linq;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.Test;
using NUnit.Framework;
using TestProject.BO;


namespace Habanero.Fluent.Tests
{
   
    [TestFixture]
    public class TestClassDefBuilder
    {

        private string GetRandomString()
        {
            return RandomValueGenerator.GetRandomString();
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
            var classDef = new ClassDefBuilder<Car>().Build();
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
            var classDefBuilder = new ClassDefBuilder<Car>();
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
            var classDefBuilder = new ClassDefBuilder<Car>();
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
                    var classDefBuilder = new ClassDefBuilder<Car>();
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
        [Test]
        public void Test_CreateClassDef_WithRelationshipsLambda_ShouldBuildRelationships()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = new ClassDefBuilder<Car>();
            var classDef = classDefBuilder
                    .WithSingleRelationship(c => c.SteeringWheel).Return()
                    .WithMultipleRelationship(c => c.Drivers).Return()
                    .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, classDef.RelationshipDefCol.Count);

            var relationshipDef1 = classDef.RelationshipDefCol["SteeringWheel"];
            Assert.IsNotNull(relationshipDef1.RelationshipName);
            Assert.AreEqual("SteeringWheel", relationshipDef1.RelatedObjectClassName);

            var relationshipDef2 = classDef.RelationshipDefCol["Drivers"];
            Assert.IsNotNull(relationshipDef2.RelationshipName);
            Assert.AreEqual("Driver", relationshipDef2.RelatedObjectClassName);
        }
          
        [Test]
        public void Test_CreateClassDef_WithRelationships_ShouldBuildRelationships()
        {
            //---------------Set up test pack-------------------
            string relationshipName1 = "C" + GetRandomString();
            string relationshipName2 = "F" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = new ClassDefBuilder<Car>();
            var classDef = classDefBuilder
                    .WithSingleRelationship<Car>(relationshipName1).Return()
                    .WithMultipleRelationship<Car>(relationshipName2).Return()
                    .Build();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, classDef.RelationshipDefCol.Count);
            Assert.IsNotNull(classDef.RelationshipDefCol[relationshipName1].RelationshipName);
            Assert.IsNotNull(classDef.RelationshipDefCol[relationshipName2].RelationshipName);
        }

        [Test]
        public void Test_CreateClassDef_WithPrimaryKey_IntProp_ShouldBuildPrimary_WithIsGuidObjectID_False()
        {
            //---------------Set up test pack-------------------
            string propertyName1 = "A" + GetRandomString();
            string propertyName2 = "B" + GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = new ClassDefBuilder<Car>();
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
            var classDefBuilder = new ClassDefBuilder<Car>();
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
            var classDefBuilder = new ClassDefBuilder<Car>();
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
            var classDefBuilder = new ClassDefBuilder<Car>();
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
            var classDefBuilder = new ClassDefBuilder<Car>();
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
            var classDefBuilder = new ClassDefBuilder<Car>();
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
            var classDefBuilder = new ClassDefBuilder<Car>();
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
            var classDefBuilder = new ClassDefBuilder<Car>();
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
            var classDefBuilder = new ClassDefBuilder<Car>();
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
