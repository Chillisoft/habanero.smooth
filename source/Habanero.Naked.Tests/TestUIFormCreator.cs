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
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using NUnit.Framework;

namespace Habanero.Naked.Tests
{
    [TestFixture]
    public class TestUIFormCreator
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
            var formCreator = new UIFormCreator(GetFactory());
            //---------------Test Result -----------------------
            Assert.IsNotNull(formCreator);
        }

        [Test]
        public void Test_CreateUIForm_ShouldReturnNewUIDef()
        {
            //---------------Set up test pack-------------------
            var formCreator = new UIFormCreator(GetFactory());
            IClassDef classDef = typeof(FakeBo).MapClass();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            IUIForm returnedUIForm = formCreator.CreateUIForm(classDef);
            //---------------Test Result -----------------------
            Assert.IsNotNull(returnedUIForm);
        }

        [Test]
        public void Test_CreateUIForm_ShouldSetTitle()
        {
            //---------------Set up test pack-------------------
            var formCreator = new UIFormCreator(GetFactory());
            IClassDef classDef = typeof(FakeBo).MapClass();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, classDef.UIDefCol.Count);
            //---------------Execute Test ----------------------
            IUIForm returnedUIForm = formCreator.CreateUIForm(classDef);
            //---------------Test Result -----------------------
            Assert.IsNotNull(returnedUIForm);
            Assert.AreEqual("Fake Bo Form", returnedUIForm.Title);
        }

        [Test]
        public void Test_CreateUIForm_WhenNotHasViewAndHasStringProp_ShouldCreateUIField()
        {
            //---------------Set up test pack-------------------
            var formCreator = new UIFormCreator(GetFactory());
            IClassDef classDef = typeof(FakeBo).MapClass();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, classDef.UIDefCol.Count);
            //---------------Execute Test ----------------------
            IUIForm returnedUIForm = formCreator.CreateUIForm(classDef);
            //---------------Test Result -----------------------
            Assert.IsNotNull(returnedUIForm);
            Assert.AreEqual(1, returnedUIForm.Count, "Should create tab");
            IUIFormTab returnedUIFormTab = returnedUIForm[0];
            Assert.AreSame(returnedUIForm, returnedUIFormTab.UIForm);
            Assert.AreEqual("default", returnedUIFormTab.Name);
            Assert.AreEqual(1, returnedUIFormTab.Count, "Should create col");
            IUIFormColumn returnedUIFormColumn = returnedUIFormTab[0];
            Assert.AreSame(returnedUIFormTab, returnedUIFormColumn.UIFormTab);
            Assert.AreEqual(1, returnedUIFormColumn.Count, "Should create field");
            IUIFormField returnedUIFormField = returnedUIFormColumn[0];
            Assert.AreEqual("Fake Bo Name", returnedUIFormField.Label);
            Assert.AreEqual("FakeBoName", returnedUIFormField.PropertyName);
        }

        private IUIFormField GetFormField(IUIForm uiForm, int index)
        {
            IUIFormTab uiFormTab = uiForm[0];
            IUIFormColumn uiFormColumn = uiFormTab[0];
            IUIFormField uiFormField = uiFormColumn[index];
            return uiFormField;
        }

        [Test]
        public void Test_CreateUIForm_With2Props_ShouldCreate2UIFields()
        {
            //---------------Set up test pack-------------------
            var formCreator = new UIFormCreator(GetFactory());
            IClassDef classDef = typeof(FakeBoW2Props).MapClass();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, classDef.UIDefCol.Count);
            Assert.AreEqual(3, classDef.PropDefcol.Count, "2 Props and IDProp");
            //---------------Execute Test ----------------------
            IUIForm returnedUIForm = formCreator.CreateUIForm(classDef);
            //---------------Test Result -----------------------
            IUIFormField uiFormField1 = GetFormField(returnedUIForm, 0);
            Assert.AreEqual("Fake Bo Name", uiFormField1.Label);
            Assert.AreEqual("FakeBoName", uiFormField1.PropertyName);

            IUIFormField uiFormField2 = GetFormField(returnedUIForm, 1);
            Assert.AreEqual("Fake Bo Name 2", uiFormField2.Label);
            Assert.AreEqual("FakeBoName2", uiFormField2.PropertyName);
        }

        [Test]
        public void Test_GetUIFormField_WhenBool_ShouldCreateCheckBox()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = new PropDefFake(typeof (bool));
            var formCreator = new UIFormCreatorSpy(GetFactory());
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(bool), propDef.PropertyType);
            //---------------Execute Test ----------------------
            var formField = formCreator.CallGetUIFormField(propDef);
            //---------------Test Result -----------------------
            Assert.AreEqual("System.Windows.Forms", formField.ControlAssemblyName, "Should create a windows control");
            Assert.AreEqual("CheckBox", formField.ControlTypeName);
        }
        [Test]
        public void Test_GetUIFormField_WhenInt_ShouldCreateTextBox()
        {
            //---------------Set up test pack-------------------
            var propType = typeof (int);
            IPropDef propDef = new PropDefFake(propType);
            var formCreator = new UIFormCreatorSpy(GetFactory());
            //---------------Assert Precondition----------------
            Assert.AreSame(propType, propDef.PropertyType);
            //---------------Execute Test ----------------------
            var formField = formCreator.CallGetUIFormField(propDef);
            //---------------Test Result -----------------------
            Assert.AreEqual("System.Windows.Forms", formField.ControlAssemblyName, "Should create a windows control");
            Assert.AreEqual("TextBox", formField.ControlTypeName);
        }
        [Test]
        public void Test_GetUIFormField_WhenDateTime_ShouldCreateDateTimePicker()
        {
            //---------------Set up test pack-------------------
            var propType = typeof (DateTime);
            IPropDef propDef = new PropDefFake(propType);
            var formCreator = new UIFormCreatorSpy(GetFactory());
            //---------------Assert Precondition----------------
            Assert.AreSame(propType, propDef.PropertyType);
            //---------------Execute Test ----------------------
            var formField = formCreator.CallGetUIFormField(propDef);
            //---------------Test Result -----------------------
            Assert.AreEqual("System.Windows.Forms", formField.ControlAssemblyName, "Should create a windows control");
            Assert.AreEqual("DateTimePicker", formField.ControlTypeName);
        }
        [Test]
        public void Test_GetUIFormField_Lookup_ShouldBeComboBoxColumnType()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = new PropDefFake
            {
                LookupList = new SimpleLookupList(new Dictionary<string, string>())
            };
            var formCreator = new UIFormCreatorSpy(GetFactory());
            //---------------Assert Precondition----------------
            Assert.IsTrue(propDef.HasLookupList(), "Prop Def should have lookupList");
            //---------------Execute Test ----------------------
            var formField = formCreator.CallGetUIFormField(propDef);
            //---------------Test Result -----------------------
            Assert.AreEqual("System.Windows.Forms", formField.ControlAssemblyName, "Should create a windows control");
            Assert.AreEqual("ComboBox", formField.ControlTypeName);
        }
    }
    internal class UIFormCreatorSpy : UIFormCreator
    {
        public UIFormCreatorSpy(IDefClassFactory factory) : base(factory)
        {
        }

        public IUIFormField CallGetUIFormField(IPropDef propDef)
        {
            return this.GetUIFormField(propDef);
        }
    }
}