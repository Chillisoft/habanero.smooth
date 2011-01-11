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
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Base;
using NUnit.Framework;
// ReSharper disable InconsistentNaming
namespace Habanero.Smooth.Test
{
    [TestFixture]
    public class TestInheritanceAutoMapper
    {
        [SetUp]
        public void Setup()
        {
            AllClassesAutoMapper.PropNamingConvention = new DefaultPropNamingConventions();
            AllClassesAutoMapper.ClassDefCol = null;
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
            var inheritanceDef = subClass.MapInheritance();
            //---------------Test Result -----------------------
            Assert.IsNotNull(inheritanceDef);
            Assert.AreSame(superClass, inheritanceDef.SuperClassClassDef.ClassType);
            Assert.AreEqual(ORMapping.SingleTableInheritance, inheritanceDef.ORMapping);
            Assert.AreEqual("FakeBOSuperClassType", inheritanceDef.Discriminator);
        }

        [Test]
        public void Test_Map_ShouldSetInheritanceDef_ClassName()
        {
            //---------------Set up test pack-------------------
            Type superClass = typeof(FakeBOSuperClass);
            Type subClass = typeof(FakeBOSubClass);
            //---------------Assert Precondition----------------
            Assert.AreSame(superClass, subClass.BaseType);
            Assert.IsTrue(superClass.ToTypeWrapper().IsBusinessObject);
            //---------------Execute Test ----------------------
            var inheritanceDef = subClass.MapInheritance();
            //---------------Test Result -----------------------
            Assert.IsNotNull(inheritanceDef);
            Assert.AreSame(superClass, inheritanceDef.SuperClassClassDef.ClassType);
            Assert.AreEqual("FakeBOSuperClass", inheritanceDef.ClassName);
            Assert.AreEqual(superClass.ToTypeWrapper().AssemblyName, inheritanceDef.AssemblyName);
        }
        [Test]
        public void Test_Map_WhenInheritance_ShouldCreateDiscriminatorPropIfNonExists()
        {
            //---------------Set up test pack-------------------
            Type superClass = typeof(FakeBOSuperClass);
            Type subClass = typeof(FakeBOSubClass);
            //---------------Assert Precondition----------------
            Assert.AreSame(superClass, subClass.BaseType);
            Assert.IsTrue(superClass.ToTypeWrapper().IsBusinessObject);
            //---------------Execute Test ----------------------
            var inheritanceDef = subClass.MapInheritance();
            //---------------Test Result -----------------------
            Assert.AreEqual(ORMapping.SingleTableInheritance, inheritanceDef.ORMapping);
            var superClassClassDef = inheritanceDef.SuperClassClassDef;
            superClassClassDef.PropDefcol.ShouldContain(def => def.PropertyName== inheritanceDef.Discriminator);
        }

        [Test]
        public void Test_Map_When2LayersOfInheritance_ShouldMapInheritenceRelationship()
        {
            //---------------Set up test pack-------------------
            Type superSuperClass = typeof(FakeBOSuperClass);
            Type superClass = typeof(FakeBOSubClass);
            Type subClass = typeof(FakeBOSubSubClass);
            //---------------Assert Precondition----------------
            Assert.AreSame(superClass, subClass.BaseType);
            Assert.AreSame(superSuperClass, superClass.BaseType);
            Assert.IsTrue(superSuperClass.ToTypeWrapper().IsBusinessObject);
            //---------------Execute Test ----------------------
            var inheritanceDef = subClass.MapInheritance();
            //---------------Test Result -----------------------
            Assert.IsNotNull(inheritanceDef);
            var superClassClassDef = inheritanceDef.SuperClassClassDef;
            Assert.AreSame(superClass, superClassClassDef.ClassType);
            Assert.IsNotNull(superClassClassDef.SuperClassDef);
            Assert.AreSame(superSuperClass, superClassClassDef.SuperClassDef.SuperClassClassDef.ClassType);
        }
       
        [Test]
        public void Test_Map_When2LayersOfInheritance_ShouldCreateDiscriminatorOnSuperSuperTypeOnly()
        {
            //---------------Set up test pack-------------------
            Type superSuperClass = typeof(FakeBOSuperClass);
            Type superClass = typeof(FakeBOSubClass);
            Type subClass = typeof(FakeBOSubSubClass);
            //---------------Assert Precondition----------------
            Assert.AreSame(superClass, subClass.BaseType);
            Assert.AreSame(superSuperClass, superClass.BaseType);
            Assert.IsTrue(superSuperClass.ToTypeWrapper().IsBusinessObject);
            //---------------Execute Test ----------------------
            var inheritanceDef = subClass.MapInheritance();
            //---------------Test Result -----------------------
            var superClassClassDef = inheritanceDef.SuperClassClassDef;
            Assert.AreEqual("FakeBOSuperClassType", inheritanceDef.Discriminator);
            ISuperClassDef superClassInheritanceDef = superClassClassDef.SuperClassDef;
            Assert.AreEqual("FakeBOSuperClassType", superClassInheritanceDef.Discriminator);
            var superSuperClassClassDef = superClassInheritanceDef.SuperClassClassDef;
            superClassClassDef.PropDefcol.ShouldHaveCount(0, 
                    "No Properties Should be created for SuperClass since ID and Discriminator will be on SuperSuperClass");
            superSuperClassClassDef.PropDefcol.ShouldHaveCount(2, "Discriminator and ID Prop should be created");
        }

        [Test]
        public void Test_Map_When2LayersOfInheritance_AndHasDiscriminatorProp_ShouldNotCreateDiscriminator()
        {
            //---------------Set up test pack-------------------
            Type superSuperClass = typeof(FakeBOSuperClassWithDesc);
            Type superClass = typeof(FakeBOSubClassSuperHasDesc);
            Type subClass = typeof(FakeBOSubSubClassSuperHasDesc);
            //---------------Assert Precondition----------------
            Assert.AreSame(superClass, subClass.BaseType);
            Assert.AreSame(superSuperClass, superClass.BaseType);
            Assert.IsTrue(superSuperClass.ToTypeWrapper().IsBusinessObject);
            //---------------Execute Test ----------------------
            var inheritanceDef = subClass.MapInheritance();
            //---------------Test Result -----------------------
            var superClassClassDef = inheritanceDef.SuperClassClassDef;
            var superSuperClassClassDef = superClassClassDef.SuperClassDef.SuperClassClassDef;
            superClassClassDef.PropDefcol.ShouldHaveCount(0, 
                    "No Properties Should be created for SuperClass since ID and Discriminator will be on SuperSuperClass");
            IPropDefCol superSuperClassProps = superSuperClassClassDef.PropDefcol;
            superSuperClassProps.ShouldHaveCount(2, "Discriminator and ID Prop should be created");
            superSuperClassProps.ShouldContain(def => def.PropertyName == "FakeBOSuperClassWithDescType");
        }
       
        [Test]
        public void Test_Map_WhenNoInheritance_ShouldRetNull()
        {
            //---------------Set up test pack-------------------
            Type classWithNoSuper = typeof(FakeBoNoProps);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var inheritanceDef = classWithNoSuper.MapInheritance();
            //---------------Test Result -----------------------
            inheritanceDef.ShouldBeNull();
        }

        [Test]
        public void Test_Map_WhenNotBO_ShouldRetNull()
        {
            //---------------Set up test pack-------------------
            Type nonBoSubClassClass = typeof(SomeNonBoClassSubClass);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(nonBoSubClassClass.BaseType);
            Assert.IsFalse(nonBoSubClassClass.ToTypeWrapper().IsBusinessObject);
            //---------------Execute Test ----------------------
            var inheritanceDef = nonBoSubClassClass.MapInheritance();
            //---------------Test Result -----------------------
            inheritanceDef.ShouldBeNull();
        }

    }
    // ReSharper restore InconsistentNaming
}