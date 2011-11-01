﻿#region Licensing Header
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
using Habanero.BO.ClassDefinition;

namespace Habanero.Naked
{
    public class UIViewCreator
    {
        private readonly IDefClassFactory _factory;

        public UIViewCreator(IDefClassFactory factory)
        {
            _factory = factory;
        }

        public IUIDef GetDefaultUIDef(IClassDef classDef)
        {
            if (classDef == null) throw new ArgumentNullException("classDef");

            if (classDef.UIDefCol.Contains("default"))
            {
                return classDef.UIDefCol["default"];
            }
            IUIForm uiForm = CreateUIForm(classDef);
            var uiGrid = CreateUIGrid(classDef);
            var uiDef = _factory.CreateUIDef("default", uiForm, uiGrid);
            uiDef.ClassDef = classDef;
            return uiDef;
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

        public IUIGrid CreateUIGrid(IClassDef classDef)
        {
            var gridCreator = new UIGridCreator(_factory);
            return gridCreator.CreateUIGrid(classDef);
        }
        public IUIForm CreateUIForm(IClassDef classDef)
        {
            var formCreator = new UIFormCreator(_factory);
            return formCreator.CreateUIForm(classDef);
        }
       
    }

    internal class UIControlType
    {
        public virtual String AssemblyName { get; set; }

        public virtual String TypeName { get; set; }
    }

    public static class UIFormCreatorExtensions
    {
     
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
