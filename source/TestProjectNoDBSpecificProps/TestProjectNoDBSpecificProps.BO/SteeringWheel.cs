
// ------------------------------------------------------------------------------
// This class was auto-generated for use with the Habanero Enterprise Framework.
// ------------------------------------------------------------------------------

namespace TestProjectNoDBSpecificProps.BO
{
    using System;
    using Habanero.BO;
    
    
    public class SteeringWheel:BusinessObject
    {
        #region Properties
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
