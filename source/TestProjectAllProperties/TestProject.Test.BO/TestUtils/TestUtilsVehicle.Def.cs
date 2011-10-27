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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using NUnit.Framework;
using TestProject.BO;

namespace TestProject.Test.BO
{
    // Creates sample Vehicle objects.
    // If these methods create invalid data, cut the offending code and paste it into the
    // other half of this partial class, where you can adapt it.  You will need to cut the code
    // again each time you regenerate.
	public partial class TestUtilsVehicle
    {
        /// <summary>
        /// Creates a new saved Vehicle with a random value assigned to every property
        /// </summary>
        public static Vehicle CreateSavedVehicle()
		{
		    Vehicle vehicle = CreateUnsavedValidVehicle();
			vehicle.Save();
			return vehicle;
		}

        /// <summary>
        /// Creates a new unsaved Vehicle with a random value assigned to every property
        /// </summary>
		public static Vehicle CreateUnsavedValidVehicle()
		{
			Vehicle vehicle = new Vehicle();
			vehicle.MaxSpeed = (double)TestUtilsShared.GetRandomInt();
			return vehicle;
		}
	    
	    /// <summary>
        /// Creates a new unsaved Vehicle where all properties are null, except ID properties
        /// and those with default values.  If there are compulsory properties without
        /// defaults, saving the object will throw an exception.
        /// </summary>
		public static Vehicle CreateUnsavedDefaultVehicle()
		{
			Vehicle vehicle = new Vehicle();
			return vehicle;
		}
    }
}