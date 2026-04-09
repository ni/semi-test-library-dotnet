using System.Collections.Generic;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// This class provides example methods demonstrating how to perform Hardware Level Sequencing with SMUs
    /// using DCPower Instrument Abstraction methods from the Semiconductor Test Library.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Configures an advanced sequence for the specified SMU pins without setting it as the active sequence, allowing it to be initiated later in the test flow.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">SMU pin names to be configured.</param>
        public static void ConfigureSMUAdvancedSequence(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle dcPowerPins = sessionManager.DCPower(smuPinNames);

            // Measurements can be taken during sequence execution, with exactly one sample for each step,
            // but to enable this, the MeasureWhen property must be set to AutomaticallyAfterSourceComplete.
            dcPowerPins.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);

            // Configure common source settings before calling ConfigureAdvancedSequence,
            // as ConfigureSourceSettings will set Source Mode to SinglePoint.
            dcPowerPins.ConfigureSourceSettings(new DCPowerSourceSettings()
            {
                SourceDelayInSeconds = 10,
                TransientResponse = DCPowerSourceTransientResponse.Normal,
            });

            // Commit must be called before ConfigureAdvancedSequence if setAsActiveSequence is set to false.
            // If Commit is called after ConfigureAdvancedSequence without first initiating the advanced sequence or setting setAsActiveSequence as true,
            // the driver will throw an exception..
            dcPowerPins.Commit();

            // Configure the advanced sequence upfront, but do not set it as the active sequence.
            // This allows you to initiate the advanced sequence later in your test flow without needing to reconfigure it.
            string advanceSequenceName = "MyAdvancedSequence";
            var advanceSequenceSettings = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 0.25, ApertureTime = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 0.5, ApertureTime = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 0.75, ApertureTime = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
            };

            dcPowerPins.ConfigureAdvancedSequence(advanceSequenceName, advanceSequenceSettings, setAsActiveSequence: false);
        }
    }
}
