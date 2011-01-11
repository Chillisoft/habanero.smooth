
// ------------------------------------------------------------------------------
// This class was auto-generated for use with the Habanero Enterprise Framework.
// ------------------------------------------------------------------------------

namespace TestProjectNoDBSpecificProps.BO
{
    using System;
    using Habanero.BO;
    
    
    public class Car: Vehicle
    {
        #region Properties
        public virtual string Make
        {
            get
            {
                return ((String)(base.GetPropertyValue("Make")));
            }
            set
            {
                base.SetPropertyValue("Make", value);
            }
        }
        public virtual String Model
        {
            get
            {
                return ((String)(base.GetPropertyValue("Model")));
            }
            set
            {
                base.SetPropertyValue("Model", value);
            }
        }
        #endregion

        #region Relationships
        public virtual BusinessObjectCollection<Driver> Drivers
        {
            get
            {
                return Relationships.GetRelatedCollection<Driver>("Drivers");
            }
        }

        public virtual SteeringWheel SteeringWheel
        {
            get
            {
                return Relationships.GetRelatedObject<SteeringWheel>("SteeringWheel");
            }
            set
            {
                Relationships.SetRelatedObject("SteeringWheel", value);
            }
        }
        #endregion
    }
}
