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
using TestProjectNoDBSpecificProps.Test.BO.TestUtils;

namespace TestProjectNoDBSpecificProps.Test.BO
{
    // Creates sample SteeringWheel objects.
    // If these methods create invalid data, cut the offending code and paste it into the
    // other half of this partial class, where you can adapt it.  You will need to cut the code
    // again each time you regenerate.
	public partial class TestUtilsSteeringWheel
    {
        /// <summary>
        /// Creates a new saved SteeringWheel with a random value assigned to every property
        /// </summary>
        public static SteeringWheel CreateSavedSteeringWheel()
		{
		    SteeringWheel steeringWheel = CreateUnsavedValidSteeringWheel();
			steeringWheel.Save();
			return steeringWheel;
		}

        /// <summary>
        /// Creates a new unsaved SteeringWheel with a random value assigned to every property
        /// </summary>
		public static SteeringWheel CreateUnsavedValidSteeringWheel()
		{
			SteeringWheel steeringWheel = new SteeringWheel();
            steeringWheel.Car = TestUtilsCar.CreateSavedCar();
			return steeringWheel;
		}
	    
	    /// <summary>
        /// Creates a new unsaved SteeringWheel where all properties are null, except ID properties
        /// and those with default values.  If there are compulsory properties without
        /// defaults, saving the object will throw an exception.
        /// </summary>
		public static SteeringWheel CreateUnsavedDefaultSteeringWheel()
		{
			SteeringWheel steeringWheel = new SteeringWheel();
			return steeringWheel;
		}
    }
}