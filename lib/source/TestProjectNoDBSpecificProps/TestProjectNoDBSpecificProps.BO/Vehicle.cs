
// ------------------------------------------------------------------------------
// This class was auto-generated for use with the Habanero Enterprise Framework.
// ------------------------------------------------------------------------------

namespace TestProjectNoDBSpecificProps.BO
{
    using System;
    using Habanero.BO;
    
    
    public class Vehicle:BusinessObject
    {

        #region Properties

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
        #endregion
    }
}
