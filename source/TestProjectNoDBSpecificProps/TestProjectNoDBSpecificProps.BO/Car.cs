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
