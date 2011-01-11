using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using NUnit.Framework;
using Habanero.Smooth;

namespace Habanero.Naked.Tests
{
    [TestFixture]
    public class TestUIViewCreator
    {
        [Test]
        public void Test_Construct_ShouldConstruct()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var viewCreator = new UIViewCreator();
            //---------------Test Result -----------------------
            Assert.IsNotNull(viewCreator);
        }

        [Test]
        public void Test_GetDefaultUIDef_WithNullclassDef_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                var uiViewCreator = new UIViewCreator();
                uiViewCreator.GetDefaultUIDef(null);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("classDef", ex.ParamName);
            }
        }
        [Test]
        public void Test_GetDefaultUIDef_WhenHasDefaultUIView_ShouldReturnUIDef()
        {
            //---------------Set up test pack-------------------
            var viewCreator = new UIViewCreator();
            IClassDef classDef = typeof (FakeBo).MapClass();
            var expectedUIDef = new UIDef("default", new UIForm(), new UIGrid());
            classDef.UIDefCol.Add(expectedUIDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, classDef.UIDefCol.Count);
            Assert.AreEqual("default", classDef.UIDefCol["default"].Name);
            //---------------Execute Test ----------------------
            IUIDef returnedUIDef = viewCreator.GetDefaultUIDef(classDef);
            //---------------Test Result -----------------------
            Assert.IsNotNull(returnedUIDef);
            Assert.AreEqual(expectedUIDef, returnedUIDef);
        }

        [Test]
        public void Test_GetDefaultUIDef_WhenNotHasDefaultUIView_ShouldReturnNewUIDef()
        {
            //---------------Set up test pack-------------------
            var viewCreator = new UIViewCreator();
            IClassDef classDef = typeof(FakeBo).MapClass();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, classDef.UIDefCol.Count);
            //---------------Execute Test ----------------------
            IUIDef returnedUIDef = viewCreator.GetDefaultUIDef(classDef);
            //---------------Test Result -----------------------
            Assert.IsNotNull(returnedUIDef);
            Assert.AreEqual("default", returnedUIDef.Name);
            Assert.IsNotNull(returnedUIDef.UIForm);
            Assert.IsNotNull(returnedUIDef.UIGrid);
            Assert.AreSame(classDef, returnedUIDef.ClassDef);
            Assert.AreSame(classDef, returnedUIDef.UIGrid.ClassDef);
        }

        [Test]
        public void Test_GetDefaultUIDef_WhenNotHas_ShouldSetTitle()
        {
            //---------------Set up test pack-------------------
            var viewCreator = new UIViewCreator();
            IClassDef classDef = typeof(FakeBo).MapClass();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, classDef.UIDefCol.Count);
            //---------------Execute Test ----------------------
            IUIDef returnedUIDef = viewCreator.GetDefaultUIDef(classDef);
            //---------------Test Result -----------------------
            Assert.IsNotNull(returnedUIDef.UIForm);
            Assert.AreEqual("Fake Bo Form", returnedUIDef.UIForm.Title);
        }

        [Test]
        public void Test_GetDefaultUIDef_WhenNotHasViewAndHasStringProp_ShouldCreateUIField()
        {
            //---------------Set up test pack-------------------
            var viewCreator = new UIViewCreator();
            IClassDef classDef = typeof(FakeBo).MapClass();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, classDef.UIDefCol.Count);
            //---------------Execute Test ----------------------
            IUIDef returnedUIDef = viewCreator.GetDefaultUIDef(classDef);
            //---------------Test Result -----------------------
            IUIForm uiForm = returnedUIDef.UIForm;
            Assert.IsNotNull(uiForm);
            Assert.AreSame(returnedUIDef, uiForm.UIDef);
            Assert.AreEqual(1, uiForm.Count, "Should create tab");
            IUIFormTab uiFormTab = uiForm[0];
            Assert.AreSame(uiForm, uiFormTab.UIForm);
            Assert.AreEqual("default", uiFormTab.Name);
            Assert.AreEqual(1, uiFormTab.Count, "Should create col");
            IUIFormColumn uiFormColumn = uiFormTab[0];
            Assert.AreSame(uiFormTab, uiFormColumn.UIFormTab);
            Assert.AreEqual(1, uiFormColumn.Count, "Should create field");
            IUIFormField uiFormField = uiFormColumn[0];
            Assert.AreEqual("Fake Bo Name", uiFormField.Label);
            Assert.AreEqual("FakeBoName", uiFormField.PropertyName);
        }

        private IUIFormField GetFormField(IUIDef uiDef, int index)
        {
            IUIForm uiForm = uiDef.UIForm;
            IUIFormTab uiFormTab = uiForm[0];
            IUIFormColumn uiFormColumn = uiFormTab[0];
            IUIFormField uiFormField = uiFormColumn[index];
            return uiFormField;
        }

        [Test]
        public void Test_GetDefaultUIDef_With2Props_ShouldCreate2UIFields()
        {
            //---------------Set up test pack-------------------
            var viewCreator = new UIViewCreator();
            IClassDef classDef = typeof(FakeBoW2Props).MapClass();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, classDef.UIDefCol.Count);
            Assert.AreEqual(3, classDef.PropDefcol.Count, "2 Props and IDProp");
            //---------------Execute Test ----------------------
            IUIDef returnedUIDef = viewCreator.GetDefaultUIDef(classDef);
            //---------------Test Result -----------------------
            IUIFormField uiFormField1 = GetFormField(returnedUIDef, 0);
            Assert.AreEqual("Fake Bo Name", uiFormField1.Label);
            Assert.AreEqual("FakeBoName", uiFormField1.PropertyName);

            IUIFormField uiFormField2 = GetFormField(returnedUIDef, 1);
            Assert.AreEqual("Fake Bo Name 2", uiFormField2.Label);
            Assert.AreEqual("FakeBoName2", uiFormField2.PropertyName);
        }


        [Test]
        public void Test_GetDefaultUIDef_WhenNotHasViewAndHasStringProp_ShouldCreateUIGridColumn()
        {
            //---------------Set up test pack-------------------
            var viewCreator = new UIViewCreator();
            IClassDef classDef = typeof(FakeBoW2Props).MapClass();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, classDef.UIDefCol.Count);
            Assert.AreEqual(3, classDef.PropDefcol.Count, "2 Props and IDProp");
            //---------------Execute Test ----------------------
            IUIDef returnedUIDef = viewCreator.GetDefaultUIDef(classDef);
            //---------------Test Result -----------------------
            Assert.IsNotNull(returnedUIDef.UIGrid);
            var returnedUIGrid = returnedUIDef.UIGrid;
            Assert.AreEqual(2, returnedUIGrid.Count);
            IUIGridColumn uiFormField1 = returnedUIGrid[0];
            Assert.AreEqual("Fake Bo Name", uiFormField1.Heading);
            Assert.AreEqual("FakeBoName", uiFormField1.PropertyName);

            IUIGridColumn uiFormField2 = returnedUIGrid[1];
            Assert.AreEqual("Fake Bo Name 2", uiFormField2.Heading);
            Assert.AreEqual("FakeBoName2", uiFormField2.PropertyName);
        }

        //TODO brett 15 Jun 2010: still to be implemented
        //Should Create Form Fields for Single Relationships.
        //Should Create Grid Columns for single relationships.

        //Should Create Grid Filter for correct type.
        //Should Create Grid Filter for correct type for single relationships.
        [Test]
        [Ignore("Not Yet Implemented")] //TODO Brett 15 Jun 2010: Ignored Test - Not Yet Implemented
        public void Test_GetDefaultUIDef_WhenNotHasViewAndHasStringProp_ShouldCreateUIGridFilter()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }
    }

}
