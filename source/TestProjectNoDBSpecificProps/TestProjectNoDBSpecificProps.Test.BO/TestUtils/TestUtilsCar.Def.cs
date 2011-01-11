// ------------------------------------------------------------------------------
// This partial class was auto-generated for use with the Habanero Architecture.
// NB: This class is regenerated and you run the risk of losing any changes
// you make directly.  Try placing any custom code in the non "Def" code file.
// ------------------------------------------------------------------------------


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