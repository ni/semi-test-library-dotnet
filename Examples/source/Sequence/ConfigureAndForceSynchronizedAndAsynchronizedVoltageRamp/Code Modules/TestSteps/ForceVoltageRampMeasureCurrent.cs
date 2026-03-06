using NationalInstruments.ModularInstruments.NIDCPower;
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
        /// Applies a voltage ramp to specified SMU pins and measures the resulting current, publishing the measurement
        /// results.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">Array of SMU pin names to which the voltage ramp is applied.</param>
        public static void ForceVoltageRampMeasureCurrent(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var voltageSequence = HelperMethods.CreateRampSequence(outputStart: 0, outputStop: 3, numberOfPoints: 10);
            var dcPowerPins = sessionManager.DCPower(smuPinNames);

            dcPowerPins.ConfigureMeasureSettings(new DCPowerMeasureSettings() { MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete });
            // dcPowerPins.DisableTriggers();
            dcPowerPins.ForceVoltageSequence(voltageSequence);

            const string publishedId = "Current";
            dcPowerPins.MeasureAndPublishCurrent(publishedId, out _);
        }
    }
}
