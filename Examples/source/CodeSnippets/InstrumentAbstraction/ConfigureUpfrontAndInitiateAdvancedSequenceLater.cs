using System.Collections.Generic;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions(specifically extensions related to hardware level sequencing) from the Semiconductor Test Library.
    /// Specifically, how to measure current for pins mapped to DCPower Instruments.
    /// Note that DCPower Instruments include both Source Measurement Units (SMUs) and Programmable Power Supplies (PPS) devices.
    /// This class, and it's methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// with any dependent instrument sessions have been already initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    public static class ConfigureUpfrontAndInitiateAdvancedSequenceLater
    {
        /// <summary>
        /// Configures measurement and source settings for specified SMU pins and sets up an advanced sequence that can
        /// be initiated later in the test flow.
        /// </summary>
        /// <remarks>
        /// This example demonstrates how to configure measurement and source settings, as well
        /// as an advanced sequence, upfront without immediately activating the sequence. The advanced sequence can be
        /// initiated at a later point in the test flow, allowing for flexible test execution and reducing the need for
        /// repeated configuration.
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">SMU pin names to be configured.</param>
        public static void ConfigureUpfrontAndInitiateAdvancedSequenceLaterExample(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var dcPowerPins = sessionManager.DCPower(smuPinNames);

            // Configure the measure settings upfront
            dcPowerPins.ConfigureMeasureSettings(new DCPowerMeasureSettings() { MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete });

            // Configure the source settings upfront(ConfigureSourceSettings should be called before ConfigureAdvancedSequence as ConfigureSourceSettings sets Source.Mode to SinglePoint)
            dcPowerPins.ConfigureSourceSettings(new DCPowerSourceSettings() { SourceDelayInSeconds = 10, TransientResponse = DCPowerSourceTransientResponse.Normal });

            dcPowerPins.Commit();

            // configure the advanced sequence upfront, but do not set it as the active sequence. This allows you to initiate the advanced sequence later in your test flow without needing to reconfigure it.
            var advancedSequenceName = "MyAdvancedSequence";
            var advancedSequenceSettings = new List<DCPowerAdvancedSequenceStepProperties>
            {
                [0] = new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, ApertureTime = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                [1] = new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, ApertureTime = 2.1, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                [2] = new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, ApertureTime = 2.2, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            dcPowerPins.ConfigureAdvancedSequence(advancedSequenceName, advancedSequenceSettings, setAsActiveSequence: false);

            // Initiate the advanced sequence that was configured earlier
            dcPowerPins.InitiateAdvancedSequence(advancedSequenceName);

            // Clear the active advanced sequence before deleting and post usage
            dcPowerPins.ClearActiveAdvancedSequence();
            // Then delete the advanced sequence, this will also switch the Source.Mode back to SinglePoint
            dcPowerPins.DeleteAdvancedSequence(advancedSequenceName);
        }

        /// <summary>
        /// Configures multiple advanced sequences for specified SMU pins and initiates one of the sequences later in the test flow. This enables upfront configuration of test sequences for flexible execution.
        /// </summary>
        /// <remarks>
        /// This method allows advanced sequences to be configured in advance without immediately
        /// activating them. Sequences can then be initiated as needed during the test flow, reducing reconfiguration
        /// overhead and enabling more efficient test execution. Ensure that the provided pin names are valid and that
        /// the advanced sequences are properly defined before initiation.
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">SMU pin names to be configured.</param>
        internal static void ConfigureMultipleAdvanceSequencesUpfrontAndInitiateAdvancedSequencesLaterExample(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var dcPowerPins = sessionManager.DCPower(smuPinNames);

            // Configure the measure settings upfront
            dcPowerPins.ConfigureMeasureSettings(new DCPowerMeasureSettings() { MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete });

            // Configure the source settings upfront
            dcPowerPins.ConfigureSourceSettings(new DCPowerSourceSettings() { SourceDelayInSeconds = 10, TransientResponse = DCPowerSourceTransientResponse.Normal });

            dcPowerPins.Commit();

            // configure the advanced sequence upfront, but do not set it as the active sequence. This allows you to initiate the advanced sequence later in your test flow without needing to reconfigure it.
            var firstAdvancedSequence = "MyAdvancedSequence1";

            var advanceSequenceSettingsForFirstAdvancedSequences = new List<DCPowerAdvancedSequenceStepProperties>
            {
                [0] = new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, ApertureTime = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                [1] = new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, ApertureTime = 2.1, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                [2] = new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, ApertureTime = 2.2, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };

            // configure another advanced sequence with different settings
            var secondAdvancedSequence = "MyAdvancedSequence2";

            var voltages = new double[] { 0, 2, 3, 4 };
            var apertureTimes = new double[] { 2, 3.1, 4.3, 2.4 };
            var advancedSequenceSettingsForSecondAdvancedSequences = new DCPowerAdvancedSequenceStepProperties[4];
            for (int i = 0; i < voltages.Length; i++)
            {
                var stepSetting = new DCPowerAdvancedSequenceStepProperties
                {
                    VoltageLevel = voltages[i],
                    ApertureTime = apertureTimes[i]
                };
                advancedSequenceSettingsForSecondAdvancedSequences[i] = stepSetting;
            }

            // Configure both advanced sequences upfront without setting either of them as the active sequence. This allows you to initiate either of the advanced sequences later in your test flow without needing to reconfigure them.
            dcPowerPins.ConfigureAdvancedSequence(firstAdvancedSequence, advanceSequenceSettingsForFirstAdvancedSequences, setAsActiveSequence: false);
            dcPowerPins.ConfigureAdvancedSequence(secondAdvancedSequence, advancedSequenceSettingsForSecondAdvancedSequences, setAsActiveSequence: false);

            // Initiate the advanced sequence that was configured earlier
            dcPowerPins.InitiateAdvancedSequence(firstAdvancedSequence);

            // Clear the active advanced sequence before deleting and post usage
            dcPowerPins.ClearActiveAdvancedSequence();
            // Then delete all the advanced sequence, this will also switch the Source.Mode back to SinglePoint
            dcPowerPins.DeleteAdvancedSequence(firstAdvancedSequence);
            dcPowerPins.DeleteAdvancedSequence(secondAdvancedSequence);
        }
    }
}
