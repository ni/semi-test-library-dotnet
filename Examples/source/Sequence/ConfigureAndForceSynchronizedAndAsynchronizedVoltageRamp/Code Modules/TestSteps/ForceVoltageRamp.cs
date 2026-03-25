using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// This class provides example methods to demonstrate how to use the extension method required for Hardware Level Sequencing from the Semiconductor Test Library.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// This example demonstrates how to force the same voltage sequence created using <see cref="HelperMethods.CreateRampSequence(double, double, int)"/> on the specified pins.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">SMU pin names to be configured.</param>
        public static void ForceVoltageRamp(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var dcPowerPins = sessionManager.DCPower(smuPinNames);

            var voltageSequence = HelperMethods.CreateRampSequence(outputStart: 0, outputStop: 3, numberOfPoints: 10);
            dcPowerPins.ForceVoltageSequence(voltageSequence);
        }
    }
}
