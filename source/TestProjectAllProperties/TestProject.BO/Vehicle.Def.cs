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
