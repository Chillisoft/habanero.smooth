
// ------------------------------------------------------------------------------
// This class was auto-generated for use with the Habanero Enterprise Framework.
// NB Custom code should be placed in the provided stub class.
// Please do not modify this class directly!
// ------------------------------------------------------------------------------

namespace TestProject.BO
{
    using System;
    using Habanero.BO;
    
    
    public partial class SteeringWheel : BusinessObject
    {
        
        #region Properties
        public virtual Guid? SteeringWheelID
        {
            get
            {
                return ((Guid?)(base.GetPropertyValue("SteeringWheelID")));
            }
            set
            {
                base.SetPropertyValue("SteeringWheelID", value);
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
