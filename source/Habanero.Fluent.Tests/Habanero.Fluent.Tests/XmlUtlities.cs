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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Habanero.Fluent.Tests
{
    public static class XmlUtilities
    {
        public static XmlElement createXmlElement(XmlElement parentNode, string nodeName)
        {
            return createXmlElement(parentNode, nodeName, null);
        }

        public static XmlElement createXmlElement(XmlElement parentNode, string nodeName, string nodeValue)
        {
            XmlDocument xmldoc = parentNode.OwnerDocument;
            XmlElement newElement = xmldoc.CreateElement(nodeName);
            if (nodeValue != null)
            {
                newElement.InnerText = nodeValue;
            }
            return (XmlElement)parentNode.AppendChild(newElement);
        }

        public static void setXmlAttribute(XmlElement parentElement,
                                             string attributeName, object attributeValue)
        {
            setXmlAttribute(parentElement, attributeName, attributeValue, new object[] { });
        }

        public static void setXmlAttribute(XmlElement parentElement,
                                             string attributeName, object attributeValue, object attributeDefault)
        {
            setXmlAttribute(parentElement, attributeName, attributeValue, new[] { attributeDefault });
        }

        public static void setXmlAttribute(XmlElement parentElement,
                                             string attributeName, object attributeValue, object attributeDefault, object attributeDefault2)
        {
            setXmlAttribute(parentElement, attributeName, attributeValue, new[] { attributeDefault, attributeDefault2 });
        }

        public static void setXmlAttribute(XmlElement parentElement,
                                             string attributeName, object attributeValue, object[] attributeDefaults)
        {
            string valueString;
            valueString = ObjectValueToString(attributeValue);
            bool isDefault = false;
            foreach (object attributeDefault in attributeDefaults)
            {
                string defaultString = ObjectValueToString(attributeDefault);
                isDefault |= (valueString == defaultString);
            }
            isDefault |= (valueString == "");
            isDefault |= (valueString == null);
            if (parentElement != null && !isDefault)
            {
                parentElement.SetAttribute(attributeName, valueString);
            }
        }

        public static string ObjectValueToString(object obj)
        {
            string valueString;
            if (obj == null)
            {
                valueString = null;
            }
            else if (obj is bool)
            {
                return obj.ToString().ToLower();
            }
            else if (obj is DateTime)
            {
                valueString = ((DateTime)obj).ToString("dd MMM yyyy HH:mm:ss");
            }
            else if (obj.GetType().IsEnum)
            {
                valueString = EnumValueToString(obj);
            }
            else
                valueString = obj.ToString();
            return valueString;
        }

        internal static bool getSingleNodeText(XmlElement parentElement,
                                               string XPath, out string nodeText, out XmlElement foundElement)
        {
            bool found = false;
            nodeText = "";
            foundElement = null;
            if (parentElement != null && parentElement.HasChildNodes)
            {
                foundElement = (XmlElement)parentElement.SelectSingleNode(XPath);
                if (foundElement != null)
                {
                    found = true;
                    nodeText = foundElement.InnerText;
                }
            }
            return found;
        }

        internal static string GetXmlAttribute(XmlElement parentElement, string attributeName)
        {
            return GetXmlAttribute(parentElement, attributeName, "");
        }

        internal static string GetXmlAttribute(XmlElement parentElement, string attributeName, string defaultValue)
        {
            return GetXmlAttribute<string>(parentElement, attributeName, defaultValue);
        }

        internal static T GetXmlAttribute<T>(XmlElement parentElement, string attributeName, T defaultValue)
        {
            string nodeText = null;
            if (parentElement.HasAttribute(attributeName))
            {
                nodeText = parentElement.GetAttribute(attributeName);
            }
            Type returnType = typeof(T);
            T returnResult = defaultValue;
            bool useDefault = false;
            if (returnType.Equals(typeof(bool)))
            {
                bool result;
                if (bool.TryParse(nodeText, out result))
                {
                    returnResult = (T)(object)result;
                }
                else useDefault = true;
            }
            else if (returnType.Equals(typeof(DateTime)))
            {
                DateTime result;
                if (DateTime.TryParse(nodeText, out result))
                {
                    returnResult = (T)(object)result;
                }
                else useDefault = true;
            }
            else if (returnType.Equals(typeof(Int32)))
            {
                Int32 result;
                if (Int32.TryParse(nodeText, out result))
                {
                    returnResult = (T)(object)result;
                }
                else useDefault = true;
            }
            else if (returnType.Equals(typeof(Double)))
            {
                Double result;
                if (Double.TryParse(nodeText, out result))
                {
                    returnResult = (T)(object)result;
                }
                else useDefault = true;
            }
            else if (returnType.IsEnum)
            {
                try
                {
                    object result;
                    result = Enum.Parse(typeof(T), nodeText, true);
                    returnResult = (T)result;
                }
                catch
                {
                    useDefault = true;
                }
            }
            else
            {
                try
                {
                    returnResult = (T)(object)nodeText;
                }
                catch
                {
                    useDefault = true;
                }
            }
            if (!useDefault)
            {
                return returnResult;
            }
            else
            {
                return defaultValue;
            }
        }

        internal static string EnumValueToString(object enumValue)
        {
            return Enum.GetName(enumValue.GetType(), enumValue);
        }

        #region FileControl

        internal delegate T LoaderMethod<T>(string xml);
        internal static T LoadFromFile<T>(string fileName, LoaderMethod<T> loaderMethod)
            where T : new()
        {
            if (fileName != null && File.Exists(fileName))
            {
                StreamReader fileReader = new StreamReader(fileName);
                string strXml = fileReader.ReadToEnd();
                fileReader.Close();
                return loaderMethod(strXml);
            }
            else
            {
                return new T();
            }
        }

        public static void SaveToFile(string fileName, string xml)
        {
            CreateXmlFile(Path.GetDirectoryName(fileName), Path.GetFileName(fileName), xml, true, null);

            //string path = Path.GetDirectoryName(fileName);
            //if (path.Length > 0 && !Directory.Exists(path))
            //{
            //    Directory.CreateDirectory(path);
            //}
            //if (File.Exists(fileName))
            //{
            //    File.Delete(fileName);
            //}
            //XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            //xmlWriterSettings.Indent = true;
            //xmlWriterSettings.CloseOutput = true;
            //xmlWriterSettings.OmitXmlDeclaration = true;

            //XmlWriter xmlWriter = XmlWriter.Create(fileName, xmlWriterSettings);
            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.LoadXml(xml);
            //xmlDoc.Save(xmlWriter);
            //xmlWriter.Close();
        }

        public static string CreateXmlFile(string folderPath, string filePath, string xml, bool overwrite, Encoding encoding)
        {
            string destFile = Path.Combine(folderPath, filePath);
            string path = Path.GetDirectoryName(destFile);
            if (!File.Exists(destFile) || overwrite)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (File.Exists(destFile))
                {
                    File.Delete(destFile);
                }
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.CloseOutput = true;
                xmlWriterSettings.OmitXmlDeclaration = true;
                if (encoding != null) xmlWriterSettings.Encoding = encoding;
                if (!String.IsNullOrEmpty(xml))
                {
                    XmlWriter xmlWriter = XmlWriter.Create(destFile, xmlWriterSettings);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xml);
                    xmlDoc.Save(xmlWriter);
                    xmlWriter.Close();
                }
                else
                {
                    File.WriteAllText(destFile, xml);
                }

            }
            return destFile;
        }

        #endregion
    }

}
