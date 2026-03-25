using System.Collections.Generic;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// This class provides example methods to demonstrate how to use the extension method for Hardware Level Sequencing from the Semiconductor Test Library.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// This method demonstrates how to configure an advanced sequence upfront without setting it as the active sequence, allowing you to initiate it later in your test flow without needing to reconfigure it.
        /// </summary>
        /// <remarks>
        /// This method is not supported by all instruments. Refer to the <a href="https://www.ni.com/docs/en-US/bundle/ni-dcpower-c-api-ref/page/group____root__nidcpower__supported__functions__by__device.html">Supported Functions by Device</a> topic in the NI DC Power Supplies and SMUs Help for information about supported instruments.
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">SMU pin names to be configured.</param>
        public static void ConfigureUpfrontAndInitiateAdvancedSequenceLater(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var dcPowerPins = sessionManager.DCPower(smuPinNames);

            // Configure the measure settings upfront
            dcPowerPins.ConfigureMeasureSettings(new DCPowerMeasureSettings()
            {
                MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete,
            });

            // Configure the source settings upfront
            dcPowerPins.ConfigureSourceSettings(new DCPowerSourceSettings()
            {
                SourceDelayInSeconds = 10,
                TransientResponse = DCPowerSourceTransientResponse.Normal,
            });

            dcPowerPins.Commit();

            // configure the advanced sequence upfront, but do not set it as the active sequence. This allows you to initiate the advanced sequence later in your test flow without needing to reconfigure it.
            var advanceSequenceName = "MyAdvancedSequence";
            var advanceSequenceSettings = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, ApertureTime = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, ApertureTime = 2.1, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, ApertureTime = 2.2, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
            };

            dcPowerPins.ConfigureAdvancedSequence(advanceSequenceName, advanceSequenceSettings, setAsActiveSequence: false);
        }

        /// <summary>
        /// This method demonstrates initiating a previously configured advanced sequence.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to initiate the advanced sequence on.</param>
        /// <param name="advanceSequenceName">The name of the advanced sequence to initiate.(advanced sequence name should be same as what was created in <see cref="ConfigureUpfrontAndInitiateAdvancedSequenceLater"/>).</param>
        public static void InitiateAdvancedSequence(ISemiconductorModuleContext tsmContext, string[] smuPinNames, string advanceSequenceName)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var dcPowerPins = sessionManager.DCPower(smuPinNames);
            // Initiate the advanced sequence that was configured earlier
            dcPowerPins.InitiateAdvancedSequence(advanceSequenceName);
        }
    }
}
