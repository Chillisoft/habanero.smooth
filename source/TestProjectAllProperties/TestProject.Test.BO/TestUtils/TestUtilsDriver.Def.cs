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
//using TestProjectNoDBSpecificProps.BO;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using NUnit.Framework;
using TestProject.BO;

namespace TestProject.Test.BO
{
    // Creates sample Driver objects.
    // If these methods create invalid data, cut the offending code and paste it into the
    // other half of this partial class, where you can adapt it.  You will need to cut the code
    // again each time you regenerate.
	public partial class TestUtilsDriver
    {
        /// <summary>
        /// Creates a new saved Driver with a random value assigned to every property
        /// </summary>
        public static Driver CreateSavedDriver()
		{
		    Driver driver = CreateUnsavedValidDriver();
			driver.Save();
			return driver;
		}

        /// <summary>
        /// Creates a new unsaved Driver with a random value assigned to every property
        /// </summary>
		public static Driver CreateUnsavedValidDriver()
		{
			Driver driver = new Driver();
			driver.DriverName = TestUtilsShared.GetRandomString();
			driver.Age = TestUtilsShared.GetRandomInt();
			driver.DOB = TestUtilsShared.GetRandomDate();
			driver.LicenseRaing = (double)TestUtilsShared.GetRandomInt();
            driver.Car = TestUtilsCar.CreateSavedCar();
			return driver;
		}
	    
	    /// <summary>
        /// Creates a new unsaved Driver where all properties are null, except ID properties
        /// and those with default values.  If there are compulsory properties without
        /// defaults, saving the object will throw an exception.
        /// </summary>
		public static Driver CreateUnsavedDefaultDriver()
		{
			Driver driver = new Driver();
			return driver;
		}
    }
}