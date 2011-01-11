// ------------------------------------------------------------------------------
// This partial class was auto-generated for use with the Habanero Architecture.
// NB: This class is regenerated and you run the risk of losing any changes
// you make directly.  Try placing any custom code in the non "Def" code file.
// ------------------------------------------------------------------------------

using TestProject.BO;
using TestProject.Test.Base;

namespace TestProject.Test.BO.TestUtils
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