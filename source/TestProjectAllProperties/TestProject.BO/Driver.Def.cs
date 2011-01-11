
// ------------------------------------------------------------------------------
// This class was auto-generated for use with the Habanero Enterprise Framework.
// NB Custom code should be placed in the provided stub class.
// Please do not modify this class directly!
// ------------------------------------------------------------------------------

namespace TestProject.BO
{
    using System;
    using Habanero.BO;
    
    
    public partial class Driver : BusinessObject
    {
        
        #region Properties
        public virtual Guid? DriverID
        {
            get
            {
                return ((Guid?)(base.GetPropertyValue("DriverID")));
            }
            set
            {
                base.SetPropertyValue("DriverID", value);
            }
        }
        
        public virtual String DriverName
        {
            get
            {
                return ((String)(base.GetPropertyValue("DriverName")));
            }
            set
            {
                base.SetPropertyValue("DriverName", value);
            }
        }
        
        public virtual Int32? Age
        {
            get
            {
                return ((Int32?)(base.GetPropertyValue("Age")));
            }
            set
            {
                base.SetPropertyValue("Age", value);
            }
        }
        
        public virtual DateTime? DOB
        {
            get
            {
                return ((DateTime?)(base.GetPropertyValue("DOB")));
            }
            set
            {
                base.SetPropertyValue("DOB", value);
            }
        }
        
        public virtual Double? LicenseRaing
        {
            get
            {
                return ((Double?)(base.GetPropertyValue("LicenseRaing")));
            }
            set
            {
                base.SetPropertyValue("LicenseRaing", value);
            }
        }
        
        public virtual Guid? CarID
        {
            get
            {
                return ((Guid?)(base.GetPropertyValue("CarID")));
            }
            set
            {
                base.SetPropertyValue("CarID", value);
            }
        }
        #endregion
        
        #region Relationships
        public virtual Car Car
        {
            get
            {
                return Relationships.GetRelatedObject<Car>("Car");
            }
            set
            {
                Relationships.SetRelatedObject("Car", value);
            }
        }
        #endregion
    }
}
