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
using Habanero.BO;
using TestProjectNoDBSpecificProps.BO;

namespace TestProjectNoDBSpecificProps.Test.BO
{
    // Creates sample Car objects.
    // If these methods create invalid data, cut the offending code and paste it into the
    // other half of this partial class, where you can adapt it.  You will need to cut the code
    // again each time you regenerate.
	public partial class TestUtilsCar
    {
        /// <summary>
        /// Creates a new saved Car with a random value assigned to every property
        /// </summary>
        public static Car CreateSavedCar()
		{
		    Car car = CreateUnsavedValidCar();
			car.Save();
			return car;
		}

        /// <summary>
        /// Creates a new unsaved Car with a random value assigned to every property
        /// </summary>
		public static Car CreateUnsavedValidCar()
		{
			Car car = new Car();
			car.Make = TestUtilsShared.GetRandomString();
			car.Model = TestUtilsShared.GetRandomString();
			car.MaxSpeed = (double)TestUtilsShared.GetRandomInt();
			return car;
		}
	    
	    /// <summary>
        /// Creates a new unsaved Car where all properties are null, except ID properties
        /// and those with default values.  If there are compulsory properties without
        /// defaults, saving the object will throw an exception.
        /// </summary>
		public static Car CreateUnsavedDefaultCar()
		{
			Car car = new Car();
			return car;
		}
    }
}