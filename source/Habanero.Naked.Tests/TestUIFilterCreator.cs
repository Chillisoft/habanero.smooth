#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
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
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using NUnit.Framework;

namespace Habanero.Naked.Tests
{
    [TestFixture]
    public class TestUIFilterCreator
    {
        protected virtual IDefClassFactory GetFactory()
        {
            return new DefClassFactory();
        }

        [Test]
        public void Test_Construct_ShouldConstruct()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var uiFilterCreator = new UIFilterCreator(GetFactory());
            //---------------Test Result -----------------------
            Assert.IsNotNull(uiFilterCreator);
        }

        [Test]
        public void Test_CreateUIFilter_ShouldReturnIFilterDef()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();
            IClassDef classDef = typeof(FakeBo).MapClass();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var filterDef = uiFilterCreator.CreateUIFilter(classDef);
            //---------------Test Result -----------------------
            Assert.IsNotNull(filterDef);
            Assert.IsInstanceOf<IFilterDef>(filterDef);
        }

        [Test]
        public void Test_CreateFilterDef_WhenClassDefHasOnePropAndOneIdentityProp_ShouldCreateFilterDefSingleFilterPropertyDef()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();
            IClassDef classDef = typeof(FakeBo).MapClass();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, classDef.UIDefCol.Count);
            //---------------Execute Test ----------------------
            var filterDef = uiFilterCreator.CreateUIFilter(classDef);
            //---------------Test Result -----------------------
            Assert.AreEqual(1, filterDef.FilterPropertyDefs.Count);
            Assert.AreEqual("Fake Bo Name", filterDef.FilterPropertyDefs[0].Label);
            Assert.AreEqual("FakeBoName", filterDef.FilterPropertyDefs[0].PropertyName);
        }

        [Test]
        public void Test_CreateFilterDef_WhenClassDefHasTwoProps_ShouldCreateFilterDefTwoFilterPropertyDefs()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();
            IClassDef classDef = typeof(FakeBoW2Props).MapClass();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, classDef.UIDefCol.Count);
            Assert.AreEqual(3, classDef.PropDefcol.Count, "2 Props and IDProp");
            //---------------Execute Test ----------------------
            var filterDef = uiFilterCreator.CreateUIFilter(classDef);
            //---------------Test Result -----------------------
            Assert.AreEqual(2, filterDef.FilterPropertyDefs.Count);

            Assert.AreEqual("Fake Bo Name", filterDef.FilterPropertyDefs[0].Label);
            Assert.AreEqual("FakeBoName", filterDef.FilterPropertyDefs[0].PropertyName);

            Assert.AreEqual("Fake Bo Name 2", filterDef.FilterPropertyDefs[1].Label);
            Assert.AreEqual("FakeBoName2", filterDef.FilterPropertyDefs[1].PropertyName);
        }

        [Test]
        public void Test_CreateFilterDef_WhenClassDefHasTwoProps_ShouldSetAppropriateControlTypes()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();
            IClassDef classDef = typeof(FakeBoW2Props).MapClass();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, classDef.UIDefCol.Count);
            Assert.AreEqual(3, classDef.PropDefcol.Count, "2 Props and IDProp");
            //---------------Execute Test ----------------------
            var filterDef = uiFilterCreator.CreateUIFilter(classDef);
            //---------------Test Result -----------------------
            Assert.AreEqual(2, filterDef.FilterPropertyDefs.Count);

            //AssertIsControlOfType("TextBox", "System.Windows.Forms", filterDef.FilterPropertyDefs[0]);
            //AssertIsControlOfType("CheckBox", "System.Windows.Forms", filterDef.FilterPropertyDefs[1]);
            AssertIsControlOfType("StringTextBoxFilter", "Habanero.Faces.Base", filterDef.FilterPropertyDefs[0]);
            AssertIsControlOfType("BoolCheckBoxFilter", "Habanero.Faces.Base", filterDef.FilterPropertyDefs[1]);
        }

        [Test]
        public void Test_CreateFilterDef_WhenClassDefHasTwoProps_ShouldSetAppropriateFilterClauseOperator()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();
            IClassDef classDef = typeof(FakeBoW2Props).MapClass();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, classDef.UIDefCol.Count);
            Assert.AreEqual(3, classDef.PropDefcol.Count, "2 Props and IDProp");
            //---------------Execute Test ----------------------
            var filterDef = uiFilterCreator.CreateUIFilter(classDef);
            //---------------Test Result -----------------------
            Assert.AreEqual(2, filterDef.FilterPropertyDefs.Count);

            Assert.AreEqual(FilterClauseOperator.OpLike, filterDef.FilterPropertyDefs[0].FilterClauseOperator);
            Assert.AreEqual(FilterClauseOperator.OpEquals, filterDef.FilterPropertyDefs[1].FilterClauseOperator);
        }

        [Test]
        public void Test_CreateUIFilter_WhenClassDefParamIsNull_ShouldThrowArgumentNullException()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();

            //---------------Assert Precondition----------------

            try
            {
                //---------------Execute Test ----------------------
                uiFilterCreator.CreateUIFilter(null);
                
                Assert.Fail("Expected to throw an ArgumentNullException");
            } 
            catch (Exception ex)
            {
                //---------------Test Result -----------------------
                Assert.IsInstanceOf<ArgumentNullException>(ex);
                StringAssert.Contains("classDef", ex.Message);
            }
        }

        [Test]
        public void Test_CreateUIFilterProperty_ShouldReturnIFilterPropertyDef()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();
            IPropDef propDef = new PropDefFake(typeof(string));
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var filterPropertyDef = uiFilterCreator.CreateUIFilterProperty(propDef);
            //---------------Test Result -----------------------
            Assert.IsNotNull(filterPropertyDef);
        }

        [Test]
        public void Test_CreateUIFilterProperty_WhenPropDefParamIsNull_ShouldThrowArgumentNullException()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();
            //---------------Assert Precondition----------------

            try
            {
                //---------------Execute Test ----------------------
                uiFilterCreator.CreateUIFilterProperty(null);
                
                Assert.Fail("Expected to throw an ArgumentNullException");
            } 
            catch (Exception ex)
            {
                //---------------Test Result -----------------------
                Assert.IsInstanceOf<ArgumentNullException>(ex);
                StringAssert.Contains("propDef", ex.Message);
            }
            //---------------Test Result -----------------------
        }


        [Test]
        public void Test_CreateUIFilterProperty_When_PropertyType_IsString_ShouldHaveTextBox()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();
            IPropDef propDef = new PropDefFake(typeof(string));
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(string), propDef.PropertyType);
            //---------------Execute Test ----------------------
            var filterPropertyDef = uiFilterCreator.CreateUIFilterProperty(propDef);
            //---------------Test Result -----------------------
            AssertIsControlOfType("StringTextBoxFilter", "Habanero.Faces.Base", filterPropertyDef);
        }


        [Test]
        public void Test_CreateUIFilterProperty_When_PropertyType_IsBool_ShouldHaveCheckBox()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();
            IPropDef propDef = new PropDefFake(typeof(bool));
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(bool), propDef.PropertyType);
            //---------------Execute Test ----------------------
            var filterPropertyDef = uiFilterCreator.CreateUIFilterProperty(propDef);
            //---------------Test Result -----------------------
            AssertIsControlOfType("BoolCheckBoxFilter", "Habanero.Faces.Base", filterPropertyDef);
        }

        [Test]
        public void Test_CreateUIFilterProperty_When_PropertyType_IsDateTime_ShouldHaveDateTimePicker()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();
            IPropDef propDef = new PropDefFake(typeof(DateTime));
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), propDef.PropertyType);
            //---------------Execute Test ----------------------
            var filterPropertyDef = uiFilterCreator.CreateUIFilterProperty(propDef);
            //---------------Test Result -----------------------
            AssertIsControlOfType("DateTimePickerFilter", "Habanero.Faces.Base", filterPropertyDef);
        }

        [Test]
        public void Test_CreateUIFilterProperty_When_PropertyType_IsNoSpecific_ShouldHaveTextBox()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();
            IPropDef propDef = new PropDefFake(typeof(object));
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(object), propDef.PropertyType);
            //---------------Execute Test ----------------------
            var filterPropertyDef = uiFilterCreator.CreateUIFilterProperty(propDef);
            //---------------Test Result -----------------------
            AssertIsControlOfType("StringTextBoxFilter", "Habanero.Faces.Base", filterPropertyDef);
        }

        [Test]
        public void Test_CreateUIFilterProperty_When_PropertyType_HasLookupList_ShouldHaveComboBox()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();
            IPropDef propDef = new PropDefFake
            {
                LookupList = new SimpleLookupList(new Dictionary<string, string>())
            };
            //---------------Assert Precondition----------------
            Assert.IsTrue(propDef.HasLookupList(), "Prop Def should have lookupList");
            //---------------Execute Test ----------------------
            var filterPropertyDef = uiFilterCreator.CreateUIFilterProperty(propDef);
            //---------------Test Result -----------------------
            AssertIsControlOfType("StringComboBoxFilter", "Habanero.Faces.Base", filterPropertyDef);
        }

        [Test]
        public void Test_CreateUIFilterProperty_When_PropertyType_IsString_ShouldHaveFilterClauseOperator_OpLike()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();
            IPropDef propDef = new PropDefFake(typeof(string));
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(string), propDef.PropertyType);
            //---------------Execute Test ----------------------
            var filterPropertyDef = uiFilterCreator.CreateUIFilterProperty(propDef);
            //---------------Test Result -----------------------
            Assert.AreEqual(FilterClauseOperator.OpLike, filterPropertyDef.FilterClauseOperator);
        }

        [Test]
        public void Test_CreateUIFilterProperty_When_PropertyType_IsBool_ShouldHaveFilterClauseOperator_OpEquals()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();
            IPropDef propDef = new PropDefFake(typeof(bool));
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(bool), propDef.PropertyType);
            //---------------Execute Test ----------------------
            var filterPropertyDef = uiFilterCreator.CreateUIFilterProperty(propDef);
            //---------------Test Result -----------------------
            Assert.AreEqual(FilterClauseOperator.OpEquals, filterPropertyDef.FilterClauseOperator);
        }

        [Test]
        public void Test_CreateUIFilterProperty_When_PropertyType_IsDateTime_ShouldHaveFilterClauseOperator_OpEquals()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();
            IPropDef propDef = new PropDefFake(typeof(DateTime));
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), propDef.PropertyType);
            //---------------Execute Test ----------------------
            var filterPropertyDef = uiFilterCreator.CreateUIFilterProperty(propDef);
            //---------------Test Result -----------------------
            Assert.AreEqual(FilterClauseOperator.OpEquals, filterPropertyDef.FilterClauseOperator);
        }

        [Test]
        public void Test_CreateUIFilterProperty_When_PropertyType_IsNonSpecific_ShouldHaveFilterClauseOperator_OpLike()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();
            IPropDef propDef = new PropDefFake(typeof(object));
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(object), propDef.PropertyType);
            //---------------Execute Test ----------------------
            var filterPropertyDef = uiFilterCreator.CreateUIFilterProperty(propDef);
            //---------------Test Result -----------------------
            Assert.AreEqual(FilterClauseOperator.OpLike, filterPropertyDef.FilterClauseOperator);
        }

        [Test]
        public void Test_CreateUIFilterProperty_When_PropertyType_HasLookupList_ShouldHaveFilterClauseOperator_OpEquals()
        {
            //---------------Set up test pack-------------------
            var uiFilterCreator = GetUIFilterCreator();
            IPropDef propDef = new PropDefFake
            {
                LookupList = new SimpleLookupList(new Dictionary<string, string>())
            };
            //---------------Assert Precondition----------------
            Assert.IsTrue(propDef.HasLookupList(), "Prop Def should have lookupList");
            //---------------Execute Test ----------------------
            var filterPropertyDef = uiFilterCreator.CreateUIFilterProperty(propDef);
            //---------------Test Result -----------------------
            Assert.AreEqual(FilterClauseOperator.OpEquals, filterPropertyDef.FilterClauseOperator);
        }

        private static void AssertIsControlOfType(string controlName, string assemblyName, IFilterPropertyDef filterPropertyDef)
        {
            Assert.AreEqual(controlName, filterPropertyDef.FilterType);
            Assert.AreEqual(assemblyName, filterPropertyDef.FilterTypeAssembly);
        }

        protected virtual UIFilterCreator GetUIFilterCreator()
        {
            return new UIFilterCreator(GetFactory());
        }
    }
}
