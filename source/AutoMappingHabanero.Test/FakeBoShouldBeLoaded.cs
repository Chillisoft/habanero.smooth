using System;
using System.Xml;
using System.Xml.Schema;
using Habanero.Base;
using Habanero.BO;
// ReSharper disable UnusedTypeParameter

namespace AutoMappingHabanero.Test
{
    public class FakeBoShouldBeLoadedOnlyImplementingInterfaceShouldBeLoaded : IBusinessObject
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

        public event EventHandler<BOEventArgs> Updated;
        public event EventHandler<BOEventArgs> Saved;
        public event EventHandler<BOEventArgs> Deleted;
        public event EventHandler<BOEventArgs> Restored;
        public event EventHandler<BOEventArgs> IDUpdated;
        public event EventHandler<BOPropUpdatedEventArgs> PropertyUpdated;
        public event EventHandler<BOEventArgs> MarkedForDeletion;

        #endregion
    }

    public class FakeBoShouldBeLoaded : BusinessObject
    {
    }
    public interface IFakeBoInterfaceShouldNotBeLoaded : IBusinessObject
    {
    }
    /// <summary>
    /// Currently Habanero Cannot deal with abstract classes since they are required to be created in the loader.
    /// </summary>
    public abstract class FakeAbstractBoShouldNotBeLoaded : BusinessObject
    {
    }

    /// <summary>
    /// Currently Habanero Cannot deal with abstract classes since they are required to be created in the loader.
    /// </summary>
    public class FakeGenericBOShouldNotBeLoaded<T> : BusinessObject
    {
    }
    // ReSharper restore UnusedTypeParameter
}