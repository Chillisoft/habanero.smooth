// ------------------------------------------------------------------------------
// This partial class was auto-generated for use with the Habanero Architecture.
// NB: This class is regenerated and you run the risk of losing any changes
// you make directly.  Try placing any custom code in the non "Def" code file.
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using TestProjectNoDBSpecificProps.BO;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using NUnit.Framework;
using TestProjectNoDBSpecificProps.Test.Base;

namespace TestProjectNoDBSpecificProps.Test.BO
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