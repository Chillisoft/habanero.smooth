
// ------------------------------------------------------------------------------
// This class was auto-generated for use with the Habanero Enterprise Framework.
// NB Custom code should be placed in the provided stub class.
// Please do not modify this class directly!
// ------------------------------------------------------------------------------

using Habanero.Smooth;

namespace TestProject.BO
{
    using System;
    using Habanero.BO;
    
    public partial class Car : Vehicle
    {
        
        #region Properties
        public virtual String Make
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
      
        public virtual int? NoOfDoors
        {
            get
            {
                return ((int?)(base.GetPropertyValue("NoOfDoors")));
            }
            set
            {
                base.SetPropertyValue("NoOfDoors", value);
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
