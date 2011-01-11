using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Habanero.Base;
using Habanero.BO.ClassDefinition;

namespace Habanero.Naked
{
    public class UIViewCreator
    {
        public IUIDef GetDefaultUIDef(IClassDef classDef)
        {
            if (classDef == null) throw new ArgumentNullException("classDef");

            if (classDef.UIDefCol.Contains("default"))
            {
                return classDef.UIDefCol["default"];
            }
            IUIForm uiForm = classDef.CreateUIForm();
            var uiGrid = classDef.CreateUIGrid();
            return new UIDef("default", uiForm, uiGrid) {ClassDef = classDef};
        }
/*
        public UIView CreateUIView(DMClass dmClass)
        {
            if (dmClass == null) throw new ArgumentNullException("dmClass");
            if (dmClass.Solution == null)
            {
                throw new HabaneroArgumentException
                    ("dmClass.Solution", "You cannot create a UIView for a "
                    + "DMClass that is not associated with a Solution - (DMClass '" + dmClass.ClassNameBO + "')");
            }
            UIView view = new UIView();
            view.Class = dmClass;
            UIBOEditorCreator boEditorCreator = new UIBOEditorCreator();

            UIFormInfo boEditor = boEditorCreator.CreateBOEditor(dmClass);
            view.UIFormInfo = boEditor;

            UIGridCreator gridCreator = new UIGridCreator();
            UIGridInfo gridInfo = gridCreator.CreateGrid(dmClass, dmClass.Properties);
            view.UIGridInfo = gridInfo;

            UIGridFilterCreator gridFilterCreator = new UIGridFilterCreator((DMSolution)dmClass.Solution);
            UIGridFilter gridFilterInfo = gridFilterCreator.CreateUIGridFilter(dmClass.Properties);
            gridInfo.UIGridFilter = gridFilterInfo;
            return view;
        }*/
    }

    internal class UIControlType
    {
        public virtual String AssemblyName { get; set; }

        public virtual String TypeName { get; set; }
    }

    public static class UIFormCreatorExtensions
    {
        public static IUIForm CreateUIForm(this IClassDef classDef)
        {
            var formCreator = new UIFormCreator();
            return formCreator.CreateUIForm(classDef);
        }
        public static IUIGrid CreateUIGrid(this IClassDef classDef)
        {
            var gridCreator = new UIGridCreator();
            return gridCreator.CreateUIGrid(classDef);
        }

        public static bool IsPartOfObjectIdentity(this IPropDef propDef)
        {
            //It is assumed that all Guids are FK Props or PrimaryKey Props and that
            // only Guids are PK Props (i.e. natural PK's,
            // autonumber PK's and all these variants will not be catered for here.)
            return propDef.PropertyType == typeof(System.Guid);
        }

        public static bool IsPropForeignKey(this IPropDef propDef)
        {
            //It is assumed that all Guids are FK Props or PrimaryKey Props and that
            // only Guids are PK Props (i.e. natural PK's,
            // autonumber PK's and all these variants will not be catered for here.)
            return propDef.PropertyType == typeof(System.Guid);
        }
    }
}
