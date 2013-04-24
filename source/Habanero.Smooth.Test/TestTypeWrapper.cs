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
using Habanero.Base;
using Habanero.BO;
using Habanero.Smooth.ReflectionWrappers;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Smooth.Test
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestTypeWrapper
    {
        [SetUp]
        public void Setup()
        {
            AllClassesAutoMapper.PropNamingConvention = new DefaultPropNamingConventions();
        }

        
        [Test]
        public void Test_IdentityNameConvention_ShouldBeDefaultIfNotSet()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var idConvention = TypeWrapper.PropNamingConvention;
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
            Assert.AreSame(expectedConvention, TypeWrapper.PropNamingConvention);
        }

        #region WrapperProps

        [Test]
        public void Test_ToStringShouldReturnTypeToString()
        {
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            var expectedToString = GetRandomString();
            type.Stub(type1 => type1.ToString()).Return(expectedToString);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedToString, type.ToString());
            //---------------Execute Test ----------------------
            var toString = wrapper.ToString();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedToString, toString);
        }
        [Test]
        public void Test_Name_ShouldReturnTypeName()
        {
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            var expectedName = GetRandomString();
            type.Stub(type1 => type1.Name).Return(expectedName);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedName, type.Name);
            //---------------Execute Test ----------------------
            var name = wrapper.Name;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedName, name);
        }
        [Test]
        public void Test_NameSpace_ShouldReturnTypeNameSpace()
        {
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            var expectedNameSpace = GetRandomString();
            type.Stub(type1 => type1.Namespace).Return(expectedNameSpace);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedNameSpace, type.Namespace);
            //---------------Execute Test ----------------------
            var nameSpace = wrapper.Namespace;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedNameSpace, nameSpace);
        }

        [Test]
        public void Test_IsGenericType_ShouldReturnFromType()
        {
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            var expectedIsGenericType = GetRandomBool();
            type.Stub(type1 => type1.IsGenericType).Return(expectedIsGenericType);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedIsGenericType, type.IsGenericType);
            //---------------Execute Test ----------------------
            var isGenericType = wrapper.IsGenericType;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedIsGenericType, isGenericType);
        }
        [Test]
        public void Test_BaseType_ShouldReturnFromType()
        {
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            var expectedBaseType = typeof(FakeBoNoProps);
            type.Stub(type1 => type1.BaseType).Return(expectedBaseType);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedBaseType, type.BaseType);
            //---------------Execute Test ----------------------
            var baseType = wrapper.BaseType;
            //---------------Test Result -----------------------
            Assert.AreSame(expectedBaseType, baseType.UnderlyingType);
        }
        [Test]
        public void Test_BaseType_WhenNoBaseType_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            type.Stub(type1 => type1.BaseType).Return(null);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.IsNull(type.BaseType);
            //---------------Execute Test ----------------------
            var baseType = wrapper.BaseType;
            //---------------Test Result -----------------------
            Assert.IsNull(baseType);
        }
        [Test]
        public void Test_HasBaseType_WhemHas_ShouldRetTrue()
        {
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            var expectedBaseType = typeof(FakeBoNoProps);
            type.Stub(type1 => type1.BaseType).Return(expectedBaseType);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedBaseType, type.BaseType);
            //---------------Execute Test ----------------------
            var hasBaseType = wrapper.HasBaseType;
            //---------------Test Result -----------------------
            Assert.IsTrue(hasBaseType);
        }
        [Test]
        public void Test_BaseType_WhenNoBaseType_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            type.Stub(type1 => type1.BaseType).Return(null);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.IsNull(type.BaseType);
            //---------------Execute Test ----------------------
            var hasBaseType = wrapper.HasBaseType;
            //---------------Test Result -----------------------
            Assert.IsFalse(hasBaseType);
        }
        [Test]
        public void Test_IsBaseTypeLayerSuperType_WhenInheritsDirectlyFromBO_ShouldReturnTrue()
        {
            //If the Type direclty inherits from BusinessObject
            // Then it inherits from a layer super type.
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            var expectedBaseType = typeof(BusinessObject);
            type.Stub(type1 => type1.BaseType).Return(expectedBaseType);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedBaseType, type.BaseType);
            //---------------Execute Test ----------------------
            var isBaseTypeLayerSuperType = wrapper.IsBaseTypeBusinessObject;
            //---------------Test Result -----------------------
            Assert.IsTrue(isBaseTypeLayerSuperType);
        }
        [Test]
        public void Test_IsBaseTypeLayerSuperType_WhenDoesNotInherit_ShouldReturnFalse()
        {
            //If the Type direclty inherits from BusinessObject
            // Then it inherits from a layer super type.
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            type.Stub(type1 => type1.BaseType).Return(null);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.IsNull(type.BaseType);
            //---------------Execute Test ----------------------
            var isBaseTypeLayerSuperType = wrapper.IsBaseTypeBusinessObject;
            //---------------Test Result -----------------------
            Assert.IsFalse(isBaseTypeLayerSuperType);
        }
        [Test]
        public void Test_IsBaseTypeLayerSuperType_WhenInheritFromAnotherBO_ShouldReturnFalse()
        {
            //If the Type direclty inherits from BusinessObject
            // Then it inherits from a layer super type.
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            type.Stub(type1 => type1.BaseType).Return(typeof(FakeBoNoProps));
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            type.BaseType.AssertIsOfType<IBusinessObject>();
            //---------------Execute Test ----------------------
            var isBaseTypeLayerSuperType = wrapper.IsBaseTypeBusinessObject;
            //---------------Test Result -----------------------
            Assert.IsFalse(isBaseTypeLayerSuperType);
        }
        [Test]
        public void Test_IsBaseTypeLayerSuperType_WhenInheritFromGenericBO_ShouldReturnTrue()
        {
            //If the Type direclty inherits from BusinessObject
            // Then it inherits from a layer super type.
            //---------------Set up test pack-------------------
            var type = typeof(FakeBOGeneric);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            type.BaseType.AssertIsOfType<IBusinessObject>();
            //---------------Execute Test ----------------------
            var isBaseTypeLayerSuperType = wrapper.IsBaseTypeBusinessObject;
            //---------------Test Result -----------------------
            Assert.IsTrue(isBaseTypeLayerSuperType);
        }

        [Test]
        public void Test_IsRealType_WhenAbstract_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeAbstractBoShouldNotBeLoaded);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.IsTrue(type.IsAbstract);
            Assert.IsFalse(type.IsGenericType);
            Assert.IsFalse(type.IsInterface);
            //---------------Execute Test ----------------------
            var isRealClass = wrapper.IsRealClass;
            //---------------Test Result -----------------------
            Assert.IsFalse(isRealClass);
        }
        [Test]
        public void Test_IsRealType_WhenGeneric_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            type.Stub(type1 => type1.IsGenericType).Return(true);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.IsFalse(type.IsAbstract);
            Assert.IsTrue(type.IsGenericType);
            Assert.IsFalse(type.IsInterface);
            //---------------Execute Test ----------------------
            var isRealClass = wrapper.IsRealClass;
            //---------------Test Result -----------------------
            Assert.IsFalse(isRealClass);
        }

        [Test]
        public void Test_IsRealType_WhenIsInterface_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var type = typeof (IFakeBoInterfaceShouldNotBeLoaded);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.IsTrue(type.IsInterface);
            Assert.IsFalse(type.IsGenericType);
//            Assert.IsFalse(type.IsAbstract);
            //---------------Execute Test ----------------------
            var isRealClass = wrapper.IsRealClass;
            //---------------Test Result -----------------------
            Assert.IsFalse(isRealClass);
        }

        [Test]
        public void Test_GetGenericArguments_ShouldReturnFromType()
        {
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            var expectedGetGenericArguments = GetRandomTypeArray();
            type.Stub(type1 => type1.GetGenericArguments()).Return(expectedGetGenericArguments);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedGetGenericArguments.Count(), type.GetGenericArguments().Count());
            Assert.AreSame(expectedGetGenericArguments, type.GetGenericArguments());
            //---------------Execute Test ----------------------
            var getGenericArguments = wrapper.GetGenericArguments();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedGetGenericArguments.Count(), getGenericArguments.Count());
        }

        [Test]
        public void Test_GetProperty_ShouldReturnFromType()
        {
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            var propName = GetRandomString();
            var expectedGetProperty = type.SetProperty(propName);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.AreSame(expectedGetProperty, type.GetProperty(propName));
            //---------------Execute Test ----------------------
            var getProperty = wrapper.GetProperty(propName);
            //---------------Test Result -----------------------
            Assert.AreSame(expectedGetProperty, getProperty.PropertyInfo);
        }
        [Test]
        public void Test_HasProperty_WhenHas_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            var propName = GetRandomString();
            type.SetProperty(propName);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(type.GetProperty(propName));
            Assert.IsNotNull(wrapper.GetProperty(propName));
            //---------------Execute Test ----------------------
            var hasProperty = wrapper.HasProperty(propName);
            //---------------Test Result -----------------------
            Assert.IsTrue(hasProperty);
        }
        [Test]
        public void Test_HasProperty_WhenNotHas_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            var propName = GetRandomString();
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.IsNull(type.GetProperty(propName));
            Assert.IsNull(wrapper.GetProperty(propName));
            //---------------Execute Test ----------------------
            var hasProperty = wrapper.HasProperty(propName);
            //---------------Test Result -----------------------
            Assert.IsFalse(hasProperty);
        }

        [Test]
        public void Test_IsOfType_WhenInnerTypeIsOfType_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var type = typeof (FakeBOWProps);
            var parentType = typeof (IBusinessObject);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.IsTrue(parentType.IsAssignableFrom(type), string.Format("{0} should inherit from {1}", type, parentType));
            //---------------Execute Test ----------------------
            var isOfType = wrapper.IsOfType(parentType);
            //---------------Test Result -----------------------
            Assert.IsTrue(isOfType);
        }

        [Test]
        public void Test_IsOfTypeGeneric_WhenInnerTypeIsOfType_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var type = typeof (FakeBOWProps);
            var parentType = typeof (IBusinessObject);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.IsTrue(parentType.IsAssignableFrom(type), string.Format("{0} should inherit from {1}", type, parentType));
            //---------------Execute Test ----------------------
            var isOfType = wrapper.IsOfType<IBusinessObject>();
            //---------------Test Result -----------------------
            Assert.IsTrue(isOfType);
        }

        [Test]
        public void Test_IsOfType_WhenInnerTypeNotIsOfType_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var type = typeof (FakeBOWProps);
            var parentType = typeof (string);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.IsFalse(parentType.IsAssignableFrom(type), string.Format("{0} should not inherit from {1}", type, parentType));
            //---------------Execute Test ----------------------
            var isOfType = wrapper.IsOfType(parentType);
            //---------------Test Result -----------------------
            Assert.IsFalse(isOfType);
        }

        [Test]
        public void Test_GetProperties_ShouldReturnFromType()
        {
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            var expectedGetPropertiess = GetRandomPropInfoArray();
            type.Stub(type1 => type1.GetProperties()).Return(expectedGetPropertiess);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedGetPropertiess.Count(), type.GetProperties().Count());
            Assert.AreSame(expectedGetPropertiess, type.GetProperties());
            //---------------Execute Test ----------------------
            var getProperties = wrapper.GetProperties();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedGetPropertiess.Count(), getProperties.Count());
        }
        [Test]
        public void Test_GetUnderlyingSystemType_ShouldReturnType()
        {
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var underlyingType = wrapper.UnderlyingType;
            //---------------Test Result -----------------------
            Assert.AreSame(type, underlyingType);
        }


        [Test]
        public void Test_AssemblyQualifiedName_ShouldReturnFromType()
        {
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            var expectedAssemblyQualifiedName = GetRandomString();
            type.Stub(type1 => type1.AssemblyQualifiedName).Return(expectedAssemblyQualifiedName);
            TypeWrapper wrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedAssemblyQualifiedName, type.AssemblyQualifiedName);
            //---------------Execute Test ----------------------
            var name = wrapper.AssemblyQualifiedName;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedAssemblyQualifiedName, name);
        }
        [Test]
        public void Test_MakeGenericBusinessObjectCollection_ShouldReturnBOColForInnerType()
        {
            //---------------Set up test pack-------------------
            var type = MockRepository.GenerateMock<Type>();
            TypeWrapper wrapper = new TypeWrapper(type);
            var expectedGenericType = typeof(BusinessObjectCollection<>).MakeGenericType(type);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var newType = wrapper.MakeGenericBusinessObjectCollection();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedGenericType.Name, newType.Name);
        }

        [Test]
        public void Test_GetPKPropName_WhenStdNamingPropNonExist_WhenUsePrimaryKeyAttribute_ShouldReturnAttributeProp()
        {
            //---------------Set up test pack-------------------
            Type classType = typeof(FakeBOAttributePK);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var pkPropName = classType.ToTypeWrapper().GetPKPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual("PublicGuidProp", pkPropName);
        }
        [Test]
        public void Test_GetPKPropName_WhenStdNamingPropNonExist_WhenNoAttribute_ShouldRetStdNamePropName()
        {
            //---------------Set up test pack-------------------
            Type classType = typeof(FakeBoNoProps);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var pkPropName = classType.ToTypeWrapper().GetPKPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual(GetFKPropName(classType), pkPropName);
        }
        [Test]
        public void Test_GetPKPropName_WhenNonStdNaming_WhenNoAttribute_ShouldRetNonStdNamePropName()
        {
            //---------------Set up test pack-------------------
            Type classType = typeof(FakeBoNoProps);
            AllClassesAutoMapper.PropNamingConvention = new FakeConvetion();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var pkPropName = classType.ToTypeWrapper().GetPKPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual(GetFKPropName(classType), pkPropName);
        }
        [Test]
        public void Test_GetPKPropName_WhenInheritance_ShouldBasePropName()
        {
            //---------------Set up test pack-------------------
            Type classType = typeof(FakeBOSubClass);
            var typeWrapper = classType.ToTypeWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(typeWrapper.HasBaseType);
            Assert.IsFalse(typeWrapper.IsBaseTypeBusinessObject);
            //---------------Execute Test ----------------------
            var pkPropName = typeWrapper.GetPKPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual("FakeBOSuperClassID", pkPropName);
        }

        [Test]
        public void Test_GetPKPropName_WhenInheritsFromGenericBO_ShouldUseGenericTypeToDetermineName()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeBOGeneric);
            var typeWrapper = classType.ToTypeWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(typeWrapper.BaseType.IsGenericType);
            //---------------Execute Test ----------------------
            var pkPropName = typeWrapper.GetPKPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual("FakeBOGenericID", pkPropName);
        }

        [Test]
        public void Test_GetPKPropName_WhenInheritsFromGenericBOSuperType_ShouldNotUseBasePropToDetermineName()
        {
            //---------------Set up test pack-------------------
            var classType = typeof(FakeGenericBOSubType);
            var typeWrapper = classType.ToTypeWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(typeWrapper.BaseType.IsGenericType);
            //---------------Execute Test ----------------------
            var pkPropName = typeWrapper.GetPKPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual("FakeGenericBOSubTypeID", pkPropName);
        }

        [Test]
        public void Test_GetPKPropName_WhenSubClass_WhenIDPropDeclaredInClassDefXml_ShouldReturnDefinedClassDef_FixBug1355()
        {
            //---------------Set up test pack-------------------
            
            var superClassDef = SuperClassWithPKFromClassDef.LoadClassDef();//Loaded from XML
            var defCol = new ClassDefCol { superClassDef };

            var superClassWithPKFromClassDef = typeof(SubClassWithPKFromClassDef);

            AllClassesAutoMapper.ClassDefCol = defCol;
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var pkPropName = superClassWithPKFromClassDef.ToTypeWrapper().GetPKPropName();
            //---------------Test Result -----------------------
            Assert.AreEqual("MYPKID", pkPropName);
        }

        [Test]
        public void Test_IsBusinessObject_WhenTypeImplementsIBo_ShouldRetTrue()
        {
            //---------------Set up test pack-------------------
            Type classType = typeof(FakeBoNoProps);
            var typeWrapper = classType.ToTypeWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(typeof(IBusinessObject).IsAssignableFrom(classType));
            //---------------Execute Test ----------------------
            var isBusinessObject = typeWrapper.IsBusinessObject;
            //---------------Test Result -----------------------
            Assert.IsTrue(isBusinessObject);
        }
        [Test]
        public void Test_IsBusinessObject_WhenTypeNotImplementsIBo_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            Type classType = typeof(SomeNonBoClass);
            var typeWrapper = classType.ToTypeWrapper();
            //---------------Assert Precondition----------------
            Assert.IsFalse(typeof(IBusinessObject).IsAssignableFrom(classType));
            //---------------Execute Test ----------------------
            var isBusinessObject = typeWrapper.IsBusinessObject;
            //---------------Test Result -----------------------
            Assert.IsFalse(isBusinessObject);
        }
        [Test]
        public void Test_IsGenericBusinessObject_WhenTypeNotImplementsIBo_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            Type classType = typeof(SomeNonBoClass);
            var typeWrapper = classType.ToTypeWrapper();
            //---------------Assert Precondition----------------
            Assert.IsFalse(typeof(IBusinessObject).IsAssignableFrom(classType));
            //---------------Execute Test ----------------------
            var isGenericBusinessObject = typeWrapper.IsGenericBusinessObject;
            //---------------Test Result -----------------------
            Assert.IsFalse(isGenericBusinessObject);
        }
        [Test]
        public void Test_IsGenericBusinessObject_WhenTypeImplementsIBo_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            Type classType = typeof(FakeBoNoProps);
            var typeWrapper = classType.ToTypeWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(typeof(IBusinessObject).IsAssignableFrom(classType));
            //---------------Execute Test ----------------------
            var isGenericBusinessObject = typeWrapper.IsGenericBusinessObject;
            //---------------Test Result -----------------------
            Assert.IsFalse(isGenericBusinessObject);
        }
        [Test]
        public void Test_IsGenericBusinessObject_WhenTypeImplementsGenericBO_ShouldRetTrue()
        {
            //---------------Set up test pack-------------------
            Type classType = typeof(FakeBOGeneric);
            var typeWrapper = classType.ToTypeWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(typeof(IBusinessObject).IsAssignableFrom(classType));
            //---------------Execute Test ----------------------
            var isGenericBusinessObject = typeWrapper.IsGenericBusinessObject;
            //---------------Test Result -----------------------
            Assert.IsTrue(isGenericBusinessObject);
        }
        [Test]
        public void Test_IsGenericBusinessObject_WhenTypeInheritsFromTypeThatImplementsGenericBO_ShouldRetTrue()
        {
            //---------------Set up test pack-------------------
            Type classType = typeof(FakeBOGenericSubType);
            var typeWrapper = classType.ToTypeWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(typeof(IBusinessObject).IsAssignableFrom(classType));
            //---------------Execute Test ----------------------
            var isGenericBusinessObject = typeWrapper.IsGenericBusinessObject;
            //---------------Test Result -----------------------
            Assert.IsTrue(isGenericBusinessObject);
        }

//        FakeBOGeneric
        [Test]
        public void Test_HasIgnoreAttribute_WhenHas_ShouldRetTrue()
        {
            //---------------Set up test pack-------------------
            Type classType = typeof(FakeBoIgnore);
            var typeWrapper = classType.ToTypeWrapper();
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, classType.GetCustomAttributes(typeof(AutoMapIgnoreAttribute), true).Count());
            //---------------Execute Test ----------------------
            var hasIgnoreAttribute = typeWrapper.HasIgnoreAttribute;
            //---------------Test Result -----------------------
            Assert.IsTrue(hasIgnoreAttribute);
        }

        [Test] public void Test_HasIgnoreAttribute_WhenNotHas_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            Type classType = typeof(FakeBoWithNoSingleReverse);
            var typeWrapper = classType.ToTypeWrapper();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, classType.GetCustomAttributes(typeof(AutoMapIgnoreAttribute), true).Count());
            //---------------Execute Test ----------------------
            var hasIgnoreAttribute = typeWrapper.HasIgnoreAttribute;
            //---------------Test Result -----------------------
            Assert.IsFalse(hasIgnoreAttribute);
        }

        [Test] public void Test_IsNullableType_WhenIs_ShouldRetTrue()
        {
            //---------------Set up test pack-------------------
            Type type = typeof(Guid?);
            var typeWrapper = type.ToTypeWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(type.IsGenericType);
            Assert.IsTrue(type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
            //---------------Execute Test ----------------------
            var isNullableType = typeWrapper.IsNullableType;
            //---------------Test Result -----------------------
            Assert.IsTrue(isNullableType);
        }
        [Test] public void Test_IsNullableType_WhenNot_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            Type type = typeof(Guid);
            var typeWrapper = type.ToTypeWrapper();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var isNullableType = typeWrapper.IsNullableType;
            //---------------Test Result -----------------------
            Assert.IsFalse(isNullableType);
        }
        [Test]
        public void Test_GetNullableUndelyingType_WhenIs_ShouldRetGuid()
        {
            //---------------Set up test pack-------------------
            Type type = typeof(Guid?);
            var typeWrapper = type.ToTypeWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(type.IsGenericType);
            Assert.IsTrue(type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
            //---------------Execute Test ----------------------
            var undelyingType = typeWrapper.GetNullableUndelyingType();
            //---------------Test Result -----------------------
            Assert.AreSame(typeof(Guid), undelyingType);
        }
        [Test]
        public void Test_GetNullableUndelyingType_WhenNot_ShouldRetGuid()
        {
            //---------------Set up test pack-------------------
            Type type = typeof(Guid);
            var typeWrapper = type.ToTypeWrapper();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var undelyingType = typeWrapper.GetNullableUndelyingType();
            //---------------Test Result -----------------------
            Assert.AreSame(typeof(Guid), undelyingType);
        }

        [Test]
        public void Test_AssemblyName_ReturnsCorrectName()
        {
            //---------------Set up test pack-------------------
            Type type = typeof(FakeBONoPK);
            var typeWrapper = type.ToTypeWrapper();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var assemblyName = typeWrapper.AssemblyName;
            //---------------Test Result -----------------------
            Assert.AreEqual("Habanero.Smooth.Test", assemblyName);
        }

        [Test]
        public void Test_GetAttribute_WhenHasAttribute_ShouldReturnAttribute()
        {
            //---------------Set up test pack-------------------
            const string tableName = "tbMyFakeBo";
            Type type = typeof (FakeBoWithTableName);
            var typeWrapper = type.ToTypeWrapper();
            //---------------Assert Precondition----------------
            
            //---------------Execute Test ----------------------
            var attribute = typeWrapper.GetAttribute<AutoMapTableNameAttribute>();
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<AutoMapTableNameAttribute>(attribute);
            Assert.AreEqual(tableName,attribute.TableName);
        }

        [Test]
        public void Test_GetAttribute_WhenHasNoAttribute_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            Type type = typeof(FakeBoWithoutTableName);
            var typeWrapper = type.ToTypeWrapper();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var attribute = typeWrapper.GetAttribute<AutoMapTableNameAttribute>();
            //---------------Test Result -----------------------
             Assert.IsNull(attribute);
        }

        [Test]
        public void Test_TableName_ReturnsCorrectName()
        {
            //---------------Set up test pack-------------------
            string tableName = "tbMyFakeBo";
            Type type = typeof(FakeBoWithTableName);
            var typeWrapper = type.ToTypeWrapper();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var mappedTableName = typeWrapper.GetTableName();
            //---------------Test Result -----------------------
            Assert.AreEqual(tableName, mappedTableName);
        }

        [Test]
        public void Test_TableName_WhenAttributeNotDefined_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            Type type = typeof(FakeBoWithoutTableName);
            var typeWrapper = type.ToTypeWrapper();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var mappedTableName = typeWrapper.GetTableName();
            //---------------Test Result -----------------------
            Assert.IsNull(mappedTableName);
        }




        private static string GetFKPropName(Type classType)
        {
            var idConvention = TypeWrapper.PropNamingConvention;
            return idConvention.GetIDPropertyName(classType.ToTypeWrapper());
        }
        private static string GetRandomString()
        {
            return RandomValueGenerator.GetRandomString();
        }

        private static bool GetRandomBool()
        {
            return RandomValueGenerator.GetRandomBoolean();
        }

        private static Type[] GetRandomTypeArray()
        {
            var randomInt = RandomValueGenerator.GetRandomInt(1, 3);
            Type[] types = new Type[randomInt];
            for (int i = 0; i < randomInt; i++)
            {
                types[i] = MockRepository.GenerateMock<Type>();
            }
            return types;
        }
        private static PropertyInfo[] GetRandomPropInfoArray()
        {
            var randomInt = RandomValueGenerator.GetRandomInt(1, 3);
            PropertyInfo[] types = new PropertyInfo[randomInt];
            for (int i = 0; i < randomInt; i++)
            {
                types[i] = MockRepository.GenerateMock<PropertyInfo>();
            }
            return types;
        }


        #endregion

        #region TestEquality

        [TestCase(1, false)]
        [TestCase(typeof(FakeEnum), true)]
        [TestCase("FakeEnum", true)]
        [TestCase("fdafdfasdfa", false)]
        [TestCase(typeof(FakeBoNoProps), false)]
        [TestCase(null, false)]
        [TestCase((Type)null, false)]
        [TestCase((string)null, false)]
        public void Test_Equals(object other, bool expectedResult)
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeEnum);
            var typeWrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = typeWrapper.Equals(other);
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedResult, isEqual
                    , string.Format("'{0}' Equals '{1}' should be '{2}'"
                    , other, typeWrapper, expectedResult));
        }
        [TestCase(typeof(FakeBoNoProps), false)]
        [TestCase(typeof(FakeEnum), true)]
        [TestCase(null, false)]
        public void Test_Equals_WhenType(Type otherType, bool expectedResult)
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeEnum);
            TypeWrapper typeWrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = typeWrapper.Equals(otherType);
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedResult, isEqual
                , string.Format("'{0}' Equals '{1}' should be '{2}'"
                , otherType, typeWrapper, expectedResult));
        }
        [Test]
        public void Test_Equals_WhenSelf_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeEnum);
            TypeWrapper typeWrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = typeWrapper.Equals(typeWrapper);
            //---------------Test Result -----------------------
            Assert.IsTrue(isEqual);
        }
        [Test]
        public void Test_Equals_WhenSecondRefToSelf_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeEnum);
            TypeWrapper typeWrapper = new TypeWrapper(type);
            TypeWrapper typeWrapperSecond = typeWrapper;
            //---------------Assert Precondition----------------
            Assert.AreSame(typeWrapperSecond, typeWrapper);
            //---------------Execute Test ----------------------
            var isEqual = typeWrapper.Equals(typeWrapperSecond);
            //---------------Test Result -----------------------
            Assert.IsTrue(isEqual);
        }
        [Test]
        public void Test_Equals_WhenSelfObject_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeEnum);
            TypeWrapper typeWrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = typeWrapper.Equals((object)typeWrapper);
            //---------------Test Result -----------------------
            Assert.IsTrue(isEqual);
        }

        // ReSharper disable ExpressionIsAlwaysNull
        [Test]
        public void Test_Equals_WhenStringNull_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeEnum);
            var typeWrapper = new TypeWrapper(type);
            string someString = null;
            //---------------Assert Precondition----------------
            Assert.IsNull(someString);
            //---------------Execute Test ----------------------
            var isEqual = typeWrapper.Equals(someString);
            //---------------Test Result -----------------------
            Assert.IsFalse(isEqual);
        }
        // ReSharper restore ExpressionIsAlwaysNull
        [TestCase(typeof(FakeBoNoProps), false)]
        [TestCase(typeof(FakeEnum), true)]
        public void Test_Equals_WhenTypeWrapperWrapsType(Type otherType, bool expectedResult)
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeEnum);
            var typeWrapper = new TypeWrapper(type);
            var otherTypeWrapper = new TypeWrapper(otherType);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = typeWrapper.Equals(otherTypeWrapper);
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedResult, isEqual);
        }
        [TestCase(typeof(FakeEnum), true)]
        [TestCase(typeof(FakeBoNoProps), false)]
        [TestCase(null, false)]
        public void Test_EqualEquals(Type otherType, bool expectedResult)
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeEnum);
            TypeWrapper typeWrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = typeWrapper == otherType;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedResult, isEqual);
        }
        [TestCase(typeof(FakeEnum), true)]
        [TestCase(typeof(FakeBoNoProps), false)]
        [TestCase(null, false)]
        public void Test_TypeEqualEquals(Type otherType, bool expectedResult)
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeEnum);
            TypeWrapper typeWrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = otherType == typeWrapper ;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedResult, isEqual);
        }
        [TestCase(typeof(FakeEnum), false)]
        [TestCase(typeof(FakeBoNoProps), true)]
        [TestCase(null, true)]
        public void Test_NotEqual(Type otherType, bool expectedResult)
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeEnum);
            TypeWrapper typeWrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = typeWrapper != otherType;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedResult, isEqual);
        }
        [TestCase(typeof(FakeEnum), false)]
        [TestCase(typeof(FakeBoNoProps), true)]
        [TestCase(null, true)]
        public void Test_TypeNotEquals(Type otherType, bool expectedResult)
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeEnum);
            TypeWrapper typeWrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = otherType != typeWrapper ;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedResult, isEqual);
        }

        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        [Test]
        public void Test_EqualEquals_WhenBothNullTypeWrapper_ShouldRetTrue()
        {
            //---------------Set up test pack-------------------
            TypeWrapper typeWrapper = null;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = typeWrapper == (TypeWrapper)null;

            //---------------Test Result -----------------------
            Assert.IsTrue(isEqual);
        }
        [Test]
        public void Test_EqualEquals_WhenNullTypeWrapper_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeEnum);
            var typeWrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEqual = typeWrapper == (TypeWrapper)null;
            //---------------Test Result -----------------------
            Assert.IsFalse(isEqual);
        }
        [Test]
        public void Test_NotEquals_WhenNullTypeWrapper_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeEnum);
            var typeWrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isNotEqual = typeWrapper != (TypeWrapper)null;
            //---------------Test Result -----------------------
            Assert.IsTrue(isNotEqual);
        }
        // ReSharper restore ConditionIsAlwaysTrueOrFalse
        [Test]
        public void Test_GetHashCode_ShouldReturnCalculatedCode()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeEnum);
            var expectedHashCode = type.GetHashCode() * 397 ^ type.Name.GetHashCode();
            var typeWrapper = new TypeWrapper(type);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var hashCode = typeWrapper.GetHashCode();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedHashCode, hashCode);
        }
#endregion

        [Test]
        public void Test_IsEnumType_WhenIsEnum_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var enumType = typeof (FakeEnum).ToTypeWrapper();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var isEnumType = enumType.IsEnumType();
            //---------------Test Result -----------------------
            Assert.IsTrue(isEnumType, "Should be enum");
        }
        [Test]
        public void Test_IsEnumType_WhenIsNullableEnum_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var enumType = typeof(FakeEnum?).ToTypeWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(enumType.IsNullableType);
            //---------------Execute Test ----------------------
            var isEnumType = enumType.IsEnumType();
            //---------------Test Result -----------------------
            Assert.IsTrue(isEnumType, "Should be enum");
        }
    }
}