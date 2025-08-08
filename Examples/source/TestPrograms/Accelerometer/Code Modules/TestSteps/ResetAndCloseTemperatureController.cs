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
        /// Resets the temperature controller to room temperatures and performs required shutdown steps.
        /// This code module is intended to be called only once during test program execution, when the lot ends.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void ResetAndCloseTemperatureController(ISemiconductorModuleContext semiconductorModuleContext)
        {
            // The SimulatedTemperatureController object reference was stored at the beginning of the test program execution,
            // by the ConfigureTemperatureController code module, and must now be retrieved.
            var temperatureController = (SimulatedTemperatureController)semiconductorModuleContext.GetGlobalData(TemperatureControllerDataId);
            temperatureController.ResetToRoomTemperature();
            temperatureController.Shutdown();
        }
    }
}
