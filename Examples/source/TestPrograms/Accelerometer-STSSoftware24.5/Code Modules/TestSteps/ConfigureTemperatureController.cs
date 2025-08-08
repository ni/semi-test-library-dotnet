using Accelerometer.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace Accelerometer
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Initializes and configures the temperature controller.
        /// This code module is intended to be called only once during test program execution, when the lot starts.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void ConfigureTemperatureController(ISemiconductorModuleContext semiconductorModuleContext)
        {
            double highTemperature = semiconductorModuleContext.GetSpecificationsValue("Temperature.High");
            var temperatureController = new SimulatedTemperatureController();
            temperatureController.SetTemperature(highTemperature);
            // Store temperature controller so you can access it in the rest of the test code.
            semiconductorModuleContext.SetGlobalData(TemperatureControllerDataId, temperatureController);
        }
    }
}
