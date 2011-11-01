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
    
    
    public class Driver: BusinessObject
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
