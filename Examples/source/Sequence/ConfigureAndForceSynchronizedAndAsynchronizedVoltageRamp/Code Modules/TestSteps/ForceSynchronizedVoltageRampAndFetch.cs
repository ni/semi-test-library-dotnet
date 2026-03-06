using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using System.Linq;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// This class provides example methods to demonstrate how to use the extension method required for Hardware Level Sequencing from the Semiconductor Test Library.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// This example demonstrates how to force a hardware-timed sequence of voltage sequence, created using <see cref="HelperMethods.CreateRampSequence(double, double, int)"/> on the specified pins across all sites using.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to force voltage sequence on.</param>
        public static void ForceSynchronizedVoltageRampAndFetch(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var voltageSequence = HelperMethods.CreateRampSequence(outputStart: 0, outputStop: 3, numberOfPoints: 10);
            var dcPowerPins = sessionManager.DCPower(smuPinNames);

            dcPowerPins.ForceVoltageSequenceSynchronized(voltageSequence, waitForSequenceCompletion: true, sequenceTimeoutInSeconds: 10);

            dcPowerPins.MeasureAndPublishVoltage("Voltage", out _);

            // Disabling all the trigger post using ForceVoltageSequenceSynchronized and fetching the measurements to clean up and avoid any unintended consequences on later test steps that may use the same pins.
            dcPowerPins.DisableTriggers();
        }
    }
}
