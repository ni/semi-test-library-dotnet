using System;
using System.Threading;
using NationalInstruments.Examples.SemiconductorTestLibrary.Accelerometer.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.Examples.SemiconductorTestLibrary.Accelerometer.Common.Constants;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.Accelerometer
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Waits for the temperature to reach the configured temperature value.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <exception cref="TimeoutException">
        /// This exception is thrown if the configured temperature is not reached within the timeout limit (15 seconds).
        /// </exception>
        public static void WaitForTemperature(ISemiconductorModuleContext semiconductorModuleContext)
        {
            var temperatureController = semiconductorModuleContext.GetGlobalData<SimulatedTemperatureController>(TemperatureControllerDataId);

            const int timeoutSeconds = 15;
            DateTime endTime = DateTime.Now.AddSeconds(timeoutSeconds);
            while (DateTime.Now < endTime)
            {
                if (temperatureController.CurrentTemperature == temperatureController.RequestedTemperature)
                {
                    return;
                }
                Thread.Sleep(100);
            }
            throw new TimeoutException("Timeout occurred waiting for temperature controller to reach desired temperature.");
        }
    }
}