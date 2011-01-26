using System;
using System.Collections;
using Habanero.Base;
using Habanero.BO.ClassDefinition;

namespace Habanero.Naked
{
    public class UIGridCreator
    {
        private readonly IDefClassFactory _factory;

        public UIGridCreator(IDefClassFactory factory)
        {
            _factory = factory;
        }

        public IUIGrid CreateUIGrid(IClassDef classDef)
        {
            IUIGrid uiGrid = _factory.CreateUIGridDef();

            //The Properties are loaded in the ordinal position order
            // that they occur in the XML ClassDef so this will be the 
            // correct order they should be shown in the UI.
            foreach (var propDef in classDef.PropDefcol)
            {
                if (propDef.IsPartOfObjectIdentity()) continue;
                if (propDef.IsPropForeignKey()) continue;
                uiGrid.Add(GetUIGridColumn(propDef));
            }            
            return uiGrid;
        }


        protected IUIGridColumn GetUIGridColumn(IPropDef propDef)
        {
            if (propDef == null) throw new ArgumentNullException("propDef");
            UIControlType controlType = GetControlType(propDef);
            var gridColumn = _factory.CreateUIGridProperty(propDef.DisplayName, propDef.PropertyName, controlType.TypeName, controlType.AssemblyName,true, 100, PropAlignment.left, new Hashtable());
            //SetControlType(propDef, gridColumn);

            return gridColumn;
        }

        private static UIControlType GetControlType(IPropDef propDef)
        {
            UIControlType type;
            if (propDef.HasLookupList())
            {
                type = GetComboBoxControlType();
            }
            else
            {
            switch (propDef.PropertyType.Name)
            {
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
             }
            return type;
        }


        private static UIControlType GetDateTimeControlType()
        {
            return GetControlType("Habanero.Faces.Win", "DataGridViewDateTimeColumnWin");
        }

        private static UIControlType GetComboBoxControlType()
        {
            return GetControlType("System.Windows.Forms", "DataGridViewComboBoxColumn");
        }

        private static UIControlType GetControlType(string assemblyName, string formFieldTypeName)
        {
            return new UIControlType {AssemblyName = assemblyName, TypeName = formFieldTypeName};
        }

        private static UIControlType GetTextBoxControlType()
        {
            return GetControlType("System.Windows.Forms", "DataGridViewTextBoxColumn");
        }

        private static UIControlType GetCheckBoxControlType()
        {
            return GetControlType("System.Windows.Forms", "DataGridViewCheckBoxColumn");
        }

/*
        public UIFormInfo CreateBOEditor(DMClass dmClass)
        {
            if (dmClass == null) throw new ArgumentNullException("dmClass");
            BusinessObjectCollection<DMProperty> properties = dmClass.Properties;

            UIFormInfo boEditor = new UIFormInfo();
            UITab tab = boEditor.UITabs.CreateBusinessObject();
            tab.Name = "default";
            UIColumnLayout columnLayout = tab.UIColumnLayouts.CreateBusinessObject();
            columnLayout.UITab = tab;
            foreach (DMProperty property in properties)
            {
                //                if (property.IsPartOfObjectIdentity) continue;
                if (property.IsPartOfSingleRelationship()) continue;
                columnLayout.UIFields.Add(CreateUIField(property));
            }

            foreach (DMRelationship relationship in dmClass.OwnerRelationships)
            {
                if (relationship.Cardinality == Cardinality.Multiple) continue;
                columnLayout.UIFields.Add(CreateUIField(relationship));
            }
            return boEditor;
        }*/

        /*      public UIField CreateUIField(DMProperty property)
        {
            if (property == null) throw new ArgumentNullException("property");
            if (property.PropertyType == null)
            {
                throw new HabaneroApplicationException("The property Type for '" + property.PropertyName + "' cannot be null in method 'UIBOEditorCreator.GetControlType'");
            }

            ValidatePropertySetupCorrectly(property);
            UIField formField = new UIField
            {
                PropertyName = property.PropertyName,
                OrdinalPosition = property.OrdinalPosition,
                UIControlType = GetControlType(property)
            };
            return formField;
        }

        public UIField CreateUIField(DMRelationship relationship)
        {
            if (relationship == null) throw new ArgumentNullException("relationship");

            UIField formField = new UIField { PropertyName = relationship.RelationshipName };
            if (relationship.RelationshipProperties.Count > 0)
            {
                IDMProperty ownerProperty = relationship.RelationshipProperties[0].OwnerProperty;
                formField.OrdinalPosition = ownerProperty == null ? 0 : ownerProperty.OrdinalPosition;
            }
            else
            {
                formField.OrdinalPosition = 0;
            }

            formField.UIControlType = GetComboBoxControlType(relationship.OwnerClass.Solution);
            formField.UIControlMapperType = GetRelationshipControlMapperType(relationship.OwnerClass.Solution);
            return formField;
        }

        private static UIControlMapperType GetRelationshipControlMapperType()
        {
            return ((DMSolution)solution).GetUIControlMapperType(BOBroker.GetMapperAssemblyName(), "AutoLoadingRelationshipComboBoxMapper");
        }*/
    }
}