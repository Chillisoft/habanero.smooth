using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using NUnit.Framework;

namespace Habanero.Smooth.Test
{
    [TestFixture]
    public class TestClassDefBuilder
    {
        [Test]
        public void Test_CreateClassDef_WithAssemblyClassName()
        {
            //---------------Set up test pack-------------------
            string assemblyName = "A" + RandomValueGenerator.GetRandomString();
            string className = "C" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            ClassDef classDef = new ClassDefBuilder()
                    .WithAssemblyName(assemblyName)
                    .WithClassName(className);
            //---------------Test Result -----------------------
            Assert.AreEqual(assemblyName, classDef.AssemblyName);
            Assert.AreEqual(className, classDef.ClassName);
            Assert.IsNull(classDef.PropDefcol);
            Assert.IsNull(classDef.PrimaryKeyDef);
            Assert.IsNull(classDef.KeysCol);
            Assert.IsNull(classDef.RelationshipDefCol);
            Assert.AreEqual(0, classDef.UIDefCol.Count());
        }

        //[Test]
        //public void Test_CreateClassDef_WithOnePropDef()
        //{
        //    //---------------Set up test pack-------------------
        //    string assemblyName = "A" + RandomValueGenerator.GetRandomString();
        //    string className = "C" + RandomValueGenerator.GetRandomString();
        //    string propertyName = "P" + RandomValueGenerator.GetRandomString();
        //    //---------------Assert Precondition----------------

        //    //---------------Execute Test ----------------------
        //    var classDefBuilder = new ClassDefBuilder();
        //    ClassDef classDef = classDefBuilder.WithAssemblyName(assemblyName).WithClassName(className).AddPropDef(propertyName);
        //    //---------------Test Result -----------------------
        //    Assert.AreEqual(assemblyName, classDef.AssemblyName);
        //    Assert.AreEqual(className, classDef.ClassName);
        //    Assert.IsNotNull(classDef.PropDefcol);
        //    Assert.AreEqual(1, classDef.PropDefcol.Count());
        //    Assert.IsNotNull(classDef.PropDefcol[propertyName]);
        //    Assert.IsNull(classDef.PrimaryKeyDef);
        //    Assert.IsNull(classDef.KeysCol);
        //    Assert.IsNull(classDef.RelationshipDefCol);
        //    Assert.AreEqual(0, classDef.UIDefCol.Count());
        //}

        [Test]
        public void Test_CreateClassDef_AddOnePropDef()
        {
            //---------------Set up test pack-------------------
            string assemblyName = "A" + RandomValueGenerator.GetRandomString();
            string className = "C" + RandomValueGenerator.GetRandomString();
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var classDefBuilder = new ClassDefBuilder();
            ClassDef classDef = classDefBuilder.WithAssemblyName(assemblyName).WithClassName(className)
                            .AddPropDef(new PropDefBuilder()
                                        .WithPropertyName(propertyName));
            //---------------Test Result -----------------------
            Assert.AreEqual(assemblyName, classDef.AssemblyName);
            Assert.AreEqual(className, classDef.ClassName);
            Assert.IsNotNull(classDef.PropDefcol);
            Assert.AreEqual(1, classDef.PropDefcol.Count());
            Assert.IsNotNull(classDef.PropDefcol[propertyName]);
            Assert.IsNull(classDef.PrimaryKeyDef);
            Assert.IsNull(classDef.KeysCol);
            Assert.IsNull(classDef.RelationshipDefCol);
            Assert.AreEqual(0, classDef.UIDefCol.Count());
        }

    }


    public class ClassDefBuilder
    {

        internal string ClassAssemblyName { get; set; }
        internal string ClassName { get; set; }
        internal static PropDefCol _propDefCol;

        public ClassDefBuilder WithClassName(string className)
        {
            ClassName = className;
            return this;

        }

        public ClassDefBuilder WithAssemblyName(string assemblyName)
        {
            ClassAssemblyName = assemblyName;
            return this;
        }

        public static implicit operator ClassDef(ClassDefBuilder builder)
        {
            return new ClassDef(builder.ClassAssemblyName, builder.ClassName, null, _propDefCol, null, null, null);
        }



        //public ClassDefBuilder AddPropDef(string propertyName)
        //{
        //    PropDef propDef = new PropDefBuilder().WithPropertyName(propertyName);
        //    if (_propDefCol == null) _propDefCol = new PropDefCol();
        //    _propDefCol.Add(propDef);
        //    return this;
        //}

        public ClassDefBuilder AddPropDef(PropDef propDef)
        {
           if (_propDefCol == null) _propDefCol = new PropDefCol();
            _propDefCol.Add(propDef);
            return this;
        }



    }

    [TestFixture]
    public class TestPropDefBuilder
    {
        [Test]
        public void Test_CreatePropDef_WithPropertyName()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            PropDef propDef = new PropDefBuilder()
                .WithPropertyName(propertyName);

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.AreEqual("System", propDef.PropertyTypeAssemblyName);
            Assert.AreEqual("String", propDef.PropertyTypeName);
            Assert.AreEqual(PropReadWriteRule.ReadWrite, propDef.ReadWriteRule);
            Assert.AreEqual(propertyName, propDef.DatabaseFieldName);
            Assert.IsNull(propDef.DefaultValueString);
            Assert.IsFalse(propDef.Compulsory);
            Assert.IsFalse(propDef.AutoIncrementing);
            Assert.IsFalse(propDef.KeepValuePrivate);
        }

        [Test]
        public void Test_CreatePropDef_WithAssemblyName()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            string assemblyName = "A" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            PropDef propDef = new PropDefBuilder()
                .WithPropertyName(propertyName)
                .WithAssemblyName(assemblyName);

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.AreEqual(assemblyName, propDef.PropertyTypeAssemblyName);
        }

        [Test]
        public void Test_CreatePropDef_WithTypeName()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            string typeName = "T" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            PropDef propDef = new PropDefBuilder()
                .WithPropertyName(propertyName)
                .WithTypeName(typeName);

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.AreEqual(typeName, propDef.PropertyTypeName);
        }


        [Test]
        public void Test_CreatePropDef_WithReadWriteRule()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            const PropReadWriteRule propReadWriteRule = PropReadWriteRule.WriteNew;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            PropDef propDef = new PropDefBuilder()
                .WithPropertyName(propertyName)
                .WithReadWriteRule(propReadWriteRule);

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.AreEqual(propReadWriteRule, propDef.ReadWriteRule);
        }

        [Test]
        public void Test_CreatePropDef_WithDatabaseFieldName()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            string fieldName = "F" + RandomValueGenerator.GetRandomString();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            PropDef propDef = new PropDefBuilder()
                .WithPropertyName(propertyName)
                .WithDatabaseFieldName(fieldName);

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.AreEqual(fieldName, propDef.DatabaseFieldName);
        }

        [Test]
        public void Test_CreatePropDef_WithDefaultValue()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            string defaultValue = "V" + RandomValueGenerator.GetRandomString();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            PropDef propDef = new PropDefBuilder()
                .WithPropertyName(propertyName)
                .WithDefaultValue(defaultValue);

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.AreEqual(defaultValue, propDef.DefaultValueString);
        }


        [Test]
        public void Test_CreatePropDef_WithCompulsoryProp()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            PropDef propDef = new PropDefBuilder()
                .WithPropertyName(propertyName)
                .IsCompulsory();

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.IsTrue(propDef.Compulsory);
        }

        [Test]
        public void Test_CreatePropDef_WithAutoIncrementingProp()
        {
            //---------------Set up test pack-------------------
            string propertyName = "P" + RandomValueGenerator.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            PropDef propDef = new PropDefBuilder()
                .WithPropertyName(propertyName)
                .IsAutoIncrementing();

            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, propDef.PropertyName);
            Assert.IsTrue(propDef.AutoIncrementing);
        }

    }

    public class PropDefBuilder
    {
        private static bool _isCompulsory;
        private static bool _isAutoIncrementing;

        internal string PropertyName { get; set; }
        internal static string PropertyTypeAssemblyName { get; set; }
        internal static string PropertyTypeName { get; set; }
        internal static PropReadWriteRule ReadWriteRule { get; set; }

        static PropDefBuilder()
        {
            PropertyTypeAssemblyName = "System";
            PropertyTypeName = "String";
            _isCompulsory = false;
        }


        public PropDefBuilder WithPropertyName(string propertyName)
        {
            PropertyName = propertyName;
            return this;

        }

        public static implicit operator PropDef(PropDefBuilder builder)
        {
            return new PropDef(builder.PropertyName, PropertyTypeAssemblyName, PropertyTypeName, ReadWriteRule, DatabaseFieldName, DefaultValueString, _isCompulsory, _isAutoIncrementing);
        }


        public PropDef IsCompulsory()
        {
            _isCompulsory = true;
            return this;
        }

        public PropDef WithAssemblyName(string assemblyName)
        {
            PropertyTypeAssemblyName = assemblyName;
            return this;
        }

        public PropDef WithTypeName(string typeName)
        {
            PropertyTypeName = typeName;
            return this;
        }

        public PropDef WithReadWriteRule(PropReadWriteRule propReadWriteRule)
        {
            ReadWriteRule = propReadWriteRule;
            return this;
        }

        public PropDef WithDatabaseFieldName(string fieldName)
        {
            DatabaseFieldName = fieldName;
            return this;
        }

        internal static string DatabaseFieldName { get; set; }

        public PropDef WithDefaultValue(string defaultValue)
        {
            DefaultValueString = defaultValue;
            return this;  
        }

        internal static string DefaultValueString { get; set; }

        public PropDef IsAutoIncrementing()
        {
            _isAutoIncrementing = true;
            return this;

        }
    }
}
