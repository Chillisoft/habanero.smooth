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
    public class TestUIGridCreator
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
            var gridCreator = new UIGridCreator(GetFactory());
            //---------------Test Result -----------------------
            Assert.IsNotNull(gridCreator);
        }

        [Test]
        public void Test_CreateUIGrid_ShouldReturnNewUIDef()
        {
            //---------------Set up test pack-------------------
            var gridCreator = new UIGridCreator(GetFactory());
            IClassDef classDef = typeof(FakeBo).MapClass();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            IUIGrid returnedUIGrid = gridCreator.CreateUIGrid(classDef);
            //---------------Test Result -----------------------
            Assert.IsNotNull(returnedUIGrid);
        }
        [Test]
        public void Test_CreateUIGrid_WhenNotHasViewAndHasStringProp_ShouldCreateGridColumn()
        {
            //---------------Set up test pack-------------------
            var gridCreator = new UIGridCreator(GetFactory());
            IClassDef classDef = typeof(FakeBo).MapClass();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, classDef.UIDefCol.Count);
            //---------------Execute Test ----------------------
            IUIGrid returnedUIGrid = gridCreator.CreateUIGrid(classDef);
            //---------------Test Result -----------------------
            Assert.IsNotNull(returnedUIGrid);
            Assert.AreEqual(1, returnedUIGrid.Count, "Should create Column");

            var returnedUIGridColumn = returnedUIGrid[0];
            Assert.AreEqual("Fake Bo Name", returnedUIGridColumn.Heading);
            Assert.AreEqual("FakeBoName", returnedUIGridColumn.PropertyName);
            Assert.AreEqual(PropAlignment.left, returnedUIGridColumn.Alignment, "Alignment should be left by default");
            Assert.AreEqual(100, returnedUIGridColumn.Width, "Width should be 100 by default");
            Assert.IsTrue(returnedUIGridColumn.Editable, "Grid Column Should be editable by default");
        }

        [Test]
        public void Test_CreateUIGrid_ShouldCreateFilterDefForGrid()
        {
            //---------------Set up test pack-------------------
            var gridCreator = new UIGridCreator(GetFactory());
            IClassDef classDef = typeof(FakeBo).MapClass();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, classDef.UIDefCol.Count);
            //---------------Execute Test ----------------------
            IUIGrid returnedUIGrid = gridCreator.CreateUIGrid(classDef);

            //---------------Test Result -----------------------
            Assert.IsNotNull(returnedUIGrid.FilterDef);
        }

        [Test]
        public void Test_CreateUIGrid_WhenClassDefHasOnePropAndOnePKProp_ShouldCreateFilterDefWithOneFilterPropertyDef()
        {
            //---------------Set up test pack-------------------
            var gridCreator = new UIGridCreator(GetFactory());
            IClassDef classDef = typeof(FakeBo).MapClass();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, classDef.UIDefCol.Count);
            //---------------Execute Test ----------------------
            IUIGrid returnedUIGrid = gridCreator.CreateUIGrid(classDef);

            //---------------Test Result -----------------------
            Assert.AreEqual(1, returnedUIGrid.FilterDef.FilterPropertyDefs.Count);
            Assert.AreEqual("Fake Bo Name", returnedUIGrid.FilterDef.FilterPropertyDefs[0].Label);
            Assert.AreEqual("FakeBoName", returnedUIGrid.FilterDef.FilterPropertyDefs[0].PropertyName);
        }

        [Test]
        public void Test_CreateUIGrid_With2Props_ShouldCreate2GridColumns()
        {
            //---------------Set up test pack-------------------
            var gridCreator = new UIGridCreator(GetFactory());
            IClassDef classDef = typeof(FakeBoW2Props).MapClass();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, classDef.UIDefCol.Count);
            Assert.AreEqual(3, classDef.PropDefcol.Count, "2 Props and IDProp");
            //---------------Execute Test ----------------------
            IUIGrid returnedUIGrid = gridCreator.CreateUIGrid(classDef);
            //---------------Test Result -----------------------
            IUIGridColumn uiFormField1 = returnedUIGrid[0];
            Assert.AreEqual("Fake Bo Name", uiFormField1.Heading);
            Assert.AreEqual("FakeBoName", uiFormField1.PropertyName);

            IUIGridColumn uiFormField2 = returnedUIGrid[1];
            Assert.AreEqual("Fake Bo Name 2", uiFormField2.Heading);
            Assert.AreEqual("FakeBoName2", uiFormField2.PropertyName);
        }

        [Test]
        public void Test_CreateGridInfo_TwoProperty_ShouldHaveAppropriateColumnInfo()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = typeof(FakeBoW2Props).MapClass();
            UIGridCreator gridCreator = new UIGridCreator(GetFactory());
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(bool), classDef.PropDefcol["FakeBoName2"].PropertyType);
            //---------------Execute Test ----------------------
            IUIGrid returnedUIGrid = gridCreator.CreateUIGrid(classDef);
            //---------------Test Result -----------------------
            Assert.IsNotNull(returnedUIGrid);
            Assert.AreEqual(2, returnedUIGrid.Count);
            AssertGridColumnTypeIsTextBox(returnedUIGrid[0]);
            AssertGridColumnTypeIsCheckBox(returnedUIGrid[1]);
        }

        [Test]
        public void Test_GetUIFormField_WhenPropNull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var gridCreator = new UIGridCreatorSpy(GetFactory());
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                gridCreator.CallGetUIGridColumn(null);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("propDef", ex.ParamName);
            }
        }

        [Test]
        public void Test_GetUIFormField_WhenString_ShouldHaveTextBoxPropertyAndColumnType()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = new PropDefFake(typeof(string));
            var gridCreator = new UIGridCreatorSpy(GetFactory());
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(string), propDef.PropertyType);
            //---------------Execute Test ----------------------
            var gridColumn = gridCreator.CallGetUIGridColumn(propDef);
            //---------------Test Result -----------------------
            Assert.IsNotNull(gridColumn);
            Assert.IsNotNull(gridColumn.PropertyName);
            AssertGridColumnTypeIsTextBox(gridColumn);
        }

        [Test]
        public void Test_GetUIFormField_WhenBool_ShouldCreateCheckBox()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = new PropDefFake(typeof(bool));
            var gridCreator = new UIGridCreatorSpy(GetFactory());
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(bool), propDef.PropertyType);
            //---------------Execute Test ----------------------
            var gridColumn = gridCreator.CallGetUIGridColumn(propDef);
            //---------------Test Result -----------------------
            AssertGridColumnTypeIsCheckBox(gridColumn);
        }

        [Test]
        public void Test_GetUIFormField_WhenInt_ShouldCreateTextBox()
        {
            //---------------Set up test pack-------------------
            var propType = typeof(int);
            IPropDef propDef = new PropDefFake(propType);
            var gridCreator = new UIGridCreatorSpy(GetFactory());
            //---------------Assert Precondition----------------
            Assert.AreSame(propType, propDef.PropertyType);
            //---------------Execute Test ----------------------
            var gridColumn = gridCreator.CallGetUIGridColumn(propDef);
            //---------------Test Result -----------------------
            AssertGridColumnTypeIsTextBox(gridColumn);
        }

        [Test]
        public void Test_GetUIFormField_WhenDateTime_ShouldCreateDateTimePicker()
        {
            //---------------Set up test pack-------------------
            var propType = typeof(DateTime);
            IPropDef propDef = new PropDefFake(propType);
            var gridCreator = new UIGridCreatorSpy(GetFactory());
            //---------------Assert Precondition----------------
            Assert.AreSame(propType, propDef.PropertyType);
            //---------------Execute Test ----------------------
            var gridColumn = gridCreator.CallGetUIGridColumn(propDef);
            //---------------Test Result -----------------------
            AssertGridColumnTypeIsDateTime(gridColumn);
        }

        [Test]
        public void Test_GetUIFormField_Lookup_ShouldBeComboBoxColumnType()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = new PropDefFake
                       {
                           LookupList = new SimpleLookupList(new Dictionary<string, string>())
                       };
            var gridCreator = new UIGridCreatorSpy(GetFactory());
            //---------------Assert Precondition----------------
            Assert.IsTrue(propDef.HasLookupList(), "Prop Def should have lookupList");
            //---------------Execute Test ----------------------
            var gridColumn = gridCreator.CallGetUIGridColumn(propDef);
            //---------------Test Result -----------------------
            AssertGridColumnTypeIsComboBox(gridColumn);
        }
        private static void AssertGridColumnTypeIsDateTime(IUIGridColumn gridColumn)
        {
            Assert.AreEqual("Habanero.Faces.Win", gridColumn.GridControlAssemblyName);
            Assert.AreEqual("DataGridViewDateTimeColumnWin", gridColumn.GridControlTypeName);
        }
        private static void AssertGridColumnTypeIsComboBox(IUIGridColumn gridColumn)
        {
            Assert.AreEqual("System.Windows.Forms", gridColumn.GridControlAssemblyName);
            Assert.AreEqual("DataGridViewComboBoxColumn", gridColumn.GridControlTypeName);
        }

        private static void AssertGridColumnTypeIsCheckBox(IUIGridColumn gridColumn)
        {
            Assert.AreEqual("System.Windows.Forms", gridColumn.GridControlAssemblyName);
            Assert.AreEqual("DataGridViewCheckBoxColumn", gridColumn.GridControlTypeName);
        }

        private static void AssertGridColumnTypeIsTextBox(IUIGridColumn gridColumn)
        {
            Assert.AreEqual("System.Windows.Forms", gridColumn.GridControlAssemblyName);
            Assert.AreEqual("DataGridViewTextBoxColumn", gridColumn.GridControlTypeName);
        }
/*

        [Test]
        public void Test_CreateGridInfo_TwoProperty_OneObjectID_ShouldHaveObjectIDColumn()
        {
            //---------------Set up test pack-------------------
            DMClass dmClass = CreateClassWithOneLookupProperty();
            DMProperty dmProperty = dmClass.Properties.CreateBusinessObject();
            TestUtilsDMProperty.SetDMPropBooleanType(dmProperty);
            DMProperty dmProperty1 = dmClass.Properties.CreateBusinessObject();
            TestUtilsDMProperty.SetDMPropStringType(dmProperty1);
            dmClass.CreateObjectIdentity(dmProperty1);
            UIGridCreator gridCreator = new UIGridCreator(GetFactory());
            //---------------Assert Precondition----------------
            Assert.IsNotNull(dmClass);
            Assert.AreEqual(3, dmClass.Properties.Count);
            //---------------Execute Test ----------------------
            UIGridInfo gridInfo = gridCreator.CreateGrid(dmClass, dmClass.Properties);
            //---------------Test Result -----------------------
            Assert.IsNotNull(gridInfo);
            Assert.AreEqual(3, gridInfo.UIColumns.Count);
        }

        [Test]
        public void Test_CreateGridInfo_TwoProperty_OneKeepValuePrivate_ShouldHaveKeepvaluePrivateColumn()
        {
            //---------------Set up test pack-------------------
            DMClass dmClass = CreateClassWithOneLookupProperty();
            DMProperty dmProperty = dmClass.Properties.CreateBusinessObject();
            TestUtilsDMProperty.SetDMPropBooleanType(dmProperty);
            DMProperty property = dmClass.Properties.CreateBusinessObject();
            TestUtilsDMProperty.SetDMPropStringType(property);
            property.KeepValuePrivate = true;
            UIGridCreator gridCreator = new UIGridCreator(GetFactory());
            //---------------Assert Precondition----------------
            Assert.IsNotNull(dmClass);
            Assert.AreEqual(3, dmClass.Properties.Count);
            Assert.IsTrue(property.KeepValuePrivate.Value);
            //---------------Execute Test ----------------------
            UIGridInfo gridInfo = gridCreator.CreateGrid(dmClass, dmClass.Properties);
            //---------------Test Result -----------------------
            Assert.IsNotNull(gridInfo);
            Assert.AreEqual(3, gridInfo.UIColumns.Count);
        }

        [Test]
        public void Test_CreateGridInfo_TwoProperty_OneKeepValuePrivateNull_ShouldCreateAllColumns()
        {
            //---------------Set up test pack-------------------
            DMClass dmClass = CreateClassWithOneLookupProperty();
            DMProperty dmProperty = TestUtilsDMProperty.CreateUnsavedValidDMProperty(dmClass);
            TestUtilsDMProperty.SetDMPropBooleanType(dmProperty);
            DMProperty property = TestUtilsDMProperty.CreateUnsavedValidDMProperty(dmClass);
            property.KeepValuePrivate = null;
            UIGridCreator gridCreator = new UIGridCreator(GetFactory());
            //---------------Assert Precondition----------------
            Assert.IsNotNull(dmClass);
            Assert.AreEqual(3, dmClass.Properties.Count);
            Assert.IsNull(property.KeepValuePrivate);
            //---------------Execute Test ----------------------
            UIGridInfo gridInfo = gridCreator.CreateGrid(dmClass, dmClass.Properties);
            //---------------Test Result -----------------------
            Assert.IsNotNull(gridInfo);
            Assert.AreEqual(3, gridInfo.UIColumns.Count);
        }


        [Test]
        public void Test_CreateGridInfo_TwoProperty_ShouldHaveCorrectOrder()
        {
            //---------------Set up test pack-------------------
            DMClass dmClass = CreateClassWithOneLookupProperty();
            DMProperty dmProperty1 = dmClass.Properties[0];
            dmProperty1.DisplayName = TestUtilsShared.GetRandomString();
            DMProperty dmProperty2 = dmClass.Properties.CreateBusinessObject();
            dmProperty2.DisplayName = TestUtilsShared.GetRandomString();
            TestUtilsDMProperty.SetDMPropBooleanType(dmProperty2);
            UIGridCreator gridCreator = new UIGridCreator(GetFactory());
            BusinessObjectCollection<DMProperty> selectedProps = new BusinessObjectCollection<DMProperty>();
            selectedProps.Add(dmProperty2);
            selectedProps.Add(dmProperty1);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(dmClass);
            Assert.AreEqual(2, dmClass.Properties.Count);
            Assert.AreEqual(1, dmProperty1.OrdinalPosition);
            Assert.AreEqual(2, dmProperty2.OrdinalPosition);
            //---------------Execute Test ----------------------
            UIGridInfo gridInfo = gridCreator.CreateGrid(dmClass, selectedProps);
            UIView uiView = dmClass.UIViews.CreateBusinessObject();
            gridInfo.UIView = uiView;
            uiView.Save();
            //---------------Test Result -----------------------
            Assert.IsNotNull(gridInfo);
            Assert.AreEqual(2, gridInfo.UIColumns.Count);
            gridInfo.UIColumns.Sort("OrdinalPosition", true, true);
            UIGridColumnInfo column1 = gridInfo.UIColumns[0];
            Assert.AreSame(dmProperty1.PropertyName, column1.PropertyName);
            Assert.AreEqual(dmProperty1.OrdinalPosition, column1.OrdinalPosition);
            UIGridColumnInfo column2 = gridInfo.UIColumns[1];
            Assert.AreSame(dmProperty2.PropertyName, column2.PropertyName);
            Assert.AreEqual(dmProperty2.OrdinalPosition, column2.OrdinalPosition);
        }

        private static DMClass CreateClassWithOneLookupProperty()
        {
            SolutionCreator creator = new SolutionCreator();
            DMProperty property = TestUtilsDMProperty.CreateUnsavedDefaultDMPropertyWithClass(creator.CreateSolution());
            TestUtilsDMProperty.SetDMPropWithLookupList(property);
            return (DMClass)property.Class;
        }
*/
    }
    internal class UIGridCreatorSpy : UIGridCreator
    {
        public UIGridCreatorSpy(IDefClassFactory factory) : base(factory)
        {
        }

        public IUIGridColumn CallGetUIGridColumn(IPropDef propDef)
        {
            return this.GetUIGridColumn(propDef);
        }
    }
}