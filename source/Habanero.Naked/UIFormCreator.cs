using System;
using Habanero.Base;
using Habanero.BO.ClassDefinition;

namespace Habanero.Naked
{
    public class UIFormCreator
    {
        public IUIForm CreateUIForm(IClassDef classDef)
        {
            var uiForm = new UIForm {Title = classDef.DisplayName + " Form"};
            var uiFormTab = new UIFormTab("default");
            var uiFormColumn = new UIFormColumn();
            //The Properties are loaded in the ordinal position order
            // that they occur in the XML ClassDef so this will be the 
            // correct order they should be shown in the UI.
            foreach (var propDef in classDef.PropDefcol)
            {
                if (propDef.IsPartOfObjectIdentity()) continue;
                if (propDef.IsPropForeignKey()) continue;
                uiFormColumn.Add(GetUIFormField(propDef));
            }
            uiFormTab.Add(uiFormColumn);
            uiForm.Add(uiFormTab);
            return uiForm;
        }


        protected UIFormField GetUIFormField(IPropDef propDef)
        {
            if (propDef == null) throw new ArgumentNullException("propDef");
            var uiFormField = new UIFormField(propDef.DisplayName, propDef.PropertyName);
            SetControlType(propDef, uiFormField);
            return uiFormField;
        }

        private void SetControlType(IPropDef propDef, IUIFormField uiFormField)
        {
            var uiControlType = GetControlType(propDef);
            uiFormField.ControlAssemblyName = uiControlType.AssemblyName;
            uiFormField.ControlTypeName = uiControlType.TypeName;
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
            return GetControlType("System.Windows.Forms", "DateTimePicker");
        }

        private static UIControlType GetComboBoxControlType()
        {
            return GetControlType("System.Windows.Forms", "ComboBox");
        }

        private static UIControlType GetControlType(string assemblyName, string formFieldTypeName)
        {
            return new UIControlType {AssemblyName = assemblyName, TypeName = formFieldTypeName};
        }

        private static UIControlType GetTextBoxControlType()
        {
            return GetControlType("System.Windows.Forms", "TextBox");
        }

        private static UIControlType GetCheckBoxControlType()
        {
            return GetControlType("System.Windows.Forms", "CheckBox");
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