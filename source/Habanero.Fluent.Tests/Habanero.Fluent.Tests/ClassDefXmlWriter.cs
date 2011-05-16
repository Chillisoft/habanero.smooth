using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Habanero.Base;

namespace Habanero.Fluent.Tests
{
    public static class ClassDefXmlWriter
    {

        public static void CreateXml(IClassDef classDef, XmlElement parentElement)
        {
            XmlElement classDMElement = XmlUtilities.createXmlElement(parentElement, "class");
            XmlUtilities.setXmlAttribute(classDMElement, "name", classDef.ClassName);
            XmlUtilities.setXmlAttribute(classDMElement, "assembly", classDef.AssemblyName);
            XmlUtilities.setXmlAttribute(classDMElement, "table", classDef.TableName, classDef.ClassName);
            XmlUtilities.setXmlAttribute(classDMElement, "displayName", classDef.DisplayName); //, DMClass.GetDisplayNameFromClassName(classDef.ClassName));

            createSuperClassDMXml(classDMElement, classDef);

            foreach (IPropDef propDef in classDef.PropDefcol)
            {
                createPropertyDMXml(classDMElement, propDef);
            }

            createKeyDMXml(classDMElement, classDef);

            createPrimaryKeyXml(classDMElement, classDef);

            foreach (IRelationshipDef relationshipDef in classDef.RelationshipDefCol)
            {
                createRelationshipXml(classDMElement, relationshipDef);
            }
        }

        private static void createSuperClassDMXml(XmlElement classDMElement, IClassDef DMClass)
        {
            ISuperClassDef superClassDef = DMClass.SuperClassDef;
            if (superClassDef == null) return;
            //if (!inheritanceRelationship.HasSuperClass) return;
            IClassDef superClassClassDef = superClassDef.SuperClassClassDef;
            if (superClassClassDef == null) return;
            XmlElement SuperClassDMElement = XmlUtilities.createXmlElement(classDMElement, "superClass");
            XmlUtilities.setXmlAttribute(SuperClassDMElement, "class", superClassClassDef.ClassName);
            XmlUtilities.setXmlAttribute(SuperClassDMElement, "assembly", superClassClassDef.AssemblyName);
            XmlUtilities.setXmlAttribute(SuperClassDMElement, "orMapping", superClassDef.ORMapping,
                                         ORMapping.ClassTableInheritance);
            switch (superClassDef.ORMapping)
            {
                case ORMapping.ClassTableInheritance:
                    XmlUtilities.setXmlAttribute(SuperClassDMElement, "discriminator", superClassDef.Discriminator, "");
                    XmlUtilities.setXmlAttribute(SuperClassDMElement, "id", superClassDef.ID, "");
                    break;
                case ORMapping.SingleTableInheritance:
                    XmlUtilities.setXmlAttribute(SuperClassDMElement, "discriminator", superClassDef.Discriminator, "");
                    break;
                case ORMapping.ConcreteTableInheritance:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void createPropertyDMXml(XmlElement classDMElement, IPropDef propDef)
        {
            XmlElement propDMElement = XmlUtilities.createXmlElement(classDMElement, "property");
            XmlUtilities.setXmlAttribute(propDMElement, "name", propDef.PropertyName);
            XmlUtilities.setXmlAttribute(propDMElement, "type", propDef.PropertyTypeName, "String");
            XmlUtilities.setXmlAttribute(propDMElement, "assembly", propDef.PropertyTypeAssemblyName == "CommonLanguageRuntimeLibrary" ? "System" : propDef.PropertyTypeAssemblyName, "System");
            XmlUtilities.setXmlAttribute(propDMElement, "readWriteRule", propDef.ReadWriteRule, PropReadWriteRule.ReadWrite);
            XmlUtilities.setXmlAttribute(propDMElement, "databaseField", propDef.DatabaseFieldName, propDef.PropertyName);
            XmlUtilities.setXmlAttribute(propDMElement, "default", propDef.DefaultValue);
            XmlUtilities.setXmlAttribute(propDMElement, "compulsory", propDef.Compulsory, false);
            XmlUtilities.setXmlAttribute(propDMElement, "autoIncrementing", propDef.AutoIncrementing, false);
            XmlUtilities.setXmlAttribute(propDMElement, "displayName", propDef.DisplayName, propDef.PropertyName);
            XmlUtilities.setXmlAttribute(propDMElement, "description", propDef.Description);
            XmlUtilities.setXmlAttribute(propDMElement, "keepValuePrivate", propDef.KeepValuePrivate, false);

            createPropRulesXml(propDMElement, propDef);
            createLookupListXml(propDMElement, propDef);
        }

        private static void createPropRulesXml(XmlElement propDMElement, IPropDef propDef)
        {
            foreach (IPropRule propRule in propDef.PropRules)
            {
                createPropRuleXml(propDMElement, propRule);
            }
        }

        private static void createPropRuleXml(XmlElement propDMElement, IPropRule propRule)
        {
            if (propRule == null) return;
            if (propRule.Name == null) return;
            if (propRule.Parameters.Count == 0) return;
            if (new List<object>(propRule.Parameters.Values).TrueForAll(o => o == null)) return;

            //if (!propRule.HasRule) return;
            XmlElement ruleElement = XmlUtilities.createXmlElement(propDMElement, "rule");
            XmlUtilities.setXmlAttribute(ruleElement, "name", propRule.Name);
            //TODO Mark 28 Sep 2009: Investigate what is needed to support custom rules
            //if (propRule.IsCustomRule)
            //{
            //    XmlUtilities.setXmlAttribute(ruleElement, "class", propRule.ClassName);
            //    XmlUtilities.setXmlAttribute(ruleElement, "assembly", propRule.AssemblyName);
            //}
            XmlUtilities.setXmlAttribute(ruleElement, "message", propRule.Message);
            foreach (KeyValuePair<string, object> pair in propRule.Parameters)
            {
                addKeyValuePair(ruleElement, pair.Key, pair.Value, null);
            }
        }

        private static void addKeyValuePair(XmlElement ruleElement, string key, object attributeValue, object attributeDefault)
        {
            string value = XmlUtilities.ObjectValueToString(attributeValue);
            string defaultValue = XmlUtilities.ObjectValueToString(attributeDefault);
            bool isDefault = (value == defaultValue);
            isDefault |= string.IsNullOrEmpty(value);
            if (ruleElement != null && !string.IsNullOrEmpty(key) && !isDefault)
            {
                XmlElement addElement = XmlUtilities.createXmlElement(ruleElement, "add");
                XmlUtilities.setXmlAttribute(addElement, "key", key);
                XmlUtilities.setXmlAttribute(addElement, "value", value);
            }
        }

        private static void createLookupListXml(XmlElement propDMElement, IPropDef propDef)
        {
            ILookupList lookupListBase = propDef.LookupList;
            if (lookupListBase == null) return;
            //IClassDef lookupClass = lookupList.LookupClass;
            XmlElement lookupElement;
            if (lookupListBase is ISimpleLookupList)
            {
                ISimpleLookupList lookupList = (ISimpleLookupList)lookupListBase;

                lookupElement = XmlUtilities.createXmlElement(propDMElement, "simpleLookupList");
                foreach (KeyValuePair<string, string> pair in lookupList.GetLookupList())
                {
                    XmlElement addElement = XmlUtilities.createXmlElement(lookupElement, "item");
                    XmlUtilities.setXmlAttribute(addElement, "display", pair.Key);
                    XmlUtilities.setXmlAttribute(addElement, "value", pair.Value);
                }
            }

            if (lookupListBase is IDatabaseLookupList)
            {
                IDatabaseLookupList lookupList = (IDatabaseLookupList)lookupListBase;
                lookupElement = XmlUtilities.createXmlElement(propDMElement, "databaseLookupList");
                XmlUtilities.setXmlAttribute(lookupElement, "sql", lookupList.SqlString);
                XmlUtilities.setXmlAttribute(lookupElement, "timeout", lookupList.TimeOut, 10000);
                XmlUtilities.setXmlAttribute(lookupElement, "class", lookupList.ClassName);
                XmlUtilities.setXmlAttribute(lookupElement, "assembly", lookupList.AssemblyName);
            }

            if (lookupListBase is IBusinessObjectLookupList)
            {
                IBusinessObjectLookupList lookupList = (IBusinessObjectLookupList)lookupListBase;
                lookupElement = XmlUtilities.createXmlElement(propDMElement, "businessObjectLookupList");
                XmlUtilities.setXmlAttribute(lookupElement, "class", lookupList.ClassName);
                XmlUtilities.setXmlAttribute(lookupElement, "assembly", lookupList.AssemblyName);
                XmlUtilities.setXmlAttribute(lookupElement, "criteria", lookupList.CriteriaString);
                XmlUtilities.setXmlAttribute(lookupElement, "timeout", lookupList.TimeOut, 10000);
                //TODO Mark 28 Sep 2009: Check that this sort string is getting built correctly
                XmlUtilities.setXmlAttribute(lookupElement, "sort", lookupList.SortString);
            }
        }

        private static void createKeyDMXml(XmlElement classDMElement, IClassDef classDef)
        {
            foreach (IKeyDef keyDef in classDef.KeysCol)
            {
                XmlElement keyDMElement = XmlUtilities.createXmlElement(classDMElement, "key");
                string keyName = keyDef.KeyName;
                XmlUtilities.setXmlAttribute(keyDMElement, "name", keyName);
                XmlUtilities.setXmlAttribute(keyDMElement, "message", keyDef.Message);
                XmlUtilities.setXmlAttribute(keyDMElement, "ignoreIfNull", keyDef.IgnoreIfNull, false);

                foreach (IPropDef propDef in keyDef)
                {
                    XmlElement keyPropElement = XmlUtilities.createXmlElement(keyDMElement, "prop");
                    string propName = propDef.PropertyName;
                    //keyName = keyName.Replace(propName, "");
                    XmlUtilities.setXmlAttribute(keyPropElement, "name", propName);
                }
                keyName = keyName.Replace("_", "");
                if (keyName.Length == 0)
                    keyDMElement.RemoveAttribute("name");
            }
        }

        private static void createPrimaryKeyXml(XmlElement classDMElement, IClassDef classDef)
        {

            IPrimaryKeyDef primaryKeyDef = classDef.PrimaryKeyDef;
            if (primaryKeyDef == null)
            {
                return;
            }

            if (primaryKeyDef.Count == 0) return;
            XmlElement primaryKeyDMElement = XmlUtilities.createXmlElement(classDMElement, "primaryKey");
            XmlUtilities.setXmlAttribute(primaryKeyDMElement, "isObjectID", primaryKeyDef.IsGuidObjectID, true);
            foreach (IPropDef dmKeyProperty in primaryKeyDef)
            {
                XmlElement keyPropElement = XmlUtilities.createXmlElement(primaryKeyDMElement, "prop");
                XmlUtilities.setXmlAttribute(keyPropElement, "name", dmKeyProperty.PropertyName);
            }
        }

        private static void createRelationshipXml(XmlElement classDMElement, IRelationshipDef relationshipDef)
        {
            //IClassDef relatedClassDef = relationshipDef.RelatedObjectClassDef;//.MyRelatedClass.CurrentClass;
            //if (relatedClassDef == null) return;
            XmlElement relationshipDMElement = XmlUtilities.createXmlElement(classDMElement, "relationship");
            XmlUtilities.setXmlAttribute(relationshipDMElement, "name", relationshipDef.RelationshipName);
            bool isMultiple = false;
            //TODO Mark 28 Sep 2009: Review this and change it to use the IRelationshipDef interface methods or classes
            if (relationshipDef.IsManyToOne || relationshipDef.IsOneToOne)
                XmlUtilities.setXmlAttribute(relationshipDMElement, "type", "single");
            else if (relationshipDef.IsOneToMany)
            {
                XmlUtilities.setXmlAttribute(relationshipDMElement, "type", "multiple");
                isMultiple = true;
            }
            if (isMultiple)
            {
                XmlUtilities.setXmlAttribute(relationshipDMElement, "timeout", relationshipDef.TimeOut, 0);
            }
            XmlUtilities.setXmlAttribute(relationshipDMElement, "relatedClass", relationshipDef.RelatedObjectClassName);
            XmlUtilities.setXmlAttribute(relationshipDMElement, "reverseRelationship", relationshipDef.ReverseRelationshipName);
            XmlUtilities.setXmlAttribute(relationshipDMElement, "relatedAssembly", relationshipDef.RelatedObjectAssemblyName);
            XmlUtilities.setXmlAttribute(relationshipDMElement, "keepReference", relationshipDef.KeepReferenceToRelatedObject, true);
            XmlUtilities.setXmlAttribute(relationshipDMElement, "deleteAction", relationshipDef.DeleteParentAction, DeleteParentAction.Prevent);
            XmlUtilities.setXmlAttribute(relationshipDMElement, "relationshipType", relationshipDef.RelationshipType, RelationshipType.Association);
            // XmlUtilities.setXmlAttribute(relationshipDMElement, "relationshipDescription", relationshipDef.);
            if (isMultiple)
            {
                XmlUtilities.setXmlAttribute(relationshipDMElement, "orderBy", relationshipDef.OrderCriteriaString);
            }
            else
            {
                XmlUtilities.setXmlAttribute(relationshipDMElement, "owningBOHasForeignKey", relationshipDef.OwningBOHasForeignKey, true);
            }

            foreach (IRelPropDef relPropDef in relationshipDef.RelKeyDef)
            {
                if (relPropDef.OwnerPropertyName != null && relPropDef.RelatedClassPropName != null)
                {
                    XmlElement relPropElement = XmlUtilities.createXmlElement(relationshipDMElement, "relatedProperty");
                    XmlUtilities.setXmlAttribute(relPropElement, "property", relPropDef.OwnerPropertyName);
                    XmlUtilities.setXmlAttribute(relPropElement, "relatedProperty", relPropDef.RelatedClassPropName);
                }
            }
            classDMElement.AppendChild(relationshipDMElement);
        }

    }

}
