// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2010 Chillisoft Solutions
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
using System;
using System.Xml;
using System.Xml.Schema;
using Habanero.Base;
using Habanero.BO;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedTypeParameter
namespace FakeBosInSeperateAssembly
{
    public class FakeExtBoOnlyImplementingInterfaceShouldBeLoaded : IBusinessObject
    {
        #region implementation Methods


        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public bool IsEditable(out string message)
        {
            throw new NotImplementedException();
        }

        public bool IsDeletable(out string message)
        {
            throw new NotImplementedException();
        }

        public object GetPropertyValue(string propName)
        {
            throw new NotImplementedException();
        }

        public object GetPropertyValue(Source source, string propName)
        {
            throw new NotImplementedException();
        }

        public void SetPropertyValue(string propName, object newPropValue)
        {
            throw new NotImplementedException();
        }

        public IBusinessObject Save()
        {
            throw new NotImplementedException();
        }

        public void Restore()
        {
            throw new NotImplementedException();
        }

        public void CancelEdits()
        {
            throw new NotImplementedException();
        }

        public void MarkForDelete()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public bool IsValid(out string invalidReason)
        {
            throw new NotImplementedException();
        }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }

        public bool IsCreatable(out string message)
        {
            throw new NotImplementedException();
        }

        public object GetPersistedPropertyValue(Source source, string propName)
        {
            throw new NotImplementedException();
        }

        public string GetPropertyValueString(string propName)
        {
            throw new NotImplementedException();
        }

        public IPrimaryKey ID
        {
            get { throw new NotImplementedException(); }
        }

        public IClassDef ClassDef
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IRelationshipCol Relationships
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IBOStatus Status
        {
            get { throw new NotImplementedException(); }
        }

        public IBOPropCol Props
        {
            get { throw new NotImplementedException(); }
        }

#pragma warning disable 
        public event EventHandler<BOEventArgs> Updated;
        public event EventHandler<BOEventArgs> Saved;
        public event EventHandler<BOEventArgs> Deleted;
        public event EventHandler<BOEventArgs> Restored;
        public event EventHandler<BOEventArgs> IDUpdated;
        public event EventHandler<BOPropUpdatedEventArgs> PropertyUpdated;
        public event EventHandler<BOEventArgs> MarkedForDeletion;
#pragma warning restore 
        #endregion
    }
    public class FakeExtBoShouldBeLoaded : BusinessObject
    {
    }
    public interface IFakeExtBoInterfaceShouldNotBeLoaded : IBusinessObject
    {
    }
    /// <summary>
    /// Currently Habanero Cannot deal with abstract classes since they are required to be created in the loader.
    /// </summary>
    public interface FakeExtAbstractBoInterfaceShouldNotBeLoaded : IBusinessObject
    {
    }

    /// <summary>
    /// Currently Habanero Cannot deal with abstract classes since they are required to be created in the loader.
    /// </summary>
    public interface FakeExtGenericBOShouldNotBeLoaded<T> : IBusinessObject
    {
    }
}
// ReSharper restore InconsistentNaming
// ReSharper restore UnusedTypeParameter

