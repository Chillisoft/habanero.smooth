
// ------------------------------------------------------------------------------
// This class was auto-generated for use with the Habanero Enterprise Framework.
// NB Custom code should be placed in the provided stub class.
// Please do not modify this class directly!
// ------------------------------------------------------------------------------

namespace TestProject.BO
{
    using System;
    using Habanero.BO;
    
    
    public partial class Vehicle : BusinessObject
    {
        
        #region Properties
        public virtual Guid? VehicleID
        {
            get
            {
                return ((Guid?)(base.GetPropertyValue("VehicleID")));
            }
            set
            {
                base.SetPropertyValue("VehicleID", value);
            }
        }
        
        public virtual String VehicleType
        {
            get
            {
                return ((String)(base.GetPropertyValue("VehicleType")));
            }
            set
            {
                base.SetPropertyValue("VehicleType", value);
            }
        }
        
        public virtual Double? MaxSpeed
        {
            get
            {
                return ((Double?)(base.GetPropertyValue("MaxSpeed")));
            }
            set
            {
                base.SetPropertyValue("MaxSpeed", value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string StringProp
        {
            get { return ((string) (base.GetPropertyValue("StringProp"))); }
            set { base.SetPropertyValue("StringProp", value); }
        }
        #endregion
    }
}
