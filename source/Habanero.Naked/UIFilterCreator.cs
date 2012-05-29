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
using Habanero.BO.ClassDefinition;

namespace Habanero.Naked
{
    public class UIFilterCreator
    {
        private readonly IDefClassFactory _factory;

        public UIFilterCreator(IDefClassFactory factory)
        {
            _factory = factory;
        }

        public IFilterDef CreateUIFilter(IClassDef classDef)
        {
            if (classDef==null) throw new ArgumentNullException("classDef");
            var filterPropertyDefs = new List<IFilterPropertyDef>();
            foreach (var propDef in classDef.PropDefcol)
            {
                if (propDef.IsPartOfObjectIdentity()) continue;
                if (propDef.IsPropForeignKey()) continue;
                filterPropertyDefs.Add(CreateUIFilterProperty(propDef));
            }
            IFilterDef filterDef = _factory.CreateFilterDef(filterPropertyDefs);
            return filterDef;
        }

        public IFilterPropertyDef CreateUIFilterProperty(IPropDef propDef)
        {
            if (propDef==null) throw new ArgumentNullException("propDef");
            UIControlType uiControlType = GetControlType(propDef);
            var filterClauseOperator = GetFilterClauseOperator(propDef);
            IFilterPropertyDef filterPropertyDef = _factory.CreateFilterPropertyDef(propDef.PropertyName,
                                                                                    propDef.DisplayName,
                                                                                    uiControlType.TypeName,
                                                                                    uiControlType.AssemblyName,
                                                                                    filterClauseOperator,
                                                                                    new Dictionary<string, string>());
            return filterPropertyDef;
        }


        private static FilterClauseOperator GetFilterClauseOperator(IPropDef propDef)
        {
            if (propDef.HasLookupList()) return FilterClauseOperator.OpEquals;
            if (propDef.PropertyType.Name == "Boolean" || propDef.PropertyType.Name == "DateTime") 
                return FilterClauseOperator.OpEquals;

            return FilterClauseOperator.OpLike;
        }

        private static UIControlType GetControlType(IPropDef propDef)
        {
            UIControlType type;
            if (propDef.HasLookupList()) return GetComboBoxControlType();
            switch (propDef.PropertyType.Name)
            {
                case "String":
                    type = GetTextBoxControlType();
                    break;
                case "Boolean":
                    type = GetCheckBoxControlType();
                    break;
                case "DateTime":
                    type = GetDateTimeControlType();
                    break;
                default:
                    type = GetTextBoxControlType();
                    break;
            }

            return type;
        }

        private static UIControlType GetControlType(string assemblyName, string formFieldTypeName)
        {
            return new UIControlType { AssemblyName = assemblyName, TypeName = formFieldTypeName };
        }

        private static UIControlType GetTextBoxControlType()
        {
            return GetControlType("Habanero.Faces.Base", "StringTextBoxFilter");
        }

        private static UIControlType GetCheckBoxControlType()
        {
            return GetControlType("Habanero.Faces.Base", "BoolCheckBoxFilter");
        }

        private static UIControlType GetDateTimeControlType()
        {
            return GetControlType("Habanero.Faces.Base", "DateTimePickerFilter");
        }

        private static UIControlType GetComboBoxControlType()
        {
            return GetControlType("Habanero.Faces.Base", "StringComboBoxFilter");
        }
    }
}
